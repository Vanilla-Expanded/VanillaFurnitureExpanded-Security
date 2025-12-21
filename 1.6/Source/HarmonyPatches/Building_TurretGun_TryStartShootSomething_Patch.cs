using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HotSwappableAttribute : Attribute
    {
    }
    [HotSwappable]
    [HarmonyPatch(typeof(Building_TurretGun), "TryStartShootSomething")]
    public static class Building_TurretGun_TryStartShootSomething_Patch
    {
        public static bool Prefix(Building_TurretGun __instance)
        {
            if (__instance.def == DefsOf.VFES_Turret_AntiAir)
            {
                return false;
            }
            return true;
        }
    }
}
