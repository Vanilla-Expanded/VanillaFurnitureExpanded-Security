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
            var comp = __instance.parentTurret.GetComp<CompConcealed>();
            if (comp != null && comp.Submerged)
            {
                return false;
            }
            return true;
        }
    }
}
