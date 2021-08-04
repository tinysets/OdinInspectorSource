#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="HideInInspectorAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HideInInspectorAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HideInInspector"/>
    /// </summary>
    /// <seealso cref="HideInInspector"/>
    /// <seealso cref="ShowIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    [DrawerPriority(1000, 0, 0)]
    public sealed class HideInInspectorAttributeDrawer : OdinAttributeDrawer<HideInInspector>
    {
        private bool showInInspectorAttribute;
        private PropertyContext<bool> isInReference;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            // The ShowInInspector attribute should always overrule the HideInInspector attribute.
            this.showInInspectorAttribute = this.Property.Attributes.HasAttribute<ShowInInspectorAttribute>();
        }

        /// <summary>
        /// Draws the property under certain conditions.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Draw if we are a collection element
            if (this.showInInspectorAttribute || this.Property.Parent != null && this.Property.Parent.ChildResolver is ICollectionResolver)
            {
                this.CallNextDrawer(label);
                return;
            }

            // Draw if we are in a reference
            if (this.isInReference == null)
            {
                this.isInReference = this.Property.Context.GetGlobal("is_in_reference", false);
            }

            if (this.isInReference.Value)
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif