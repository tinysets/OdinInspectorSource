#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="RequiredAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="RequiredAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;
    using Sirenix.Utilities.Editor;

    /// <summary>
    /// Draws properties marked with <see cref="RequiredAttribute"/>.
    /// </summary>
    /// <seealso cref="RequiredAttribute"/>
    /// <seealso cref="InfoBoxAttribute"/>
    /// <seealso cref="ValidateInputAttribute"/>
    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public sealed class RequiredAttributeDrawer<T> : OdinAttributeDrawer<RequiredAttribute, T> where T: class
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Message context.
            PropertyContext<StringMemberHelper> context = null;
            if (this.Attribute.ErrorMessage != null)
            {
                context = this.Property.Context.Get<StringMemberHelper>(this, "ErrorMessage", (StringMemberHelper)null);
                if (context.Value == null)
                {
                    //context.Value = new StringMemberHelper(property.ParentType, attribute.ErrorMessage);
                    context.Value = new StringMemberHelper(this.Property, this.Attribute.ErrorMessage);
                }

                if (context.Value.ErrorMessage != null)
                {
                    SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
                }
            }

            var isMissing = CheckIsMissing(this.Property);

            if (isMissing)
            {
                string msg = this.Attribute.ErrorMessage != null ? context.Value.GetString(this.Property) : (this.Property.NiceName + " is required.");
                if (this.Attribute.MessageType == InfoMessageType.Warning)
                {
                    SirenixEditorGUI.WarningMessageBox(msg);
                }
                else if (this.Attribute.MessageType == InfoMessageType.Error)
                {
                    SirenixEditorGUI.ErrorMessageBox(msg);
                }
                else
                {
                    EditorGUILayout.HelpBox(msg, (MessageType)this.Attribute.MessageType);
                }
            }

            var key = UniqueDrawerKey.Create(this.Property, this);
            SirenixEditorGUI.BeginShakeableGroup(key);
            this.CallNextDrawer(label);
            SirenixEditorGUI.EndShakeableGroup(key);

            if (!isMissing && CheckIsMissing(this.Property))
            {
                SirenixEditorGUI.StartShakingGroup(key);
            }
        }

        private static bool CheckIsMissing(InspectorProperty property)
        {
            bool isMissing = property.ValueEntry.WeakSmartValue == null;

            if (isMissing == false && property.ValueEntry.WeakSmartValue is UnityEngine.Object)
            {
                var unityObject = property.ValueEntry.WeakSmartValue as UnityEngine.Object;
                if (unityObject == null)
                {
                    isMissing = true;
                }
            }
            else if (isMissing == false && property.ValueEntry.WeakSmartValue is string)
            {
                if (string.IsNullOrEmpty((string)property.ValueEntry.WeakSmartValue))
                {
                    isMissing = true;
                }
            }

            return isMissing;
        }
    }
}
#endif