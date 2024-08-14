using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(Pawn_FilthTracker), "Notify_EnteredNewCell")]
    public static class Pawn_FilthTracker_Notify_EnteredNewCell_Patch
    {
        public static void Postfix(Pawn_FilthTracker __instance)
        {
            var terrain = __instance.pawn.Position.GetTerrain(__instance.pawn.Map);
            var extension = terrain.GetModExtension<TerrainDefExtension>();
            if (extension != null && extension.terrainHediff != null)
            {
                var hediff = __instance.pawn.health.AddHediff(extension.terrainHediff) as HediffDependOnTerrain;
                if (hediff != null)
                {
                    hediff.terrain = terrain;
                }
            }
        }
    }

    public class HediffDependOnTerrain : HediffWithComps
    {
        public TerrainDef terrain;

        public override bool ShouldRemove => pawn.Spawned is false || pawn.Position.GetTerrain(pawn.Map) != terrain;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look(ref terrain, "terrain");
        }
    }
}