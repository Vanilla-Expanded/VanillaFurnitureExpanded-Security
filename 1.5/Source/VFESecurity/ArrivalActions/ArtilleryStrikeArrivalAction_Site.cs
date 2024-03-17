using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace VFESecurity
{
    public class ArtilleryStrikeArrivalAction_Site : ArtilleryStrikeArrivalAction_MapParent
    {
        public ArtilleryStrikeArrivalAction_Site()
        {
        }

        public ArtilleryStrikeArrivalAction_Site(MapParent worldObject)
        {
            this.mapParent = worldObject;
        }

        protected Site Site => mapParent as Site;

        protected override bool CanDoArriveAction => Site != null && Site.Spawned;

        protected override void PostStrikeAction(bool destroyed)
        {
            base.PostStrikeAction(destroyed);
            if (destroyed)
            {
                QuestUtility.SendQuestTargetSignals(Site.questTags, QuestUtility.QuestTargetSignalPart_AllEnemiesDefeated, Site.Named("SUBJECT"));
                Site.allEnemiesDefeatedSignalSent = true;
                if (Find.WorldObjects.Contains(mapParent))
                {
                    Find.WorldObjects.Remove(mapParent);
                }
                var quests = Find.QuestManager.QuestsListForReading.Where(x => x.ended is false
                    && x.QuestLookTargets.Contains(mapParent)).ToList();
                foreach (var quest in quests)
                {
                    var part = quest.PartsListForReading.OfType<QuestPart_NoWorldObject>().FirstOrDefault();
                    if (part != null)
                    {
                        part.Complete();
                    }
                    else
                    {
                        quest.End(QuestEndOutcome.Unknown);
                    }
                }
            }
        }
    }
}