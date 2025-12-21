using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace VFESecurity
{
    public class Gizmo_SetSecondaryFuelLevel : Gizmo_Slider
    {
        private CompRefuelable_DualFuel refuelable;

        public override float Target
        {
            get
            {
                return refuelable.SecondaryTargetFuelLevel / refuelable.Props.secondaryFuelCapacity;
            }
            set
            {
                refuelable.SecondaryTargetFuelLevel = value * refuelable.Props.secondaryFuelCapacity;
            }
        }

        public override float ValuePercent => refuelable.SecondaryFuelPercentOfMax;

        public override string Title => refuelable.Props.SecondaryFuelGizmoLabel;

        public override bool IsDraggable => refuelable.Props.targetSecondaryFuelLevelConfigurable;

        public override string BarLabel => refuelable.SecondaryFuel.ToStringDecimalIfSmall() + " / " +
                                             refuelable.Props.secondaryFuelCapacity.ToStringDecimalIfSmall();

        public override bool DraggingBar { get; set; }

        public Gizmo_SetSecondaryFuelLevel(CompRefuelable_DualFuel refuelable)
        {
            this.refuelable = refuelable;
        }

        public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
        {
            if (!refuelable.Props.showAllowAutoRefuelSecondaryToggle)
            {
                return base.GizmoOnGUI(topLeft, maxWidth, parms);
            }

            return base.GizmoOnGUI(topLeft, maxWidth, parms);
        }

        public override void DrawHeader(Rect headerRect, ref bool mouseOverElement)
        {
            if (refuelable.Props.showAllowAutoRefuelSecondaryToggle)
            {
                headerRect.xMax -= 24f;
                Rect rect = new Rect(headerRect.xMax, headerRect.y, 24f, 24f);
                GUI.DrawTexture(rect, refuelable.Props.SecondaryFuelIcon);
                GUI.DrawTexture(new Rect(rect.center.x, rect.y, rect.width / 2f, rect.height / 2f),
                    refuelable.allowAutoRefuelSecondary ? Widgets.CheckboxOnTex : Widgets.CheckboxOffTex);
                if (Widgets.ButtonInvisible(rect))
                {
                    ToggleAutoRefuel();
                }
            }
            base.DrawHeader(headerRect, ref mouseOverElement);
        }

        private void ToggleAutoRefuel()
        {
            refuelable.allowAutoRefuelSecondary = !refuelable.allowAutoRefuelSecondary;
            if (refuelable.allowAutoRefuelSecondary)
            {
                SoundDefOf.Tick_High.PlayOneShotOnCamera();
            }
            else
            {
                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
            }
        }

        public override string GetTooltip()
        {
            return "";
        }
    }
}
