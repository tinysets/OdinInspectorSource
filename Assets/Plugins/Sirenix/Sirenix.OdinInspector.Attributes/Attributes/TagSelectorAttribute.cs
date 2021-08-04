//-----------------------------------------------------------------------// <copyright file="TagSelectorAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TagSelectorAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>The TagSelector attribute draws a dropdown selector for string properties to select tags from.</para>
    /// <para>Use this to provide an easy way for users to select a tag from the inspector.</para>
    /// </summary>
    /// <example>
    /// <para>The following example demonstrates the use of the TagSelector attribute.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///     [TagSelector]
    ///     public string MyTag;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ValueDropdownAttribute"/>
    /// <seealso cref="AssetListAttribute"/>
    /// <seealso cref="SceneAssetAttribute"/>
    //[AttributeUsage(AttributeTargets.All)]
    //public class TagSelectorAttribute : Attribute
    //{
    //}
}