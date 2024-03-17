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
using HarmonyLib;

namespace VFESecurity
{

    public class ActiveArtilleryStrike : Thing
    {

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref missRadius, "missRadius");
            Scribe_Defs.Look(ref shellDef, "shellDef");
            Scribe_Values.Look(ref shellCount, "count");
            Scribe_References.Look(ref manningPawn, "manningPawn");
        }

        public override void Tick()
        {
        }

        public float missRadius;
        public ThingDef shellDef;
        public int shellCount;
        public Pawn manningPawn;

        public float Speed => shellDef.projectile.speed;
    }
}
