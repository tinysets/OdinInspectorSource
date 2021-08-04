#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DelayedGUIPainter.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------// <copyright file="DelayedGUIPainter.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DelayedGUIPainter.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    public class DelayedGUIDrawer
    {
        private bool drawGUI;
        private Rect areaRect;
        private Vector2 screenPos;

        public void Begin(float width, float height, bool drawGUI = false)
        {
            this.Begin(new Vector2(width, height), drawGUI);
        }

        public void Begin(Vector2 size, bool drawGUI = false)
        {
            this.drawGUI = drawGUI;
            if (this.drawGUI) return;

            if (Event.current.type != EventType.Layout)
            {
                var p = GUIUtility.ScreenToGUIPoint(this.screenPos);
                this.areaRect = new Rect(p.x, p.y, size.x, size.y);
            }

            GUIHelper.BeginIgnoreInput();
            GUILayout.BeginArea(this.areaRect, SirenixGUIStyles.None);
        }

        public void End()
        {
            if (this.drawGUI) { return; }

            GUILayout.EndArea();
            GUIHelper.EndIgnoreInput();
        }


        public void Draw(Vector2 position)
        {
            if (Event.current.type != EventType.Layout)
            {
                this.screenPos = GUIUtility.GUIToScreenPoint(position);
            }
        }
    }
}
#endif