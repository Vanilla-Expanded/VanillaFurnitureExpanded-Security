using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace VFESecurity;

public class Verb_BurnerFireSpew : Verb
{
    private readonly List<IntVec3> tmpCells = [];

    public override bool TryCastShot()
    {
        var map = caster.Map;
        if (currentTarget.HasThing && currentTarget.Thing.Map != map)
            return false;

        var extension = EquipmentSource.def.GetModExtension<BurnerFireSpewExtension>();

        GenExplosion.DoExplosion(
            center: currentTarget.Cell,
            map: map,
            radius: 0f,
            damType: DamageDefOf.Flame,
            instigator: caster,
            damAmount: extension.damAmount,
            postExplosionSpawnThingDef: extension.filthDef,
            postExplosionSpawnChance: 1f,
            chanceToStartFire: 1f,
            doVisualEffects: false,
            propagationSpeed: 0.6f,
            doSoundEffects: false,
            flammabilityChanceCurve: verbProps.flammabilityAttachFireChanceCurve,
            overrideCells: AffectedCells(currentTarget));

        var drawPos = caster.DrawPos;
        var drawPosCell = drawPos.Yto0().ToIntVec3();

        if (GenSpawn.Spawn(ThingDefOf.IncineratorSpray, drawPosCell, map) is IncineratorSpray incineratorSpray)
        {
            var numStreams = extension.numStreams;
            var baseDirection = (currentTarget.CenterVector3 - drawPos).normalized;
            for (var i = 0; i < numStreams; i++)
            {
                var streamDirection = baseDirection.RotatedBy(Rand.Range(-extension.coneSizeDegrees, extension.coneSizeDegrees));
                var furthestTargetPosition = (drawPos + streamDirection * (extension.range + Rand.Value * extension.rangeNoise)).ToIntVec3();
                var targetPosition = GenSight.LastPointOnLineOfSight(drawPosCell, furthestTargetPosition, c => c.CanBeSeenOverFast(map), true);
                if (!targetPosition.IsValid)
                    targetPosition = furthestTargetPosition;

                if (Vector3.Dot((targetPosition.ToVector3() - drawPos).normalized, streamDirection) > 0.5f)
                {
                    var distance = Vector3.Distance(targetPosition.ToVector3(), drawPos);
                    var scale = Mathf.Clamp01(distance / extension.sizeReductionDistanceThreshold);

                    var mote = MoteMaker.MakeInteractionOverlay(extension.moteDef, new TargetInfo(drawPosCell, map), new TargetInfo(targetPosition, map));
                    incineratorSpray.Add(new IncineratorProjectileMotion
                    {
                        mote = mote,
                        targetDest = currentTarget.Cell,
                        worldSource = drawPos + streamDirection * extension.barrelOffsetDistance,
                        worldTarget = targetPosition.ToVector3(),
                        moveVector = streamDirection,
                        startScale = Rand.Range(0.8f, 1.2f) * scale,
                        endScale = (1f + Rand.Range(0.1f, 0.4f)) * scale,
                        lifespanTicks = Mathf.FloorToInt(distance * 5f) + Rand.Range(-extension.lifespanNoise, extension.lifespanNoise)
                    });

                    map.effecterMaintainer.AddEffecterToMaintain(extension.effecterDef.Spawn(targetPosition, map), targetPosition, 100);
                }
            }
        }

        lastShotTick = Find.TickManager.TicksGame;
        return true;
    }

    public override void DrawHighlight(LocalTargetInfo target)
    {
        base.DrawHighlight(target);

        if (target.IsValid)
            GenDraw.DrawFieldEdges(AffectedCells(target));
    }

    private List<IntVec3> AffectedCells(LocalTargetInfo target)
    {
        var extension = EquipmentSource.def.GetModExtension<BurnerFireSpewExtension>();

        tmpCells.Clear();
        var vector = caster.Position.ToVector3Shifted().Yto0();
        var intVec = target.Cell.ClampInsideMap(caster.Map);
        if (caster.Position == intVec)
            return tmpCells;

        var lengthHorizontal = (intVec - caster.Position).LengthHorizontal;
        var rangeX = (intVec.x - caster.Position.x) / lengthHorizontal;
        var rangeY = (intVec.z - caster.Position.z) / lengthHorizontal;

        intVec.x = Mathf.RoundToInt(caster.Position.x + rangeX * verbProps.range);
        intVec.z = Mathf.RoundToInt(caster.Position.z + rangeY * verbProps.range);

        var angle = Vector3.SignedAngle(intVec.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up);
        var lineWidthEnd = extension.lineWidthEnd / 2f;
        var maxAngle = 57.29578f * Mathf.Asin(lineWidthEnd / Mathf.Sqrt(Mathf.Pow((intVec - caster.Position).LengthHorizontal, 2f) + Mathf.Pow(lineWidthEnd, 2f)));

        var cells = GenRadial.NumCellsInRadius(verbProps.range);
        for (var i = 0; i < cells; i++)
        {
            var cell = caster.Position + GenRadial.RadialPattern[i];
            if (CanUseCell(cell) && Mathf.Abs(Mathf.DeltaAngle(Vector3.SignedAngle(cell.ToVector3Shifted().Yto0() - vector, Vector3.right, Vector3.up), angle)) <= maxAngle)
                tmpCells.Add(cell);
        }

        var list = GenSight.BresenhamCellsBetween(caster.Position, intVec);
        for (var i = 0; i < list.Count; i++)
        {
            var cell = list[i];
            if (!tmpCells.Contains(cell) && CanUseCell(cell))
                tmpCells.Add(cell);
        }

        return tmpCells;

        bool CanUseCell(IntVec3 c)
        {
            return c.InBounds(caster.Map) &&
                   c != caster.Position &&
                   (extension.canHitFilledCells || !c.Filled(caster.Map)) && 
                   c.InHorDistOf(caster.Position, verbProps.range) && 
                   TryFindShootLineFromTo(caster.Position, c, out _);
        }
    }
}