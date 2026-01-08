using RimWorld;
using Verse;

namespace VFESecurity
{
    [DefOf]
    public static class DefsOf
    {
        public static ThingDef VFES_Turret_Catapult;
        public static ThingDef VFES_Bullet_Catapult;
        public static DesignationDef VFES_RearmTrap;
        [DefAlias("VFES_RearmTrap")] public static JobDef VFES_RearmTrap_Job;
        public static RecordDef VFES_TrapsRearmed;
        public static ThingDef VFES_Turret_AntiAir;
        public static ThingDef VFES_Turret_TeslaBlaster;
        public static ThingDef Gun_MiniTurret;
        public static JobDef VFES_ManSecondary;
        public static WorldObjectDef VFES_ArtilleryProjectile;
        public static StatDef VFES_TrapMeleeHits;
        public static ThingDef VFES_Complex_Charge;
        public static ThingDef VFES_Complex_Minigun;
        public static ThingDef VFES_Complex_Hmg;
        public static HediffDef VFES_Dazzled;
        public static ThingDef VFES_Mote_SpotLight;
    }
}
