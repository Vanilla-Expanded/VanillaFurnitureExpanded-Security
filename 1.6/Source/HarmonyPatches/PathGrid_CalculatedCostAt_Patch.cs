using System.Collections.Generic;
using HarmonyLib;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    [HarmonyPatch(typeof(PathGrid), nameof(PathGrid.CalculatedCostAt))]
    public static class PathGrid_CalculatedCostAt_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            var matcher = new CodeMatcher(instr);

            // Find "thing.def.pathCost;"
            matcher.MatchEndForward(
                CodeMatch.LoadsLocal(),
                CodeMatch.LoadsField(typeof(Thing).DeclaredField(nameof(Thing.def))),
                CodeMatch.LoadsField(typeof(BuildableDef).DeclaredField(nameof(BuildableDef.pathCost)))
            );

            // Wrap the "thing.def.pathCost" with our method and insert the Thing argument
            matcher.InsertAfter(
                // Clone the instruction loading the Thing argument
                matcher.InstructionAt(-2).Clone(),
                CodeInstruction.Call(() => ModifyPathCostIfConcealed)
            );

            return matcher.Instructions();
        }

        private static int ModifyPathCostIfConcealed(int pathCost, Thing thing)
        {
            // Thing is IConcealedBuilding, has a non-null comp, and is submerged
            if (thing is IConcealedBuilding { ConcealedComp.Submerged: true })
                return 0;
            return pathCost;
        }
    }
}
