using RimWorld;
using Verse;

namespace VFESecurity;

public class CompStunnableAsCooldown : CompStunnable
{
    public override string CompInspectStringExtra()
    {
        if (!stunHandler.Stunned)
            return null;
        return "VFES_OnCooldown".Translate(stunHandler.StunTicksLeft.ToStringSecondsFromTicks());
    }
}