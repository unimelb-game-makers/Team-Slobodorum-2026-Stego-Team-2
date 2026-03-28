using System.Collections.Generic;
using TeamSlobodorum.Entities.Flammable;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamSlobodorum.Particles
{
    public class Fire : MonoBehaviour
    {
        [SerializeField] private List<GameObject> firePrefabs;
        [SerializeField] private float disappearSpeed = 5f;

        private float _currentScale;
        private float _targetScale;
        private bool _disappear;

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
            
            _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * disappearSpeed);
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);
            
            if (_disappear && _currentScale < 0.05f)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<Flammable>(out var flammable))
            {
                if (Physics.Raycast(transform.position, (other.transform.position - transform.position).normalized,
                        out var hit))
                {
                    if (hit.collider == other)
                    {
                        flammable.Ignite(hit.point);
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