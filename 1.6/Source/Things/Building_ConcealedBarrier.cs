using RimWorld;
using Verse;

namespace VFESecurity
{
    public class Building_ConcealedBarrier : Building
    {
        private CompConcealed concealedComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            concealedComp = GetComp<CompConcealed>();
        }


        public override Graphic Graphic
        {
            get
            {
                if (concealedComp != null && concealedComp.Submerged && concealedComp.Props.submergedGraphic != null)
                {
                    return concealedComp.Props.submergedGraphic.Graphic;
                }
                return base.Graphic;
            }
        }
    }
}
