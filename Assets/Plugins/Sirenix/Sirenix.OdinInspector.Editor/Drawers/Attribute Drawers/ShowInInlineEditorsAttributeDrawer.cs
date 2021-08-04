#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ShowInInlineEditorsAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ShowInInlineEditorsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="ShowInInlineEditorsAttribute"/>.
    /// </summary>
    [DrawerPriority(1000, 0, 0)]
    public sealed class ShowInInlineEditorsAttributeDrawer : OdinAttributeDrawer<ShowInInlineEditorsAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (InlineEditorAttributeDrawer.CurrentInlineEditorDrawDepth == 0)
                return;

            this.CallNextDrawer(label);
        }
    }
}
#endif