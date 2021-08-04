#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="LabelTextAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="LabelTextAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;
    using UnityEditor;

    /// <summary>
    /// Draws properties marked with <see cref="LabelTextAttribute"/>.
    /// Creates a new GUIContent, with the provided label text, before calling further down in the drawer chain.
    /// </summary>
    /// <seealso cref="LabelTextAttribute"/>
    /// <seealso cref="HideLabelAttribute"/>
    /// <seealso cref="TooltipAttribute"/>
    /// <seealso cref="LabelWidthAttribute"/>
    /// <seealso cref="TitleAttribute"/>
    /// <seealso cref="HeaderAttribute"/>
    /// <seealso cref="GUIColorAttribute"/>

    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class LabelTextAttributeDrawer : OdinAttributeDrawer<LabelTextAttribute>
    {
        private GUIContent overrideLabel;
        private StringMemberHelper stringHelper;

        protected override void Initialize()
        {
            this.stringHelper = new StringMemberHelper(this.Property, this.Attribute.Text);
        }

        /// <summary>
        /// Draws the attribute.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.stringHelper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.stringHelper.ErrorMessage);
            }
            
            GUIContent useLabel;
            
            var str = this.stringHelper.GetString(this.Property);

            if (str == null)
            {
                useLabel = null;
            }
            else
            {
                if (overrideLabel == null) overrideLabel = new GUIContent();
                overrideLabel.text = str;
                useLabel = overrideLabel;
            }
            
            this.CallNextDrawer(useLabel);
        }
    }
}
#endif