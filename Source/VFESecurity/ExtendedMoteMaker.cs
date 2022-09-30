using UnityEngine;
using Verse;

namespace VFESecurity
{

    public static class ExtendedMoteMaker
    {

        public static Mote SearchlightEffect(Vector3 loc, Map map, float size, float lifespan)
        {
            var mote = (MoteSpotLight)ThingMaker.MakeThing(ThingDefOf.VFES_Mote_SpotLight);
            mote.lifespan = lifespan;
            mote.Scale = 1.9f * size;
            mote.exactPosition = loc;
            GenSpawn.Spawn(mote, loc.ToIntVec3(), map);
            return mote;
        }

    }

}
