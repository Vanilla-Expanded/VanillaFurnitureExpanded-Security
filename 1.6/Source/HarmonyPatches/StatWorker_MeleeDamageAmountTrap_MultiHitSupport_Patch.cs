using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity;

[HarmonyPatch]
public static class StatWorker_MeleeDamageAmountTrap_MultiHitSupport_Patch
{
    // Only apply if there's a trap with VFES_TrapMeleeHits stat that's not 5 (or is not immutable)?

    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return typeof(StatWorker_MeleeDamageAmountTrap).DeclaredMethod(nameof(StatWorker_MeleeDamageAmountTrap.GetValueUnfinalized));
        yield return typeof(StatWorker_MeleeDamageAmountTrap).DeclaredMethod(nameof(StatWorker_MeleeDamageAmountTrap.GetExplanationUnfinalized));
    }

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr, MethodBase baseMethod)
    {
        var patched = 0;

        foreach (var ci in instr)
        {
            yield return ci;

            if (ci.LoadsConstant(5.0))
            {
                // Load the 1st argument/StatRequest (the 0th one is `this`, since non-static method)
                yield return CodeInstruction.LoadArgument(1);
                // Call our method
                yield return CodeInstruction.Call(() => ReturnActualHitAmount);

                patched++;
            }
        }

        const int expected = 1;
        if (patched != expected)
            Log.Error($"[VFES] Patched incorrect amount of trap hits for {baseMethod.DeclaringType?.Name}:{baseMethod.Name}. Expected: {expected}, patched: {patched}. Traps hitting more/less than 5 times may deal incorrect damage.");
    }

    internal static float ReturnActualHitAmount(float defaultHits, StatRequest request)
    {
        // If the thing is not null, return its stat value. Otherwise, return default of 5.
        return request.Thing?.GetStatValue(DefsOf.VFES_TrapMeleeHits, cacheStaleAfterTicks: 1) ?? defaultHits;
    }
}