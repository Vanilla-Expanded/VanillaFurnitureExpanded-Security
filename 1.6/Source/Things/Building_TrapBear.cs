using System;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using RimWorld;

namespace VFESecurity
{
    public class Building_TrapBear : Building_TrapDamager
    {
        private static readonly FloatRange BearTrapDamageRandomFactorRange = new FloatRange(0.8f, 1.2f);
        private static readonly FloatRange AffectableBodySizeRange = new FloatRange(0.25f, 2.99f);
        private const float LowerHeightBodySizeThreshold = 0.35f;

        public override Graphic Graphic
        {
            get
            {
                if (rearmableComp.armed || rearmableComp.UnarmedGraphic == null)
                    return base.Graphic;
                return rearmableComp.UnarmedGraphic;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            rearmableComp = GetComp<CompRearmable>();
        }

        public override float SpringChance(Pawn p)
        {
            float num = 1f;
            if (p.kindDef.immuneToTraps)
            {
                return 0f;
            }
            if (p.BodySize <= 0.25f)
            {
                return 0f;
            }
            if (p.Faction == null && p.IsAnimal && !p.InAggroMentalState)
            {
                num = 1.0f;
            }
            else if (KnowsOfTrap(p))
            {
                if (p.Faction != null)
                {
                    num = ((p.Faction != base.Faction) ? 0f : 0.005f);
                }
                else if (p.IsAnimal)
                {
                    num = 0.2f;
                    num *= def.building.trapPeacefulWildAnimalsSpringChanceFactor;
                }
                else
                {
                    num = 0.3f;
                }
            }
            num *= this.GetStatValue(StatDefOf.TrapSpringChance) * p.GetStatValue(StatDefOf.PawnTrapSpringChance);
            return Mathf.Clamp01(num);
        }

        public override void SpringSub(Pawn p)
        {
            if (rearmableComp.armed)
            {
                SoundDefOf.TrapSpring.PlayOneShot(new TargetInfo(Position, Map));
                rearmableComp.armed = false;
                Map.mapDrawer.SectionAt(Position).RegenerateAllLayers();
                if (!def.building.trapDestroyOnSpring && this.autoRearm)
                    Map.designationManager.AddDesignation(new Designation(this, DefsOf.VFES_RearmTrap));
                if (p == null || !AffectableBodySizeRange.Includes(p.BodySize))
                    return;
                p.stances.stagger.StaggerFor(90);

                var partHeight = p.BodySize >= LowerHeightBodySizeThreshold ? BodyPartHeight.Bottom : BodyPartHeight.Undefined;
                for (int i = 0; (float)i < 5f; i++)
                {
                    float damage = this.GetStatValue(RimWorld.StatDefOf.TrapMeleeDamage, true) * BearTrapDamageRandomFactorRange.RandomInRange;
                    float armourPen = damage * VerbProperties.DefaultArmorPenetrationPerDamage;
                    BodyPartRecord hitPart = p.health.hediffSet.GetRandomNotMissingPart(DamageDefOf.Stab, partHeight);
                    var dinfo = new DamageInfo(DamageDefOf.Stab, damage, armourPen, instigator: this, hitPart: hitPart);
                    var damResult = p.TakeDamage(dinfo);
                    if (i == 0)
                    {
                        BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(p, RulePackDefOf.DamageEvent_TrapSpike);
                        Find.BattleLog.Add(battleLogEntry_DamageTaken);
                        damResult.AssociateWithLog(battleLogEntry_DamageTaken);
                    }
                }
            }
        }

        private CompRearmable rearmableComp;
    }
}