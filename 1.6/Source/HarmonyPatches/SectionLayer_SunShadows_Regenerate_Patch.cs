using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(SectionLayer_SunShadows), nameof(SectionLayer_SunShadows.Regenerate))]
    public static class SectionLayer_SunShadows_Regenerate_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            var staticSunShadowHeightField = AccessTools.Field(typeof(ThingDef), nameof(ThingDef.staticSunShadowHeight));
            var thingDefField = AccessTools.Field(typeof(Thing), nameof(Thing.def));
            var helperMethod = AccessTools.Method(typeof(SectionLayer_SunShadows_Regenerate_Patch), nameof(GetAdjustedHeightForBuilding));

            for (int i = 0; i < codes.Count; i++)
            {
                if (i + 2 < codes.Count && codes[i].IsLdloc() && codes[i + 1].LoadsField(thingDefField) && codes[i + 2].LoadsField(staticSunShadowHeightField))
                {
                    var loadBuildingInstruction = codes[i];

                    yield return loadBuildingInstruction.Clone();
                    yield return new CodeInstruction(OpCodes.Call, helperMethod);

                    i += 2;
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        public static float GetAdjustedHeightForBuilding(Building building)
        {
            if (building != null)
            {
                var comp = building.GetComp<CompConcealed>();
                if (comp != null && comp.Submerged)
                {
                    return 0f;
                }
                return building.def.staticSunShadowHeight;
            }
            return 0f;
        }
    }
}