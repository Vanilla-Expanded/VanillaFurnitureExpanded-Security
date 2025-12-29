using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace VFESecurity;

public class CompProperties_StunnableCodeTrigger : CompProperties_Stunnable
{
    // Prevent errors due to no damageDefs defined triggerable through code
    public override IEnumerable<string> ConfigErrors(ThingDef parentDef) => base.ConfigErrors(parentDef).Where(x => x != "CompProperties_Stunnable requires at least one affectedDamageDef");
}