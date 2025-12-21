using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFESecurity
{
    [Flags]
    public enum ArtilleryFiringMode
    {
        None = 0,
        GroundToGround = 1,
        GroundToSpace = 2,
        SpaceToSpace = 4,
        SpaceToGround = 8
    }

    public class CompProperties_WorldArtillery : CompProperties
    {
        public int worldMapAttackRange;
        public ArtilleryFiringMode firingMode;

        public CompProperties_WorldArtillery()
        {
            compClass = typeof(CompWorldArtillery);
        }
    }
    [HotSwappable]
    [StaticConstructorOnStartup]
    public class CompWorldArtillery : ThingComp
    {
        public GlobalTargetInfo worldTarget = GlobalTargetInfo.Invalid;
        public LocalTargetInfo target = LocalTargetInfo.Invalid;
        private PlanetTile cachedClosest;
        private PlanetTile cachedOrigin;
        private PlanetLayer cachedLayer;
        private static readonly Texture2D WorldTargetIcon = ContentFinder<Texture2D>.Get("UI/Commands/ArtillerySetWorldTarget");

        public static readonly Material AttackRadiusMat = MaterialPool.MatFrom(GenDraw.OneSidedLineTexPath, ShaderDatabase.WorldOverlayTransparent, ColorLibrary.Red, 3590);

        public static readonly Material AttackRadiusMatHighVis = MaterialPool.MatFrom(GenDraw.OneSidedLineOpaqueTexPath, ShaderDatabase.WorldOverlayAdditiveTwoSided, ColorLibrary.Red, 3590);

        private Building_Artillery Turret => parent as Building_Artillery;

        public CompProperties_WorldArtillery Props => props as CompProperties_WorldArtillery;

        public static Material GetAttackRadiusMat(PlanetTile tile)
        {
            if (!tile.LayerDef.isSpace)
            {
                return AttackRadiusMat;
            }
            return AttackRadiusMatHighVis;
        }


        public virtual float FinalForcedMissRadius(GlobalTargetInfo target)
        {
            var launcher = parent as Building_Artillery;
            var verb = launcher.AttackVerb;
            var baseMissRadius = verb.verbProps.ForcedMissRadius;
            var distance = Find.WorldGrid.ApproxDistanceInTiles(launcher.Map.Tile, target.Tile);
            var worldMultiplier = 1f;
            if (distance > 49)
            {
                worldMultiplier = 2.0f;
            }
            else if (distance > 25)
            {
                worldMultiplier = 1.6f;
            }
            else if (distance > 9)
            {
                worldMultiplier = 1.2f;
            }
            var adjustedRadius = baseMissRadius * worldMultiplier;

            var manningPawn = launcher.mannableComp.ManningPawn;
            if (manningPawn is Pawn pawn)
            {
                adjustedRadius *= verb.verbProps.GetForceMissFactorFor(launcher, pawn);
            }

            return adjustedRadius;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_TargetInfo.Look(ref worldTarget, "worldTarget", GlobalTargetInfo.Invalid);
            Scribe_TargetInfo.Look(ref target, "target", LocalTargetInfo.Invalid);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            if (parent.Faction != Faction.OfPlayer)
            {
                yield break;
            }
            var worldTargetGizmo = new Command_Action
            {
                defaultLabel = "VFES_SetWorldTarget".Translate(),
                defaultDesc = "VFES_SetWorldTargetDesc".Translate(),
                icon = WorldTargetIcon,
                action = delegate { StartWorldTargeting(); }
            };

            if (Turret.mannableComp.ManningPawn is null)
            {
                worldTargetGizmo.Disable("VFES_NeedsManned".Translate());
            }
            yield return worldTargetGizmo;
        }

        private void ResetCachedTile()
        {
            cachedClosest = PlanetTile.Invalid;
            cachedOrigin = PlanetTile.Invalid;
            cachedLayer = null;
        }

        private void StartWorldTargeting()
        {
            ResetCachedTile();
            CameraJumper.TryJump(CameraJumper.GetWorldTarget(parent));
            Find.WorldSelector.ClearSelection();
            Find.WorldTargeter.BeginTargeting(
                (GlobalTargetInfo globalTarget) =>
                {
                    string failReason;
                    if (!IsWithinRange(globalTarget, out failReason))
                    {
                        Messages.Message(failReason, MessageTypeDefOf.RejectInput, false);
                        return false;
                    }
                    if (!IsValidTargetForFiringMode(globalTarget, out failReason))
                    {
                        Messages.Message(failReason, MessageTypeDefOf.RejectInput, false);
                        return false;
                    }
                    if (Find.WorldObjects.MapParentAt(globalTarget.Tile) is MapParent mapParent && mapParent.Map != null)
                    {
                        var map = mapParent.Map;
                        Current.Game.CurrentMap = map;
                        var targetingParameters = new TargetingParameters
                        {
                            canTargetPawns = true,
                            canTargetBuildings = true,
                            canTargetLocations = true
                        };
                        var turret = parent as Building_TurretGun;
                        Find.Targeter.BeginTargeting(targetingParameters, delegate (LocalTargetInfo target)
                        {
                            StartAttack(globalTarget, target, turret);
                            Current.Game.CurrentMap = turret.Map;
                            Find.CameraDriver.JumpToCurrentMapLoc(turret.Position);
                        }, highlightAction: delegate (LocalTargetInfo x)
                        {
                            if (x.IsValid)
                            {
                                GenDraw.DrawTargetHighlight(x);
                            }
                        }, null, onGuiAction: delegate (LocalTargetInfo x)
                        {
                            Texture2D icon = (Texture2D)Turret.def.building.turretTopMat.mainTexture;
                            GenUI.DrawMouseAttachment(icon);
                        }, onUpdateAction: delegate (LocalTargetInfo x)
                        {
                            DrawTargetHighlightField(x, globalTarget);
                        });
                        return true;
                    }
                    return false;
                },
                true,
                WorldTargetIcon,
                true, onUpdate: delegate
                {
                    PlanetTile tile = parent.Map.Tile;
                    PlanetTile planetTile;
                    if (cachedLayer != Find.WorldSelector.SelectedLayer || cachedOrigin != tile)
                    {
                        cachedLayer = Find.WorldSelector.SelectedLayer;
                        cachedOrigin = tile;
                        planetTile = cachedClosest = Find.WorldSelector.SelectedLayer.GetClosestTile_NewTemp(tile);
                    }
                    else
                    {
                        planetTile = cachedClosest;
                    }
                    int maxAttackDistance = Mathf.FloorToInt((float)Props.worldMapAttackRange / PlanetLayer.Selected.Def.rangeDistanceFactor);
                    GenDraw.DrawWorldRadiusRing(planetTile, maxAttackDistance);
                },
                null,
                null,
                null
            );
        }

        public void StartAttack(GlobalTargetInfo globalTarget, LocalTargetInfo target, Building_TurretGun turret)
        {
            var cell = FindEdgeCell(turret.Map, globalTarget);
            turret.OrderAttack(cell);
            this.worldTarget = globalTarget;
            this.target = target;
        }

        public override void CompTickInterval(int delta)
        {
            base.CompTickInterval(delta);
            if (target.IsValid && target.ThingDestroyed)
            {
                var parent = this.parent as Building_TurretGun;
                parent.ResetForcedTarget();
            }
        }

        private void DrawTargetHighlightField(LocalTargetInfo target, GlobalTargetInfo worldTarget)
        {
            var turret = parent as Building_Artillery;
            ThingDef projectile = turret.AttackVerb.GetProjectile();
            float num = projectile.projectile.explosionRadius + projectile.projectile.explosionRadiusDisplayPadding;
            float forcedMissRadius = FinalForcedMissRadius(worldTarget);
            if (forcedMissRadius > 0f && turret.AttackVerb.BurstShotCount > 1)
            {
                num += forcedMissRadius;
            }
            if (!(num > 0.2f))
            {
                return;
            }
            GenExplosion.RenderPredictedAreaOfEffect(target.Cell, num, turret.AttackVerb.verbProps.explosionRadiusRingColor);
        }

        public LocalTargetInfo FindEdgeCell(Map map, GlobalTargetInfo worldTarget)
        {
            float angle = ArtilleryUtils.GetAngle(map.Tile, worldTarget.Tile);
            var edgeCells = new CellRect(0, 0, map.Size.x, map.Size.z).EdgeCells;
            var parentPos = parent.TrueCenter();
            IntVec3 targetCell = edgeCells.MinBy(c => Mathf.Abs(angle - (c.ToVector3() - parentPos).AngleFlat()));
            Vector3 normalized = (targetCell.ToVector3() - parentPos).normalized;
            IntVec3 outCell = new IntVec3(targetCell.x + (int)Math.Round(normalized.x), targetCell.y, targetCell.z + (int)Math.Round(normalized.z));
            return new LocalTargetInfo(outCell);
        }

        private bool IsValidTargetForFiringMode(GlobalTargetInfo target, out string failReason)
        {
            var sourceMap = parent.Map;
            if (sourceMap == null)
            {
                failReason = "VFES_ArtilleryNeedsVisibleMap".Translate();
                return false;
            }
            bool sourceIsOnGround = sourceMap.Tile.Valid && !sourceMap.Tile.LayerDef.isSpace;
            var mapParent = Find.WorldObjects.MapParentAt(target.Tile);
            if (mapParent == null || mapParent.Map == null)
            {
                failReason = "VFES_ArtilleryNeedsVisibleMap".Translate();
                return false;
            }
            bool targetIsOnGround = mapParent.Map.Tile.Valid && !mapParent.Map.Tile.LayerDef.isSpace;

            ArtilleryFiringMode requiredMode;
            string invalidModeKey;
            if (sourceIsOnGround && targetIsOnGround)
            {
                requiredMode = ArtilleryFiringMode.GroundToGround;
                invalidModeKey = "VFES_ArtilleryInvalidGroundToGround";
            }
            else if (sourceIsOnGround && !targetIsOnGround)
            {
                requiredMode = ArtilleryFiringMode.GroundToSpace;
                invalidModeKey = "VFES_ArtilleryInvalidGroundToSpace";
            }
            else if (!sourceIsOnGround && !targetIsOnGround)
            {
                requiredMode = ArtilleryFiringMode.SpaceToSpace;
                invalidModeKey = "VFES_ArtilleryInvalidSpaceToSpace";
            }
            else if (!sourceIsOnGround && targetIsOnGround)
            {
                requiredMode = ArtilleryFiringMode.SpaceToGround;
                invalidModeKey = "VFES_ArtilleryInvalidSpaceToGround";
            }
            else
            {
                failReason = "VFES_ArtilleryInvalidFiringMode".Translate();
                return false;
            }
            bool isValid = (Props.firingMode & requiredMode) != 0;
            failReason = isValid ? null : invalidModeKey.Translate();
            return isValid;
        }

        private bool IsWithinRange(GlobalTargetInfo target, out string failReason)
        {
            var distance = Find.WorldGrid.ApproxDistanceInTiles(parent.Map.Tile, target.Tile);
            bool isWithinRange = distance <= Props.worldMapAttackRange;
            failReason = isWithinRange ? null : "VFES_ArtilleryOutOfRange".Translate();
            return isWithinRange;
        }

        public void Reset()
        {
            worldTarget = GlobalTargetInfo.Invalid;
            target = LocalTargetInfo.Invalid;
        }
    }
}

