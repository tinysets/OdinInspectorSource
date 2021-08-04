#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AnimationCurveDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AnimationCurveDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities;
    using System;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// Animation curve property drawer.
    /// </summary>
    public sealed class AnimationCurveDrawer : DrawWithUnityBaseDrawer<AnimationCurve>
    {
        private static Action clearCache;

        static AnimationCurveDrawer()
        {
            MethodInfo mi = null;
            var type = AssemblyUtilities.GetTypeByCachedFullName("UnityEditorInternal.AnimationCurvePreviewCache");
            if (type != null)
            {
                var method = type.GetMethod("ClearCache", Flags.StaticAnyVisibility);
                var pars = method.GetParameters();
                if (pars != null && pars.Length == 0)
                {
                    mi = method;
                }
            }

            if (mi != null)
            {
                clearCache = EmitUtilities.CreateStaticMethodCaller(mi);
            }
#if SIRENIX_INTERNAL
            else
            {
                Debug.LogError("AnimationCurve fix no longer works, has Unity fixed it?");
            }
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (clearCache != null)
            {
                // Unity bugfix:
                // The preview of animations curves doesn't work well with reordering, 
                // I suspect they use ControlId's as the pointer to the preview cache lookup.
                clearCache();
            }
        }
    }
}
#endif