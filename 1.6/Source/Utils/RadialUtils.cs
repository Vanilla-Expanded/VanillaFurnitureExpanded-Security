using System.Collections.Generic;
using Verse;

namespace VFESecurity;

public static class RadialUtils
{
    // Basically the same as GenRadial.RadialDistinctThingsAround, except that it's better suited for pawns.
    // The main issues with RadialDistinctThingsAround is that if it encounters anything with a size that's
    // bigger than 1x1, it'll create a HashSet to avoid returning the same thing multiple times.
    // Since we only ever care about pawns (and all pawns are 1x1), we simply don't care about it.
    // It's going to be faster if we just skip anything that's not a pawn and only return pawns.
    public static IEnumerable<Pawn> RadialDistinctPawnsAround(IntVec3 center, Map map, float radius, bool useCenter)
    {
        var numCells = GenRadial.NumCellsInRadius(radius);

        for (var i = useCenter ? 0 : 1; i < numCells; i++)
        {
            var pos = GenRadial.RadialPattern[i] + center;

            if (pos.InBounds(map))
            {
                var thingList = pos.GetThingList(map);
                for (var j = 0; j < thingList.Count; j++)
                {
                    if (thingList[j] is Pawn pawn)
                        yield return pawn;
                }
            }
        }
    }
}