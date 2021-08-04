#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="HeaderAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HeaderDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HeaderAttribute"/>.
    /// </summary>
    /// <seealso cref="HeaderAttribute"/>
    /// <seealso cref="TitleAttribute"/>
    /// <seealso cref="HideLabelAttribute"/>
    /// <seealso cref="LabelTextAttribute"/>
    /// <seealso cref="SpaceAttribute"/>

    [DrawerPriority(1, 0, 0)]
    public sealed class HeaderAttributeDrawer : OdinAttributeDrawer<HeaderAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;

            // Don't draw for collection elements
            if (property.Parent != null && property.Parent.ChildResolver is ICollectionResolver)
            {
                this.CallNextDrawer(label);
                return;
            }

            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            var headerContext = property.Context.Get<StringMemberHelper>(this, "Header", (StringMemberHelper)null);
            if (headerContext.Value == null)
            {
                //headerContext.Value = new StringMemberHelper(property.ParentType, this.Attribute.header);
                headerContext.Value = new StringMemberHelper(property, this.Attribute.header);
            }

            EditorGUILayout.LabelField(headerContext.Value.GetString(property), EditorStyles.boldLabel);
            this.CallNextDrawer(label);
        }
    }
}
#endif