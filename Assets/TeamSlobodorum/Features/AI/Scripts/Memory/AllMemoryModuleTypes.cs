using TeamSlobodorum.Entities.Player;
using UnityEngine;

namespace TeamSlobodorum.AI.Memory
{
    public static class AllMemoryModuleTypes
    {
        public static readonly MemoryModuleType<PlayerEntity> PlayerInSight =
            MemoryModuleTypeRegistry.RegisterMemoryModuleType<PlayerEntity>("player_in_sight");
        public static readonly MemoryModuleType<Vector3> LastKnownPlayerPosition =
            MemoryModuleTypeRegistry.RegisterMemoryModuleType<Vector3>("last_known_player_position");
        public static readonly MemoryModuleType<float> TimeLastSawPlayer =
            MemoryModuleTypeRegistry.RegisterMemoryModuleType<float>("time_last_saw_player");
    }
}