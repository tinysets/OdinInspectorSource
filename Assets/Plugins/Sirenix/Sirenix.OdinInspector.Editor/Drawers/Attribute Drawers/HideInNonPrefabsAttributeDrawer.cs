#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="HideInNonPrefabsAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="HideInNonPrefabsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HideInNonPrefabsAttribute"/>.
    /// </summary>

    [DrawerPriority(1000, 0, 0)]
    public sealed class HideInNonPrefabsAttributeDrawer : OdinAttributeDrawer<HideInNonPrefabsAttribute>
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

            PropertyContext<bool> hide;

            if (property.Context.Get(this, "hide", out hide))
            {
                var type = PrefabUtility.GetPrefabType(unityObjectTarget);
                hide.Value =
                    type == PrefabType.None ||
                    type == PrefabType.MissingPrefabInstance ||
                    type == PrefabType.DisconnectedModelPrefabInstance ||
                    type == PrefabType.DisconnectedPrefabInstance;
            }

            if (hide.Value == false)
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif