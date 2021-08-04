#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ButtonParameterPropertyResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ButtonParameterPropertyResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class ButtonParameterPropertyResolver : OdinPropertyResolver
    {
        private Dictionary<int, InspectorPropertyInfo> childInfos = new Dictionary<int, InspectorPropertyInfo>();
        private Dictionary<string, int> indexNameLookup = new Dictionary<string, int>();
        private ParameterInfo[] parameters;
        private object[] parameterValues;

        public override bool CanResolveForPropertyFilter(InspectorProperty property)
        {
            return property.Info.PropertyType == PropertyType.Method;
        }

        protected override void Initialize()
        {
            var mi = this.Property.Info.GetMemberInfo() as MethodInfo;

            if (mi == null)
            {
                mi = this.Property.Info.GetMethodDelegate().Method;
            }

            this.parameters = mi.GetParameters();
            this.parameterValues = new object[parameters.Length];

            for (int i = 0; i < this.parameters.Length; i++)
            {
                var val = this.parameters[i].DefaultValue;

                if (val != DBNull.Value)
                {
                    this.parameterValues[i] = val;
                }
            }
        }

        public override int ChildNameToIndex(string name)
        {
            int index;
            if (this.indexNameLookup.TryGetValue(name, out index))
            {
                return index;
            }

            return -1;
        }

        public override InspectorPropertyInfo GetChildInfo(int childIndex)
        {
            InspectorPropertyInfo info;
            if (this.childInfos.TryGetValue(childIndex, out info))
            {
                return info;
            }

            var parameter = this.parameters[childIndex];

            var type = parameter.ParameterType;

            if (type.IsByRef)
            {
                type = type.GetElementType();
            }

            var getterSetterType = typeof(GetterSetter<>).MakeGenericType(type);
            var getterSetter = Activator.CreateInstance(getterSetterType, new object[] { this.parameterValues, childIndex }) as IValueGetterSetter;

            info = InspectorPropertyInfo.CreateValue(parameter.Name, childIndex, SerializationBackend.None, getterSetter, parameter.GetAttributes());
            this.childInfos[childIndex] = info;

            return info;
        }

        protected override int CalculateChildCount()
        {
            return this.parameterValues.Length;
        }

        private class GetterSetter<T> : IValueGetterSetter<object, T>
        {
            private readonly object[] parameterValues;
            private readonly int index;

            public bool IsReadonly { get { return false; } }
            public Type OwnerType { get { return typeof(object); } }
            public Type ValueType { get { return typeof(T); } }

            public GetterSetter(object[] parameterValues, int index)
            {
                this.parameterValues = parameterValues;
                this.index = index;
            }

            public T GetValue(ref object owner)
            {
                if (this.parameterValues[this.index] == null)
                {
                    return default(T);
                }

                try
                {
                    return (T)this.parameterValues[this.index];
                }
                catch
                {
                }

                return default(T);
            }

            public object GetValue(object owner)
            {
                return this.parameterValues[this.index];
            }

            public void SetValue(ref object owner, T value)
            {
                this.parameterValues[this.index] = value;
            }

            public void SetValue(object owner, object value)
            {
                this.parameterValues[this.index] = value;
            }
        }
    }
}
#endif