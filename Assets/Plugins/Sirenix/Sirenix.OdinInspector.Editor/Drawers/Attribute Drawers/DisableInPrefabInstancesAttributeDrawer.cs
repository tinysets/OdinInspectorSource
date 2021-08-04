#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DisableInPrefabInstancesAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HideInInlineEditorsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="DisableInPrefabInstancesAttribute"/>.
    /// </summary>

    [DrawerPriority(1000, 0, 0)]
    public sealed class DisableInPrefabInstancesAttributeDrawer : OdinAttributeDrawer<DisableInPrefabInstancesAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var unityObjectTarget = property.Tree.WeakTargets[0] as UnityEngine.Object;

            if (unityObjectTarget == null)
            {
                this.CallNextDrawer(label);
                return;
            }

            PropertyContext<bool> disable;

            if (property.Context.Get(this, "disable", out disable))
            {
                var type = PrefabUtility.GetPrefabType(unityObjectTarget);
                disable.Value =
                    type == PrefabType.ModelPrefabInstance ||
                    type == PrefabType.PrefabInstance;
            }

            if (disable.Value)
            {
                GUIHelper.PushGUIEnabled(false);
                this.CallNextDrawer(label);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif