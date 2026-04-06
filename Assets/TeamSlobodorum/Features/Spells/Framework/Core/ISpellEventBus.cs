using System;

namespace TeamSlobodorum.Spells.Core
{
    public interface ISpellEventBus
    {
        void Publish<T>(T evt) where T : ISpellEvent;
        void Subscribe<T>(Action<T> handler) where T : ISpellEvent;
        void Unsubscribe<T>(Action<T> handler) where T : ISpellEvent;
    }
}