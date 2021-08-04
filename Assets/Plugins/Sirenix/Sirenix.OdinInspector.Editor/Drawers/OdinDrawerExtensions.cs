#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinDrawerExtensions.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="OdinDrawerExtensions.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Serialization;
    using System;
    using Sirenix.OdinInspector.Editor.Drawers;

    /// <summary>
    /// OdinDrawer extensions.
    /// </summary>
    public static class OdinDrawerExtensions
    {
        /// <summary>
        /// Gets a persistent value that will survive past multiple Unity Editor Application sessions. 
        /// The value is stored in the PersistentContextCache, which has a customizable max cache size.
        /// </summary>
        public static LocalPersistentContext<T> GetPersistentValue<T>(this OdinDrawer drawer, string key, T defaultValue = default(T))
        {
            var a = TwoWaySerializationBinder.Default.BindToName(drawer.GetType());
            var b = TwoWaySerializationBinder.Default.BindToName(drawer.Property.Tree.TargetType);
            var c = drawer.Property.Path;
            var d = new DrawerStateSignature(drawer.Property.RecursiveDrawDepth, InlineEditorAttributeDrawer.CurrentInlineEditorDrawDepth, drawer.Property.DrawerChainIndex);
            var e = key;

            GlobalPersistentContext<T> global;
            if (PersistentContext.Get(a, b, c, d, e, out global))
            {
                global.Value = defaultValue;
            }

            return LocalPersistentContext<T>.Create(global);
        }

        [Serializable]
        private struct DrawerStateSignature : IEquatable<DrawerStateSignature>
        {
            public int RecursiveDrawDepth;
            public int CurrentInlineEditorDrawDepth;
            public int DrawerChainIndex;

            public DrawerStateSignature(int recursiveDrawDepth, int currentInlineEditorDrawDepth, int drawerChainIndex)
            {
                this.RecursiveDrawDepth = recursiveDrawDepth;
                this.CurrentInlineEditorDrawDepth = currentInlineEditorDrawDepth;
                this.DrawerChainIndex = drawerChainIndex;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 31 + this.RecursiveDrawDepth;
                    hash = hash * 31 + this.CurrentInlineEditorDrawDepth;
                    hash = hash * 31 + this.DrawerChainIndex;
                    return hash;
                }
            }

            public override bool Equals(object obj)
            {
                return obj is DrawerStateSignature && this.Equals((DrawerStateSignature)obj);
            }

            public bool Equals(DrawerStateSignature other)
            {
                return this.RecursiveDrawDepth == other.RecursiveDrawDepth
                    && this.CurrentInlineEditorDrawDepth == other.CurrentInlineEditorDrawDepth
                    && this.DrawerChainIndex == other.DrawerChainIndex;
            }
        }
    }
}
#endif