#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InspectorPropertyValueGetter.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="StringMemberHelper.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using System.Reflection;
    using System;
    using System.Collections.Generic;
    using Sirenix.Utilities.Editor.Expressions;

    /// <summary>
    ///	Helper class to get values from InspectorProperties.
    /// </summary>
    public class InspectorPropertyValueGetter<TReturnType>
    {
        private string errorMessage;
        private Func<TReturnType> staticValueGetter;
        private Func<object, TReturnType> instanceValueGetter;
        private InspectorProperty memberProperty;

        // TODO: Expressions temporarily disabled.
        //private Delegate expressionMethod;
        //private bool isStaticExpression;

        public static readonly bool IsValueType = typeof(TReturnType).IsValueType;

        /// <summary>
        /// If any error occurred while looking for members, it will be stored here.
        /// </summary>
        public string ErrorMessage { get { return this.errorMessage; } }

        /// <summary>
        /// Gets the referenced member information.
        /// </summary>
        [Obsolete("A member is no longer guaranteed.", true)]
        public MemberInfo MemberInfo { get { throw new NotSupportedException("How have you even called this?? Just stop!"); } }

        /// <summary>
        /// Creates a StringMemberHelper to get a display string.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="memberName">The member name.</param>
        /// <param name="allowInstanceMember">If <c>true</c>, then StringMemberHelper will look for instance members.</param>
        /// <param name="allowStaticMember">If <c>true</c>, then StringMemberHelper will look for static members.</param>
        /// <exception cref="System.InvalidOperationException">Require either allowInstanceMember or allowStaticMember to be true.</exception>
        public InspectorPropertyValueGetter(InspectorProperty property, string memberName, bool allowInstanceMember = true, bool allowStaticMember = true)
        {
            this.memberProperty = property.FindParent(x => x.Info.GetMemberInfo() != null, true);

            // TODO: Expression temporarily disabled.
            //if (memberName != null && memberName.Length > 0 && memberName[0] == '$')
            //{
            //    var expression = memberName.Substring(1);
            //    this.isStaticExpression = property.ParentValueProperty == null && property.Tree.IsStatic;

            //    this.expressionMethod = ExpressionCompilerUtility.CompileExpression(expression, this.isStaticExpression, property.ParentType, out this.errorMessage);

            //    if (this.expressionMethod != null && this.expressionMethod.Method.ReturnType.IsCastableTo(typeof(TReturnType)) == false)
            //    {
            //        this.errorMessage = "Cannot cast type of " + this.expressionMethod.Method.ReturnType + " to type of " + typeof(TReturnType).Name + ".";
            //        this.expressionMethod = null;
            //    }
            //}
            //else
            {
                var parentType = this.memberProperty.ParentType;

                var finder = MemberFinder.Start(parentType)
                    .HasReturnType<TReturnType>(true)
                    .IsNamed(memberName)
                    .HasNoParameters();

                if (!allowInstanceMember && !allowStaticMember)
                {
                    throw new InvalidOperationException("Require either allowInstanceMember and/or allowStaticMember to be true.");
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
                        memberName += "()";
                    }

                    if (member.IsStatic())
                    {
                        this.staticValueGetter = DeepReflection.CreateValueGetter<TReturnType>(parentType, memberName);
                    }
                    else
                    {
                        this.instanceValueGetter = DeepReflection.CreateWeakInstanceValueGetter<TReturnType>(parentType, memberName);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public TReturnType GetValue()
        {
            // TODO: Expressions temporarily disabled.
            //if (this.expressionMethod != null)
            //{
            //    if (this.isStaticExpression)
            //    {
            //        return DoCast(this.expressionMethod.DynamicInvoke());
            //    }
            //    else
            //    {
            //        return DoCast(this.expressionMethod.DynamicInvoke(this.memberProperty.ParentValues[0]));
            //    }
            //}

            if (this.staticValueGetter != null)
            {
                return this.staticValueGetter();
            }

            var instance = this.memberProperty.ParentValues[0];

            if (instance != null && this.instanceValueGetter != null)
            {
                return this.instanceValueGetter(instance);
            }

            return default(TReturnType);
        }

        /// <summary>
        /// Gets all values from all targets.
        /// </summary>
        public IEnumerable<TReturnType> GetValues()
        {
            // TODO: Expressions temporarily disabled.
            //if (this.expressionMethod != null)
            //{
            //    if (this.isStaticExpression)
            //    {
            //        yield return DoCast(this.expressionMethod.DynamicInvoke());
            //    }
            //    else
            //    {
            //        for (int i = 0; i < this.memberProperty.ParentValues.Count; i++)
            //        {
            //            yield return DoCast(this.expressionMethod.DynamicInvoke(this.memberProperty.ParentValues[i]));
            //        }
            //    }

            //    yield break;
            //}

            if (this.staticValueGetter != null)
            {
                // TODO: @Bjarke Should this not be yield once for each selected object? Signed Mikkel/Tor.
                yield return this.staticValueGetter();
                yield break;
            }

            for (int i = 0; i < this.memberProperty.ParentValues.Count; i++)
            {
                var instance = this.memberProperty.ParentValues[i];
                if (instance != null && this.instanceValueGetter != null)
                {
                    yield return this.instanceValueGetter(instance);
                }
            }
        }

        private static TReturnType DoCast(object value)
        {
            if (value is TReturnType) return (TReturnType)value;

            return (TReturnType)Convert.ChangeType(value, typeof(TReturnType));
        }
    }
}
#endif