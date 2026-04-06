using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public interface ITargetingService
    {
        bool Raycast(Vector3 origin, Vector3 direction, float distance, LayerMask mask, out RaycastHit hit);


    }
}