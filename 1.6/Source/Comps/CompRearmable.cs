using UnityEngine;
using Verse;
using Verse.Sound;

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
    public class CompRearmable : ThingComp
    {
        public CompProperties_Rearmable Props => (CompProperties_Rearmable)props;
        
        public Graphic UnarmedGraphic
        {
            get
            {
                if (Props.unarmedGraphicData != null)
                {
                    if (cachedUnarmedGraphic == null)
                        cachedUnarmedGraphic = Props.unarmedGraphicData.GraphicColoredFor(parent);
                    return cachedUnarmedGraphic;
                }
                return null;
            }
        }

        public override string CompInspectStringExtra()
        {
            return (armed ? "VFES_Armed" : "VFES_NeedsRearming").Translate();
        }

        public void Rearm()
        {
            armed = true;
            Props.rearmSound.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            armed = Props.initiallyArmed;
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref armed, "armed");
            base.PostExposeData();
        }

        public bool armed;

        [Unsaved]
        private Graphic cachedUnarmedGraphic;
    }
}
