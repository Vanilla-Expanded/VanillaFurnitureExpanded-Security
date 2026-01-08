using Verse;

namespace VFESecurity;

public class MoteSpotLight : MoteAttached
{
    public BodyPartDef bodyPart;
    public HediffDef hediff;
    public float radius;
    public IntVec3 parentPos;
    public float maxDistanceSquared;

    public override float Alpha => 0.5f;

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        TryApply();
    }

    public override void Tick()
    {
        base.Tick();

        if (this.IsHashIntervalTick(10))
        {
            if (link1.Linked && parentPos.DistanceToSquared(Position) > maxDistanceSquared)
                link1 = MoteAttachLink.Invalid;

            TryApply();
        }
    }

    private void TryApply()
    {
        if (Map != null)
        {
            foreach (var pawn in RadialUtils.RadialDistinctPawnsAround(Position, Map, radius, true))
            {
                if (pawn.RaceProps.IsFlesh)
                    ApplyHediffToParts(pawn, hediff, bodyPart);
            }
        }
    }

    public override void ExposeData()
    {
        base.ExposeData();

        Scribe_Values.Look(ref radius, "radius", 3.9f);
    }

    // We originally used HediffGiverUtility.TryApply, but it's not the best for what we're doing here.
    // For one, it required us to specify amount of body parts we want to affect (and we want all).
    // It would then iterate over all body parts however many times you specified (or 1 more than the body parts,
    // as it would not find another body part at the +1 check and return early). Our code instead just iterates
    // all body parts once, and applies the hediff to all the body parts we've found.
    private static void ApplyHediffToParts(Pawn pawn, HediffDef hediffDef, BodyPartDef partToAffect)
    {
        foreach (var record in pawn.health.hediffSet.GetNotMissingParts())
        {
            if (record.def == partToAffect)
            {
                Hediff hediff = null;

                foreach (var h in pawn.health.hediffSet.hediffs)
                {
                    if (h.Part == record)
                    {
                        hediff = h;
                        var disappearsComp = hediff.TryGetComp<HediffComp_Disappears>();
                        // Reset
                        if (disappearsComp != null)
                            disappearsComp.ticksToDisappear = disappearsComp.disappearsAfterTicks;
                        break;
                    }
                }

                if (hediff == null)
                {
                    hediff = HediffMaker.MakeHediff(hediffDef, pawn);
                    pawn.health.AddHediff(hediff, record);
                }
            }
        }
    }
}