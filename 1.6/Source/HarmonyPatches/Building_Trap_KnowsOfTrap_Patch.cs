using HarmonyLib;
using RimWorld;
using Verse;

namespace VFESecurity
{
    [HarmonyPatch(typeof(Building_Trap), nameof(Building_Trap.KnowsOfTrap))]
    public class Building_Trap_KnowsOfTrap_Patch
    {
        static bool Prefix(Building_Trap __instance, Pawn p, ref bool __result)
        {
            if (__instance is Building_TrapBear bearTrap)
            {
                if (p.Faction == null && p.IsAnimal && !p.InAggroMentalState)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}