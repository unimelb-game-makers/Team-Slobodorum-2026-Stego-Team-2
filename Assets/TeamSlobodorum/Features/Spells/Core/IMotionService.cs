using TeamSlobodorum.Spells.Motion;
using UnityEngine;

namespace TeamSlobodorum.Spells.Core
{
    public interface IMotionService
    {
        public void Submit(Vector3 acceleration, float duration, bool overrideXZ = false, bool overrideY = false);

    }
}