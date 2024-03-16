using Verse;

namespace VFESecurity
{

    public class Verb_Dazzle : Verb
    {
        public ExtendedVerbProperties ExtendedVerbProps => EquipmentSource.def.GetModExtension<ExtendedVerbProperties>() ?? ExtendedVerbProperties.defaultValues;

        public Mote prevMote;
        public override bool CanHitTarget(LocalTargetInfo targ)
        {
            // Target and caster are on different maps
            if ((targ.HasThing && (targ.Thing.Map != caster.Map)) || !(targ.Thing is Pawn))
            {
                return false;
            }

            // No LoS
            if (verbProps.stopBurstWithoutLos && !GenSight.LineOfSight(caster.Position, targ.Cell, caster.Map))
            {
                return false;
            }
            return base.CanHitTarget(targ);
        }

        public override bool TryCastShot()
        {
            if (prevMote != null)
            {
                prevMote.Destroy();
            }
            // Throw mote at target cell
            prevMote = ExtendedMoteMaker.SearchlightEffect(currentTarget.Thing, caster.Map, ExtendedVerbProps.illuminatedRadius, 2);
            return true;
        }
    }

}
