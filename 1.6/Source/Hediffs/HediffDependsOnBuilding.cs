using Verse;

namespace VFESecurity;

public class HediffDependsOnBuilding : Hediff
{
    public ThingDef building;

    // ShouldRemove is called up to twice each tick, so it's cheaper performance-wise to do this check manually once every couple of ticks instead

    private bool CanRemove
    {
        get
        {
            // Not spawned, flying, or no building - remove
            if (!pawn.Spawned || pawn.Flying || building == null)
                return true;
            var edifice = pawn.Position.GetEdificeSafe(pawn.Map)?.def;
            return edifice == null || edifice != building;
        }
    }

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);

        if (pawn.IsHashIntervalTick(60, delta) && CanRemove)
            pawn.health.RemoveHediff(this);
    }
}