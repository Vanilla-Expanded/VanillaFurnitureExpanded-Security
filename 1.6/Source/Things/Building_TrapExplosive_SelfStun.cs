using RimWorld;
using Verse;

namespace VFESecurity;

public class Building_TrapExplosive_SelfStun : Building_TrapExplosive
{
    private SelfStunTrapExtension selfStunTrapExtension;

    public override void SpringSub(Pawn p)
    {
        base.SpringSub(p);
        
        if (selfStunTrapExtension == null)
            stunner.StunFor(10000, null, false, false);
        else
            stunner.StunFor(selfStunTrapExtension.stunTicks, null, false, selfStunTrapExtension.showMote);
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        // Initialize stunner
        _ = IsStunned;
    }

    public override void PostMake()
    {
        base.PostMake();

        selfStunTrapExtension = def.GetModExtension<SelfStunTrapExtension>();
    }

    public override void ExposeData()
    {
        base.ExposeData();

        if (Scribe.mode == LoadSaveMode.LoadingVars)
            selfStunTrapExtension = def.GetModExtension<SelfStunTrapExtension>();
    }
}