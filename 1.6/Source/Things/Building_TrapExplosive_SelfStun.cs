using RimWorld;
using Verse;

namespace VFESecurity;

public class Building_TrapExplosive_SelfStun : Building_TrapExplosive
{
    private const string OffGraphicSuffix = "_Off";

    private SelfStunTrapExtension selfStunTrapExtension;

    private Graphic offGraphic;
    private bool stunnedLastTick = false;

    public override Graphic Graphic
    {
        get
        {
            if (stunner is not { Stunned: true } || !Spawned)
                return base.Graphic;
    
            if (offGraphic == null)
            {
                offGraphic = GraphicDatabase.Get(def.graphicData.graphicClass, def.graphicData.texPath + OffGraphicSuffix, def.graphicData.shaderType.Shader, def.graphicData.drawSize, DrawColor, DrawColorTwo);
                if (offGraphic == BaseContent.BadGraphic)
                    return offGraphic;
            }
    
            return offGraphic;
        }
    }

    public override void TickInterval(int delta)
    {
        base.TickInterval(delta);

        // Need to dirty map mesh to have the graphic change after stun runs out
        var stunned = IsStunned;
        if (stunnedLastTick != stunned)
        {
            stunnedLastTick = stunned;
            if (!stunned && graphicInt != offGraphic && Spawned)
                DirtyMapMesh(Map);
        }
    }

    public override void SpringSub(Pawn p)
    {
        base.SpringSub(p);

        if (selfStunTrapExtension == null)
            stunner.StunFor(10000, null, false, false);
        else
            stunner.StunFor(selfStunTrapExtension.stunTicks, null, false, selfStunTrapExtension.showMote);
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);

        // Initialize stunner
        stunnedLastTick = IsStunned;
    }

    public override void PostMake()
    {
        base.PostMake();

        selfStunTrapExtension = def.GetModExtension<SelfStunTrapExtension>();
    }

    public override void ExposeData()
    {
        base.ExposeData();

        if (Scribe.mode == LoadSaveMode.LoadingVars)
            selfStunTrapExtension = def.GetModExtension<SelfStunTrapExtension>();
    }
}