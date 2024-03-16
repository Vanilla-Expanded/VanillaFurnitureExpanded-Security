using RimWorld;
using Verse;

namespace VFESecurity
{

    public class Bullet_ExplosiveAlt : Bullet
    {
        private ProjectileProperties ProjectileProps => (def.GetModExtension<ExtendedProjectileProperties>() ?? ExtendedProjectileProperties.defaultValues).projectile2;

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            var usedTargetInfo = hitThing ?? new TargetInfo(usedTarget.Cell, Map);
            DoExplosion(usedTargetInfo);
            base.Impact(hitThing, blockedByShield);
        }

        private void DoExplosion(TargetInfo targetInfo)
        {
            var map = Map;
            var props = ProjectileProps;

            if (props.explosionEffect != null)
            {
                var effecter = props.explosionEffect.Spawn();
                effecter.Trigger(targetInfo, targetInfo);
                effecter.Cleanup();
            }

            GenExplosion.DoExplosion(targetInfo.Cell,
                                     map,
                                     props.explosionRadius,
                                     props.damageDef,
                                     launcher,
                                     props.GetDamageAmount(weaponDamageMultiplier),
                                     props.GetArmorPenetration(weaponDamageMultiplier),
                                     props.soundExplode,
                                     equipmentDef,
                                     def,
                                     this.intendedTarget.Thing,
                                     props.postExplosionSpawnThingDef,
                                     props.postExplosionSpawnChance,
                                     props.postExplosionSpawnThingCount,
                                     props.postExplosionGasType,
                                     props.applyDamageToExplosionCellsNeighbors,
                                     props.preExplosionSpawnThingDef,
                                     props.preExplosionSpawnChance,
                                     props.preExplosionSpawnThingCount,
                                     props.explosionChanceToStartFire,
                                     props.explosionDamageFalloff);
        }

    }

}
