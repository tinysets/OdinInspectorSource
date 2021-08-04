#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TypeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TypeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Serialization;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Type property drawer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DrawerPriority(0, 0, 2001)]
    public class TypeDrawer<T> : OdinValueDrawer<T> where T : Type
    {
        private static readonly TwoWaySerializationBinder Binder = new DefaultSerializationBinder();

        private class Context
        {
            public string TypeNameTemp;
            public bool IsValid = true;
            public string UniqueControlName;
            public bool WasFocusedControl;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            Context context;
            if (entry.Context.Get(this, "context", out context))
            {
                context.UniqueControlName = Guid.NewGuid().ToString();
            }

            if (!context.IsValid)
            {
                GUIHelper.PushColor(Color.red);
            }

            GUI.SetNextControlName(context.UniqueControlName);
            
            Rect rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            
            Rect fieldRect = rect;
            Rect dropdownRect = rect.AlignRight(18);

            // Dropdown button.
            EditorGUIUtility.AddCursorRect(dropdownRect, MouseCursor.Arrow);
            if (GUI.Button(dropdownRect, GUIContent.none, GUIStyle.none))
            {
                TypeSelector selector = new TypeSelector(AssemblyTypeFlags.All, false);

                selector.SelectionConfirmed += t =>
                {
                    var type = t.FirstOrDefault();

                    entry.Property.Tree.DelayAction(() =>
                    {
                        entry.WeakSmartValue = type;
                        context.IsValid = true;
                        entry.ApplyChanges();
                    });
                };

                selector.SetSelection(entry.SmartValue);
                selector.ShowInPopup(rect, 350);
            }

            // Reset type name.
            if (Event.current.type == EventType.Layout)
            {
                context.TypeNameTemp = entry.SmartValue != null ? Binder.BindToName(entry.SmartValue) : null;
            }

            EditorGUI.BeginChangeCheck();
            context.TypeNameTemp = SirenixEditorFields.DelayedTextField(fieldRect, context.TypeNameTemp);

            // Draw dropdown button.
            EditorIcons.TriangleDown.Draw(dropdownRect);

            if (!context.IsValid)
            {
                GUIHelper.PopColor();
            }

            bool isFocused = GUI.GetNameOfFocusedControl() == context.UniqueControlName;
            bool defocused = false;

            if (isFocused != context.WasFocusedControl)
            {
                defocused = !isFocused;
                context.WasFocusedControl = isFocused;
            }

            if (EditorGUI.EndChangeCheck())
            {
                if (context.TypeNameTemp == null || string.IsNullOrEmpty(context.TypeNameTemp.Trim()))
                {
                    // String is empty
                    entry.SmartValue = null;
                    context.IsValid = true;
                }
                else
                {
                    Type type = Binder.BindToType(context.TypeNameTemp);

                    if (type == null)
                    {
                        type = AssemblyUtilities.GetTypeByCachedFullName(context.TypeNameTemp);
                    }

                    if (type == null)
                    {
                        context.IsValid = false;
                    }
                    else
                    {
                        // Use WeakSmartValue in case of a different Type-derived instance showing up somehow, so we don't get cast errors
                        entry.WeakSmartValue = type;
                        context.IsValid = true;
                    }
                }
            }

            if (defocused)
            {
                // Ensure we show the full type name when the control is defocused
                context.TypeNameTemp = entry.SmartValue == null ? "" : Binder.BindToName(entry.SmartValue);
                context.IsValid = true;
            }
        }
    }
}
#endif