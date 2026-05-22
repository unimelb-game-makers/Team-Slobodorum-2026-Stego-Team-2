namespace TeamSlobodorum.AI.Memory
{
    public interface IMemoryModuleType
    {
        public int Id { get; }
        public string Name { get; }
    }

    public class MemoryModuleType<T> : IMemoryModuleType
    {
        public int Id { get; }
        public string Name { get; }

        public MemoryModuleType(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is IMemoryModuleType other)
            {
                return Id.Equals(other.Id);
            }

            return false;
        }
    }
}