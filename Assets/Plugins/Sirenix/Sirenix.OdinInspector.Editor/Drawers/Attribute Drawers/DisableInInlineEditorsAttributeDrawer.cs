#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DisableInInlineEditorsAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HideInInlineEditorsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties marked with the <see cref="DisableInInlineEditorsAttribute"/> attribute.
    /// </summary>

    [DrawerPriority(1000, 0, 0)]
    public sealed class DisableInInlineEditorsAttributeDrawer : OdinAttributeDrawer<DisableInInlineEditorsAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (InlineEditorAttributeDrawer.CurrentInlineEditorDrawDepth > 0)
            {
                GUIHelper.PushGUIEnabled(false);
                this.CallNextDrawer(label);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif