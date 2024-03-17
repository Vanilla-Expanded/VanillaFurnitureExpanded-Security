using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFESecurity
{

    public abstract class ArtilleryStrikeArrivalAction_MapParent : ArtilleryStrikeArrivalAction
    {

        protected abstract bool CanDoArriveAction
        {
            get;
        }

        public override void Arrived(List<ActiveArtilleryStrike> artilleryStrikes, int tile)
        {
            // Boom
            if (CanDoArriveAction)
            {
                var harmfulStrikes = artilleryStrikes.Where(s => s.shellDef.projectile.damageDef.harmsHealth);
                if (harmfulStrikes.Any())
                {
                    PreStrikeAction();
                    bool destroyed = false;
                    var strikeList = harmfulStrikes.ToList();
                    for (int i = 0; i < strikeList.Count; i++)
                    {
                        var strike = strikeList[i];
                        StrikeAction(strike, ref destroyed);
                    }
                        
                    PostStrikeAction(destroyed);
                }
            }
        }

        protected virtual void PreStrikeAction()
        {
        }

        protected void StrikeAction(ActiveArtilleryStrike strike, ref bool destroyed)
        {
            for (int j = 0; j < strike.shellCount; j++)
            {
                if (mapParent.HasMap)
                {
                    ArtilleryStrikeUtility.SpawnArtilleryStrikeSkyfaller(strike.shellDef, mapParent.Map, mapParent.Map.AllCells.RandomElement());
                }
                else
                {
                    var comp = mapParent.GetComponent<ArtilleryComp>();
                    var projectile = strike.shellDef.projectile;
                    if (projectile.damageDef != null && projectile.damageDef.harmsHealth)
                    {
                        var shootingLevel = Mathf.Lerp(1f, 5f, (float)strike.manningPawn.skills.GetSkill(SkillDefOf.Shooting).Level / 20f);
                        var damageToDeal = projectile.explosionRadius * shootingLevel;
                        comp.damageTaken += damageToDeal;
                        if (comp.damageTaken >= comp.MaxDamageToDestroy)
                        {
                            destroyed = true;
                            Messages.Message("VFESecurity.ArtileryHitDestroyedMessage".Translate(mapParent.Label), mapParent, MessageTypeDefOf.NeutralEvent);
                            return;
                        }
                        else
                        {
                            Messages.Message("VFESecurity.ArtileryHitMessage".Translate(mapParent.Label, comp.DestructionPct.ToStringPercent()), mapParent, MessageTypeDefOf.NeutralEvent);
                        }
                    }
                }
            }
        }

        protected virtual void PostStrikeAction(bool destroyed)
        {
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref mapParent, "mapParent");
            Scribe_References.Look(ref sourceMap, "sourceMap");
            base.ExposeData();
        }

        protected MapParent mapParent;
        protected Map sourceMap;

    }

}
