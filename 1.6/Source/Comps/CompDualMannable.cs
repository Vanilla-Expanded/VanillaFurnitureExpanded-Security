using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    public class CompProperties_DualMannable : CompProperties_Mannable
    {
        public IntVec3 secondaryInteractionCellOffset;

        public CompProperties_DualMannable()
        {
            compClass = typeof(CompDualMannable);
        }
    }
    
    [HotSwappable]
    public class CompDualMannable : CompMannable
    {
        public new CompProperties_DualMannable Props => (CompProperties_DualMannable)props;

        public IntVec3 SecondaryInteractionCell => parent.Position + Props.secondaryInteractionCellOffset.RotatedBy(parent.Rotation);

        private int lastManTickSecondary = -1;
        private Pawn lastManPawnSecondary;

        public bool MannedNowSecondary
        {
            get
            {
                if (Find.TickManager.TicksGame - lastManTickSecondary <= 1 && lastManPawnSecondary != null)
                {
                    return lastManPawnSecondary.Spawned;
                }
                return false;
            }
        }

        public Pawn ManningPawnSecondary
        {
            get
            {
                if (!MannedNowSecondary)
                {
                    return null;
                }
                return lastManPawnSecondary;
            }
        }

        public void ManForATickSecondary(Pawn pawn)
        {
            lastManTickSecondary = Find.TickManager.TicksGame;
            lastManPawnSecondary = pawn;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref lastManPawnSecondary, "lastManPawnSecondary");
            Scribe_Values.Look(ref lastManTickSecondary, "lastManTickSecondary", -1);
        }

        public override void PostDraw()
        {
            base.PostDraw();
            if (Find.Selector.IsSelected(parent))
            {
                GenDraw.DrawInteractionCell(parent.def, Props.secondaryInteractionCellOffset, parent.Position, parent.Rotation);
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn pawn)
        {
            foreach (var option in base.CompFloatMenuOptions(pawn))
            {
                yield return option;
            }

            var secondaryCell = SecondaryInteractionCell;
            if (pawn.RaceProps.ToolUser && pawn.CanReach(secondaryCell, PathEndMode.OnCell, Danger.Deadly))
            {
                if (Props.manWorkType != 0 && pawn.WorkTagIsDisabled(Props.manWorkType))
                {

                }
                else
                {
                    yield return new FloatMenuOption("OrderManThing".Translate(parent.LabelShort, parent) + "VFES_ManSecondary".Translate(), delegate
                    {
                        var job = JobMaker.MakeJob(DefsOf.VFES_ManSecondary, parent, secondaryCell);
                        pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                    });
                }
            }
        }
    }
}
