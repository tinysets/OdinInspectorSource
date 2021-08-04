#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="BoxGroupAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="BoxGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="BoxGroupAttribute"/>
    /// </summary>
    /// <seealso cref="BoxGroupAttribute"/>
    public class BoxGroupAttributeDrawer : OdinGroupDrawer<BoxGroupAttribute>
    {
        private StringMemberHelper labelGetter;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.labelGetter = new StringMemberHelper(this.Property, this.Attribute.GroupName);
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            string headerLabel = null;
            if (this.Attribute.ShowLabel)
            {
                headerLabel = this.labelGetter.GetString(this.Property);
                if (string.IsNullOrEmpty(headerLabel))
                {
                    headerLabel = "Null"; // The user has asked for a header. So he gets a header.
                }
            }

            SirenixEditorGUI.BeginBox(headerLabel, this.Attribute.CenterLabel);

            for (int i = 0; i < this.Property.Children.Count; i++)
            {
                var child = this.Property.Children[i];
                child.Draw(child.Label);
            }

            SirenixEditorGUI.EndBox();
        }
    }
}
#endif