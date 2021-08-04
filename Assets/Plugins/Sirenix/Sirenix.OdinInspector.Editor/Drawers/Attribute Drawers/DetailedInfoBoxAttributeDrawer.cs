#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DetailedInfoBoxAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DetailedInfoBoxAttributeDrawer.cs" company="Sirenix IVS">
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

    /// <summary>
    ///	Draws properties marked with <see cref="DetailedInfoBoxAttribute"/>.
    /// </summary>
    /// <seealso cref="DetailedInfoBoxAttribute"/>
    /// <seealso cref="InfoBoxAttribute"/>
    /// <seealso cref="RequiredAttribute"/>
    /// <seealso cref="OnInspectorGUIAttribute"/>
    [DrawerPriority(0, 100, 0)]
    public sealed class DetailedInfoBoxAttributeDrawer : OdinAttributeDrawer<DetailedInfoBoxAttribute>
    {
        private bool hideDetailedMessage;
        private bool drawMessageBox;
        private string errorMessage;
        private Func<object, object, bool> instanceValidationParameterMethodCaller;
        private Func<object, bool> staticValidationParameterMethodCaller;
        private Func<object, bool> instanceValidationMethodCaller;
        private WeakValueGetter instanceValueGetter;
        private Func<bool> staticValidationCaller;
        private StringMemberHelper messageHelper;
        private StringMemberHelper detailsHelper;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.hideDetailedMessage = true;
            this.messageHelper = new StringMemberHelper(this.Property, this.Attribute.Message);
            this.detailsHelper = new StringMemberHelper(this.Property, this.Attribute.Details);
            this.errorMessage = this.messageHelper.ErrorMessage ?? this.detailsHelper.ErrorMessage;

            if (this.Attribute.VisibleIf != null)
            {
                MemberInfo memberInfo;

                // Parameter functions
                if (this.Property.ValueEntry != null && this.Property.ParentType.FindMember()
                    .IsMethod()
                    .HasReturnType<bool>()
                    .HasParameters(this.Property.ValueEntry.BaseValueType)
                    .IsNamed(this.Attribute.VisibleIf)
                    .TryGetMember(out memberInfo, out this.errorMessage))
                {
                    if (this.errorMessage == null)
                    {
                        if (memberInfo is MethodInfo)
                        {
                            var method = memberInfo as MethodInfo;

                            if (memberInfo.IsStatic())
                            {
                                // TODO: Emit dis.
                                this.staticValidationParameterMethodCaller = p => (bool)method.Invoke(null, new object[] { p });
                            }
                            else
                            {
                                // TODO: Emit dis.
                                this.instanceValidationParameterMethodCaller = (i, p) => (bool)method.Invoke(i, new object[] { p });
                            }
                        }
                        else
                        {
                            this.errorMessage = "Invalid member type!";
                        }
                    }
                }

                // Fields, properties, and no-parameter functions.
                else if (this.Property.ParentType.FindMember()
                    .HasReturnType<bool>()
                    .HasNoParameters()
                    .IsNamed(this.Attribute.VisibleIf)
                    .TryGetMember(out memberInfo, out this.errorMessage))
                {
                    if (this.errorMessage == null)
                    {
                        if (memberInfo is FieldInfo)
                        {
                            if (memberInfo.IsStatic())
                            {
                                this.staticValidationCaller = EmitUtilities.CreateStaticFieldGetter<bool>(memberInfo as FieldInfo);
                            }
                            else
                            {
                                this.instanceValueGetter = EmitUtilities.CreateWeakInstanceFieldGetter(this.Property.ParentType, memberInfo as FieldInfo);
                            }
                        }
                        else if (memberInfo is PropertyInfo)
                        {
                            if (memberInfo.IsStatic())
                            {
                                this.staticValidationCaller = EmitUtilities.CreateStaticPropertyGetter<bool>(memberInfo as PropertyInfo);
                            }
                            else
                            {
                                this.instanceValueGetter = EmitUtilities.CreateWeakInstancePropertyGetter(this.Property.ParentType, memberInfo as PropertyInfo);
                            }
                        }
                        else if (memberInfo is MethodInfo)
                        {
                            if (memberInfo.IsStatic())
                            {
                                this.staticValidationCaller = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), memberInfo as MethodInfo);
                            }
                            else
                            {
                                this.instanceValidationMethodCaller = EmitUtilities.CreateWeakInstanceMethodCallerFunc<bool>(memberInfo as MethodInfo);
                            }
                        }
                        else
                        {
                            this.errorMessage = "Invalid member type!";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }
            else
            {
                if (Event.current.type == EventType.Layout)
                {
                    try
                    {
                        var entry = this.Property.ValueEntry;
                        if (entry != null)
                        {
                            var parentValue = entry.Property.ParentValues[0];
                            this.drawMessageBox =
                                this.Attribute.VisibleIf == null ||
                                (this.staticValidationParameterMethodCaller != null && this.staticValidationParameterMethodCaller(entry.WeakSmartValue)) ||
                                (this.instanceValidationParameterMethodCaller != null && this.instanceValidationParameterMethodCaller(entry.Property.ParentValues[0], entry.WeakSmartValue)) ||
                                (this.instanceValidationMethodCaller != null && this.instanceValidationMethodCaller(entry.Property.ParentValues[0])) ||
                                (this.instanceValueGetter != null && (bool)this.instanceValueGetter(ref parentValue)) ||
                                (this.staticValidationCaller != null && this.staticValidationCaller());
                        }
                        else
                        {
                            this.drawMessageBox = true;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                if (this.drawMessageBox)
                {
                    UnityEditor.MessageType type = UnityEditor.MessageType.None;

                    switch (this.Attribute.InfoMessageType)
                    {
                        case InfoMessageType.None:
                            type = UnityEditor.MessageType.None;
                            break;

                        case InfoMessageType.Info:
                            type = UnityEditor.MessageType.Info;
                            break;

                        case InfoMessageType.Warning:
                            type = UnityEditor.MessageType.Warning;
                            break;

                        case InfoMessageType.Error:
                            type = UnityEditor.MessageType.Error;
                            break;

                        default:
                            SirenixEditorGUI.ErrorMessageBox("Unknown InfoBoxType: " + this.Attribute.InfoMessageType.ToString());
                            break;
                    }

                    this.hideDetailedMessage = SirenixEditorGUI.DetailedMessageBox(this.messageHelper.GetString(this.Property), this.detailsHelper.GetString(this.Property), type, this.hideDetailedMessage);
                }
            }

            this.CallNextDrawer(label);
        }
    }
}
#endif