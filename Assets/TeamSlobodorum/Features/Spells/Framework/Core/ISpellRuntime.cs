using System;

namespace TeamSlobodorum.Spells.Core
{
    public interface ISpellRuntime
    {
        SpellHandle Handle { get; }
        SpellDefinition Definition { get; }
        SpellExecutionState State { get; }

        bool IsFinished { get; }

        void Initialize(SpellHandle handle, SpellDefinition definition);
        void Begin(SpellContext context);
        void Tick(float deltaTime);
        void FixedTick(float fixedDeltaTime);
        void Cancel(SpellCancelReason reason);

        // TODO: For future coordinate spells
        void ReceiveEvent(ISpellEvent evt);
    }
}