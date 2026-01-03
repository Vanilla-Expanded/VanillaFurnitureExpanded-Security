using RimWorld;
using Verse;

namespace VFESecurity;

public class CompProperties_ExplosiveExtraDamage : CompProperties_Explosive
{
    public DamageDef extraExplosiveDamageType;
    public int extraDamageAmountBase;
    public float extraArmorPenetrationBase;
    public float extraChanceToStartFire;
    public bool extraDamageFalloff;
}