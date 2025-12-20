using RimWorld;
using Verse;

namespace VFESecurity
{
    [DefOf]
    public static class DefsOf
    {
        public static ThingDef VFES_Turret_Catapult;
        public static ThingDef VFES_Bullet_Catapult;
        public static ThingDef VFES_BearTrap;
        public static DesignationDef VFES_RearmTrap;
        [DefAlias("VFES_RearmTrap")] public static JobDef VFES_RearmTrap_Job;
        public static RecordDef VFES_TrapsRearmed;
    }
}
