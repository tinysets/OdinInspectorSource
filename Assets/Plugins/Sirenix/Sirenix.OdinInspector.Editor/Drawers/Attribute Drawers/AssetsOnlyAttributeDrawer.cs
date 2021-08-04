#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AssetsOnlyAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AssetsOnlyAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// Draws Unity object properties marked with <see cref="AssetsOnlyAttribute"/>.
    /// </summary>
    /// <seealso cref="AssetsOnlyAttribute"/>
    /// <seealso cref="SceneObjectsOnlyAttribute"/>
    /// <seealso cref="AssetListAttribute"/>
    /// <seealso cref="RequiredAttribute"/>
    /// <seealso cref="ValidateInputAttribute"/>

    [DrawerPriority(DrawerPriorityLevel.WrapperPriority)]
    public sealed class AssetsOnlyAttributeDrawer<T> : OdinAttributeDrawer<AssetsOnlyAttribute, T> where T : UnityEngine.Object
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;

            for (int i = 0; i < entry.Values.Count; i++)
            {
                var val = entry.Values[i];
                if (val != null)
                {
                    if (AssetDatabase.Contains(val) == false)
                    {
                        string name = val.name;
                        var component = val as Component;
                        if (component != null)
                        {
                            name = "from " + component.gameObject.name;
                        }

                        SirenixEditorGUI.ErrorMessageBox((val as object).GetType().GetNiceName() + " " + name + " is not an asset.");
                        break;
                    }
                }
            }
            this.CallNextDrawer(label);
        }
    }
}
#endif