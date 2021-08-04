#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InspectorPropertyInfo.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="InspectorPropertyInfo.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using UnityEditor;
    using Utilities;

    /// <summary>
    /// Contains meta-data information about a property in the inspector, that can be used to create an actual property instance.
    /// </summary>
    public sealed class InspectorPropertyInfo
    {
        private MemberInfo[] memberInfos;
        private List<Attribute> attributes;
        private ImmutableList<Attribute> attributesImmutable;
        private Type typeOfOwner;
        private Type typeOfValue;
        private IValueGetterSetter getterSetter;
        private InspectorPropertyInfo[] groupInfos;
        private bool isUnityPropertyOnly;
        private Delegate @delegate;

        private static readonly DoubleLookupDictionary<Type, Type, Func<MemberInfo, bool, IValueGetterSetter>> GetterSetterCreators = new DoubleLookupDictionary<Type, Type, Func<MemberInfo, bool, IValueGetterSetter>>(FastTypeComparer.Instance, FastTypeComparer.Instance);
        private static readonly Type[] GetterSetterConstructorSignature = new Type[] { typeof(MemberInfo), typeof(bool) };

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this InspectorPropertyInfo has any backing members.
        /// </summary>
        public bool HasBackingMembers { get { return this.memberInfos != null && this.memberInfos.Length > 0; } }

        /// <summary>
        /// Gets a value indicating whether this InspectorPropertyInfo has only a single backing member.
        /// </summary>
        public bool HasSingleBackingMember { get { return this.memberInfos != null && this.memberInfos.Length == 1; } }

        /// <summary>
        /// The member info of the property. If the property has many member infos, such as if it is a group property, the first member info of <see cref="MemberInfos"/> is returned.
        /// </summary>
        [Obsolete("Use GetMemberInfo() instead, and note that there might not be a member at all, even if there is a value.", true)]
        public MemberInfo MemberInfo { get { return this.GetMemberInfo(); } }

        /// <summary>
        /// Indicates which type of property it is.
        /// </summary>
        public PropertyType PropertyType { get; private set; }

        /// <summary>
        /// The serialization backend for this property.
        /// </summary>
        public SerializationBackend SerializationBackend { get; private set; }

        /// <summary>
        /// The type on which this property is declared.
        /// </summary>
        public Type TypeOfOwner { get { return this.typeOfOwner; } }

        /// <summary>
        /// The base type of the value which this property represents. If there is no value, this will be null.
        /// </summary>
        public Type TypeOfValue { get { return this.typeOfValue; } }

        /// <summary>
        /// Whether this property is editable or not.
        /// </summary>
        public bool IsEditable { get; private set; }

        /// <summary>
        /// All member infos of the property. There will only be more than one member if it is an <see cref="InspectorPropertyGroupInfo"/>.
        /// </summary>
        [Obsolete("Use GetMemberInfos() instead, and note that there might not be any members at all, even if there is a value.", true)]
        public MemberInfo[] MemberInfos { get { return this.memberInfos; } }

        /// <summary>
        /// The order value of this property. Properties are (by convention) ordered by ascending order, IE, lower order values are shown first in the inspector. The final actual ordering of properties is decided upon by the property resolver.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// The attributes associated with this property.
        /// </summary>
        public ImmutableList<Attribute> Attributes
        {
            get
            {
                if (this.attributes == null) return null;

                if (this.attributesImmutable == null)
                {
                    this.attributesImmutable = new ImmutableList<Attribute>(this.attributes);
                }

                return this.attributesImmutable;
            }
        }

        /// <summary>
        /// Whether this property only exists as a Unity <see cref="SerializedProperty"/>, and has no associated managed member to represent it.
        /// This case requires some special one-off custom behaviour in a few places.
        /// </summary>
        public bool IsUnityPropertyOnly { get { return this.isUnityPropertyOnly; } }

        public static InspectorPropertyInfo CreateForDelegate(string name, int order, Type typeOfOwner, Delegate @delegate, params Attribute[] attributes)
        {
            return CreateForDelegate(name, order, typeOfOwner, @delegate, (IEnumerable<Attribute>)attributes);
        }

        public static InspectorPropertyInfo CreateForDelegate(string name, int order, Type typeOfOwner, Delegate @delegate, IEnumerable<Attribute> attributes)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (typeOfOwner == null)
            {
                throw new ArgumentNullException("typeOfOwner");
            }

            if (@delegate == null)
            {
                throw new ArgumentNullException("@delegate");
            }

            if (name.Contains('.'))
            {
                throw new ArgumentException("Property names may not contain '.'; was given the name '" + name + "'.");
            }

            var result = new InspectorPropertyInfo();

            result.memberInfos = new MemberInfo[0];

            result.typeOfOwner = typeOfOwner;

            result.PropertyName = name;
            result.PropertyType = PropertyType.Method;
            result.SerializationBackend = SerializationBackend.None;

            if (attributes == null)
            {
                result.attributes = new List<Attribute>();
            }
            else
            {
                result.attributes = attributes.Where(attr => attr != null).ToList();
            }

            result.@delegate = @delegate;

            return result;
        }

        public static InspectorPropertyInfo CreateForUnityProperty(string unityPropertyName, Type typeOfOwner, Type typeOfValue, bool isEditable, params Attribute[] attributes)
        {
            return CreateForUnityProperty(unityPropertyName, typeOfOwner, typeOfValue, isEditable, (IEnumerable<Attribute>)attributes);
        }

        public static InspectorPropertyInfo CreateForUnityProperty(string unityPropertyName, Type typeOfOwner, Type typeOfValue, bool isEditable, IEnumerable<Attribute> attributes)
        {
            if (unityPropertyName == null)
            {
                throw new ArgumentNullException("unityPropertyName");
            }

            if (typeOfOwner == null)
            {
                throw new ArgumentNullException("typeOfOwner");
            }

            if (typeOfValue == null)
            {
                throw new ArgumentNullException("typeOfValue");
            }

            if (unityPropertyName.Contains('.'))
            {
                throw new ArgumentException("Property names may not contain '.'; was given the name '" + unityPropertyName + "'.");
            }

            var result = new InspectorPropertyInfo();

            result.memberInfos = new MemberInfo[0];

            result.typeOfOwner = typeOfOwner;
            result.typeOfValue = typeOfValue;

            result.PropertyName = unityPropertyName;
            result.PropertyType = PropertyType.Value;
            result.SerializationBackend = SerializationBackend.Unity;
            result.IsEditable = isEditable;

            if (attributes == null)
            {
                result.attributes = new List<Attribute>();
            }
            else
            {
                result.attributes = attributes.Where(attr => attr != null).ToList();
            }

            result.isUnityPropertyOnly = true;

            return result;
        }

        public static InspectorPropertyInfo CreateValue(string name, int order, SerializationBackend serializationBackend, IValueGetterSetter getterSetter, params Attribute[] attributes)
        {
            return CreateValue(name, order, serializationBackend, getterSetter, (IEnumerable<Attribute>)attributes);
        }

        public static InspectorPropertyInfo CreateValue(string name, int order, SerializationBackend serializationBackend, IValueGetterSetter getterSetter, IEnumerable<Attribute> attributes)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (getterSetter == null)
            {
                throw new ArgumentNullException("getterSetter");
            }

            if (name.Contains('.'))
            {
                throw new ArgumentException("Property names may not contain '.'; was given the name '" + name + "'.");
            }

            var result = new InspectorPropertyInfo();

            result.memberInfos = new MemberInfo[0];

            result.typeOfOwner = getterSetter.OwnerType;
            result.typeOfValue = getterSetter.ValueType;

            if (attributes == null)
            {
                result.attributes = new List<Attribute>();
            }
            else
            {
                result.attributes = attributes.Where(attr => attr != null).ToList();
            }

            result.PropertyName = name;
            result.PropertyType = PropertyType.Value;
            result.SerializationBackend = serializationBackend;
            result.IsEditable = !getterSetter.IsReadonly;

            result.getterSetter = getterSetter;

            return result;
        }

        public static InspectorPropertyInfo CreateForMember(InspectorProperty parentProperty, MemberInfo member, bool allowEditable, params Attribute[] attributes)
        {
            var list = new List<Attribute>(attributes.Length);

            for (int i = 0; i < attributes.Length; i++)
            {
                list.Add(attributes[i]);
            }

            return CreateForMember(member, allowEditable, InspectorPropertyInfoUtility.GetSerializationBackend(parentProperty, member), list);
        }

        public static InspectorPropertyInfo CreateForMember(InspectorProperty parentProperty, MemberInfo member, bool allowEditable, IEnumerable<Attribute> attributes)
        {
            return CreateForMember(member, allowEditable, InspectorPropertyInfoUtility.GetSerializationBackend(parentProperty, member), attributes.ToList());
        }

        public static InspectorPropertyInfo CreateForMember(MemberInfo member, bool allowEditable, SerializationBackend serializationBackend, params Attribute[] attributes)
        {
            var list = new List<Attribute>(attributes.Length);

            for (int i = 0; i < attributes.Length; i++)
            {
                list.Add(attributes[i]);
            }

            return CreateForMember(member, allowEditable, serializationBackend, list);
        }

        public static InspectorPropertyInfo CreateForMember(MemberInfo member, bool allowEditable, SerializationBackend serializationBackend, IEnumerable<Attribute> attributes)
        {
            return CreateForMember(member, allowEditable, serializationBackend, attributes.ToList());
        }

        public static InspectorPropertyInfo CreateForMember(MemberInfo member, bool allowEditable, SerializationBackend serializationBackend, List<Attribute> attributes)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (!(member is FieldInfo || member is PropertyInfo || member is MethodInfo))
            {
                throw new ArgumentException("Can only create inspector properties for field, property and method members.");
            }

            if (member is MethodInfo && serializationBackend != SerializationBackend.None)
            {
                throw new ArgumentException("Serialization backend can only be None for method members.");
            }

            if (member is MethodInfo && allowEditable)
            {
                //throw new ArgumentException("allowEditable can only be false for method members.");
                allowEditable = false;
            }

            if (allowEditable && member is FieldInfo && (member as FieldInfo).IsLiteral)
            {
                allowEditable = false;
            }

            string name = null;

            if (member is MethodInfo)
            {
                var mi = member as MethodInfo;
                var parameters = mi.GetParameters();
                if (parameters.Length > 0)
                {
                    name = mi.GetNiceName();
                }
            }

            if (name == null)
            {
                name = member.Name;
            }

            if (name.Contains("."))
            {
                var index = name.LastIndexOf(".") + 1;

                if (index < name.Length)
                {
                    name = name.Substring(index);
                }
                else
                {
                    throw new ArgumentException("A member name somehow had a '.' as the last character. This shouldn't be possible, but the '" + member.Name + "' has messed things up for everyone now. Good job!");
                }
            }

            var result = new InspectorPropertyInfo();

            if (member.IsDefined(typeof(OmitFromPrefabModificationPathsAttribute), true))
            {
                name = "#" + name;
            }

            result.memberInfos = new MemberInfo[] { member };
            result.PropertyName = name;
            result.PropertyType = member is MethodInfo ? PropertyType.Method : PropertyType.Value;
            result.SerializationBackend = serializationBackend;

            if (attributes == null)
            {
                result.attributes = new List<Attribute>();
            }
            else
            {
                result.attributes = attributes;

                for (int i = attributes.Count - 1; i >= 0; i--)
                {
                    var attr = attributes[i];

                    if (attr == null)
                    {
                        attributes.RemoveAt(i);
                        continue;
                    }

                    var orderAttr = attr as PropertyOrderAttribute;
                    if (orderAttr != null)
                    {
                        result.Order = orderAttr.Order;
                    }
                }
            }

            result.typeOfOwner = member.DeclaringType;

            if (member is FieldInfo || member is PropertyInfo)
            {
                var valueType = member.GetReturnType();

                result.typeOfValue = valueType;

                result.getterSetter = GetEmittedGetterSetterCreator(member.DeclaringType, valueType)(member, !allowEditable);

                //if (member is FieldInfo)
                //{
                //    //TwoArgsObjectArray[0] = member;
                //    //TwoArgsObjectArray[1] = !allowEditable;

                //    //var con = getterSetterType.GetConstructor(GetterSetterFieldConstructorSignature);
                //    //result.getterSetter = (IValueGetterSetter)con.Invoke(TwoArgsObjectArray);

                //    //result.getterSetter = (IValueGetterSetter)Activator.CreateInstance(typeof(GetterSetter<,>).MakeGenericType(member.DeclaringType, valueType), member, !allowEditable);

                //    result.getterSetter = GetEmittedFieldGetterSetterCreator(member.DeclaringType, valueType)(member as FieldInfo, !allowEditable);
                //}
                //else
                //{
                //    //OneArgObjectArray[0] = member;

                //    //var con = getterSetterType.GetConstructor(GetterSetterPropertyConstructorSignature);
                //    //result.getterSetter = (IValueGetterSetter)con.Invoke(OneArgObjectArray);

                //    //result.getterSetter = (IValueGetterSetter)Activator.CreateInstance(typeof(GetterSetter<,>).MakeGenericType(member.DeclaringType, valueType), member);

                //}

                result.IsEditable = allowEditable && !attributes.HasAttribute<ReadOnlyAttribute>() && !result.getterSetter.IsReadonly;
            }

            return result;
        }

        public static InspectorPropertyInfo CreateGroup(string name, Type typeOfOwner, int order, InspectorPropertyInfo[] groupInfos, params Attribute[] attributes)
        {
            return CreateGroup(name, typeOfOwner, order, groupInfos, (IEnumerable<Attribute>)attributes);
        }

        public static InspectorPropertyInfo CreateGroup(string name, Type typeOfOwner, int order, InspectorPropertyInfo[] groupInfos, IEnumerable<Attribute> attributes)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (typeOfOwner == null)
            {
                throw new ArgumentNullException("typeOfOwner");
            }

            if (groupInfos == null)
            {
                throw new ArgumentNullException("groupInfos");
            }

            if (name.Contains('.'))
            {
                throw new ArgumentException("Group names or paths may not contain '.'; was given the path/name '" + name + "'.");
            }

            if (name.Length == 0 || name[0] != '#')
            {
                throw new ArgumentException("The first character in a property group name must be '#'; was given the name '" + name + "'.");
            }

            var result = new InspectorPropertyInfo();

            result.memberInfos = groupInfos.SelectMany(n => n.GetMemberInfos()).ToArray();

            if (attributes == null)
            {
                result.attributes = new List<Attribute>();
            }
            else
            {
                result.attributes = attributes.ToList();
            }

            result.Order = order;
            result.typeOfOwner = typeOfOwner;
            result.PropertyName = name;
            result.PropertyType = PropertyType.Group;
            result.SerializationBackend = SerializationBackend.None;
            result.IsEditable = false;
            result.groupInfos = groupInfos;

            return result;
        }

        private InspectorPropertyInfo()
        {
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.PropertyType == PropertyType.Group)
            {
                return this.GetAttribute<PropertyGroupAttribute>().GroupID + " (type: " + this.PropertyType + ", order: " + this.Order + ")";
            }
            else
            {
                return this.PropertyName + " (type: " + this.PropertyType + ", backend: " + this.SerializationBackend + ", order: " + this.Order + ")";
            }
        }

        /// <summary>
        /// Gets the first attribute of a given type on this property.
        /// </summary>
        public T GetAttribute<T>() where T : Attribute
        {
            if (this.attributes != null)
            {
                T result;

                for (int i = 0; i < this.attributes.Count; i++)
                {
                    result = this.attributes[i] as T;

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first attribute of a given type on this property, which is not contained in a given hashset.
        /// </summary>
        /// <param name="exclude">The attributes to exclude.</param>
        public T GetAttribute<T>(HashSet<Attribute> exclude) where T : Attribute
        {
            if (this.attributes != null)
            {
                for (int i = 0; i < this.attributes.Count; i++)
                {
                    T attr = this.attributes[i] as T;

                    if (attr != null && (exclude == null || !exclude.Contains(attr)))
                    {
                        return attr;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all attributes of a given type on the property.
        /// </summary>
        public IEnumerable<T> GetAttributes<T>() where T : Attribute
        {
            if (this.attributes != null)
            {
                T result;

                for (int i = 0; i < this.attributes.Count; i++)
                {
                    result = this.attributes[i] as T;

                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// The <see cref="InspectorPropertyInfo"/>s of all the individual properties in this group.
        /// </summary>
        public InspectorPropertyInfo[] GetGroupInfos()
        {
            return this.groupInfos;
        }

        public MemberInfo GetMemberInfo()
        {
            return this.memberInfos.Length == 0 ? null : this.memberInfos[0];
        }

        public MemberInfo[] GetMemberInfos()
        {
            return this.memberInfos;
        }

        public IValueGetterSetter GetGetterSetter()
        {
            return this.getterSetter;
        }

        /// <summary>
        /// Gets the property's method delegate, if there is one. Note that this is null if a method property is backed by an actual method member.
        /// </summary>
        public Delegate GetMethodDelegate()
        {
            return this.@delegate;
        }

        public bool TryGetStrongGetterSetter<TOwner, TValue>(out IValueGetterSetter<TOwner, TValue> result)
        {
            if (this.PropertyType != PropertyType.Value)
            {
                result = null;
                return false;
            }

            result = this.getterSetter as IValueGetterSetter<TOwner, TValue>;

            if (result != null)
            {
                return true;
            }

            if (this.getterSetter.OwnerType.IsAssignableFrom(typeof(TOwner)) && this.getterSetter.ValueType.IsAssignableFrom(typeof(TValue)))
            {
                var type = typeof(AliasGetterSetter<,,,>).MakeGenericType(typeof(TOwner), typeof(TValue), this.getterSetter.OwnerType, this.getterSetter.ValueType);
                result = (IValueGetterSetter<TOwner, TValue>)Activator.CreateInstance(type, this.getterSetter);
                return true;
            }

            result = null;
            return false;
        }

        public List<Attribute> GetEditableAttributesList()
        {
            return this.attributes;
        }

        private static Func<MemberInfo, bool, IValueGetterSetter> GetEmittedGetterSetterCreator(Type ownerType, Type valueType)
        {
            Func<MemberInfo, bool, IValueGetterSetter> result;

            if (!GetterSetterCreators.TryGetInnerValue(ownerType, valueType, out result))
            {
                var type = typeof(GetterSetter<,>).MakeGenericType(ownerType, valueType);
                var constructor = type.GetConstructor(GetterSetterConstructorSignature);

                var method = new DynamicMethod("GetterSetterCreator<" + ownerType.GetNiceName() + ", " + valueType.GetNiceName() + ">", typeof(IValueGetterSetter), new Type[] { typeof(MemberInfo), typeof(bool) });

                var il = method.GetILGenerator();

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Newobj, constructor);
                il.Emit(OpCodes.Ret);

                result = (Func<MemberInfo, bool, IValueGetterSetter>)method.CreateDelegate(typeof(Func<MemberInfo, bool, IValueGetterSetter>));
                GetterSetterCreators.AddInner(ownerType, valueType, result);
            }

            return result;
        }
    }
}
#endif