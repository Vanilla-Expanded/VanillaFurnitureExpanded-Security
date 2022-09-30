using Verse;

namespace VFESecurity
{

    public class MoteSpotLight : Mote
    {

        protected override bool EndOfLife => AgeSecs > lifespan;

        public override float Alpha => 0.5f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref lifespan, "lifespan");
            base.ExposeData();
        }

        public float lifespan;

    }

}
