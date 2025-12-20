using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFESecurity
{
    [StaticConstructorOnStartup]
    public class CompConcealed : ThingComp
    {
        private bool submerged;
        private int lastUsedTick;
        private int transitionTicks;
        private int transitionDuration;
        private Effecter progressBar;

        private static readonly Texture2D DeployIcon = ContentFinder<Texture2D>.Get("UI/Commands/Gizmo_StructureDeploy");
        private static readonly Texture2D SubmergeIcon = ContentFinder<Texture2D>.Get("UI/Commands/Gizmo_StructureSubmerge");

        public CompProperties_Concealed Props => (CompProperties_Concealed)props;

        private CompPowerTrader powerComp;

        public bool Submerged => submerged;

        private bool InTransition => transitionTicks > 0;

        private float TransitionProgress => 1f - ((float)transitionTicks / transitionDuration);

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            powerComp = parent.GetComp<CompPowerTrader>();
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref submerged, "submerged", false);
            Scribe_Values.Look(ref lastUsedTick, "lastUsedTick", 0);
            Scribe_Values.Look(ref transitionTicks, "transitionTicks", 0);
            Scribe_Values.Look(ref transitionDuration, "transitionDuration", 0);
        }

        public override void CompTick()
        {
            base.CompTick();
            if (InTransition)
            {
                transitionTicks--;
                if (progressBar != null)
                {
                    progressBar.EffectTick(parent, parent);
                    if (progressBar.children[0] is SubEffecter_ProgressBar subEffecter)
                    {
                        subEffecter.mote.progress = TransitionProgress;
                    }
                }

                if (!InTransition)
                {
                    submerged = !submerged;
                    lastUsedTick = Find.TickManager.TicksGame;
                    progressBar?.Cleanup();
                    progressBar = null;
                    parent.Map.pathing.RecalculatePerceivedPathCostAt(parent.Position);
                    parent.Map.mapDrawer.MapMeshDirty(parent.Position, MapMeshFlagDefOf.Things | MapMeshFlagDefOf.Buildings);
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (submerged)
            {
                var deploy = new Command_Action
                {
                    defaultLabel = "VFES_Deploy".Translate(),
                    defaultDesc = "VFES_DeployDesc".Translate(),
                    icon = DeployIcon,
                    action = () => StartTransition(false),
                    disabled = IsOnCooldown() || IsPowerOff() || InTransition,
                    groupKey = 2173836
                };
                if (IsOnCooldown())
                {
                    deploy.disabledReason = "VFES_OnCooldown".Translate(CooldownTimeLeft().ToStringSecondsFromTicks());
                }
                else if (IsPowerOff())
                {
                    deploy.disabledReason = "NoPower".Translate();
                }
                else if (InTransition)
                {
                    deploy.disabledReason = "VFES_Transitioning".Translate();
                }
                yield return deploy;
            }
            else
            {
                var submerge = new Command_Action
                {
                    defaultLabel = "VFES_Submerge".Translate(),
                    defaultDesc = "VFES_SubmergeDesc".Translate(),
                    icon = SubmergeIcon,
                    action = () => StartTransition(true),
                    disabled = IsOnCooldown() || IsPowerOff() || InTransition,
                    groupKey = 2173837
                };
                if (IsOnCooldown())
                {
                    submerge.disabledReason = "VFES_OnCooldown".Translate(CooldownTimeLeft().ToStringSecondsFromTicks());
                }
                else if (IsPowerOff())
                {
                    submerge.disabledReason = "NoPower".Translate();
                }
                else if (InTransition)
                {
                    submerge.disabledReason = "VFES_Transitioning".Translate();
                }
                yield return submerge;
            }
        }

        private void StartTransition(bool submerge)
        {
            var things = Find.Selector.SelectedObjects.OfType<Building>().Select(b => b.GetComp<CompConcealed>()).Where(c => c != null);
            foreach (var comp in things)
            {
                comp.transitionDuration = submerge ? comp.Props.submergeSeconds * 60 : comp.Props.deploySeconds * 60;
                comp.transitionTicks = comp.transitionDuration;
                comp.progressBar = EffecterDefOf.ProgressBar.Spawn();
            }
        }

        private bool IsOnCooldown() => TicksSinceLastUse() < Props.cooldownSeconds * 60;

        private int TicksSinceLastUse() => Find.TickManager.TicksGame - lastUsedTick;

        private int CooldownTimeLeft() => (Props.cooldownSeconds * 60) - TicksSinceLastUse();

        private bool IsPowerOff() => powerComp != null && !powerComp.PowerOn;
    }
}
