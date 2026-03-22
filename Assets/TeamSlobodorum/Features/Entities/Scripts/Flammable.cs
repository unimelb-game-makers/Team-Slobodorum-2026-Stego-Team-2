using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamSlobodorum.Particles;
using UnityEngine;

namespace TeamSlobodorum.Entities
{
    public class VoxelData
    {
        public readonly Vector3Int GridCoord;
        [CanBeNull] public Fire Fire;
        public float Resistance;
        public float BurningTime;

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
        [SerializeField] private Fire firePrefab;
        [SerializeField] private float voxelSize = 1f;
        [SerializeField] private float resistance = 10f;
        [SerializeField] private float burningTime = 120f;
        [SerializeField] private float spreadSpeed = 200f;
        [SerializeField] private float spreadInterval = 1f;

        private MeshFilter _meshFilter;

        private readonly Dictionary<Vector3Int, VoxelData> _voxelMap = new();
        private float _currentTime;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Start()
        {
            GenerateVoxelMap();
            print(_voxelMap.Count);
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
            foreach (var (gridCoord, voxelData) in _voxelMap)
            {
                if (voxelData.Fire)
                {
                    foreach (var neighbor in GetNeighbors(gridCoord))
                    {
                        if (neighbor.Resistance > 0)
                        {
                            neighbor.Resistance -= Time.deltaTime * spreadSpeed * spreadInterval;
                            if (neighbor.Resistance <= 0 && neighbor.BurningTime > 0 && !neighbor.Fire)
                            {
                                var center = transform.TransformPoint(GridCoordToLocal(neighbor.GridCoord));
                                var x = Random.Range(center.x - voxelSize / 2, center.x + voxelSize / 2);
                                var y = Random.Range(center.y - voxelSize / 2, center.y + voxelSize / 2);
                                var z = Random.Range(center.z - voxelSize / 2, center.z + voxelSize / 2);
                                neighbor.Fire = SpawnFire(new Vector3(x, y, z));
                            }
                        }
                    }

                    if (voxelData.BurningTime > 0)
                    {
                        voxelData.BurningTime -= Time.deltaTime * spreadInterval * 1000f;
                        if (voxelData.BurningTime <= 0)
                        {
                            voxelData.Fire.Disappear();
                        }
                    }
                }

                yield return null;
            }
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
                    }
                }
            }
        }

        private Fire SpawnFire(Vector3 position)
        {
            return Instantiate(firePrefab, position, Quaternion.identity, transform);
        }
    }
}