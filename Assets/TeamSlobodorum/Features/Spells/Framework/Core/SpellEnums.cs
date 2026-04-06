using System;

namespace TeamSlobodorum.Spells.Core
{
    public enum SpellExecutionState
    {
        None,
        Casting,
        Active,
        Recovery,
        Completed,
        Cancelled
    }

    public enum SpellCancelReason
    {
        Interrupted,
        UserCancelled,
        InvalidState,
        Replaced,
        OwnerDied
    }

    public enum SpellCategory
    {
        Mobility,
        Projectile,
        Utility,
        Trap,
        Explosion
    }

    [Flags]
    public enum SpellTags
    {
        None = 0,
        Gravity = 1 << 0,
        Push = 1 << 1,
        Explosion = 1 << 2,
        Projectile = 1 << 3,
        Mobility = 1 << 4
    }

    [Flags]
    public enum SpellResourceChannel
    {
        None = 0,
        Movement = 1 << 0,
        Rotation = 1 << 1,
        CastHand = 1 << 2,
        PrimaryCast = 1 << 3
    }
}