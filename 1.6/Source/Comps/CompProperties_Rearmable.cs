using UnityEngine;
using Verse;

namespace VFESecurity
{
    public class CompProperties_Rearmable : CompProperties
    {
        public CompProperties_Rearmable()
        {
            compClass = typeof(CompRearmable);
        }

        public bool initiallyArmed;
        public GraphicData unarmedGraphicData;
        public int workToRearm;
        public SoundDef rearmSound;
    }
}