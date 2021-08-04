#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="CustomValueDrawerExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEditor;

    [AttributeExample(typeof(CustomValueDrawerAttribute))]
    internal class CustomValueDrawerExamples
    {
        public float From = 2, To = 7;

        [CustomValueDrawer("MyStaticCustomDrawerStatic")]
        public float CustomDrawerStatic;

        [CustomValueDrawer("MyStaticCustomDrawerInstance")]
        public float CustomDrawerInstance;

        [CustomValueDrawer("MyStaticCustomDrawerArray")]
        public float[] CustomDrawerArray;

        private static float MyStaticCustomDrawerStatic(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, value, 0f, 10f);
        }

        private float MyStaticCustomDrawerInstance(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(label, value, this.From, this.To);
        }

        private float MyStaticCustomDrawerArray(float value, GUIContent label)
        {
            return EditorGUILayout.Slider(value, this.From, this.To);
        }
    }
}
#endif