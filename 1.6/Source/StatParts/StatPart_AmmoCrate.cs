using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;

namespace VFESecurity;

public class StatPart_AmmoCrate : StatPart
{
    public ThingDef thingDef;
    public float valueFactor = 1f;

    public override void TransformValue(StatRequest req, ref float val)
    {
        if (IsAffected(req))
            val *= valueFactor;
    }

    public override string ExplanationPart(StatRequest req)
    {
        if (IsAffected(req))
            return $"{thingDef.LabelCap}: x{valueFactor.ToStringPercent()}";
        return null;
    }

    private bool IsAffected(StatRequest req)
    {
        // Check if the thing can ever be affected (is held by pawn, pawn has map)
        if (req.Thing is not { ParentHolder: Pawn_EquipmentTracker eq } thing || eq.pawn.Map == null)
            return false;

        var ammoCrates = eq.pawn.Map.listerThings.ThingsOfDef(thingDef);
        // Check if there are any reachable ammo crates in the vicinity of the pawn if the weapon isn't single-use
        if (ammoCrates.Count > 0 && thing.TryGetComp<CompEquippable>().PrimaryVerb is not Verb_ShootOneUse)
        {
            for (var i = 0; i < ammoCrates.Count; i++)
            {
                var c = ammoCrates[i];
                // Check if pawn is in range of the crate
                if (eq.pawn.Position.InHorDistOf(c.Position, thingDef.specialDisplayRadius) && !eq.pawn.Faction.HostileTo(c.Faction))
                {
                    // If the radius is over 2, also check if the pawn can reach the thing.
                    // This will prevent situations where the ammo box in encased in walls from all sides.
                    // However, this won't stop situations where the ammo box is behind a wall (but still is reachable).
                    if (thingDef.specialDisplayRadius < 2f || c.Map.reachability.CanReach(eq.pawn.Position, c, PathEndMode.Touch, TraverseParms.For(eq.pawn)))
                        return true;
                }
            }
        }
        return false;
    }

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (var configError in base.ConfigErrors())
            yield return configError;

        if (thingDef == null)
            yield return $"{nameof(thingDef)} is null";
    }
}