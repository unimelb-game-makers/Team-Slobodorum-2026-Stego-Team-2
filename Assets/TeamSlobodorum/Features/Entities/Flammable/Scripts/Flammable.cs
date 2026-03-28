using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamSlobodorum.Core;
using TeamSlobodorum.Particles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamSlobodorum.Entities.Flammable
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
        [SerializeField] private float voxelSize = 1f;
        [SerializeField] private float resistance = 10f;
        [SerializeField] private float burningTime = 120f;
        [SerializeField] private float spreadSpeed = 200f;
        [SerializeField] private float spreadInterval = 1f;
        [SerializeField] private Color emberColor = new(1, 0.3f, 0, 1);

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Material _materialInstance;

        private readonly Dictionary<Vector3Int, VoxelData> _voxelMap = new();
        private float _currentTime;

        private static readonly int BurnCenters = Shader.PropertyToID("_BurnCenters");
        private static readonly int BurnCount = Shader.PropertyToID("_BurnCount");
        private static readonly int EmberColor = Shader.PropertyToID("_EmberColor");
        private const int MaxBurnMarkCount = 20;
        private readonly Vector4[] _burnMarkPoints = new Vector4[MaxBurnMarkCount];
        private int _currentBurnMarkCount;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            GenerateVoxelMap();
            print(_voxelMap.Count);

            _materialInstance = new Material(PrefabManager.Instance.burnMarkMaterial);
            var materialsList = new List<Material>(_meshRenderer.materials) { _materialInstance };
            _meshRenderer.materials = materialsList.ToArray();
        }

        private void Update()
        {
            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
            }
            else
            {
                StartCoroutine(SpreadFire());
                _currentTime = spreadInterval;
            }
        }

        private IEnumerator SpreadFire()
        {
            var hasFire = false;
            foreach (var (gridCoord, voxelData) in _voxelMap)
            {
                if (voxelData.Fire)
                {
                    hasFire = true;
                    foreach (var neighbor in GetNeighbors(gridCoord))
                    {
                        if (neighbor.Resistance > 0)
                        {
                            neighbor.Resistance -= Time.deltaTime * spreadSpeed * spreadInterval;
                            if (neighbor.Resistance <= 0 && neighbor.BurningTime > 0 && !neighbor.Fire)
                            {
                                var localPos = GridCoordToLocal(neighbor.GridCoord);
                                var worldPos = transform.TransformPoint(localPos);
                                var x = Random.Range(worldPos.x - voxelSize / 2, worldPos.x + voxelSize / 2);
                                var y = Random.Range(worldPos.y - voxelSize / 2, worldPos.y + voxelSize / 2);
                                var z = Random.Range(worldPos.z - voxelSize / 2, worldPos.z + voxelSize / 2);
                                neighbor.Fire = SpawnFire(new Vector3(x, y, z));
                                neighbor.BurnMarkIndex = AddBurnPoint(localPos);
                            }
                        }
                    }

                    if (voxelData.BurningTime > 0)
                    {
                        voxelData.BurningTime -= Time.deltaTime * spreadInterval * 1000f;

                        if (voxelData.BurnMarkIndex > 0)
                        {
                            var progress = 1 - voxelData.BurningTime / burningTime;
                            var radius = voxelSize * 1.5f * progress;
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

            _materialInstance.SetVectorArray(BurnCenters, _burnMarkPoints);
            _materialInstance.SetInt(BurnCount, _currentBurnMarkCount);
            _materialInstance.SetColor(EmberColor, hasFire ? emberColor : new Color(0, 0, 0, 0));
        }

        public void GenerateVoxelMap()
        {
            _voxelMap.Clear();
            var bounds = _meshFilter.mesh.bounds;

            for (var x = bounds.min.x; x < bounds.max.x; x += voxelSize)
            {
                for (var y = bounds.min.y; y < bounds.max.y; y += voxelSize)
                {
                    for (var z = bounds.min.z; z < bounds.max.z; z += voxelSize)
                    {
                        var center = new Vector3(x + voxelSize / 2, y + voxelSize / 2, z + voxelSize / 2);
                        var globalPos = transform.TransformPoint(center);
                        if (Physics.CheckBox(globalPos, Vector3.one * (voxelSize / 2)))
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

        private Vector3Int LocalToGridCoord(Vector3 localPos)
        {
            return new Vector3Int(
                Mathf.FloorToInt(localPos.x / voxelSize),
                Mathf.FloorToInt(localPos.y / voxelSize),
                Mathf.FloorToInt(localPos.z / voxelSize)
            );
        }

        private Vector3 GridCoordToLocal(Vector3Int gridCoord)
        {
            return new Vector3(
                gridCoord.x * voxelSize,
                gridCoord.y * voxelSize,
                gridCoord.z * voxelSize
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

        public void Ignite(Vector3 position)
        {
            var localPos = transform.InverseTransformPoint(position);
            var gridCoord = LocalToGridCoord(localPos);
            if (_voxelMap.TryGetValue(gridCoord, out var data))
            {
                print(data);
                if (data.BurningTime > 0 && !data.Fire)
                {
                    data.Resistance = 0;
                    data.Fire = SpawnFire(position);
                    data.BurnMarkIndex = AddBurnPoint(localPos);
                }
            }
            else
            {
                var neighbors = GetNeighbors(gridCoord);
                if (neighbors.Count > 0)
                {
                    data = neighbors[0];
                    print(data);
                    if (data.BurningTime > 0 && !data.Fire)
                    {
                        data.Resistance = 0;
                        data.Fire = SpawnFire(position);
                        data.BurnMarkIndex = AddBurnPoint(localPos);
                    }
                }
            }
        }

        private Fire SpawnFire(Vector3 worldPos)
        {
            return Instantiate(PrefabManager.Instance.firePrefab, worldPos, Quaternion.identity, transform);
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