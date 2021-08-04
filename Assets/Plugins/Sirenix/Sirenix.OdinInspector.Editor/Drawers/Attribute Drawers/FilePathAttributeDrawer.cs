#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="FilePathAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="FilePathAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    public sealed class FilePathAttributeDrawer : OdinAttributeDrawer<FilePathAttribute, string>, IDefinesGenericMenuItems
    {
        private bool requireExistingPath;
        private InspectorProperty parentProperty;
        private StringMemberHelper parent;
        private StringMemberHelper extensions;
        private string errorMessage;
        private bool exists;

        //private class FilePathContext
        //{
        //    public StringMemberHelper Parent;
        //    public string ErrorMessage;
        //    public StringMemberHelper Extensions;
        //    public bool Exists;
        //}

        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
#pragma warning disable CS0618 // Type or member is obsolete.
            this.requireExistingPath = this.Attribute.RequireExistingPath || this.Attribute.RequireValidPath;
#pragma warning restore CS0618 // Type or member is obsolete.

            this.parentProperty = this.Property.FindParent(p => p.Info.HasSingleBackingMember, true);
            this.parent = new StringMemberHelper(this.parentProperty, this.Attribute.ParentFolder, ref this.errorMessage);
            this.extensions = new StringMemberHelper(this.parentProperty, this.Attribute.Extensions, ref this.errorMessage);

            this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parent.GetString(this.Property));
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Display evt. errors in creating property context.
            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }

            // Gotta check that constantly, the value can change from outside the inspector!
            if (requireExistingPath && Event.current.type == EventType.Layout)
            {
                this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parent.GetString(this.Property));
            }

            // Display required path error, if enabled.
            if (this.requireExistingPath && this.exists == false)
            {
                SirenixEditorGUI.ErrorMessageBox("The path does not exist.");
            }

            // Draw the field.
            EditorGUI.BeginChangeCheck();
            this.ValueEntry.SmartValue = SirenixEditorFields.FilePathField(
                label,
                this.ValueEntry.SmartValue,
                this.parent.GetString(this.parentProperty),
                this.extensions.GetString(this.ValueEntry),
                this.Attribute.AbsolutePath,
                this.Attribute.UseBackslashes);

            // Update exists check.
            if (EditorGUI.EndChangeCheck() && this.requireExistingPath)
            {
                this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parent.GetString(this.Property));
                GUI.changed = true;
            }
        }

        private bool PathExists(string path, string parent)
        {
            if (path.IsNullOrWhitespace())
            {
                return false;
            }

            if (parent.IsNullOrWhitespace() == false)
            {
                path = Path.Combine(parent, path);
            }

            return File.Exists(path);
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var parentProperty = property.FindParent(p => p.Info.HasSingleBackingMember, true);
            IPropertyValueEntry<string> entry = (IPropertyValueEntry<string>)property.ValueEntry;
            string parent = this.parent.GetString(parentProperty);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            string path = entry.SmartValue;

            // Create an absolute path from the current value.
            if (path.IsNullOrWhitespace() == false)
            {
                if (Path.IsPathRooted(path) == false)
                {
                    if (parent.IsNullOrWhitespace() == false)
                    {
                        path = Path.Combine(parent, path);
                    }

                    path = Path.GetFullPath(path);
                }
            }
            else if (parent.IsNullOrWhitespace() == false)
            {
                // Use the parent path instead.
                path = Path.GetFullPath(parent);
            }
            else
            {
                // Default to Unity project.
                path = Path.GetDirectoryName(Application.dataPath);
            }

            // Find first existing directory.
            if (path.IsNullOrWhitespace() == false)
            {
                while (path.IsNullOrWhitespace() == false && Directory.Exists(path) == false)
                {
                    path = Path.GetDirectoryName(path);
                }
            }

            // Show in explorer
            if (path.IsNullOrWhitespace() == false)
            {
                genericMenu.AddItem(new GUIContent("Show in explorer"), false, () => System.Diagnostics.Process.Start(path));
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Show in explorer"));
            }
        }
    }
}
#endif