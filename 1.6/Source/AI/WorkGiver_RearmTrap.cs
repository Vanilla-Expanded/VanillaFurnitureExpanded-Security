using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    public class WorkGiver_RearmTrap : WorkGiver_Scanner
    {
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            return pawn.Map.designationManager.SpawnedDesignationsOfDef(DefsOf.VFES_RearmTrap).Select(d => d.target.Thing);
        }

        public override PathEndMode PathEndMode => PathEndMode.Touch;

        public override Danger MaxPathDanger(Pawn pawn) => Danger.Deadly;

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (pawn.Map.designationManager.DesignationOn(t, DefsOf.VFES_RearmTrap) == null)
                return false;
            if (!pawn.CanReserve(t, ignoreOtherReservations: forced))
                return false;
            var thingList = t.Position.GetThingList(t.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                var thing = thingList[i];
                if (thing != t && thing.def.category == ThingCategory.Item && (thing.IsForbidden(pawn) || thing.IsInValidStorage() || !HaulAIUtility.CanHaulAside(pawn, thing, out IntVec3 storeCell)))
                    return false;
            }

            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            var thingList = t.Position.GetThingList(t.Map);
            for (int i = 0; i < thingList.Count; i++)
            {
                var thing = thingList[i];
                if (thing != t && thing.def.category == ThingCategory.Item)
                {
                    var haulAsideJob = HaulAIUtility.HaulAsideJobFor(pawn, thing);
                    if (haulAsideJob != null)
                        return haulAsideJob;
                }
            }

            return JobMaker.MakeJob(DefsOf.VFES_RearmTrap_Job, t);
        }
    }
}
