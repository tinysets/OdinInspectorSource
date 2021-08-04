#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="FolderPathAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="FolderPathAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System.IO;
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    public sealed class FolderPathAttributeDrawer : OdinAttributeDrawer<FolderPathAttribute, string>, IDefinesGenericMenuItems
    {
        private InspectorProperty parentProperty;
        private StringMemberHelper parentPath;
        private bool requireExistingPath;
        private bool exists;

        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            this.requireExistingPath = this.Attribute.RequireExistingPath || this.Attribute.RequireValidPath;
#pragma warning restore CS0618 // Type or member is obsolete

            this.parentProperty = this.Property.FindParent(p => p.Info.HasSingleBackingMember, true);
            this.parentPath = new StringMemberHelper(this.parentProperty, this.Attribute.ParentFolder);
            this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parentPath.GetString(this.Property));
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Display evt. errors in creating context.
            if (this.parentPath.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.parentPath.ErrorMessage);
            }

            // Gotta check that constantly, the value can change from outside the inspector!
            if (this.requireExistingPath && Event.current.type == EventType.Layout)
            {
                this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parentPath.GetString(this.Property));
            }

            // Display required valid path error if enabled.
            if (this.requireExistingPath && this.exists == false)
            {
                SirenixEditorGUI.ErrorMessageBox("The path is does not exist.");
            }

            // Draw field.
            EditorGUI.BeginChangeCheck();
            this.ValueEntry.SmartValue = SirenixEditorFields.FolderPathField(label, this.ValueEntry.SmartValue, this.parentPath.GetString(this.Property), this.Attribute.AbsolutePath, this.Attribute.UseBackslashes);

            // Update existing check
            if (EditorGUI.EndChangeCheck() && requireExistingPath)
            {
                this.exists = this.PathExists(this.ValueEntry.SmartValue, this.parentPath.GetString(this.Property));
            }
        }

        private bool PathExists(string path, string parent)
        {
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            else if (parent.IsNullOrWhitespace() == false)
            {
                path = Path.Combine(parent, path);
            }

            return Directory.Exists(path);
        }

        /// <summary>
        /// Adds customs generic menu options.
        /// </summary>
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var parentProperty = property.FindParent(p => p.Info.HasSingleBackingMember, true);
            IPropertyValueEntry<string> entry = (IPropertyValueEntry<string>)property.ValueEntry;
            string parent = this.parentPath.GetString(parentProperty);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            bool exists = false;
            string createDirectoryPath = entry.SmartValue;

            if (createDirectoryPath.IsNullOrWhitespace() == false)
            {
                // Get the absolute path.
                if (Path.IsPathRooted(createDirectoryPath) == false)
                {
                    if (parent.IsNullOrWhitespace() == false)
                    {
                        createDirectoryPath = Path.Combine(parent, createDirectoryPath);
                    }

                    createDirectoryPath = Path.GetFullPath(createDirectoryPath);
                }

                exists = Directory.Exists(createDirectoryPath);
            }

            string showInExplorerPath = createDirectoryPath;
            if (showInExplorerPath.IsNullOrWhitespace())
            {
                if (parent.IsNullOrWhitespace() == false)
                {
                    // Use parent path instead.
                    showInExplorerPath = Path.GetFullPath(parent);
                }
                else
                {
                    // Default to Unity project path.
                    showInExplorerPath = Path.GetDirectoryName(Application.dataPath);
                }
            }

            // Find first existing path to open.
            while (showInExplorerPath.IsNullOrWhitespace() == false && Directory.Exists(showInExplorerPath) == false)
            {
                showInExplorerPath = Path.GetDirectoryName(showInExplorerPath);
            }

            // Show in explorer
            if (showInExplorerPath.IsNullOrWhitespace() == false)
            {
                genericMenu.AddItem(new GUIContent("Show in explorer"), false, () => Application.OpenURL(showInExplorerPath));
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Show in explorer"));
            }

            // Create path
            if (exists || createDirectoryPath.IsNullOrWhitespace()) // Disable the create path option, if the directory already exists, or the path is invalid.
            {
                genericMenu.AddDisabledItem(new GUIContent("Create directory"));
            }
            else
            {
                genericMenu.AddItem(new GUIContent("Create directory"), false, () =>
                {
                    Directory.CreateDirectory(createDirectoryPath);
                    AssetDatabase.Refresh();
                });
            }
        }
    }
}
#endif