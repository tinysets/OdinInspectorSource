#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="StringMemberHelper.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="StringMemberHelper.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using System;
    using UnityEngine;
    using Sirenix.Utilities.Editor.Expressions;
    using Sirenix.Utilities;
    using System.Reflection;

    /// <summary>
    ///	Helper class to handle strings for labels and other similar purposes.
    ///	Allows for a static string, or for refering to string member fields, properties or methods,
    ///	by name, if the first character is a '$'.
    /// </summary>
    public class StringMemberHelper
    {
        private string buffer;
        private string rawString;
        private string errorMessage;
        private readonly Type objectType;

        // TODO: Expression temporarily disabled.
        //private Delegate method;
        //private bool isStatic;

        // Obsolete:
        private Func<object> staticValueGetter;
        private Func<object, object> instanceValueGetter;

        /// <summary>
        /// If any error occurred while looking for members, it will be stored here.
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage; } }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="objectType">The type of the object, to get a member string from.</param>
        /// <param name="path">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method.</param>
        /// <param name="allowInstanceMember">If <c>true</c>, then StringMemberHelper will look for instance members.</param>
        /// <param name="allowStaticMember">If <c>true</c>, then StringMemberHelper will look for static members.</param>
        [Obsolete("Use the contructor with the InspectorProperty argument instead."
//#if SIRENIX_INTERNAL
//            , true
//#endif
            )]
        public StringMemberHelper(Type objectType, string path, bool allowInstanceMember = true, bool allowStaticMember = true)
        {
            this.rawString = path;
            this.objectType = objectType;

            if (path != null && objectType != null && path.Length > 0 && path[0] == '$')
            {
                path = path.Substring(1);

                var finder = MemberFinder.Start(objectType)
                    .HasReturnType<object>(true)
                    .IsNamed(path)
                    .HasNoParameters();

                if (!allowInstanceMember && !allowStaticMember)
                {
                    throw new InvalidOperationException("Require either allowInstanceMember or allowStaticMember to be true.");
                }
                else if (!allowInstanceMember)
                {
                    finder.IsStatic();
                }
                else if (!allowStaticMember)
                {
                    finder.IsInstance();
                }

                MemberInfo member;
                if (finder.TryGetMember(out member, out this.errorMessage))
                {
                    if (member is MethodInfo)
                    {
                        path += "()";
                    }

                    if (member.IsStatic())
                    {
                        this.staticValueGetter = DeepReflection.CreateValueGetter<object>(objectType, path);
                    }
                    else
                    {
                        this.instanceValueGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(objectType, path);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="objectType">The type of the parent, to get a member string from.</param>
        /// <param name="path">The input string. If the first character is a '$', then StringMemberHelper will look for a member string field, property or method.</param>
        /// <param name="errorMessage">Error message buffer. If the string is not equal to <c>null</c>, the StringMemberHelper will not run.</param>
        /// <param name="allowInstanceMember">If <c>true</c>, then StringMemberHelper will look for instance members.</param>
        /// <param name="allowStaticMember">If <c>true</c>, then StringMemberHelper will look for static members.</param>
        [Obsolete("Use the contructor with the InspectorProperty argument instead."
//#if SIRENIX_INTERNAL
//            , true
//#endif
            )]
        public StringMemberHelper(Type objectType, string path, ref string errorMessage, bool allowInstanceMember = true, bool allowStaticMember = true)
            : this(objectType, path, allowInstanceMember, allowStaticMember)
        {
            if (errorMessage == null)
            {
                errorMessage = this.ErrorMessage;
            }
        }

        public StringMemberHelper(InspectorProperty property, string expression) : this(property.ParentType, property.ParentValueProperty == null && property.Tree.IsStatic, expression)
        {
        }

        public StringMemberHelper(InspectorProperty property, string expression, ref string errorMessage) : this(property, expression)
        {
            if (errorMessage == null)
            {
                errorMessage = this.ErrorMessage;
            }
        }
#pragma warning disable 0618
        // TODO: Expression temporarily disabled. Remove call to this(type, string, bool, bool) constructor, and restore method body.
        public StringMemberHelper(Type objectType, bool isStatic, string expression) : this(objectType, expression, isStatic == false, true)
#pragma warning restore 0618
        {
            // DO NOT DELETE.
            //this.rawString = expression;
            //if (expression != null && expression.Length > 0 && expression[0] == '$')
            //{
            //    expression = expression.Substring(1);
            //    this.isStatic = isStatic;
            //    this.method = ExpressionCompilerUtility.CompileExpression(expression, this.isStatic, objectType, out this.errorMessage);
            //}
        }

        /// <summary>
        /// Gets a value indicating whether or not the string is retrived from a from a member. 
        /// </summary>
        public bool IsDynamicString
        {
            // TODO: Expression temporarily disabled.
            get { return /*this.method != null ||*/ this.instanceValueGetter != null || this.staticValueGetter != null; }
        }

        /// <summary>
        /// Gets the type of the object.
        /// </summary>
        public Type ObjectType { get { return this.objectType; } }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="entry">The property entry, to get the instance reference from.</param>
        /// <returns>The current display string.</returns>
        public string GetString(IPropertyValueEntry entry)
        {
            return this.GetString(entry.Property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="property">The property, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string GetString(InspectorProperty property)
        {
            return this.GetString(property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// Only updates the string buffer in Layout events.
        /// </summary>
        /// <param name="instance">The instance, for evt. member references.</param>
        /// <returns>The current string.</returns>
        public string GetString(object instance)
        {
            if (this.buffer == null || Event.current == null || Event.current.type == EventType.Layout)
            {
                this.buffer = this.ForceGetString(instance);
            }

            return this.buffer;
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="entry">The property entry, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(IPropertyValueEntry entry)
        {
            return this.ForceGetString(entry.Property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="property">The property, to get the instance reference from.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(InspectorProperty property)
        {
            return this.ForceGetString(property.ParentValues[0]);
        }

        /// <summary>
        /// Gets the string from the StringMemberHelper.
        /// </summary>
        /// <param name="instance">The instance, for evt. member references.</param>
        /// <returns>The current string.</returns>
        public string ForceGetString(object instance)
        {
            if (this.errorMessage != null)
            {
                return "Error";
            }

            // TODO: Expression temporarily disabled.
            //if (this.method != null)
            //{
            //    object o;
            //    if (this.isStatic)
            //    {
            //        o = this.method.DynamicInvoke();
            //    }
            //    else
            //    {
            //        o = this.method.DynamicInvoke(instance);
            //    }

            //    return (o == null) ? "Null" : o.ToString();
            //}

            if (this.staticValueGetter != null)
            {
                var o = this.staticValueGetter();
                return (o == null) ? "Null" : o.ToString();
            }

            if (instance != null && this.instanceValueGetter != null)
            {
                var o = this.instanceValueGetter(instance);
                return (o == null) ? "Null" : o.ToString();
            }

            return this.rawString;
        }
    }
}
#endif