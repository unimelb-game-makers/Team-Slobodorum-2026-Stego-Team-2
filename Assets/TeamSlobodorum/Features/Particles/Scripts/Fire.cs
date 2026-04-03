using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamSlobodorum.Particles
{
    public class Fire : MonoBehaviour
    {
        [SerializeField] private List<GameObject> firePrefabs;
        [SerializeField] private float disappearSpeed = 5f;
        [SerializeField] private float spreadSpeed = 10f;
        [SerializeField] private float spreadRadius = 0.5f;
        [SerializeField] private float spreadInterval = 1f;

        private float _currentTime;
        private float _currentScale;
        private float _targetScale;
        private bool _disappear;
        private readonly Collider[] _results = new Collider[10];

        private void Start()
        {
            var index = Random.Range(0, firePrefabs.Count);
            var firePrefab = firePrefabs[index];
            Instantiate(firePrefab, transform);

            transform.localScale = new Vector3(0, 0, 0);
            _targetScale = 1;
        }

        private void Update()
        {
            transform.up = Vector3.up;

            if (_currentTime > 0)
            {
                _currentTime -= Time.deltaTime;
            }
            else
            {
                HandleSpread();
                _currentTime = spreadInterval;
            }

            _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * disappearSpeed);
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);

            if (_disappear && _currentScale < 0.05f)
            {
                Destroy(gameObject);
            }
        }
        //
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.TryGetComponent<Flammable.Flammable>(out var flammable))
        //     {
        //         if (Physics.Raycast(transform.position, (other.transform.position - transform.position).normalized,
        //                 out var hit))
        //         {
        //             if (hit.collider == other)
        //             {
        //                 flammable.SpreadAtPoint(hit.point, float.PositiveInfinity);
        //             }
        //         }
        //     }
        // }

        private void HandleSpread()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, spreadRadius, _results);
            for (var i = 0; i < size; i++)
            {
                if (_results[i].TryGetComponent<Flammable.Flammable>(out var flammable))
                {
                    if (Physics.Raycast(transform.position,
                            (flammable.transform.position - transform.position).normalized,
                            out var hit))
                    {
                        if (hit.collider == _results[i])
                        {
                            flammable.SpreadAtPoint(hit.point, spreadSpeed * spreadInterval);
                        }
                    }
                }
            }
        }

        public void Disappear()
        {
            _targetScale = 0;
            _disappear = true;
        }
    }
}