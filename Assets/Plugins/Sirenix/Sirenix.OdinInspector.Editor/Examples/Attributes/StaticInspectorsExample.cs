#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="StaticInspectorsExample.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    [AttributeExample(typeof(ShowInInspectorAttribute), "You can use the ShowInInspector attribute on static members to make them appear in the inspector as well.")]
    internal class StaticInspectorsExample
    {
        [ShowInInspector]
        public static List<MySomeStruct> SomeStaticField = new List<MySomeStruct>();

        [ShowInInspector, PropertyRange(0, 0.1f)]
        public static float FixedDeltaTime
        {
            get { return Time.fixedDeltaTime; }
            set { Time.fixedDeltaTime = value; }
        }

        [Button(ButtonSizes.Large), PropertyOrder(-1)]
        public static void AddToList()
        {
            int count = SomeStaticField.Count + 1000;
            while (SomeStaticField.Count < count)
            {
                SomeStaticField.Add(new MySomeStruct());
            }
        }

        [Serializable]
        public struct MySomeStruct
        {
            [HideLabel, PreviewField(45)]
            [HorizontalGroup("Split", width: 45)]
            public Texture2D Icon;

            [FoldoutGroup("Split/$Icon")]
            [HorizontalGroup("Split/$Icon/Properties", LabelWidth = 40)]
            public int Foo;

            [HorizontalGroup("Split/$Icon/Properties")]
            public int Bar;
        }
    }
}
#endif