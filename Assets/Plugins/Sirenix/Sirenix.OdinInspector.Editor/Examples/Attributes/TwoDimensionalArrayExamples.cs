#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TwoDimensionalArrayExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    // Inheriting from SerializedMonoBehaviour is only needed if you want Odin to serialize the multi-dimensional arrays for you.
    // If you prefer doing that yourself, you can still make Odin show them in the inspector using the ShowInInspector attribute.
    [ShowOdinSerializedPropertiesInInspector]
    [AttributeExample(typeof(TableMatrixAttribute))]
    internal class TwoDimensionalArrayExamples
    {
        // Unity does not serialize multi-dimensional arrays.
        // By inheriting from something like SerializedMonoBehaviour you can have Odin serialize multi-dimensional arrays for you.
        // If you prefer doing that yourself, you can still make Odin show them in the inspector using the ShowInInspector attribute.

        public bool[,] BooleanMatrix = new bool[15, 6];

        [TableMatrix(SquareCells = true)]
        public Texture2D[,] TextureMatrix = new Texture2D[8, 6];

        public InfoMessageType[,] EnumMatrix = new InfoMessageType[4, 4];

        public string[,] StringMatrix = new string[4, 4];
    }
}
#endif