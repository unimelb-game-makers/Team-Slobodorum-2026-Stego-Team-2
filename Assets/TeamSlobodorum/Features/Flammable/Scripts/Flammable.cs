using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TeamSlobodorum.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamSlobodorum.Flammable
{
    public class VoxelData
    {
        public readonly Vector3Int GridCoord;
        [CanBeNull] public Fire Fire;
        public float Resistance;
        public float BurningTime;
        public int BurnMarkIndex = -1;

        public VoxelData(Vector3Int gridCoord, float resistance, float burningTime)
        {
            GridCoord = gridCoord;
            Resistance = resistance;
            BurningTime = burningTime;
        }

        public override string ToString()
        {
            return $"VoxelData {{ GridCoord: {GridCoord}, Fire: {Fire}, Resistance: {Resistance} }}";
        }
    }

    public class Flammable : MonoBehaviour
    {
        public event Action StartBurning;
        public event Action StopBurning;
        public event Action BurnOut;

        [Header("Material Properties")]
        [SerializeField,
         Tooltip(
             "The higher the value, the longer it takes for a voxel to ignite. " +
             "For example, wet objects usually have a higher resistance value.")]
        private float resistance = 100f;

        [SerializeField,
         Tooltip(
             "The higher this value, the faster the fire spreads inside the object. This differs from the resistance " +
             "value, which also affects the spread from the outside.")]
        private float spreadSpeed = 10f;

        [SerializeField,
         Tooltip(
             "The duration for which a single voxel can burn. The higher this value, the longer it takes for the " +
             "voxel to burn out completely.")]
        private float burningTime = 30f;

        [SerializeField, Tooltip("The time interval for calculating fire propagation.")]
        private float spreadInterval = 1f;

        [SerializeField] private bool produceSmoke;

        [Header("Voxel and Bounds")]
        [SerializeField] public bool useBoundsFromMeshFilter = true;

        [SerializeField] public Bounds bounds;
        [SerializeField] public bool useVoxel;
        [SerializeField] public Vector3 voxelSize = new(1, 1, 1);

        [Header("Burn Mark")]
        [SerializeField] private bool useBurnMark = true;

        [SerializeField] private Color burnMarkColor = new(0.05f, 0.05f, 0.05f, 1f);

        [SerializeField] [ColorUsage(true, true)]
        private Color emberColor = new(6f, 0.3f, 0f, 1f);

        [Header("Break Behaviour")]
        [SerializeField] public bool breakWhenBurnOut;

        [SerializeField] public GameObject spawnWhenBreak;

        private MeshRenderer _meshRenderer;
        [CanBeNull] private Material _materialInstance;

        private Dictionary<Vector3Int, VoxelData> _voxelMap;

        private bool _isBurning;

        public bool IsBurning
        {
            get => _isBurning;
            set
            {
                var prev = _isBurning;
                _isBurning = value;
                if (!prev && value)
                {
                    StartBurning?.Invoke();
                }
                else if (prev && !value)
                {
                    StopBurning?.Invoke();
                }
            }
        }

        private float _currentTime;

        private static readonly int BurnCenters = Shader.PropertyToID("_BurnCenters");
        private static readonly int BurnCount = Shader.PropertyToID("_BurnCount");
        private static readonly int BurnColor = Shader.PropertyToID("_BurnColor");
        private static readonly int EmberColor = Shader.PropertyToID("_EmberColor");
        private const int MaxBurnMarkCount = 20;
        private readonly Vector4[] _burnMarkPoints = new Vector4[MaxBurnMarkCount];
        private int _currentBurnMarkCount;

        private void Awake()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            
            StartBurning += OnStartBurning;
            StopBurning += OnStopBurning;
            if (breakWhenBurnOut)
            {
                BurnOut += HandleBreak;
            }
        }

        private void Start()
        {
            GenerateVoxelMap();
        }

        private void Update()
        {
            if (IsBurning)
            {
                if (_currentTime > 0)
                {
                    _currentTime -= Time.deltaTime;
                }
                else
                {
                    StartCoroutine(HandleSpread());
                    _currentTime = spreadInterval;
                }
            }
        }

        private IEnumerator HandleSpread()
        {
            var hasFire = false;
            foreach (var (gridCoord, voxelData) in _voxelMap)
            {
                if (voxelData.Fire)
                {
                    hasFire = true;
                    if (useVoxel)
                    {
                        foreach (var neighbor in GetNeighbors(gridCoord))
                        {
                            SpreadToVoxel(neighbor,
                                spreadSpeed * spreadInterval * voxelData.Fire.CurrentFlameAge.spreadMultiplier);
                        }
                    }

                    if (voxelData.BurningTime > 0)
                    {
                        voxelData.BurningTime -= spreadInterval;

                        if (voxelData.BurnMarkIndex >= 0)
                        {
                            var progress = 1 - voxelData.BurningTime / burningTime;
                            // Use Math.Pow to make the radius initially grow slowly
                            var radius = voxelSize.magnitude * Mathf.Pow(progress, 3f);
                            _burnMarkPoints[voxelData.BurnMarkIndex].w = radius;
                        }

                        if (voxelData.BurningTime <= 0)
                        {
                            voxelData.Fire.Disappear();
                        }
                    }
                }

                yield return null;
            }

            if (_materialInstance)
            {
                _materialInstance.SetVectorArray(BurnCenters, _burnMarkPoints);
                _materialInstance.SetInt(BurnCount, _currentBurnMarkCount);
            }

            IsBurning = hasFire;
        }

        private void OnStartBurning()
        {
            if (useBurnMark && !_materialInstance)
            {
                _materialInstance = new Material(PrefabManager.Instance.burnMarkMaterial);
                var materialsList = new List<Material>(_meshRenderer.materials) { _materialInstance };
                _meshRenderer.materials = materialsList.ToArray();

                _materialInstance.SetColor(BurnColor, burnMarkColor);
            }
            
            if (_materialInstance)
            {
                _materialInstance.SetColor(EmberColor, emberColor);
            }
        }

        private void OnStopBurning()
        {
            if (_materialInstance)
            {
                _materialInstance.SetColor(EmberColor, new Color(0, 0, 0, 0));
            }

            if (_voxelMap.All((x) => x.Value.BurningTime <= 0))
            {
                BurnOut?.Invoke();
            }
        }

        private void HandleBreak()
        {
            if (spawnWhenBreak)
            {
                Instantiate(spawnWhenBreak, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }

        public void GenerateVoxelMap()
        {
            _voxelMap = new Dictionary<Vector3Int, VoxelData>();
            var meshBounds = useBoundsFromMeshFilter ? GetComponent<MeshFilter>().mesh.bounds : bounds;

            if (useVoxel)
            {
                for (var x = meshBounds.min.x; x < meshBounds.max.x; x += voxelSize.x)
                {
                    for (var y = meshBounds.min.y; y < meshBounds.max.y; y += voxelSize.y)
                    {
                        for (var z = meshBounds.min.z; z < meshBounds.max.z; z += voxelSize.z)
                        {
                            var center = new Vector3(x + voxelSize.x / 2, y + voxelSize.y / 2, z + voxelSize.z / 2);
                            var globalPos = transform.TransformPoint(center);
                            if (Physics.CheckBox(globalPos, voxelSize / 2))
                            {
                                // Convert Local Position to Grid Integer Coordinate
                                var gridCoord = LocalToGridCoord(center);

                                var data = new VoxelData(gridCoord, resistance, burningTime);
                                _voxelMap[gridCoord] = data;
                            }
                        }
                    }
                }
            }
            else
            {
                voxelSize = meshBounds.size;
                Vector3Int gridCoord = default;
                var data = new VoxelData(gridCoord, resistance, burningTime);
                _voxelMap[gridCoord] = data;
            }
        }

        private Vector3Int LocalToGridCoord(Vector3 localPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt(localPos.x / voxelSize.x),
                Mathf.FloorToInt(localPos.y / voxelSize.y),
                Mathf.FloorToInt(localPos.z / voxelSize.z)
            );
        }

        private Vector3 GridCoordToLocal(Vector3Int gridCoord)
        {
            return new Vector3(
                gridCoord.x * voxelSize.x,
                gridCoord.y * voxelSize.y,
                gridCoord.z * voxelSize.z
            );
        }

        private List<VoxelData> GetNeighbors(Vector3Int currentCoord)
        {
            // The 6 adjacent directions (Up, Down, Left, Right, Forward, Back)
            Vector3Int[] directions =
            {
                new(1, 0, 0), new(-1, 0, 0),
                new(0, 1, 0), new(0, -1, 0),
                new(0, 0, 1), new(0, 0, -1)
            };

            var neighbors = new List<VoxelData>();
            foreach (var dir in directions)
            {
                var neighborCoord = currentCoord + dir;
                if (_voxelMap.TryGetValue(neighborCoord, out VoxelData neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public void SpreadAtPoint(Vector3 position, float power)
        {
            Vector3Int gridCoord;
            if (useVoxel)
            {
                var localPos = transform.InverseTransformPoint(position);
                gridCoord = LocalToGridCoord(localPos);
            }
            else
            {
                gridCoord = default;
            }

            if (_voxelMap.TryGetValue(gridCoord, out var data))
            {
                SpreadToVoxel(data, power);
            }
            else
            {
                var neighbors = GetNeighbors(gridCoord);
                if (neighbors.Count > 0)
                {
                    data = neighbors[0];
                    SpreadToVoxel(data, power);
                }
            }
        }

        private void SpreadToVoxel(VoxelData voxelData, float power)
        {
            if (voxelData.Resistance > 0)
            {
                voxelData.Resistance -= power * spreadInterval;
                if (voxelData.Resistance <= 0 && voxelData.BurningTime > 0 && !voxelData.Fire)
                {
                    var localPos = GridCoordToLocal(voxelData.GridCoord);
                    var worldPos = transform.TransformPoint(localPos);
                    var x = Random.Range(worldPos.x - voxelSize.x / 2, worldPos.x + voxelSize.x / 2);
                    var y = Random.Range(worldPos.y - voxelSize.y / 2, worldPos.y + voxelSize.y / 2);
                    var z = Random.Range(worldPos.z - voxelSize.z / 2, worldPos.z + voxelSize.z / 2);
                    voxelData.Fire = SpawnFire(new Vector3(x, y, z));
                    voxelData.BurnMarkIndex = AddBurnPoint(localPos);
                    IsBurning = true;
                }
            }
        }

        private Fire SpawnFire(Vector3 worldPos)
        {
            return Instantiate(
                produceSmoke ? PrefabManager.Instance.smokedFirePrefab : PrefabManager.Instance.firePrefab,
                worldPos,
                Quaternion.identity, transform);
        }

        private int AddBurnPoint(Vector3 localPos)
        {
            if (_currentBurnMarkCount < MaxBurnMarkCount)
            {
                var index = _currentBurnMarkCount;
                _burnMarkPoints[index] = new Vector4(localPos.x, localPos.y, localPos.z, 0);
                _currentBurnMarkCount++;
                return index;
            }

            return -1;
        }
    }
}