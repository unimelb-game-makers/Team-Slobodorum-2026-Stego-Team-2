

namespace TeamSlobodorum.Spells.Core
{
    public interface ISpellFactory
    {
        ISpellRuntime Create(SpellDefinition definition, SpellHandle handle);
    }
}