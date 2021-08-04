#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="EnableIfAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="EnabledIfAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;
    using Sirenix.Utilities.Editor.Expressions;

    internal static class IfAttributesHelper
    {
        private class IfAttributesContext
        {
            public Func<bool> StaticMemberGetter;
            public Func<object, bool> InstanceMemberGetter;
            public Func<object> StaticObjectMemberGetter;
            public Func<object, object> InstanceObjectMemberGetter;

            // TODO: Expressions temporarily disabled.
            //public Delegate ExpressionMethod;
            //public bool expressionIsStatic;
            public string ErrorMessage;
            public bool Result;
            public bool UseNullComparison;
        }

        public static void HandleIfAttributesCondition(OdinDrawer drawer, InspectorProperty property, string memberName, object value, out bool result, out string errorMessage)
        {
            var context = property.Context.Get(drawer, "IfAttributeContext", (IfAttributesContext)null);

            if (context.Value == null)
            {
                Type returnType;

                context.Value = new IfAttributesContext();

                // TODO: Expressions temporarily disabled.
                //if (memberName != null && memberName.Length > 0 && memberName[0] == '$')
                //{
                //    var expression = memberName.Substring(1);

                //    context.Value.expressionIsStatic = property.ParentValueProperty == null && property.Tree.IsStatic;
                //    context.Value.ExpressionMethod = ExpressionCompilerUtility.CompileExpression(expression, context.Value.expressionIsStatic, property.ParentType, out context.Value.ErrorMessage);

                //    returnType = context.Value.ExpressionMethod != null ? context.Value.ExpressionMethod.Method.ReturnType : null;
                //}
                //else
                {
                    returnType = null;
                    MemberInfo memberInfo = property.ParentType
                        .FindMember()
                        .IsNamed(memberName)
                        .HasNoParameters()
                        .GetMember(out context.Value.ErrorMessage);

                    if (memberInfo != null)
                    {
                        string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                        if (memberInfo.GetReturnType() == typeof(bool))
                        {
                            if (memberInfo.IsStatic())
                            {
                                context.Value.StaticMemberGetter = DeepReflection.CreateValueGetter<bool>(property.ParentType, name);
                            }
                            else
                            {
                                context.Value.InstanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(property.ParentType, name);
                            }
                        }
                        else
                        {
                            if (memberInfo.IsStatic())
                            {
                                context.Value.StaticObjectMemberGetter = DeepReflection.CreateValueGetter<object>(property.ParentType, name);
                            }
                            else
                            {
                                context.Value.InstanceObjectMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<object>(property.ParentType, name);
                            }
                        }

                        returnType = memberInfo.GetReturnType();
                    }
                }

                if (returnType != null) // Should only be null in case of errors.
                {
                    context.Value.UseNullComparison = returnType != typeof(string) && (returnType.IsClass || returnType.IsInterface);
                }
            }
            errorMessage = context.Value.ErrorMessage;

            if (Event.current.type != EventType.Layout)
            {
                result = context.Value.Result;
                return;
            }

            context.Value.Result = false;

            if (context.Value.ErrorMessage == null)
            {
                // TODO: Expressions temporarily disabled.
                //if (context.Value.ExpressionMethod != null)
                //{
                //    for (int i = 0; i < property.ParentValues.Count; i++)
                //    {
                //        object val;
                //        if (context.Value.expressionIsStatic)
                //        {
                //            val = context.Value.ExpressionMethod.DynamicInvoke();
                //        }
                //        else
                //        {
                //            val = context.Value.ExpressionMethod.DynamicInvoke(property.ParentValues[i]);
                //        }

                //        if (context.Value.UseNullComparison)
                //        {
                //            if (val is UnityEngine.Object)
                //            {
                //                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                //                if (((UnityEngine.Object)val) != null)
                //                {
                //                    context.Value.Result = true;
                //                    break;
                //                }
                //            }
                //            else if (val != null)
                //            {
                //                context.Value.Result = true;
                //                break;
                //            }
                //        }
                //        else if (val is bool)
                //        {
                //            context.Value.Result = (bool)val;
                //            break;
                //        }
                //        else if (Equals(val, value))
                //        {
                //            context.Value.Result = true;
                //            break;
                //        }
                //    }
                //}
                //else 
                if (context.Value.InstanceMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        if (context.Value.InstanceMemberGetter(property.ParentValues[i]))
                        {
                            context.Value.Result = true;
                            break;
                        }
                    }
                }
                else if (context.Value.StaticMemberGetter != null)
                {
                    if (context.Value.StaticMemberGetter())
                    {
                        context.Value.Result = true;
                    }
                }
                else if (context.Value.InstanceObjectMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        var val = context.Value.InstanceObjectMemberGetter(property.ParentValues[i]);
                        if (context.Value.UseNullComparison)
                        {
                            if (val is UnityEngine.Object)
                            {
                                // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                                if (((UnityEngine.Object)val) != null)
                                {
                                    context.Value.Result = true;
                                    break;
                                }
                            }
                            else if (val != null)
                            {
                                context.Value.Result = true;
                                break;
                            }
                        }
                        else if (Equals(val, value))
                        {
                            context.Value.Result = true;
                            break;
                        }
                    }
                }
                else if (context.Value.StaticObjectMemberGetter != null)
                {
                    var val = context.Value.StaticObjectMemberGetter();
                    if (context.Value.UseNullComparison)
                    {
                        if (val is UnityEngine.Object)
                        {
                            // Unity objects can be 'fake null', and to detect that we have to test the value as a Unity object.
                            if (((UnityEngine.Object)val) != null)
                            {
                                context.Value.Result = true;
                            }
                        }
                        else if (val != null)
                        {
                            context.Value.Result = true;
                        }
                    }
                    else if (Equals(val, value))
                    {
                        context.Value.Result = true;
                    }
                }
            }

            result = context.Value.Result;
        }
    }

    /// <summary>
    /// Draws properties marked with <see cref="EnableIfAttribute"/>.
    /// </summary>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <seealso cref="ShowIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    /// <seealso cref="HideInInspector"/>
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class EnableIfAttributeDrawer : OdinAttributeDrawer<EnableIfAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (GUI.enabled == false)
            {
                this.CallNextDrawer(label);
                return;
            }

            var property = this.Property;
            var attribute = this.Attribute;

            bool result;
            string errorMessage;

            IfAttributesHelper.HandleIfAttributesCondition(this, property, attribute.MemberName, attribute.Value, out result, out errorMessage);

            if (errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(errorMessage);
                this.CallNextDrawer(label);
            }
            else if (!result)
            {
                GUIHelper.PushGUIEnabled(false);
                this.CallNextDrawer(label);
                GUIHelper.PopGUIEnabled();
            }
            else
            {
                this.CallNextDrawer(label);
            }
        }
    }
}
#endif