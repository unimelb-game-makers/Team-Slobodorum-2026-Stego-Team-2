using UnityEngine;
using UnityEngine.VFX;

namespace TeamSlobodorum.Particles
{
    public class LinkEffect : MonoBehaviour
    {
        [SerializeField] private VisualEffect visualEffect;
        public Transform endPoint;

        private void Update()
        {
            if (endPoint)
            {
                var localPosition = transform.InverseTransformPoint(endPoint.position);
                visualEffect.SetVector3("BeamEndPoint_position", localPosition);
            }
        }
    }
}