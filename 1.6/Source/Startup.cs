using RimWorld;
using Verse;

namespace VFESecurity
{
    [StaticConstructorOnStartup]
    public static class Startup
    {
        static Startup()
        {
            var thingDefs = DefDatabase<ThingDef>.AllDefsListForReading;
            for (int i = 0; i < thingDefs.Count; i++)
            {
                var tDef = thingDefs[i];
                if (tDef.IsWithinCategory(ThingCategoryDefOf.StoneChunks))
                {
                    tDef.projectileWhenLoaded = DefsOf.VFES_Bullet_Catapult;
                }
            }
        }
    }
}
