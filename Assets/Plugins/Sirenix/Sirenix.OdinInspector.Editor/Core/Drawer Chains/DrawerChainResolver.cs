#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DrawerChainResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DrawerChainResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    public abstract class DrawerChainResolver
    {
        public abstract DrawerChain GetDrawerChain(InspectorProperty property);
    }
}
#endif