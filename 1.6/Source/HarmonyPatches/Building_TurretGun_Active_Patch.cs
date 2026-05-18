using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(Building_TurretGun), "Active", MethodType.Getter)]
    public static class Building_TurretGun_Active_Patch
    {
        public static void Postfix(Building_TurretGun __instance, ref bool __result)
        {
            if (__result && __instance is IConcealedBuilding concealed && concealed.ConcealedComp != null && concealed.ConcealedComp.Submerged)
                __result = false;
        }
    }
}
