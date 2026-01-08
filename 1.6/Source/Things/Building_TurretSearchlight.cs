using RimWorld;
using Verse;

namespace VFESecurity;

public class Building_TurretSearchlight : Building_TurretGun
{
    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        if (respawningAfterLoad && CurrentTarget.HasThing && AttackVerb is Verb_Dazzle dazzle && dazzle.CanHitTarget(CurrentTarget))
            dazzle.TryCastShot();
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        base.DeSpawn(mode);

        if (AttackVerb is Verb_Dazzle dazzle)
            dazzle.Cleanup();
    }

    public override void DrawExtraSelectionOverlays()
    {
        var warmupTicks = burstWarmupTicksLeft;
        if (!AttackVerb.verbProps.drawAimPie)
            burstWarmupTicksLeft = 0;

        try
        {
            base.DrawExtraSelectionOverlays();
        }
        finally
        {
            burstWarmupTicksLeft = warmupTicks;
        }
    }
}