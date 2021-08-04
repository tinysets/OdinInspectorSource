#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ProgressBarAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ProgressBarAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Common base implementation for progress bar attribute drawers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseProgressBarAttributeDrawer<T> : OdinAttributeDrawer<ProgressBarAttribute, T>
    {
        private string errorMessage;

        private Func<Color> staticColorGetter;
        private Func<object, Color> instanceColorGetter;
        private Func<object, Color> instanceColorMethod;
        private Func<object, T, Color> instanceColorParameterMethod;

        private Func<Color> staticBackgroundColorGetter;
        private Func<object, Color> instanceBackgroundColorGetter;
        private Func<object, Color> instanceBackgroundColorMethod;
        private Func<object, T, Color> instanceBackgroundColorParameterMethod;

        private InspectorPropertyValueGetter<T> getterMinValue;
        private InspectorPropertyValueGetter<T> getterMaxValue;

        private Func<string> staticGetValueLabel;
        private Func<T, string> staticGetValueLabelValue;
        private Func<object, string> instanceGetValueLabel;
        private Func<object, T, string> instanceGetValueLabelValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            MemberInfo member;

            // Min member
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<T>(this.Property, this.Attribute.MinMember);
                this.errorMessage = this.getterMinValue.ErrorMessage;
            }

            // Max member
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<T>(this.Property, this.Attribute.MaxMember);
                this.errorMessage = this.getterMaxValue.ErrorMessage;
            }

            // Foreground color member.
            if (!this.Attribute.ColorMember.IsNullOrWhitespace())
            {
                if (MemberFinder.Start(this.Property.ParentType)
                    .IsNamed(this.Attribute.ColorMember)
                    .HasReturnType<Color>()
                    .TryGetMember(out member, out this.errorMessage))
                {
                    if (member is FieldInfo || member is PropertyInfo)
                    {
                        if (member.IsStatic())
                        {
                            this.staticColorGetter = DeepReflection.CreateValueGetter<Color>(this.Property.ParentType, this.Attribute.ColorMember);
                        }
                        else
                        {
                            this.instanceColorGetter = DeepReflection.CreateWeakInstanceValueGetter<Color>(this.Property.ParentType, this.Attribute.ColorMember);
                        }
                    }
                    else if (member is MethodInfo)
                    {
                        if (member.IsStatic())
                        {
                            this.errorMessage = "Static method members are currently not supported.";
                        }
                        else
                        {
                            var method = member as MethodInfo;
                            var p = method.GetParameters();

                            if (p.Length == 0)
                            {
                                this.instanceColorMethod = EmitUtilities.CreateWeakInstanceMethodCallerFunc<Color>(method);
                            }
                            else if (p.Length == 1 && p[0].ParameterType == typeof(T))
                            {
                                this.instanceColorParameterMethod = EmitUtilities.CreateWeakInstanceMethodCallerFunc<T, Color>(method);
                            }
                        }
                    }
                    else
                    {
                        this.errorMessage = "Unsupported member type.";
                    }
                }
            }

            // Background color member.
            if (!this.Attribute.BackgroundColorMember.IsNullOrWhitespace())
            {
                if (MemberFinder.Start(this.Property.ParentType)
                    .IsNamed(this.Attribute.BackgroundColorMember)
                    .HasReturnType<Color>()
                    .TryGetMember(out member, out this.errorMessage))
                {
                    if (member is FieldInfo || member is PropertyInfo)
                    {
                        if (member.IsStatic())
                        {
                            this.staticBackgroundColorGetter = DeepReflection.CreateValueGetter<Color>(this.Property.ParentType, this.Attribute.BackgroundColorMember);
                        }
                        else
                        {
                            this.instanceBackgroundColorGetter = DeepReflection.CreateWeakInstanceValueGetter<Color>(this.Property.ParentType, this.Attribute.BackgroundColorMember);
                        }
                    }
                    else if (member is MethodInfo)
                    {
                        if (member.IsStatic())
                        {
                            this.errorMessage = "Static method members are currently not supported.";
                        }
                        else
                        {
                            var method = member as MethodInfo;
                            var p = method.GetParameters();

                            if (p.Length == 0)
                            {
                                this.instanceBackgroundColorMethod = EmitUtilities.CreateWeakInstanceMethodCallerFunc<Color>(method);
                            }
                            else if (p.Length == 1 && p[0].ParameterType == typeof(T))
                            {
                                this.instanceBackgroundColorParameterMethod = EmitUtilities.CreateWeakInstanceMethodCallerFunc<T, Color>(method);
                            }
                        }
                    }
                    else
                    {
                        this.errorMessage = "Unsupported member type.";
                    }
                }
            }

            // Custom value string getter
            if (this.Attribute.CustomValueStringMember != null && this.Attribute.CustomValueStringMember.Length > 0)
            {
                if (MemberFinder.Start(this.Property.ParentType)
                    .IsNamed(this.Attribute.CustomValueStringMember)
                    .HasReturnType<string>()
                    .TryGetMember(out member, out this.errorMessage))
                {
                    if (member is FieldInfo || member is PropertyInfo)
                    {
                        if (member.IsStatic())
                        {
                            this.staticGetValueLabel = DeepReflection.CreateValueGetter<string>(this.Property.ParentType, this.Attribute.CustomValueStringMember);
                        }
                        else
                        {
                            this.instanceGetValueLabel = DeepReflection.CreateWeakInstanceValueGetter<string>(this.Property.ParentType, this.Attribute.CustomValueStringMember);
                        }
                    }
                    else if (member is MethodInfo)
                    {
                        var method = member as MethodInfo;
                        var parameters = method.GetParameters();

                        if (parameters.Length == 0)
                        {
                            string name = this.Attribute.CustomValueStringMember + "()";
                            if (member.IsStatic())
                            {
                                this.staticGetValueLabel = DeepReflection.CreateValueGetter<string>(this.Property.ParentType, name);
                            }
                            else
                            {
                                this.instanceGetValueLabel = DeepReflection.CreateWeakInstanceValueGetter<string>(this.Property.ParentType, name);
                            }
                        }
                        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(T))
                        {
                            if (member.IsStatic())
                            {
                                // TODO: This should be emitted.
                                this.staticGetValueLabelValue = (v) => (string)method.Invoke(null, new object[] { v });
                            }
                            else
                            {
                                // TODO: This should be emitted.
                                this.instanceGetValueLabelValue = (o, v) => (string)method.Invoke(o, new object[] { v });
                            }
                        }
                        else
                        {
                            this.errorMessage = "Was unable to find any string field or property or string method with no parameters or exactly one " + typeof(T).GetNiceName() + " parameter named " + this.Attribute.CustomValueStringMember;
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
            // Display evt. error
            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }

            ProgressBarConfig config = this.GetConfig();

            // Construct a Rect based on the configured height of the field.
            Rect rect = EditorGUILayout.GetControlRect(label != null, config.Height < EditorGUIUtility.singleLineHeight ? EditorGUIUtility.singleLineHeight : config.Height);

            // Get the min and max value either from member references or from the attribute.
            double min = this.getterMinValue != null && this.getterMinValue.ErrorMessage == null ? this.ConvertToDouble(this.getterMinValue.GetValue()) : this.Attribute.Min;
            double max = this.getterMaxValue != null && this.getterMaxValue.ErrorMessage == null ? this.ConvertToDouble(this.getterMaxValue.GetValue()) : this.Attribute.Max;

            // Get a string for the value label if any.
            string valueLabel =
                this.staticGetValueLabel != null ? this.staticGetValueLabel() :
                this.staticGetValueLabelValue != null ? this.staticGetValueLabelValue(this.ValueEntry.SmartValue) :
                this.instanceGetValueLabel != null ? this.instanceGetValueLabel(this.Property.ParentValues[0]) :
                this.instanceGetValueLabelValue != null ? this.instanceGetValueLabelValue(this.Property.ParentValues[0], this.ValueEntry.SmartValue) :
                null;

            // Draw the field.
            EditorGUI.BeginChangeCheck();
            T value = this.DrawProgressBar(rect, label, min, max, config, valueLabel);
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = value;
            }
        }

        private ProgressBarConfig GetConfig()
        {
            var config = ProgressBarConfig.Default;
            config.Height = this.Attribute.Height;
            config.DrawValueLabel = this.Attribute.DrawValueLabelHasValue ? this.Attribute.DrawValueLabel : (this.Attribute.Segmented ? false : true);
            config.ValueLabelAlignment = this.Attribute.ValueLabelAlignmentHasValue ? this.Attribute.ValueLabelAlignment : (this.Attribute.Segmented ? TextAlignment.Right : TextAlignment.Center);

            if (this.Attribute.CustomValueStringMember != null)
            {
                // Do not draw default label.
                config.DrawValueLabel = false;
            }

            // No point in updating the color in non-repaint events.
            if (Event.current.type == EventType.Repaint)
            {
                config.ForegroundColor =
                    this.staticColorGetter != null ? this.staticColorGetter() :
                    this.instanceColorGetter != null ? this.instanceColorGetter(this.Property.ParentValues[0]) :
                    this.instanceColorMethod != null ? this.instanceColorMethod(this.Property.ParentValues[0]) :
                    this.instanceColorParameterMethod != null ? this.instanceColorParameterMethod(this.Property.ParentValues[0], this.ValueEntry.SmartValue) :
                    new Color(this.Attribute.R, this.Attribute.G, this.Attribute.B, 1f);

                config.BackgroundColor =
                    this.staticBackgroundColorGetter != null ? this.staticBackgroundColorGetter() :
                    this.instanceBackgroundColorGetter != null ? this.instanceBackgroundColorGetter(this.Property.ParentValues[0]) :
                    this.instanceBackgroundColorMethod != null ? this.instanceBackgroundColorMethod(this.Property.ParentValues[0]) :
                    this.instanceBackgroundColorParameterMethod != null ? this.instanceBackgroundColorParameterMethod(this.Property.ParentValues[0], this.ValueEntry.SmartValue) :
                    config.BackgroundColor; // Use default if no other option is available.
            }

            return config;
        }

        /// <summary>
        /// Generic implementation of progress bar field drawing.
        /// </summary>
        protected abstract T DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel);

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected abstract double ConvertToDouble(T value);
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeByteDrawer : BaseProgressBarAttributeDrawer<byte>
    {
        /// <summary>
        /// Draws a progress bar for a byte property.
        /// </summary>
        protected override byte DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (byte)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (byte)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(byte value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeSbyteDrawer : BaseProgressBarAttributeDrawer<sbyte>
    {
        /// <summary>
        /// Draws a progress bar for a sbyte property.
        /// </summary>
        protected override sbyte DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (sbyte)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (sbyte)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(sbyte value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeShortDrawer : BaseProgressBarAttributeDrawer<short>
    {
        /// <summary>
        /// Draws a progress bar for a short property.
        /// </summary>
        protected override short DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (short)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (short)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(short value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeUshortDrawer : BaseProgressBarAttributeDrawer<ushort>
    {
        /// <summary>
        /// Draws a progress bar for a ushort property.
        /// </summary>
        protected override ushort DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (ushort)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (ushort)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(ushort value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeIntDrawer : BaseProgressBarAttributeDrawer<int>
    {
        /// <summary>
        /// Draws a progress bar for an int property.
        /// </summary>
        protected override int DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (int)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (int)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(int value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeUintDrawer : BaseProgressBarAttributeDrawer<uint>
    {
        /// <summary>
        /// Draws a progress bar for a uint property.
        /// </summary>
        protected override uint DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (uint)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (uint)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(uint value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeLongDrawer : BaseProgressBarAttributeDrawer<long>
    {
        /// <summary>
        /// Draws a progress bar for a long property.
        /// </summary>
        protected override long DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (long)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (long)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(long value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeUlongDrawer : BaseProgressBarAttributeDrawer<ulong>
    {
        /// <summary>
        /// Draws a progress bar for a ulong property.
        /// </summary>
        protected override ulong DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (ulong)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (ulong)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(ulong value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributeFloatDrawer : BaseProgressBarAttributeDrawer<float>
    {
        /// <summary>
        /// Draws a progress bar for a float property.
        /// </summary>
        protected override float DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (float)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (float)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(float value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributedoubleDrawer : BaseProgressBarAttributeDrawer<double>
    {
        /// <summary>
        /// Draws a progress bar for a double property.
        /// </summary>
        protected override double DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (double)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (double)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(double value)
        {
            return (double)value; ;
        }
    }

    /// <summary>
    /// Draws values decorated with <see cref="ProgressBarAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    public sealed class ProgressBarAttributedecimalDrawer : BaseProgressBarAttributeDrawer<decimal>
    {
        /// <summary>
        /// Draws a progress bar for a decimal property.
        /// </summary>
        protected override decimal DrawProgressBar(Rect rect, GUIContent label, double min, double max, ProgressBarConfig config, string valueLabel)
        {
            if (this.Attribute.Segmented)
            {
                return (decimal)SirenixEditorFields.SegmentedProgressBarField(rect, label, (long)this.ValueEntry.SmartValue, (long)min, (long)max, config, valueLabel);
            }
            else
            {
                return (decimal)SirenixEditorFields.ProgressBarField(rect, label, (double)this.ValueEntry.SmartValue, min, max, config, valueLabel);
            }
        }

        /// <summary>
        /// Converts the generic value to a double.
        /// </summary>
        /// <param name="value">The generic value to convert.</param>
        /// <returns>The generic value as a double.</returns>
        protected override double ConvertToDouble(decimal value)
        {
            return (double)value; ;
        }
    }
}
#endif