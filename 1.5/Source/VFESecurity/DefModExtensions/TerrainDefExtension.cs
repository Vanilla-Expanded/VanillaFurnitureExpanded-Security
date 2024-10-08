﻿using Verse;

namespace VFESecurity
{

    public class TerrainDefExtension : DefModExtension
    {
        private static readonly TerrainDefExtension defaultValues = new TerrainDefExtension();

        public static TerrainDefExtension Get(Def def)
        {
            return def.GetModExtension<TerrainDefExtension>() ?? defaultValues;
        }

        public int pathCostEntering = -1;
        public int pathCostLeaving = -1;
        public float coverEffectiveness;
        public HediffDef terrainHediff;
    }
}