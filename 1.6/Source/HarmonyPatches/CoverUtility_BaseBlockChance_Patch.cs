using HarmonyLib;
using System;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(CoverUtility), nameof(CoverUtility.BaseBlockChance), new Type[] { typeof(Thing) })]
    public static class CoverUtility_BaseBlockChance_Patch
    {
        public static void Postfix(Thing thing, ref float __result)
        {
            if (thing is Building building)
            {
                var comp = building.GetComp<CompConcealed>();
                if (comp != null && comp.Submerged)
                {
                    __result = 0f;
                }
            }
        }
    }
}
