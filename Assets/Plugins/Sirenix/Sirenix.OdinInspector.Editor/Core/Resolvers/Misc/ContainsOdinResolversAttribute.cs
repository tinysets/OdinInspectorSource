#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ContainsOdinResolversAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ContainsOdinResolversAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: Sirenix.OdinInspector.Editor.ContainsOdinResolvers]

namespace Sirenix.OdinInspector.Editor
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class ContainsOdinResolversAttribute : Attribute
    {
    }
}
#endif