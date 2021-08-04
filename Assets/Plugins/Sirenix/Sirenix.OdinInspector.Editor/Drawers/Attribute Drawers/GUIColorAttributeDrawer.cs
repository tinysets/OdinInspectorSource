#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="GUIColorAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="GUIColorAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;
    using System;
    using System.Reflection;
    using Sirenix.Utilities;

    /// <summary>
    /// Draws properties marked with <see cref="GUIColorAttribute"/>.
    /// This drawer sets the current GUI color, before calling the next drawer in the chain.
    /// </summary>
    /// <seealso cref="GUIColorAttribute"/>
    /// <seealso cref="LabelTextAttribute"/>
    /// <seealso cref="TitleAttribute"/>
    /// <seealso cref="HeaderAttribute"/>
    /// <seealso cref="ColorPaletteAttribute"/>
    [DrawerPriority(0.5, 0, 0)]
    public sealed class GUIColorAttributeDrawer : OdinAttributeDrawer<GUIColorAttribute>
    {
        internal static Color CurrentOuterColor = Color.white;

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var attribute = this.Attribute;

            if (attribute.GetColor == null)
            {
                GUIHelper.PushColor(attribute.Color);
                this.CallNextDrawer(label);
                GUIHelper.PopColor();
            }
            else
            {
                var context = property.Context.Get(this, "config", (InspectorPropertyValueGetter<Color>)null);
                if (context.Value == null)
                {
                    context.Value = new InspectorPropertyValueGetter<Color>(property, attribute.GetColor);
                }

                if (context.Value.ErrorMessage != null)
                {
                    SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
                    this.CallNextDrawer(label);
                }
                else
                {
                    GUIHelper.PushColor(context.Value.GetValue());
                    this.CallNextDrawer(label);
                    GUIHelper.PopColor();
                }
            }
        }
    }
}
#endif