using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(DefGenerator), "GenerateImpliedDefs_PreResolve")]
    public static class DefGenerator_GenerateImpliedDefs_PreResolve_Patch
    {
        public static bool generated;
        public static Dictionary<ThingDef, ThingDef> shellProjectileMap = new Dictionary<ThingDef, ThingDef>();
        public static void Prefix(bool hotReload)
        {
            if (!generated)
            {
                generated = true;
                foreach (var shell in DefDatabase<ThingDef>.AllDefs
                    .Where(td => td.IsShell).ToList())
                {
                    var projectile = shell.projectileWhenLoaded;
                    var newProjectileDef = new ThingDef
                    {
                        defName = "VFES_WorldArtillery_" + projectile.defName,
                        label = projectile.label,
                        description = projectile.description,
                        thingClass = typeof(Projectile_Artillery),
                        category = projectile.category,
                        altitudeLayer = projectile.altitudeLayer,
                        neverMultiSelect = projectile.neverMultiSelect,
                        useHitPoints = projectile.useHitPoints,
                        tickerType = projectile.tickerType,
                        graphicData = projectile.graphicData,
                        projectile = projectile.projectile
                    };
                    newProjectileDef.PostLoad();
                    DefGenerator.AddImpliedDef(newProjectileDef, hotReload);
                    shellProjectileMap[projectile] = newProjectileDef;
                }
            }
        }
    }
}
