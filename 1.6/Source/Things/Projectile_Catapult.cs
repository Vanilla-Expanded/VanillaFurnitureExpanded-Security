using UnityEngine;
using Verse;

namespace VFESecurity
{
    public class Projectile_Catapult : Projectile_Explosive
    {
        private new const float ArcHeightFactor = 4f;
        public override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction);
            Vector3 drawPos = DrawPos;
            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
            Graphics.DrawMesh(MeshPool.GridPlane(DrawSize), position, ExactRotation, DrawMat, 0);
            Comps_PostDraw();
        }
    }
}
