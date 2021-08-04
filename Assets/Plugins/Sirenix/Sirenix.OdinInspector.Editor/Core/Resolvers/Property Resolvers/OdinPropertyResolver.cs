#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinPropertyResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="OdinPropertyResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;

    public abstract class OdinPropertyResolver
    {
        private bool hasUpdatedChildCountEver = false;
        private int lastUpdatedTreeID = -1;
        private int childCount;

        public bool HasChildCountConflict { get; protected set; }
        public int MaxChildCountSeen { get; protected set; }

        public static OdinPropertyResolver Create(Type resolverType, InspectorProperty property)
        {
            if (resolverType == null)
            {
                throw new ArgumentNullException("resolverType");
            }

            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            if (!typeof(OdinPropertyResolver).IsAssignableFrom(resolverType))
            {
                throw new ArgumentException("Type is not a PropertyResolver");
            }

            var result = (OdinPropertyResolver)Activator.CreateInstance(resolverType);
            result.Property = property;
            result.Initialize();
            return result;
        }

        public static T Create<T>(InspectorProperty property) where T : OdinPropertyResolver, new()
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            var result = new T();
            result.Property = property;
            result.Initialize();
            return result;
        }

        protected virtual void Initialize()
        {
        }

        public virtual Type ResolverForType { get { return null; } }

        public InspectorProperty Property { get; private set; }

        public int ChildCount
        {
            get
            {
                var treeId = this.Property.Tree.UpdateID;

                if (this.lastUpdatedTreeID != treeId || !this.hasUpdatedChildCountEver)
                {
                    this.lastUpdatedTreeID = treeId;
                    this.childCount = this.CalculateChildCount();
                }

                return this.childCount;
            }
        }

        public abstract InspectorPropertyInfo GetChildInfo(int childIndex);

        public abstract int ChildNameToIndex(string name);

        protected abstract int CalculateChildCount();

        public virtual bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            return true;
        }

        public void ForceUpdateChildCount()
        {
            if (this.hasUpdatedChildCountEver) // If we've never updated the child count yet, there's no reason to actually do this, as the latest value will be given by ChildCount anyways
            {
                this.lastUpdatedTreeID = this.Property.Tree.UpdateID;
                this.childCount = this.CalculateChildCount();
            }
        }
    }

    public abstract class OdinPropertyResolver<TValue> : OdinPropertyResolver
    {
        public sealed override Type ResolverForType { get { return typeof(TValue); } }

        public IPropertyValueEntry<TValue> ValueEntry { get { return (IPropertyValueEntry<TValue>)this.Property.ValueEntry; } }

        protected sealed override int CalculateChildCount()
        {
            var valueEntry = (IPropertyValueEntry<TValue>)this.Property.ValueEntry;

            this.HasChildCountConflict = false;
            int count = int.MaxValue;
            this.MaxChildCountSeen = int.MinValue;

            for (int i = 0; i < valueEntry.ValueCount; i++)
            {
                int indexCount = this.GetChildCount(valueEntry.Values[i]);

                if (count != int.MaxValue && count != indexCount)
                {
                    this.HasChildCountConflict = true;
                }

                if (indexCount < count)
                {
                    count = indexCount;
                }

                if (indexCount > this.MaxChildCountSeen)
                {
                    this.MaxChildCountSeen = indexCount;
                }
            }

            return count;
        }

        protected abstract int GetChildCount(TValue value);
    }

    public abstract class OdinPropertyResolver<TValue, TAttribute> : OdinPropertyResolver<TValue> where TAttribute : Attribute
    {
    }
}
#endif