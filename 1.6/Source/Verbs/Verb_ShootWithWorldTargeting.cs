using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace VFESecurity
{
    [HotSwappable]
    public class Verb_ShootWithWorldTargeting : Verb_LaunchProjectile
    {
        public override int ShotsPerBurst => base.BurstShotCount;
        public Building_Artillery Turret => (Building_Artillery)caster;
        public override ThingDef Projectile
        {
            get
            {
                var originalProjectile = base.Projectile;
                if (originalProjectile != null && DefGenerator_GenerateImpliedDefs_PreResolve_Patch.shellProjectileMap.TryGetValue(originalProjectile, out var ourProjectile))
                {
                    return ourProjectile;
                }
                return originalProjectile;
            }
        }
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            var casterPawn = (caster as Building_Artillery).mannableComp.ManningPawn;
            if (casterPawn == null || casterPawn.skills == null) return;
            if (currentTarget.Thing is Pawn { Downed: false, IsColonyMech: false } pawn)
            {
                float num = (pawn.HostileTo(caster) ? 170f : 20f);
                float num2 = verbProps.AdjustedFullCycleTime(this, casterPawn);
                casterPawn.skills.Learn(SkillDefOf.Shooting, num * num2);
            }
        }

        public override bool TryCastShot()
        {
            var target = CurrentTarget;
            if (target.Cell.InBounds(caster.Map) is false)
            {
                var turret = caster as Building_Artillery;
                var comp = caster.TryGetComp<CompWorldArtillery>();
                if (comp.worldTarget.IsValid is false || comp.worldTarget.WorldObject.Destroyed || comp.worldTarget.WorldObject is not MapParent mapParent || mapParent.HasMap is false)
                {
                    turret.ResetForcedTarget();
                    return false;
                }
                ThingDef projectile = Projectile;

                if (base.EquipmentSource != null)
                {
                    base.EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
                    base.EquipmentSource.GetComp<CompApparelVerbOwner_Charged>()?.UsedOnce();
                }
                ShootLine resultingLine;
                TryFindShootLineFromTo(caster.Position, currentTarget, out resultingLine);
                Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, resultingLine.Source, caster.Map);
                ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
                Vector3 drawPos = caster.DrawPos;
                Thing equipmentSource = base.EquipmentSource;
                projectile2.Launch(turret, drawPos, resultingLine.Dest, currentTarget, projectileHitFlags4, preventFriendlyFire, equipmentSource, null);
                return true;
            }
            else
            {
                bool num = base.TryCastShot();
                if (num && CasterIsPawn)
                {
                    CasterPawn.records.Increment(RecordDefOf.ShotsFired);
                }
                return num;
            }
        }
    }
}
