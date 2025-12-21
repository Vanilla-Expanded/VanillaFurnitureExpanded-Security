using RimWorld;
using Verse;

namespace VFESecurity
{
    public class Building_Artillery : Building_TurretGun
    {
        private CompDualMannable dualMannableComp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            dualMannableComp = this.GetComp<CompDualMannable>();
        }

        public override float BurstCooldownTime()
        {
            var result = base.BurstCooldownTime();
            if (dualMannableComp.MannedNowSecondary)
            {
                result *= 0.5f;
            }
            return result;
        }
    }
}
