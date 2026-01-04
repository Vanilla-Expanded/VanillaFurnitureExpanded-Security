using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity;

[HarmonyPatch(typeof(Building_TrapDamager), nameof(Building_TrapDamager.SpringSub))]
public static class Building_TrapDamager_SpringSub_Patch
{
    // Only apply if there's a trap with VFES_TrapMeleeHits stat that's not 5 (or is not immutable)? And uses this specific trap type?

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr, ILGenerator generator)
    {
        var declaredLocal = false;
        var patched = 0;
        var localBuilder = generator.DeclareLocal(typeof(float));

        foreach (var ci in instr)
        {
            yield return ci;

            if (!declaredLocal)
            {
                if (ci.opcode == OpCodes.Stloc_3)
                {
                    // Load `this`
                    yield return CodeInstruction.LoadArgument(0);
                    // Call our method
                    yield return CodeInstruction.Call(() => GetTrapHits);
                    // Store the value we got to our new local
                    yield return CodeInstruction.StoreLocal(localBuilder.LocalIndex);

                    declaredLocal = true;
                }
            }
            else if (ci.LoadsConstant(5.0))
            {
                // Replace the current instruction (loading a constant 5.0f) with our own (loading our local value)
                ci.opcode = OpCodes.Ldloc_S;
                ci.operand = localBuilder;

                patched++;
            }
        }

        const int expected = 1;
        if (!declaredLocal)
            Log.Error("[VFES] Failed to add custom local to Building_TrapDamager:SpringSub. Traps hitting more/less than 5 times may deal incorrect damage.");
        else if (patched != expected)
            Log.Error($"[VFES] Patched incorrect amount of trap hits for Building_TrapDamager:SpringSub. Expected: {expected}, patched: {patched}. Traps hitting more/less than 5 times may deal incorrect damage.");
    }

    private static float GetTrapHits(Thing thing) => thing.GetStatValue(DefsOf.VFES_TrapMeleeHits, cacheStaleAfterTicks: 1);
}