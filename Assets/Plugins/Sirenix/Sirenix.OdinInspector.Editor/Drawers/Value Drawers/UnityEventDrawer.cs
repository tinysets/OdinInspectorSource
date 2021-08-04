#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="UnityEventDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="UnityEventDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;
    using System;
    using System.Reflection;
    using Sirenix.Utilities;

    /// <summary>
    /// Unity event drawer.
    /// </summary>
    [DrawerPriority(0, 0, 0.99)]
    public sealed class UnityEventDrawer<T> : UnityPropertyDrawer<UnityEditorInternal.UnityEventDrawer, T> where T : UnityEventBase
    {
        private class DrawerPropertyHandlerBuffer
        {
            public UnityEditorInternal.UnityEventDrawer Drawer;
            public object PropertyHandler;
        }

        protected override void Initialize()
        {
            base.Initialize();
            base.delayApplyValueUntilRepaint = true;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            PropertyContext<DrawerPropertyHandlerBuffer> buffer;
            if (entry.Context.Get(this, "DrawerPropertyHandlerBuffer", out buffer))
            {
                buffer.Value = new DrawerPropertyHandlerBuffer
                {
                    Drawer = new UnityEditorInternal.UnityEventDrawer()
                };

                if (UnityPropertyHandlerUtility.IsAvailable)
                {
                    buffer.Value.PropertyHandler = UnityPropertyHandlerUtility.CreatePropertyHandler(buffer.Value.Drawer);
                }
            }

            this.drawer = buffer.Value.Drawer;
            this.propertyHandler = buffer.Value.PropertyHandler;

            FieldInfo fieldInfo;
            SerializedProperty unityProperty = entry.Property.Tree.GetUnityPropertyForPath(entry.Property.Path, out fieldInfo);

            if (unityProperty == null)
            {
                if (UnityVersion.IsVersionOrGreater(2017, 1))
                {
                    this.CallNextDrawer(label);
                    return;
                }
                else if (!typeof(T).IsDefined<SerializableAttribute>())
                {
                    SirenixEditorGUI.ErrorMessageBox("You have likely forgotten to mark your custom UnityEvent class '" + typeof(T).GetNiceName() + "' with the [Serializable] attribute! Could not get a Unity SerializedProperty for the property '" + entry.Property.NiceName + "' of type '" + entry.TypeOfValue.GetNiceName() + "' at path '" + entry.Property.Path + "'.");
                    return;
                }
            }

            base.DrawPropertyLayout(label);
        }
    }
}
#endif