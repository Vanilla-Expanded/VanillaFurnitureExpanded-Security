using RimWorld;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using Verse;
using System.Linq;
using UnityEngine;

namespace VFESecurity
{
    [HarmonyPatch(typeof(LordToil_Siege), "SetAsBuilder")]
    public static class LordToil_Siege_SetAsBuilder_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            foreach (var instruction in codeInstructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldloca_S, 1);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(LordToil_Siege_SetAsBuilder_Patch), "SetMinLevels"));
                }
            }
        }

        public static void SetMinLevels(ref int minLevel, LordToil_Siege __instance)
        {
            minLevel = Mathf.Max(minLevel, 
                __instance.Data.blueprints.Select(x => x.def.entityDefToBuild.constructionSkillPrerequisite).Max());
        }
    }
}
