using RimWorld;
using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFESecurity
{
    public class CompProperties_PointDefense : CompProperties
    {
        public float interceptionRadius;
        public int interceptionAttemptInterval = 5;

        public CompProperties_PointDefense()
        {
            compClass = typeof(CompPointDefense);
        }
    }
    [HotSwappable]
    public class CompPointDefense : ThingComp
    {
        private CompRefuelable refuelableComp;
        private int ticksUntilNextShot;
        public CompProperties_PointDefense Props => (CompProperties_PointDefense)props;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            refuelableComp = parent.GetComp<CompRefuelable>();
        }

        public override void CompTick()
        {
            var turret = parent as Building_TurretGun;
            if (!turret.Active)
            {
                return;
            }

            if (ticksUntilNextShot > 0)
            {
                ticksUntilNextShot--;
            }

            if (ticksUntilNextShot <= 0)
            {
                ticksUntilNextShot += Props.interceptionAttemptInterval;
                if (refuelableComp.HasFuel)
                {
                    var target = FindTarget();
                    if (target == null)
                    {
                        return;
                    }
                    DefsOf.Gun_MiniTurret.verbs[0].soundCast.PlayOneShot(new TargetInfo(parent.Position, parent.Map));
                    refuelableComp.ConsumeFuel(1);
                    FleckMaker.Static(parent.Position, parent.Map, FleckDefOf.ShotFlash, 9);
                    TryIntercept(target);
                    turret.Top.CurRotation = (target.DrawPos - parent.DrawPos).AngleFlat();
                }
            }
        }

        private Thing FindTarget()
        {
            var allThings = parent.Map.listerThings.ThingsInGroup(ThingRequestGroup.Projectile).Where(IsValidProjectile)
                .Concat(parent.Map.listerThings.ThingsInGroup(ThingRequestGroup.ActiveTransporter).Where(IsValidTransporter));
            var thing = allThings.OrderBy(t => t.DrawPos.ToIntVec3().DistanceToSquared(parent.Position)).FirstOrDefault();
            if (thing != null && thing.DrawPos.ToIntVec3().DistanceToSquared(parent.Position) <= Props.interceptionRadius * Props.interceptionRadius)
                return thing;
            return null;
        }

        private bool IsValidProjectile(Thing t)
        {
            return t is Projectile projectile && projectile.def.projectile.explosionRadius > 0 && projectile.launcher.HostileTo(parent.Faction);
        }

        private bool IsValidTransporter(Thing t)
        {
            if (t is DropPodIncoming dropPod)
            {
                var allPawns = dropPod.innerContainer.Where(thing => thing is Pawn).Cast<Pawn>().ToList();
                var transporters = dropPod.innerContainer.Where(thing => thing is ActiveTransporter).Cast<ActiveTransporter>().ToList();
                foreach (var transporter in transporters)
                {
                    var transporterPawns = transporter.Contents.innerContainer.Where(thing => thing is Pawn).Cast<Pawn>().ToList();
                    allPawns.AddRange(transporterPawns);
                }
                var hostilePawns = allPawns.Where(pawn => pawn.HostileTo(parent.Faction)).ToList();
                if (hostilePawns.Any())
                {
                    return true;
                }
            }

            return false;
        }

        private void TryIntercept(Thing target)
        {
            if (InterceptChance(target))
            {
                Effecter eff = new Effecter(EffecterDefOf.Interceptor_BlockedProjectile);
                eff.Trigger(parent, new TargetInfo(target.Position, parent.Map));
                eff.Cleanup();
                FleckMaker.ThrowSmoke(target.Position.ToVector3(), parent.Map, 1.2f);
                if (target is DropPodIncoming)
                {
                    GenSpawn.Spawn(ThingDefOf.ChunkSlagSteel, target.Position, parent.Map);
                }
                target.Destroy(DestroyMode.Vanish);
            }
        }

        private bool InterceptChance(Thing target)
        {
            bool success = false;
            if (target is Projectile projectile)
            {
                float velocity = projectile.def.projectile.speed;
                float chance = 0.98f * (float)Math.Exp(-Math.Max(0, velocity - 30) / 10f);
                chance = Mathf.Clamp(chance, 0.05f, 0.98f);
                if (Rand.Chance(chance))
                {
                    success = true;
                }
            }
            else if (target is IActiveTransporter)
            {
                float chance = 0.1f;
                if (Rand.Chance(chance))
                {
                    success = true;
                }
            }
            return success;
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            base.PostDrawExtraSelectionOverlays();
            GenDraw.DrawRadiusRing(parent.Position, Props.interceptionRadius);
        }
    }
}
