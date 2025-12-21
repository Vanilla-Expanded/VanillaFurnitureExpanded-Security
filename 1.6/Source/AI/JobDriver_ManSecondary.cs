using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    public class JobDriver_ManSecondary : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetB, job, 1, 0, null, errorOnFailed);
        }

        public override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);

            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);

            var man = new Toil();
            man.tickAction = delegate
            {
                job.targetA.Thing.TryGetComp<CompDualMannable>().ManForATickSecondary(pawn);
                pawn.rotationTracker.FaceTarget(job.targetA);
            };
            man.handlingFacing = true;
            man.defaultCompleteMode = ToilCompleteMode.Never;
            yield return man;
        }
    }
}
