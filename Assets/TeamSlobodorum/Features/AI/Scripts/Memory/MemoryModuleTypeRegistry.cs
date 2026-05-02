using System.Collections.Generic;
using TeamSlobodorum.Entities.Player;

namespace TeamSlobodorum.AI.Memory
{
    public static class MemoryModuleTypeRegistry
    {
        private static readonly Dictionary<int, string> NameMap = new();
        private static int _nextId;

        public static MemoryModuleType<T> RegisterMemoryModuleType<T>(string name)
        {
            var type = new MemoryModuleType<T>(_nextId, name);
            NameMap.Add(_nextId, name);
            _nextId++;
            return type;
        }

        public static string MemoryModuleName(int id)
        {
            return NameMap[id];
        }
    }
}