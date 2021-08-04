#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="IRefreshableResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="IRefreshableResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    public interface IRefreshableResolver
    {
        bool ChildPropertyRequiresRefresh(int index, InspectorPropertyInfo info);
    }
}
#endif