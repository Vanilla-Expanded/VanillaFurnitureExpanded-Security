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

    public class ArtilleryStrikeArrivalAction_Insectoid : ArtilleryStrikeArrivalAction_Settlement
    {
        private const int RetaliationTicksPerRetaliation = GenDate.TicksPerDay * 8;
        private const int RetaliationTicksPerExtraPointsMultiplier = GenDate.TicksPerDay * 15;
        private static readonly IntRange RaidIntervalRange = new IntRange(GenDate.TicksPerDay / 2, GenDate.TicksPerDay);

        public ArtilleryStrikeArrivalAction_Insectoid()
        {
        }

        public ArtilleryStrikeArrivalAction_Insectoid(MapParent worldObject, Map sourceMap)
        {
            this.mapParent = worldObject;
            this.sourceMap = sourceMap;
        }

        protected override void PostStrikeAction(bool destroyed)
        {
            if (destroyed)
            {
                var artilleryComp = Settlement.GetComponent<ArtilleryComp>();
                var parms = new IncidentParms();
                parms.target = sourceMap;
                parms.points = StorytellerUtility.DefaultThreatPointsNow(sourceMap) * (1 + ((float)artilleryComp.recentRetaliationTicks / RetaliationTicksPerExtraPointsMultiplier));
                parms.faction = Settlement.Faction;
                parms.generateFightersOnly = true;
                parms.forced = true;
                Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + RaidIntervalRange.RandomInRange, parms);
                artilleryComp.recentRetaliationTicks += RetaliationTicksPerRetaliation;
            }
        }

    }

}
