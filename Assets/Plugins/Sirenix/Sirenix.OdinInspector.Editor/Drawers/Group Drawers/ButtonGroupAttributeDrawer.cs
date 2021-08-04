#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ButtonGroupAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ButtonGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;
    using UnityEditor;
    using System.Linq;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="ButtonGroupAttribute"/>
    /// </summary>
    /// <seealso cref="ButtonGroupAttribute"/>

    public class ButtonGroupAttributeDrawer : OdinGroupDrawer<ButtonGroupAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;

            SirenixEditorGUI.BeginIndentedHorizontal();

            PropertyContext<int> height;
            if (property.Context.Get(this, "ButtonHeight", out height))
            {
                height.Value = 0;

                for (int i = 0; i < property.Children.Count; i++)
                {
                    var button = property.Children[i].Info.GetAttribute<ButtonAttribute>();
                    if (button != null && button.ButtonHeight > 0)
                    {
                        height.Value = button.ButtonHeight;
                    }
                }
            }

            for (int i = 0; i < property.Children.Count; i++)
            {
                var style = (GUIStyle)null;

                if (property.Children.Count != 1)
                {
                    if (i == 0)
                    {
                        style = SirenixGUIStyles.ButtonLeft;
                    }
                    else if (i == property.Children.Count - 1)
                    {
                        style = SirenixGUIStyles.ButtonRight;
                    }
                    else
                    {
                        style = SirenixGUIStyles.ButtonMid;
                    }
                }

                var child = property.Children[i];

                child.Context.GetGlobal("ButtonHeight", height.Value).Value = height.Value;
                child.Context.GetGlobal("ButtonStyle", style).Value = style;
                DefaultMethodDrawer.DontDrawMethodParamaters = true;
                child.Draw(child.Label);
                DefaultMethodDrawer.DontDrawMethodParamaters = false;
            }

            SirenixEditorGUI.EndIndentedHorizontal();
        }
    }
}
#endif