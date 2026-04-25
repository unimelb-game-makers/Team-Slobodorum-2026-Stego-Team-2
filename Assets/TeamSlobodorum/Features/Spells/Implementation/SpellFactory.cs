using TeamSlobodorum.Spells.Core;

namespace TeamSlobodorum.Spells
{
    public sealed class SpellFactory : ISpellFactory
    {
        public ISpellRuntime Create(SpellDefinition definition, SpellHandle handle)
        {
            if (definition == null)
                return null;

            ISpellRuntime runtime = definition.CreateRuntime();
            if (runtime == null)
                return null;

            runtime.Initialize(handle, definition);
            return runtime;
        }
    }
}