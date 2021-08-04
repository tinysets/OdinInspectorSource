#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinDontRegisterAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DontRegisterDrawerAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;

    /// <summary>
    /// Use this attribute to prevent a type from being included in Odin systems.
    /// The attribute can be applied to Odin drawers, Odin property resolvers and Odin attribute processor types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OdinDontRegisterAttribute : Attribute
    {
    }
}
#endif