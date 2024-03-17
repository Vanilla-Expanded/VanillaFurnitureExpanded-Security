using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace VFESecurity
{
    public class ArtilleryStrikeArrivalAction_Settlement : ArtilleryStrikeArrivalAction_MapParent
    {
        public ArtilleryStrikeArrivalAction_Settlement()
        {
        }

        public ArtilleryStrikeArrivalAction_Settlement(MapParent worldObject)
        {
            this.mapParent = worldObject;
        }

        protected Settlement Settlement => mapParent as Settlement;

        protected override bool CanDoArriveAction => Settlement != null && Settlement.Spawned && Settlement.Faction != Faction.OfPlayer;

        protected override void PreStrikeAction()
        {
            Settlement.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -99999, reason: DefDatabase<HistoryEventDef>.GetNamed("VFES_ArtilleryStrike"), lookTarget: this.mapParent);
        }

        protected override void PostStrikeAction(bool destroyed)
        {
            if (!destroyed)
            {
                // Otherwise artillery retaliation
                var artilleryComp = Settlement.GetComponent<ArtilleryComp>();
                if (artilleryComp != null)
                    artilleryComp.TryStartBombardment();
            }
            else
            {
                Find.LetterStack.ReceiveLetter("LetterLabelFactionBaseDefeated".Translate(), "VFESecurity.LetterFactionBaseDefeatedStrike".Translate(Settlement.Label), LetterDefOf.PositiveEvent,
                    new GlobalTargetInfo(Settlement.Tile), Settlement.Faction, null);
                var destroyedSettlement = (DestroyedSettlement)WorldObjectMaker.MakeWorldObject(RimWorld.WorldObjectDefOf.DestroyedSettlement);
                destroyedSettlement.Tile = Settlement.Tile;
                Find.WorldObjects.Add(destroyedSettlement);
                Find.WorldObjects.Remove(Settlement);
            }
        }
    }
}