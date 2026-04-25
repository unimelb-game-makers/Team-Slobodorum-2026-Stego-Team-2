using System.Text;
using TeamSlobodorum.Entities;
using TeamSlobodorum.Spells.Core;
using UnityEngine;

public class AimTargetingService : ITargetingService
{
    private readonly Collider[] _hits = new Collider[16];

    public bool TryResolveEntity(Vector3 aimPoint, out Entity entity)
    {
        entity = ResolveEntityNearAimPoint(aimPoint, 0.2f, ~0);
        DebugDetectedEntities(aimPoint, 0.2f, ~0, entity);
        return entity != null;
    }

    public bool TryResolveEntity(Vector3 aimPoint, float radius, LayerMask mask, out Entity entity)
    {
        entity = ResolveEntityNearAimPoint(aimPoint, radius, mask);
        DebugDetectedEntities(aimPoint, radius, mask, entity);
        return entity != null;
    }

    public Entity ResolveEntityNearAimPoint(Vector3 aimPoint, float radius, LayerMask mask)
    {
        int count = Physics.OverlapSphereNonAlloc(aimPoint, radius, _hits, mask);

        Entity bestEntity = null;
        float bestDistSqr = float.MaxValue;

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (col == null || !col.TryGetComponent<Entity>(out var entity))
                continue;

            Vector3 closest = col.ClosestPoint(aimPoint);
            float distSqr = (closest - aimPoint).sqrMagnitude;

            if (distSqr < bestDistSqr)
            {
                bestDistSqr = distSqr;
                bestEntity = entity;
            }
        }

        return bestEntity;
    }

    private void DebugDetectedEntities(Vector3 aimPoint, float radius, LayerMask mask, Entity resolvedEntity)
    {
        int count = Physics.OverlapSphereNonAlloc(aimPoint, radius, _hits, mask);

        var sb = new StringBuilder();
        sb.AppendLine($"[AimTargetingService] aimPoint={aimPoint}, radius={radius}, colliderCount={count}");

        for (int i = 0; i < count; i++)
        {
            var col = _hits[i];
            if (col == null)
                continue;

            Vector3 closest = col.ClosestPoint(aimPoint);
            float dist = Vector3.Distance(closest, aimPoint);

            if (col.TryGetComponent<Entity>(out var entity))
            {
                string marker = entity == resolvedEntity ? " <-- SELECTED" : "";
                sb.AppendLine($"  [{i}] Entity={entity.name}, Collider={col.name}, Dist={dist:F3}{marker}");
            }
            else
            {
                sb.AppendLine($"  [{i}] Non-Entity Collider={col.name}, Dist={dist:F3}");
            }
        }

        Debug.Log(sb.ToString());
    }
}