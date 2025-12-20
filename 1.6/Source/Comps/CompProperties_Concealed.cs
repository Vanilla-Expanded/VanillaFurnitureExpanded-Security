using Verse;

namespace VFESecurity
{
    public class CompProperties_Concealed : CompProperties
    {
        public int submergeSeconds;
        public int deploySeconds;
        public int cooldownSeconds;
        public GraphicData submergedGraphic;
        public CompProperties_Concealed()
        {
            compClass = typeof(CompConcealed);
        }
    }
}
