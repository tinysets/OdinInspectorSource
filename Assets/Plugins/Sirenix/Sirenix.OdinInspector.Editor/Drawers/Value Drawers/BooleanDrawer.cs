#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="BooleanDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="BooleanDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Bool property drawer.
    /// </summary>
    public sealed class BooleanDrawer : OdinValueDrawer<bool>
    {
        private GUILayoutOption[] options;

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            bool value = this.ValueEntry.SmartValue;
            EditorGUI.BeginChangeCheck();
            if (label == null)
            {
                options = options ?? new GUILayoutOption[] { GUILayout.Width(15) };
                var rect = EditorGUILayout.GetControlRect(false, options);
                value = EditorGUI.Toggle(rect, value);
            }
            else
            {
                value = EditorGUILayout.Toggle(label, value);
            }
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = value;
            }
        }
    }
}
#endif