using TeamSlobodorum.Particles;
using TeamSlobodorum.Spells;
using TeamSlobodorum.Spells.Core;
using UnityEngine;



[CreateAssetMenu(menuName = "Spells/Grab")]
public class GrabDefinition : SpellDefinition
{
    public bool preserveInitialDistance = false;
    public float fixedHoldDistance = 3f;
    public float pullGain = 10f;
    public float maxSpeed = 30f;
    public LinkEffect linkEffectPrefab;

    public override ISpellRuntime CreateRuntime()
    {
        return new GrabRuntime();
    }
}
