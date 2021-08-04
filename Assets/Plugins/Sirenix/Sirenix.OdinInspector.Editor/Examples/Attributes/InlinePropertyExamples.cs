#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InlinePropertyExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using UnityEngine;

    [AttributeExample(typeof(InlinePropertyAttribute))]
    internal class InlinePropertyExamples
    {
        public Vector3 Vector3;

        public Vector3Int MyVector3Int;

        [InlineProperty(LabelWidth = 13)]
        public Vector2Int MyVector2Int;
   
        [Serializable]
        [InlineProperty(LabelWidth = 13)]
        public struct Vector3Int
        {
            [HorizontalGroup]
            public int X;

            [HorizontalGroup]
            public int Y;

            [HorizontalGroup]
            public int Z;
        }

        [Serializable]
        public struct Vector2Int
        {
            [HorizontalGroup]
            public int X;

            [HorizontalGroup]
            public int Y;
        }
    }
}
#endif