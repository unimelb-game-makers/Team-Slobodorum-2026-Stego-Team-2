using TeamSlobodorum.Entities;
using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public interface ITargetingService
    {
        bool TryResolveEntity(
            Vector3 aimPoint,
            out Entity entity);

        bool TryResolveEntity(
            Vector3 aimPoint,
            float radius,
            LayerMask mask,
            out Entity entity);

    }
}