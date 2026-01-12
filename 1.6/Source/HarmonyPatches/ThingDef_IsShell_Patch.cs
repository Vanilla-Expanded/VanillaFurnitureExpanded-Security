using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(ThingDef), nameof(ThingDef.IsShell), MethodType.Getter)]
    public static class ThingDef_IsShell_Patch
    {
        public static void Postfix(ref bool __result, ThingDef __instance)
        {
            __result = __result && !__instance.IsWithinCategory(ThingCategoryDefOf.StoneChunks);
        }
    }
}
