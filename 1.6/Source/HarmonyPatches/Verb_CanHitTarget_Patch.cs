using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(Verb), nameof(Verb.CanHitTarget))]
    public static class Verb_CanHitTarget_Patch
    {
        public static void Postfix(Verb __instance, ref bool __result, LocalTargetInfo targ)
        {
            if (__result && __instance.caster is IConcealedBuilding concealed && concealed.ConcealedComp != null && concealed.ConcealedComp.Submerged)
            {
                __result = false;
            }
        }
    }
}
