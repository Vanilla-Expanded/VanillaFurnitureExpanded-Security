using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using RimWorld;
using RimWorld.Planet;
using HarmonyLib;

namespace VFESecurity
{

    public class ArtilleryStrikeArrivalAction_Map : ArtilleryStrikeArrivalAction
    {
        public MapParent mapParent;
        public IntVec3 cell;
        public ArtilleryStrikeArrivalAction_Map()
        {
        }

        public ArtilleryStrikeArrivalAction_Map(MapParent mapParent, IntVec3 cell)
        {
            this.mapParent = mapParent;
            this.cell = cell;
        }

        public override void Arrived(List<ActiveArtilleryStrike> artilleryStrikes, int tile)
        {
            // Boom
            var map = mapParent.Map;
            if (map != null)
            {
                var verb = (source as Building_TurretGun).AttackVerb as Verb_LaunchProjectile;
                Thing manningPawn = verb.caster;
                Thing equipmentSource = verb.EquipmentSource;
                CompMannable compMannable = verb.caster.TryGetComp<CompMannable>();
                if (compMannable?.ManningPawn != null)
                {
                    manningPawn = compMannable.ManningPawn;
                    equipmentSource = verb.caster;
                }
                for (int i = 0; i < artilleryStrikes.Count; i++)
                {
                    var strike = artilleryStrikes[i];
                    for (int j = 0; j < strike.shellCount; j++)
                    {
                        float num = verb.verbProps.ForcedMissRadius;
                        if (manningPawn is Pawn pawn)
                        {
                            num *= verb.verbProps.GetForceMissFactorFor(equipmentSource, pawn);
                        }
                        IntVec3 forcedMissTarget = GetForcedMissTarget(cell, num);
                        ArtilleryStrikeUtility.SpawnArtilleryStrikeSkyfaller(strike.shellDef, map, forcedMissTarget);
                    }
                }
            }
            else
                ArtilleryComp.ResetForcedTarget();
        }

        protected IntVec3 GetForcedMissTarget(LocalTargetInfo currentTarget, float forcedMissRadius)
        {
            int maxExclusive = GenRadial.NumCellsInRadius(forcedMissRadius);
            int num = Rand.Range(0, maxExclusive);
            return currentTarget.Cell + GenRadial.RadialPattern[num];
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref mapParent, "mapParent");
            base.ExposeData();
            Scribe_Values.Look(ref cell, "cell");
        }
    }

}
