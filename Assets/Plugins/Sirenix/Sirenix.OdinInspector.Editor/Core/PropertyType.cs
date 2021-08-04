#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyType.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyType.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;

    /// <summary>
    /// Enumeration describing the different types of properties that exist.
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// OBSOLETE: Use <see cref="PropertyType.Value"/> instead.
        /// </summary>
        [Obsolete("OBSOLETE: Use PropertyType.Value instead.", true)]
        ReferenceType = 0,

        /// <summary>
        /// OBSOLETE: Use <see cref="PropertyType.Value"/> instead.
        /// </summary>
        [Obsolete("OBSOLETE: Use PropertyType.Value instead.", true)]
        ValueType = 0,

        /// <summary>
        /// Property represents a value.
        /// </summary>
        Value = 0,

        /// <summary>
        /// Property represents a method.
        /// </summary>
        Method = 1,

        /// <summary>
        /// Property represents a named group of properties.
        /// </summary>
        Group = 2
    }
}
#endif