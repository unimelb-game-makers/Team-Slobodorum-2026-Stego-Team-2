using System;
using System.Collections.Generic;
using UnityEngine;

namespace TeamSlobodorum.Flammable
{
    [Serializable]
    public struct FlameAge
    {
        public float time;
        public GameObject prefab;
        public float radius;
        public float spreadMultiplier;
    }

    public class Fire : MonoBehaviour
    {
        [SerializeField] public List<FlameAge> flameAges;
        [SerializeField] public float disappearSpeed = 5f;
        [SerializeField] public float spreadSpeed = 10f;
        [SerializeField] public float spreadInterval = 1f;

        private float _currentTime;
        private float _spreadCounter;
        private float _currentScale;
        private float _targetScale;
        private bool _disappear;

        private int _flameIndex;
        private GameObject _flame;
        public FlameAge CurrentFlameAge => flameAges[_flameIndex];

        private readonly Collider[] _collideResults = new Collider[10];

        private void Start()
        {
            _flameIndex = 0;
            _flame = Instantiate(CurrentFlameAge.prefab, transform);

            transform.localScale = new Vector3(0, 0, 0);
            _targetScale = 1;
        }

        private void Update()
        {
            transform.up = Vector3.up;

            if (_flameIndex < flameAges.Count - 1)
            {
                if (_currentTime >= flameAges[_flameIndex + 1].time)
                {
                    Destroy(_flame);
                    _flameIndex++;
                    _flame = Instantiate(CurrentFlameAge.prefab, transform);
                }
            }

            _currentTime += Time.deltaTime;

            if (_spreadCounter > 0)
            {
                _spreadCounter -= Time.deltaTime;
            }
            else
            {
                HandleSpread();
                _spreadCounter = spreadInterval;
            }

            _currentScale = Mathf.Lerp(_currentScale, _targetScale, Time.deltaTime * disappearSpeed);
            transform.localScale = new Vector3(_currentScale, _currentScale, _currentScale);

            if (_disappear && _currentScale < 0.05f)
            {
                Destroy(gameObject);
            }
        }

        private void HandleSpread()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, CurrentFlameAge.radius, _collideResults);
            for (var i = 0; i < size; i++)
            {
                if (_collideResults[i].TryGetComponent<Flammable>(out var flammable))
                {
                    if (Physics.Raycast(transform.position,
                            (flammable.transform.position - transform.position).normalized,
                            out var hit))
                    {
                        if (hit.collider == _collideResults[i])
                        {
                            flammable.SpreadAtPoint(hit.point,
                                spreadSpeed * spreadInterval * CurrentFlameAge.spreadMultiplier);
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