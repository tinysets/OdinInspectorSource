#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ReferenceDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ReferenceDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Reflection;

    /// <summary>
    /// Draws all reference type properties, which has already been drawn elsewhere. This drawer adds an additional foldout to prevent infinite draw depth.
    /// </summary>
    [AllowGUIEnabledForReadonly]
    [DrawerPriority(90, 0, 0)]
    public sealed class ReferenceDrawer<T> : OdinValueDrawer<T> where T : class
    {
        private LocalPersistentContext<bool> isToggled;
        private bool drawAsReference;
        private bool hideReferenceBox;

        /// <summary>
        /// Prevents the drawer from being applied to UnityEngine.Object references since they are shown as an object field, and is not drawn in-line.
        /// </summary>
        public override bool CanDrawTypeFilter(Type type)
        {
            return
                !typeof(MemberInfo).IsAssignableFrom(type) &&
                !typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            return base.CanDrawValueProperty(property) && !property.Attributes.HasAttribute<DoNotDrawAsReferenceAttribute>();
        }

        protected override void Initialize()
        {
            this.isToggled = this.GetPersistentValue("is_Toggled", false);
            this.hideReferenceBox = this.Property.Attributes.HasAttribute<HideDuplicateReferenceBoxAttribute>();
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;

            if (Event.current.type == EventType.Layout)
            {
                this.drawAsReference = entry.ValueState == PropertyValueState.Reference;
            }

            if (this.drawAsReference)
            {
                var targetProp = entry.Property.Tree.GetPropertyAtPath(entry.TargetReferencePath);

                if (targetProp == null)
                {
                    GUILayout.Label("Reference to " + entry.TargetReferencePath + ". But no property was found at path, which is a problem.");
                    return;
                }

                var isInReference = targetProp.Context.GetGlobal("is_in_reference", false);

                bool drawReferenceBox = true;

                if (!isInReference.Value)
                {
                    drawReferenceBox = !this.hideReferenceBox;
                }

                if (drawReferenceBox)
                {
                    SirenixEditorGUI.BeginToolbarBox();
                    SirenixEditorGUI.BeginToolbarBoxHeader();
                    Rect valueRect;
                    this.isToggled.Value = SirenixEditorGUI.Foldout(this.isToggled.Value, label, out valueRect);
                    GUI.Label(valueRect, "Reference to " + targetProp.Path, SirenixGUIStyles.LeftAlignedGreyMiniLabel);
                    SirenixEditorGUI.EndToolbarBoxHeader();
                    if (SirenixEditorGUI.BeginFadeGroup(entry.Context.Get(this, "k", 0), this.isToggled.Value))
                    {
                        bool previous = isInReference.Value;
                        isInReference.Value = true;
                        targetProp.Draw(label);
                        isInReference.Value = previous;
                    }
                    SirenixEditorGUI.EndFadeGroup();
                    SirenixEditorGUI.EndToolbarBox();
                }
                else
                {
                    bool previous = isInReference.Value;
                    isInReference.Value = true;
                    targetProp.Draw(label);
                    isInReference.Value = previous;
                }
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif