#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ResolverUtilities.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ResolverUtilities.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class ResolverUtilities
    {
        public static IEnumerable<Assembly> GetResolverAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().Where(a =>
            {
                if (a.IsDefined(typeof(ContainsOdinResolversAttribute), true)) return true;

                var flag = AssemblyUtilities.GetAssemblyTypeFlag(a);

                return (flag & AssemblyTypeFlags.CustomTypes) != 0;
            });
        }

        public static double GetResolverPriority(Type resolverType)
        {
            var attr = resolverType.GetAttribute<ResolverPriorityAttribute>(inherit: true);
            if (attr != null) return attr.Priority;

            if (resolverType.Assembly == typeof(OdinEditor).Assembly)
            {
                return -0.1;
            }

            return 0;
        }
    }
}
#endif