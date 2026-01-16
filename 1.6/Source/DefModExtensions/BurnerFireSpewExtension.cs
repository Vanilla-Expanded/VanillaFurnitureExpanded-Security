using Verse;

namespace VFESecurity;

public class BurnerFireSpewExtension : DefModExtension
{
    // Same data as CompProperties_AbilityFireSpew
    public int damAmount = -1;
    public float lineWidthEnd;
    public bool canHitFilledCells;

    public ThingDef filthDef;
    // Skipping effecter def, we're specifically using burner's effecter

    // Same data as CompProperties_AbilityBurner
    public int numStreams;
    public int lifespanNoise;
    public float coneSizeDegrees;
    public float range;
    public float rangeNoise;
    public float barrelOffsetDistance;
    public float sizeReductionDistanceThreshold;

    public ThingDef moteDef;
    public EffecterDef effecterDef;
}