using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFESecurity;

[HarmonyPatch(typeof(GhostUtility), nameof(GhostUtility.GhostGraphicFor))]
public static class GhostUtility_GhostGraphicFor_Patch
{
    // Only patch if those defs are linked graphics? In case some mod reworks it?

    private static bool Prefix(Graphic baseGraphic, ThingDef thingDef, Color ghostCol, ThingDef stuff, ref Graphic __result)
    {
        // Grab the path based on which turret it is
        if (thingDef != DefsOf.VFES_Complex_Hmg && thingDef != DefsOf.VFES_Complex_Minigun && thingDef != DefsOf.VFES_Complex_Charge)
            return true;
        if (!thingDef.graphicData.Linked)
            return true;

        var num = Gen.HashCombine(0, baseGraphic);
        num = Gen.HashCombine(num, thingDef);
        num = Gen.HashCombineStruct(num, ghostCol);
        num = Gen.HashCombine(num, stuff);

        if (!GhostUtility.ghostGraphics.TryGetValue(num, out __result))
        {
            baseGraphic ??= thingDef.graphic;

            // Base graphic of the turret.
            if (baseGraphic == thingDef.graphic)
            {
                // Hardcoded, ugh. May look into un-hardcoding it.
                var path = thingDef == DefsOf.VFES_Complex_Hmg || thingDef == DefsOf.VFES_Complex_Minigun
                    ? "NewThings/Security/ComplexSandbags_Base"
                    : "NewThings/Security/ComplexBarricade_Base";

                GraphicData graphicData = null;
                if (baseGraphic.data != null)
                {
                    graphicData = new GraphicData();
                    graphicData.CopyFrom(baseGraphic.data);
                    graphicData.shadowData = null;
                    graphicData.drawRotated = false;
                    graphicData.allowFlip = false;
                }

                __result = GraphicDatabase.Get<Graphic_Single>(path, ShaderTypeDefOf.EdgeDetect.Shader, thingDef.graphicData.drawSize * 3, ghostCol, Color.white, graphicData);
            }
            // Other graphics that use the same def (like turret tops).
            // Same handling as vanilla ghost graphics, minus the linkable handling.
            else
            {
                GraphicData graphicData = null;
                if (baseGraphic.data != null)
                {
                    graphicData = new GraphicData();
                    graphicData.CopyFrom(baseGraphic.data);
                    graphicData.shadowData = null;
                }

                if (baseGraphic is Graphic_Appearances appearances && stuff != null)
                    __result = GraphicDatabase.Get<Graphic_Single>(appearances.SubGraphicFor(stuff).path, ShaderTypeDefOf.EdgeDetect.Shader, thingDef.graphicData.drawSize, ghostCol, Color.white, graphicData, null);
                else
                    __result = GraphicDatabase.Get(baseGraphic.GetType(), baseGraphic.path, ShaderTypeDefOf.EdgeDetect.Shader, baseGraphic.drawSize, ghostCol, Color.white, graphicData, null, null);
            }

            GhostUtility.ghostGraphics.Add(num, __result);
        }

        return false;
    }
}