using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(TurretTop), nameof(TurretTop.DrawTurret))]
    public static class TurretTop_DrawTurret_Patch
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(TurretTop __instance)
        {
            if (__instance.parentTurret is IConcealedBuilding concealed && concealed.ConcealedComp != null && concealed.ConcealedComp.Submerged)
            {
                return false;
            }
            return true;
        }
    }
}
