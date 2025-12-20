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
            if (__result)
            {
                var comp = __instance.GetComp<CompConcealed>();
                if (comp != null && comp.Submerged)
                {
                    __result = false;
                }
            }
        }
    }
}
