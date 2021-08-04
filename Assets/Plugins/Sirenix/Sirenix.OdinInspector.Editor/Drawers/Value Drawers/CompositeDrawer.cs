#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="CompositeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="CompositeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Drawer for composite properties.
    /// </summary>
    [DrawerPriority(0, 0, 0.1)]
    public sealed class CompositeDrawer : OdinDrawer
    {
        private LocalPersistentContext<bool> isVisisble;

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            if (property.Children.Count == 0)
            {
                if (property.ValueEntry != null)
                {
                    if (label != null)
                    {
                        var rect = EditorGUILayout.GetControlRect();
                        GUI.Label(rect, label);
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        if (label != null)
                        {
                            EditorGUILayout.PrefixLabel(label);
                        }
                        SirenixEditorGUI.WarningMessageBox("There is no drawer defined for property " + property.NiceName + " of type " + property.Info.PropertyType + ".");
                    }
                    GUILayout.EndHorizontal();
                }

                return;
            }

            if (label == null)
            {
                for (int i = 0; i < property.Children.Count; i++)
                {
                    var child = property.Children[i];
                    child.Draw(child.Label);
                }
            }
            else
            {
                if (this.isVisisble == null)
                {
                    this.isVisisble = property.Context.GetPersistent(this, "IsVisible", SirenixEditorGUI.ExpandFoldoutByDefault);
                }
                this.isVisisble.Value = SirenixEditorGUI.Foldout(this.isVisisble.Value, label);
                if (SirenixEditorGUI.BeginFadeGroup(this, this.isVisisble.Value))
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.Children.Count; i++)
                    {
                        var child = property.Children[i];
                        child.Draw(child.Label);
                    }
                    EditorGUI.indentLevel--;
                }
                SirenixEditorGUI.EndFadeGroup();
            }
        }

        public override bool CanDrawProperty(InspectorProperty property)
        {
            return property.ValueEntry != null;// && !(property.ChildResolver is ICollectionResolver)/* && property.Children.Count > 0*/;
        }
    }
}
#endif