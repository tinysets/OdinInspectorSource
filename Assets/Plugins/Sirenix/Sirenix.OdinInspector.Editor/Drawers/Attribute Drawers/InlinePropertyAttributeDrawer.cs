#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InlinePropertyAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="CompositeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Drawer for the <see cref="InlinePropertyAttribute"/> attribute.
    /// </summary>
    [DrawerPriority(0, 0, 0.11)]
    public class InlinePropertyAttributeDrawer : OdinAttributeDrawer<InlinePropertyAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var pushLabelWidth = this.Attribute.LabelWidth > 0;
            if (label == null)
            {
                if (pushLabelWidth) GUIHelper.PushLabelWidth(this.Attribute.LabelWidth);

                this.CallNextDrawer(label);
                if (pushLabelWidth) GUIHelper.PopLabelWidth();
            }
            else
            {
                SirenixEditorGUI.BeginVerticalPropertyLayout(label);
                if (pushLabelWidth) GUIHelper.PushLabelWidth(this.Attribute.LabelWidth);
                for (int i = 0; i < this.Property.Children.Count; i++)
                {
                    var child = this.Property.Children[i];
                    child.Draw(child.Label);
                }
                if (pushLabelWidth) GUIHelper.PopLabelWidth();
                SirenixEditorGUI.EndVerticalPropertyLayout();
            }
        }
    }
}
#endif