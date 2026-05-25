using TeamSlobodorum.Spells.Core;
using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public sealed class ExplosionRuntime : SpellRuntimeBase
    {
        private ExplosionDefinition def => (ExplosionDefinition)Definition;

        public override bool IsFinished =>
            State == SpellExecutionState.Completed ||
            State == SpellExecutionState.Cancelled;

        public override void Begin(SpellContext context)
        {
            base.Begin(context);

            if (def == null)
            {
                Debug.LogWarning("Explosion definition is missing.");
                State = SpellExecutionState.Cancelled;
                return;
            }

            if (context.AimOrigin == null)
            {
                Debug.LogWarning("Explosion requires an aim origin.");
                State = SpellExecutionState.Cancelled;
                return;
            }

            Vector3 explosionPoint = context.AimOrigin.position;

            SpawnVfx(explosionPoint);
            ApplyExplosion(explosionPoint, context);

            State = SpellExecutionState.Completed;
        }

        private void SpawnVfx(Vector3 point)
        {
            if (def.explosionVfxPrefab == null)
                return;

            GameObject vfx = Object.Instantiate(def.explosionVfxPrefab, point, Quaternion.identity);

        }

        private void ApplyExplosion(Vector3 point, SpellContext context)
        {
            Collider[] hits = new Collider[Mathf.Max(1, def.maxColliders)];

            int count = Physics.OverlapSphereNonAlloc(
                point,
                def.radius,
                hits,
                def.affectedLayers,
                def.triggerInteraction
            );

            for (int i = 0; i < count; i++)
            {
                Collider hit = hits[i];
                if (hit == null)
                    continue;

                Rigidbody rb = hit.attachedRigidbody;
                if (rb == null || rb.isKinematic)
                    continue;

                if (!def.affectCaster &&
                    context.PlayerOrigin != null &&
                    rb.transform.IsChildOf(context.PlayerOrigin))
                {
                    continue;
                }

                rb.AddExplosionForce(
                    def.force,
                    point,
                    def.radius,
                    def.upwardsModifier,
                    ForceMode.VelocityChange);
            }
        }
    }
}