#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DefaultDrawerChainResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DefaultDrawerChainResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    public class DefaultDrawerChainResolver : DrawerChainResolver
    {
        public static readonly DefaultDrawerChainResolver Instance = new DefaultDrawerChainResolver();

        private static readonly Dictionary<Type, Func<OdinDrawer>> FastDrawerCreators = new Dictionary<Type, Func<OdinDrawer>>(FastTypeComparer.Instance);
        private static readonly List<TypeSearchResult> CachedResultList = new List<TypeSearchResult>(20);

        public override DrawerChain GetDrawerChain(InspectorProperty property)
        {
            List<OdinDrawer> drawers = new List<OdinDrawer>(10);

            DrawerUtilities.GetDefaultPropertyDrawers(property, CachedResultList);

            for (int i = 0; i < CachedResultList.Count; i++)
            {
                drawers.Add(CreateDrawer(CachedResultList[i].MatchedType));
            }

            return new ListDrawerChain(property, drawers);
        }

        private static OdinDrawer CreateDrawer(Type drawerType)
        {
            Func<OdinDrawer> fastCreator;

            if (!FastDrawerCreators.TryGetValue(drawerType, out fastCreator))
            {
                var constructor = drawerType.GetConstructor(Type.EmptyTypes);
                var method = new DynamicMethod(drawerType.FullName + "_FastCreator", typeof(OdinDrawer), Type.EmptyTypes);

                var il = method.GetILGenerator();

                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);

                fastCreator = (Func<OdinDrawer>)method.CreateDelegate(typeof(Func<OdinDrawer>));
                FastDrawerCreators.Add(drawerType, fastCreator);
            }

            return fastCreator();
        }
    }
}
#endif