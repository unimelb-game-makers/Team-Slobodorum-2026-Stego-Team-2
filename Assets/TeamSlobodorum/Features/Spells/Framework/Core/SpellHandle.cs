using System;

namespace TeamSlobodorum.Spells.Core
{
    [Serializable]
    public struct SpellHandle : IEquatable<SpellHandle>
    {
        public int Value;

        public SpellHandle(int value) => Value = value;

        public bool IsValid => Value > 0;

        public bool Equals(SpellHandle other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SpellHandle other && Equals(other);
        public override int GetHashCode() => Value;
        public override string ToString() => $"SpellHandle({Value})";
    }
}