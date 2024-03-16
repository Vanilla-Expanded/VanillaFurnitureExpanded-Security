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

    [DefOf]
    public static class InternalDefOf
    {

        public static WorldObjectDef PeaceTalks;
        public static SoundDef EnergyShield_Broken;
        public static ThingDef Tornado;
    }

}
