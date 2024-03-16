using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFESecurity
{
    public class MoteSpotLight : MoteAttached
    {
        protected override bool EndOfLife => AgeSecs > lifespan;
        public override float Alpha => 0.5f;

        public float radius;
        public override void Tick()
        {
            base.Tick();
            if (Map != null)
            {
                foreach (var pawn in GenRadial.RadialDistinctThingsAround(Position, Map, radius, true).OfType<Pawn>())
                {
                    if (pawn.RaceProps.IsFlesh)
                    {
                        HediffGiverUtility.TryApply(pawn, HediffDefOf.VFES_Dazzled, new List<BodyPartDef> { BodyPartDefOf.Eye }, false, 2, null);
                    }
                }
            }
        }
        public override void ExposeData()
        {
            Scribe_Values.Look(ref lifespan, "lifespan");
            base.ExposeData();
        }

        public float lifespan;

    }

}
