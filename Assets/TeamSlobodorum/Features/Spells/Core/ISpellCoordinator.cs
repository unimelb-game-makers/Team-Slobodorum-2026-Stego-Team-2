
namespace TeamSlobodorum.Spells.Core
{
    public interface ISpellCoordinator
    {
        bool TryCast(SpellCastRequest request, out SpellHandle handle);
        bool TryGetRuntime(SpellHandle handle, out ISpellRuntime runtime);
        void Cancel(SpellHandle handle, SpellCancelReason reason);
        void Publish(ISpellEvent evt);
    }
}