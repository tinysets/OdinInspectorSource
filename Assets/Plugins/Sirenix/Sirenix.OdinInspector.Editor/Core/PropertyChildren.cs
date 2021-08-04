#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyChildren.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyChildren.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the children of an <see cref="InspectorProperty"/>.
    /// </summary>
    public sealed class PropertyChildren : IEnumerable<InspectorProperty>
    {
        private Dictionary<int, InspectorPropertyInfo> infosByIndex = new Dictionary<int, InspectorPropertyInfo>();
        private Dictionary<int, InspectorProperty> childrenByIndex = new Dictionary<int, InspectorProperty>();
        private Dictionary<int, string> pathsByIndex = new Dictionary<int, string>();
        private bool allowChildren;

        private OdinPropertyResolver resolver;
        private IRefreshableResolver refreshableResolver;
        private IPathRedirector pathRedirector;
        private IHasSpecialPropertyPaths hasSpecialPropertyPaths;

        /// <summary>
        /// The <see cref="InspectorProperty"/> that this instance handles children for.
        /// </summary>
        private InspectorProperty property;

        /// <summary>
        /// Gets a child by index. This is an alias for <see cref="Get(int)" />.
        /// </summary>
        /// <param name="index">The index of the child to get.</param>
        /// <returns>The child at the given index.</returns>
        public InspectorProperty this[int index]
        {
            get { return this.Get(index); }
        }

        /// <summary>
        /// Gets a child by name. This is an alias for <see cref="Get(string)" />.
        /// </summary>
        /// <param name="name">The name of the child to get.</param>
        /// <returns>The child, if a child was found; otherwise, null.</returns>
        public InspectorProperty this[string name]
        {
            get { return this.Get(name); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChildren"/> class.
        /// </summary>
        /// <param name="property">The property to handle children for.</param>
        /// <exception cref="System.ArgumentNullException">property is null</exception>
        internal PropertyChildren(InspectorProperty property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            this.property = property;
            this.resolver = this.property.ChildResolver;
            this.refreshableResolver = this.resolver as IRefreshableResolver;
            this.pathRedirector = this.resolver as IPathRedirector;
            this.hasSpecialPropertyPaths = this.resolver as IHasSpecialPropertyPaths;
        }

        /// <summary>
        /// The number of children on the property.
        /// </summary>
        public int Count
        {
            get
            {
                return this.allowChildren ? this.property.ChildResolver.ChildCount : 0;
            }
        }

        internal void ClearAndDisposeChildren()
        {
            foreach (var child in this.childrenByIndex.Values)
            {
                child.Dispose();
            }

            this.infosByIndex.Clear();
            this.childrenByIndex.Clear();
            this.pathsByIndex.Clear();

            this.property.Tree.ClearPathCaches();
        }

        /// <summary>
        /// Updates this instance of <see cref="PropertyChildren"/>.
        /// </summary>
        public void Update()
        {
            this.allowChildren = true;

            if (this.property != this.property.Tree.SecretRootProperty &&
                this.property.ValueEntry != null &&
                (this.property.ValueEntry.ValueState == PropertyValueState.Reference
                || this.property.ValueEntry.ValueState == PropertyValueState.NullReference
                || this.property.ValueEntry.ValueState == PropertyValueState.ReferencePathConflict
                || this.property.ValueEntry.ValueState == PropertyValueState.ReferenceValueConflict))
            {
                this.allowChildren = false;
            }

            if (this.allowChildren)
            {
                this.property.ChildResolver.ForceUpdateChildCount();
            }
        }

        /// <summary>
        /// Gets a child by name.
        /// </summary>
        /// <param name="name">The name of the child to get.</param>
        /// <returns>The child, if a child was found; otherwise, null.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public InspectorProperty Get(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (!this.allowChildren || this.Count == 0) return null;

            int index = this.resolver.ChildNameToIndex(name);

            if (index >= 0 && index < this.Count)
            {
                return this.Get(index);
            }

            if (this.pathRedirector != null)
            {
                InspectorProperty result;

                if (this.pathRedirector.TryGetRedirectedProperty(name, out result))
                {
                    result.Update();
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a child by index.
        /// </summary>
        /// <param name="index">The index of the child to get.</param>
        /// <returns>
        /// The child at the given index.
        /// </returns>
        /// <exception cref="System.IndexOutOfRangeException">The given index was out of range.</exception>
        public InspectorProperty Get(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            InspectorProperty result;

            if (!this.childrenByIndex.TryGetValue(index, out result) || this.NeedsRefresh(index))
            {
                // A property already exists and must be refreshed, so it must be disposed immediately
                if (result != null)
                {
                    result.Dispose();
                    this.childrenByIndex.Remove(index);
                }

                // The order of operations here is very important. Calling result.Update() can cause all sorts of things to happen.
                // Including trying to get this very same child, resulting in an infinite loop because it hasn't
                // been set yet, so a new child will be created, ad infinitum.

                // Setting the child value, then updating, makes sure this sort of thing can never happen.

                var parent = this.property == this.property.Tree.SecretRootProperty ? null : this.property;
                result = InspectorProperty.Create(this.property.Tree, parent, this.GetInfo(index), index, false);
                this.childrenByIndex[index] = result;
                this.property.Tree.NotifyPropertyCreated(result);
            }

            result.Update();
            return result;
        }

        /// <summary>
        /// Gets the path of the child at a given index.
        /// </summary>
        /// <param name="index">The index to get the path of.</param>
        /// <returns>The path of the child at the given index.</returns>
        /// <exception cref="System.IndexOutOfRangeException">The given index was out of range.</exception>
        public string GetPath(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }

            string result;

            if (!this.pathsByIndex.TryGetValue(index, out result) || this.NeedsRefresh(index))
            {
                if (this.hasSpecialPropertyPaths != null)
                {
                    result = this.hasSpecialPropertyPaths.GetSpecialChildPath(index);
                }
                else
                {
                    result = this.property.Path + "." + this.GetInfo(index).PropertyName;
                }

                this.pathsByIndex[index] = result;
            }

            return result;
        }

        /// <summary>
        /// Returns an IEnumerable that recursively yields all children of the property, depth first.
        /// </summary>
        public IEnumerable<InspectorProperty> Recurse()
        {
            for (int i = 0; i < this.Count; i++)
            {
                var child = this[i];

                yield return child;

                foreach (var subChild in child.Children.Recurse())
                {
                    yield return subChild;
                }
            }
        }

        /// <summary>
        /// Gets the property's already created children, in no particular order.
        /// </summary>
        internal IEnumerable<InspectorProperty> GetExistingChildren()
        {
            return this.childrenByIndex.Values;
        }

        private InspectorPropertyInfo GetInfo(int index)
        {
            InspectorPropertyInfo info;

            if (!this.infosByIndex.TryGetValue(index, out info) || (this.refreshableResolver != null && this.refreshableResolver.ChildPropertyRequiresRefresh(index, info)))
            {
                info = this.resolver.GetChildInfo(index);
                this.infosByIndex[index] = info;
            }

            return info;
        }

        private bool NeedsRefresh(int index)
        {
            InspectorPropertyInfo info;
            return !this.infosByIndex.TryGetValue(index, out info) || (this.refreshableResolver != null && this.refreshableResolver.ChildPropertyRequiresRefresh(index, info));
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        public IEnumerator<InspectorProperty> GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }
    }
}
#endif