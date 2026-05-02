namespace TeamSlobodorum.AI.Memory
{
    public class MemoryModuleType<T>
    {
        public readonly int Id;
        public readonly string Name;
        
        public MemoryModuleType(int id, string name)
        {
            Id = id;
        }
    }
}