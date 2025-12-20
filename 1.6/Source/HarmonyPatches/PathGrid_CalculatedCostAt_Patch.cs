using HarmonyLib;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    [HarmonyPatch(typeof(PathGrid), nameof(PathGrid.CalculatedCostAt))]
    public static class PathGrid_CalculatedCostAt_Patch
    {
        public static void Postfix(PathGrid __instance, ref int __result, IntVec3 c, bool perceivedStatic, IntVec3 prevCell)
        {
            var things = __instance.map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i] is Building building)
                {
                    var comp = building.GetComp<CompConcealed>();
                    if (comp != null && comp.Submerged)
                    {
                        __result = 0;
                        return;
                    }
                }
            }
        }
    }
}
