#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="FoldoutGroupAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="FoldoutGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="FoldoutGroupAttribute"/>
    /// </summary>
    /// <seealso cref="FoldoutGroupAttribute"/>
    public class FoldoutGroupAttributeDrawer : OdinGroupDrawer<FoldoutGroupAttribute>
    {
        private LocalPersistentContext<bool> IsVisible;
        private StringMemberHelper TitleHelper;
        private float t;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            IsVisible = this.GetPersistentValue("IsVisible", this.Attribute.HasDefinedExpanded ? this.Attribute.Expanded : SirenixEditorGUI.ExpandFoldoutByDefault);
            this.TitleHelper = new StringMemberHelper(this.Property, this.Attribute.GroupName);
            this.t = this.IsVisible.Value ? 1 : 0;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var attribute = this.Attribute;

            if (this.TitleHelper.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.TitleHelper.ErrorMessage);
            }

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            var content = GUIHelper.TempContent(this.TitleHelper.GetString(property));
            this.IsVisible.Value = SirenixEditorGUI.Foldout(this.IsVisible.Value, content);
            SirenixEditorGUI.EndBoxHeader();
            if (Event.current.type == EventType.Layout)
            {
                EditorTimeHelper.Time.Update();
                this.t = Mathf.MoveTowards(this.t, this.IsVisible.Value ? 1 : 0, EditorTimeHelper.Time.DeltaTime * (1f / SirenixEditorGUI.DefaultFadeGroupDuration));
            }

            if (SirenixEditorGUI.BeginFadeGroup(this.t))
            {
                for (int i = 0; i < property.Children.Count; i++)
                {
                    var child = property.Children[i];
                    child.Draw(child.Label);
                }
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }
    }
}
#endif