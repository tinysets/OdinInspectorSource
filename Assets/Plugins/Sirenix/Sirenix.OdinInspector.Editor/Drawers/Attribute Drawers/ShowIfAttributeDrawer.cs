#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ShowIfAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ShowIfAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="ShowIfAttribute"/>.
    /// </summary>
    [DrawerPriority(100, 0, 0)]
    public sealed class ShowIfAttributeDrawer : OdinAttributeDrawer<ShowIfAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool result;
            string errorMessage;

            var property = this.Property;
            var attribute = this.Attribute;

            IfAttributesHelper.HandleIfAttributesCondition(this, property, attribute.MemberName, attribute.Value, out result, out errorMessage);

            if (errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(errorMessage);
                this.CallNextDrawer(label);
            }
            else
            {
                if (attribute.Animate)
                {
                    if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(property, this), result))
                    {
                        this.CallNextDrawer(label);
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
                else
                {
                    if (result)
                    {
                        this.CallNextDrawer(label);
                    }
                }
            }
        }
    }
}
#endif