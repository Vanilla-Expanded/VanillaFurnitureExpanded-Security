using System.Collections.Generic;
using Verse;

namespace VFESecurity;

public class HediffOnBuildingExtension : DefModExtension
{
    public HediffDef hediff;

    public override IEnumerable<string> ConfigErrors()
    {
        foreach (var configError in base.ConfigErrors())
            yield return configError;

        if (hediff == null)
            yield return "hediff is null";
    }
}