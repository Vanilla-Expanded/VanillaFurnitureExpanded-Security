using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace VFESecurity
{
    [StaticConstructorOnStartup]
    public static class StaticConstructorClass
    {
        static StaticConstructorClass()
        {
            ArtilleryStrikeUtility.SetCache();

            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var tDef = thingDefs[i];
                if (tDef.IsWithinCategory(ThingCategoryDefOf.StoneChunks))
                {
                    tDef.projectileWhenLoaded = ThingDefOf.VFES_Artillery_Rock;
                }
            }
            var worldObjectDefs = DefDatabase<WorldObjectDef>.AllDefsListForReading;
            foreach (var worldObjectDef in worldObjectDefs)
            {
                if (typeof(Site).IsAssignableFrom(worldObjectDef.worldObjectClass))
                {
                    var compProps = worldObjectDef.comps?.FirstOrDefault(x => x is WorldObjectCompProperties_Artillery);
                    if (compProps is null)
                    {
                        worldObjectDef.comps ??= new List<WorldObjectCompProperties>();
                        worldObjectDef.comps.Add(new WorldObjectCompProperties_Artillery());
                    }
                }
            }
        }
    }

}
