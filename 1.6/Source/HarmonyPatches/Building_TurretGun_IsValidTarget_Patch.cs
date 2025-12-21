using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(Building_TurretGun), "IsValidTarget")]
    public static class Building_TurretGun_IsValidTarget_Patch
    {
        public static void Postfix(Building_TurretGun __instance, ref bool __result, Thing t)
        {
            if (__result && __instance.def == DefsOf.VFES_Turret_TeslaBlaster)
            {
                __result = t is Pawn pawn && pawn.RaceProps.IsMechanoid && !pawn.Dead && !pawn.Downed;
            }
        }
    }
}