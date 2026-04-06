

namespace TeamSlobodorum.Spells.Core
{
    public abstract class SpellRuntimeBase : ISpellRuntime
    {
        public SpellHandle Handle { get; private set; }
        public SpellDefinition Definition { get; private set; }
        public SpellExecutionState State { get; protected set; }

        protected SpellContext Context;

        public abstract bool IsFinished { get; }

        public virtual void Initialize(SpellHandle handle, SpellDefinition definition)
        {
            Handle = handle;
            Definition = definition;
            State = SpellExecutionState.None;
        }

        public virtual void Begin(SpellContext context)
        {
            Context = context;
            State = SpellExecutionState.Casting;
        }

        public virtual void Tick(float deltaTime) { }

        public virtual void FixedTick(float fixedDeltaTime) { }

        public virtual void Cancel(SpellCancelReason reason)
        {
            State = SpellExecutionState.Cancelled;
        }

        // TODO: Future spells might want to listen to events (Cancel on skill switch etc.)
        public virtual void ReceiveEvent(ISpellEvent evt) { }
    }
}