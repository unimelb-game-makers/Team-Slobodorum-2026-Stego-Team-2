using System;

namespace TeamSlobodorum.Spells.Core
{
    [Serializable]
    public struct SpellId : IEquatable<SpellId>
    {
        public string Value;

        public SpellId(string value) => Value = value;

        public bool Equals(SpellId other) => Value == other.Value;
        public override bool Equals(object obj) => obj is SpellId other && Equals(other);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
    }
}