namespace TeamSlobodorum.AI.Memory
{
    public static class MemoryModuleTypeRegistry
    {
        private static int _nextId;

        public static MemoryModuleType<T> RegisterMemoryModuleType<T>(string name)
        {
            var type = new MemoryModuleType<T>(_nextId, name);
            _nextId++;
            return type;
        }
    }
}