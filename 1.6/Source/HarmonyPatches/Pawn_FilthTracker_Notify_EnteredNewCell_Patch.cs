using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity;

[HarmonyPatch(typeof(Pawn_FilthTracker), nameof(Pawn_FilthTracker.Notify_EnteredNewCell))]
public static class Pawn_FilthTracker_Notify_EnteredNewCell_Patch
{
    private static void Postfix(Pawn ___pawn)
    {
        if (___pawn.Flying || ___pawn.health == null)
            return;

        var edifice = ___pawn.Position.GetEdifice(___pawn.Map)?.def;
        var hediffDef = edifice?.GetModExtension<HediffOnBuildingExtension>()?.hediff;
        if (hediffDef == null)
            return;

        if (___pawn.health.GetOrAddHediff(hediffDef) is HediffDependsOnBuilding hediff)
            hediff.building = edifice;
        // The current hediff from different buildings (if present) will remove itself soon
    }
}