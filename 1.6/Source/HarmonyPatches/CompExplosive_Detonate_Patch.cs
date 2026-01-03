using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity;

[HarmonyPatch(typeof(CompExplosive), nameof(CompExplosive.Detonate))]
public class CompExplosive_Detonate_Patch
{
    // Consider making this conditional, in case some mod removes those CompProps and/or the shockmine?

    private static void Prefix(CompExplosive __instance, Map map, bool ignoreUnspawned)
    {
        if (__instance.Props is not CompProperties_ExplosiveExtraDamage props)
            return;

        if (map == null)
            return;

        if (!ignoreUnspawned && !__instance.parent.SpawnedOrAnyParentSpawned)
            return;

        var radius = __instance.ExplosiveRadius();
        if (radius <= 0)
            return;

        var parent = __instance.instigator != null && (!__instance.instigator.HostileTo(__instance.parent.Faction) || __instance.parent.Faction == Faction.OfPlayer)
            ? __instance.instigator
            : __instance.parent;

        GenExplosion.DoExplosion(parent.PositionHeld, map, radius, props.extraExplosiveDamageType, parent, props.extraDamageAmountBase, props.extraArmorPenetrationBase,
            applyDamageToExplosionCellsNeighbors: props.applyDamageToExplosionCellsNeighbors, damageFalloff: props.extraDamageFalloff, chanceToStartFire: props.extraChanceToStartFire,
            ignoredThings: __instance.thingsIgnoredByExplosion, doSoundEffects: false, doVisualEffects: false);
    }
}