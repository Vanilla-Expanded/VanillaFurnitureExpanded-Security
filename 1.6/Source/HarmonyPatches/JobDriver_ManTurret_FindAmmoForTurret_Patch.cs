using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace VFESecurity
{
    [HarmonyPatch(typeof(JobDriver_ManTurret), "FindAmmoForTurret")]
    public static class JobDriver_ManTurret_FindAmmoForTurret_Patch
    {
        public static void Postfix(Pawn pawn, Building_TurretGun gun, ref Thing __result)
        {
            if (__result != null || gun.def != DefsOf.VFES_Turret_Catapult)
            {
                return;
            }
            StorageSettings allowedShellsSettings = ((pawn.IsColonist || pawn.IsColonyMech) ? gun.gun.TryGetComp<CompChangeableProjectile>().allowedShellsSettings : null);
            __result = GenClosest.ClosestThingReachable(gun.Position, gun.Map, ThingRequest.ForGroup(ThingRequestGroup.Chunk), PathEndMode.OnCell, TraverseParms.For(pawn), 40f, StoneChunkValidator);
            bool StoneChunkValidator(Thing t)
            {
                if (t.IsForbidden(pawn) || !pawn.CanReserve(t, 10, 1))
                {
                    return false;
                }
                if (allowedShellsSettings != null && !allowedShellsSettings.AllowedToAccept(t))
                {
                    return false;
                }
                return t.def.IsWithinCategory(ThingCategoryDefOf.StoneChunks);
            }
        }
    }
}
