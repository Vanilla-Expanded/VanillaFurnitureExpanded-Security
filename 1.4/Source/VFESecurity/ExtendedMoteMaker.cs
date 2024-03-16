using Verse;

namespace VFESecurity
{

    public static class ExtendedMoteMaker
    {
        public static Mote SearchlightEffect(Thing target, Map map, float size, float lifespan)
        {
            var mote = (MoteSpotLight)ThingMaker.MakeThing(ThingDefOf.VFES_Mote_SpotLight);
            mote.lifespan = lifespan;
            mote.radius = size;
            mote.Attach(target);
            mote.Scale = 1.9f * size;
            mote.exactPosition = target.DrawPos;
            GenSpawn.Spawn(mote, target.Position, map);
            return mote;
        }
    }
}
