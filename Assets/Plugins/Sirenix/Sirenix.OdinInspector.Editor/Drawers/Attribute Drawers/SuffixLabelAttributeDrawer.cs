#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="SuffixLabelAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="SuffixLabelAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEngine;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;

    /// <summary>
    /// Draws properties marked with <see cref="SuffixLabelAttribute"/>.
    /// </summary>
    /// <seealso cref="LabelTextAttribute"/>
    /// <seealso cref="PropertyTooltipAttribute"/>
    /// <seealso cref="InlineButtonAttribute"/>
    /// <seealso cref="CustomValueDrawerAttribute"/>

    [AllowGUIEnabledForReadonly]
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public sealed class SuffixLabelAttributeDrawer : OdinAttributeDrawer<SuffixLabelAttribute>
    {
        private StringMemberHelper labelHelper;

        protected override void Initialize()
        {
            this.labelHelper = new StringMemberHelper(this.Property.FindParent(p => p.Info.HasSingleBackingMember, true), this.Attribute.Label);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            //PropertyContext<StringMemberHelper> context;
            //if (this.Property.Context.Get(this, "SuffixContext", out context))
            //{
            //    context.Value = new StringMemberHelper(this.Property.FindParent(p => p.Info.HasSingleBackingMember, true), this.Attribute.Label);
            //}

            if (this.labelHelper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.labelHelper.ErrorMessage);
            }

            if (this.Attribute.Overlay)
            {
                this.CallNextDrawer(label);
                GUIHelper.PushGUIEnabled(true);
                GUI.Label(GUILayoutUtility.GetLastRect().HorizontalPadding(0, 8), this.labelHelper.GetString(this.Property), SirenixGUIStyles.RightAlignedGreyMiniLabel);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                this.CallNextDrawer(label);
                GUILayout.EndVertical();
                GUIHelper.PushGUIEnabled(true);
                GUILayout.Label(this.labelHelper.GetString(this.Property), SirenixGUIStyles.RightAlignedGreyMiniLabel, GUILayoutOptions.ExpandWidth(false));
                GUIHelper.PopGUIEnabled();
                GUILayout.EndHorizontal();
            }
        }
    }
}
#endif