using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    public class Building_ConcealedTurret : Building_TurretGun, IConcealedBuilding
    {
        public CompConcealed ConcealedComp { get; private set; }

        public override bool IsEverThreat
        {
            get
            {
                if (ConcealedComp != null && ConcealedComp.Submerged)
                {
                    return false;
                }
                return base.IsEverThreat;
            }
        }

        public override void PostMake()
        {
            base.PostMake();
            InitComps();
        }

        public override void ExposeData()
        {
            base.ExposeData();

            if (Scribe.mode == LoadSaveMode.LoadingVars)
                InitComps();
        }

        private void InitComps()
        {
            // Init comps from PostMake and ExposeData (LoadingVars) to handle minified buildings, just in case
            ConcealedComp = GetComp<CompConcealed>();
        }

        public override Graphic Graphic
        {
            get
            {
                if (ConcealedComp != null && ConcealedComp.Submerged && ConcealedComp.Props.submergedGraphic != null)
                {
                    return ConcealedComp.Props.submergedGraphic.Graphic;
                }
                return base.Graphic;
            }
        }
    }
}
