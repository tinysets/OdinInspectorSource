#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AttributeExampleInfo.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AttributeExampleInfo.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;

    /// <summary>
    /// Descripes an attribute example.
    /// </summary>
    internal class AttributeExampleInfo
    {
        /// <summary>
        /// The type of the example object.
        /// </summary>
        public Type ExampleType;

        /// <summary>
        /// The name of the example.
        /// </summary>
        public string Name;

        /// <summary>
        /// The description of the example.
        /// </summary>
        public string Description;

        /// <summary>
        /// Raw code of the example.
        /// </summary>
        public string Code;

        /// <summary>
        /// Preview object of the example.
        /// </summary>
        public object PreviewObject;

        /// <summary>
        /// Sorting value of the example. Examples with lower order values should come before examples with higher order values.
        /// </summary>
        public int Order;
    }
}
#endif