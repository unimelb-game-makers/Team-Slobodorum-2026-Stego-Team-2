using UnityEngine;

namespace TeamSlobodorum.Spells
{
    public abstract class Spell : MonoBehaviour
    {
        public abstract bool Active { get; }

        public abstract string SpellName { get; }

        public abstract void Use();

        public virtual void CancelEffect() {}
    }
}