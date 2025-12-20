using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    public class Building_ConcealedTurret : Building_TurretGun
    {
        private CompConcealed concealedComp;

        public override bool IsEverThreat
        {
            get
            {
                var comp = GetComp<CompConcealed>();
                if (comp != null && comp.Submerged)
                {
                    return false;
                }
                return base.IsEverThreat;
            }
        }

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
