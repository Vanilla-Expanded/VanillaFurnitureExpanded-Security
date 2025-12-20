using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VFESecurity
{
    public class Designator_RearmTrap : Designator
    {
        public Designator_RearmTrap()
        {
            defaultLabel = "VFES_DesignatorRearmTraps".Translate();
            defaultDesc = "VFES_DesignatorRearmTraps_Description".Translate();
            icon = TexCommand.RearmTrap;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
            soundSucceeded = SoundDefOf.Designate_Haul;
        }

        public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.FilledRectangle;
        public override DesignationDef Designation => DefsOf.VFES_RearmTrap;

        public override AcceptanceReport CanDesignateCell(IntVec3 loc)
        {
            // Out of bounds
            if (!loc.InBounds(Map))
                return false;

            // No rearmables
            if (!RearmablesInCell(loc).Any())
                return "VFES_MessageMustDesignateRearmables".Translate();

            return true;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            var thingList = c.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
                DesignateThing(thingList[i]);
        }

        public override void DesignateThing(Thing t)
        {
            Map.designationManager.RemoveAllDesignationsOn(t);
            Map.designationManager.AddDesignation(new Designation(t, Designation));
        }

        private IEnumerable<Thing> RearmablesInCell(IntVec3 c)
        {
            // Out of bounds
            if (!c.InBounds(Map))
                yield break;

            var thingList = c.GetThingList(Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                var thing = thingList[i];
                if (CanDesignateThing(thing).Accepted)
                    yield return thing;
            }
        }

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            var rearmableComp = t.TryGetComp<CompRearmable>();
            return rearmableComp != null && !rearmableComp.armed && Map.designationManager.DesignationOn(t, Designation) == null;
        }
    }
}
