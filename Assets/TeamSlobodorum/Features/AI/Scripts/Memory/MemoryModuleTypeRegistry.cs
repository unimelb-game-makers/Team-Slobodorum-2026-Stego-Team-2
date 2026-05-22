using System.Collections.Generic;

namespace TeamSlobodorum.AI.Memory
{
    public static class MemoryModuleTypeRegistry
    {
        private static int _nextId;
        private static readonly Dictionary<string, IMemoryModuleType> NameMap = new ();

        public static MemoryModuleType<T> RegisterMemoryModuleType<T>(string name)
        {
            var type = new MemoryModuleType<T>(_nextId, name);
            _nextId++;
            NameMap.Add(name, type);
            return type;
        }

        public static IMemoryModuleType GetMemoryModuleType(string name)
        {
            return NameMap[name];
        }
    }
}