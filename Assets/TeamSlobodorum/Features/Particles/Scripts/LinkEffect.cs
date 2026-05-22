using System;
using UnityEngine;
using UnityEngine.VFX;

namespace TeamSlobodorum.Particles
{
    public class LinkEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffect visualEffect;
        public Transform endPoint;

        private SphereCollider _collider;

        private void Awake()
        {
            _collider = GetComponent<SphereCollider>();
        }

        private void Update()
        {
            if (endPoint)
            {
                var localPosition = transform.InverseTransformPoint(endPoint.position);
                visualEffect.SetVector3("BeamEndPoint_position", localPosition);
                _collider.center = localPosition;
            }
        }
    }
}