using RimWorld;
using Verse;

namespace VFESecurity;

public class Verb_Dazzle : Verb
{
    public DazzleVerbProperties DazzleVerbProperties => EquipmentSource.def.GetModExtension<DazzleVerbProperties>();

    public Mote prevMote;

    public override bool CanHitTarget(LocalTargetInfo targ)
    {
        // Target and caster are on different maps
        if (targ.Thing is not Pawn || targ.Thing?.Map != caster.Map)
            return false;

        // No LoS
        if (verbProps.stopBurstWithoutLos && !GenSight.LineOfSight(caster.Position, targ.Cell, caster.Map))
            return false;

        return base.CanHitTarget(targ);
    }

    public override bool TryCastShot()
    {
        // Only create a new mote if it doesn't exist yet
        if (prevMote is { Spawned: true })
        {
            if (CurrentTarget.Thing != prevMote.link1.Target.Thing)
                prevMote.Attach(CurrentTarget.Thing);
            prevMote.ForceSpawnTick(Find.TickManager.TicksGame);

            return true;
        }

        // Throw mote at target cell
        var props = DazzleVerbProperties;
        prevMote = SearchlightEffect(
            props?.moteDef,
            props?.hediffDef,
            props?.bodyPartDef,
            currentTarget.Thing,
            Caster.Map,
            Caster.Position,
            props?.illuminatedRadius ?? 3.9f,
            EffectiveRange);
        return true;
    }

    public void Cleanup()
    {
        if (prevMote is { Spawned: true })
            prevMote?.Destroy();
        prevMote = null;
    }

    public static Mote SearchlightEffect(ThingDef moteDef, HediffDef hediffDef, BodyPartDef bodyPartDef, Thing target, Map map, IntVec3 parentPos, float size, float maxDistance)
    {
        var mote = (MoteSpotLight)ThingMaker.MakeThing(moteDef ?? DefsOf.VFES_Mote_SpotLight);
        mote.hediff = hediffDef ?? DefsOf.VFES_Dazzled;
        mote.bodyPart = bodyPartDef ?? BodyPartDefOf.Eye;
        mote.parentPos = parentPos;
        mote.maxDistanceSquared = maxDistance * maxDistance;
        mote.radius = size;
        mote.Attach(target);
        mote.Scale = 1.9f * size;
        mote.exactPosition = target.DrawPos;
        GenSpawn.Spawn(mote, target.Position, map);
        return mote;
    }
}