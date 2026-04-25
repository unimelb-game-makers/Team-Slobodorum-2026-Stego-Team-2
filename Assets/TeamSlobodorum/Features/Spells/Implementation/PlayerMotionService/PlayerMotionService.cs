using System.Collections.Generic;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells.Motion
{
    public class PlayerMotionService : MonoBehaviour, IMotionService
    {
        [SerializeField] private float externalDrag = 2f;
        private struct ActiveMotion
        {
            public Vector3 Acceleration;
            public float TimeRemaining;
            public bool OverrideXZ;
            public bool OverrideY;
        }

        private readonly List<ActiveMotion> _motions = new();

        private Vector3 _externalVelocity;

        public void Submit(Vector3 acceleration, float duration, bool overrideXZ = false, bool overrideY = false)
        {
            _motions.Add(new ActiveMotion
            {
                Acceleration = acceleration,
                TimeRemaining = Mathf.Max(duration, 0.0001f),
                OverrideXZ = overrideXZ,
                OverrideY = overrideY
            });
        }

        public ResolvedPlayerMotion Resolve(float deltaTime)
        {
            Vector3 totalAcceleration = Vector3.zero;

            bool hasOverrideXZ = false;
            bool hasOverrideY = false;

            for (int i = _motions.Count - 1; i >= 0; --i)
            {
                var motion = _motions[i];
                totalAcceleration += motion.Acceleration;

                if (motion.OverrideXZ) hasOverrideXZ = true;
                if (motion.OverrideY) hasOverrideY = true;

                motion.TimeRemaining -= deltaTime;
                if (motion.TimeRemaining <= 0f)
                    _motions.RemoveAt(i);
                else
                    _motions[i] = motion;
            }

            // Integrate acceleration into cached external velocity
            _externalVelocity += totalAcceleration * deltaTime;
            _externalVelocity = Vector3.Lerp(_externalVelocity, Vector3.zero, 1f - Mathf.Exp(-externalDrag * deltaTime));

            return new ResolvedPlayerMotion
            {
                AdditiveVelocity = _externalVelocity,
                HasOverrideXZ = hasOverrideXZ,
                OverrideVelocityXZ = new Vector3(_externalVelocity.x, 0f, _externalVelocity.z),
                HasOverrideY = hasOverrideY,
                OverrideVelocityY = _externalVelocity.y
            };
        }

        public void Clear()
        {
            _motions.Clear();
            _externalVelocity = Vector3.zero;
        }

        public void ClearVelocity()
        {
            _externalVelocity = Vector3.zero;
        }
    }

    public struct ResolvedPlayerMotion
    {
        public Vector3 AdditiveVelocity;
        public bool HasOverrideXZ;
        public Vector3 OverrideVelocityXZ;
        public bool HasOverrideY;
        public float OverrideVelocityY;
    }
}