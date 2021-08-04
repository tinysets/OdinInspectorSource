#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyRangeAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyRangeAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws byte properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeByteDrawer : OdinAttributeDrawer<PropertyRangeAttribute, byte>
    {
        private InspectorPropertyValueGetter<byte> getterMinValue;
        private InspectorPropertyValueGetter<byte> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<byte>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<byte>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            byte min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (byte)this.Attribute.Min;
            byte max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (byte)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < byte.MinValue)
                {
                    value = byte.MinValue;
                }
                else if (value > byte.MaxValue)
                {
                    value = byte.MaxValue;
                }

                this.ValueEntry.SmartValue = (byte)value;
            }
        }
    }

    /// <summary>
    /// Draws double properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeDoubleDrawer : OdinAttributeDrawer<PropertyRangeAttribute, double>
    {
        private InspectorPropertyValueGetter<double> getterMinValue;
        private InspectorPropertyValueGetter<double> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<double>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<double>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // TODO: There's currently no SirenixEditorFields.DoubleRangeField, so we're making do with the float field. This should be fixed.
            double value = this.ValueEntry.SmartValue;
            if (value < float.MinValue)
            {
                value = float.MinValue;
            }
            else if (value > float.MaxValue)
            {
                value = float.MaxValue;
            }

            double min = this.getterMinValue != null ? this.getterMinValue.GetValue() : this.Attribute.Min;
            double max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            value = SirenixEditorFields.RangeFloatField(label, (float)value, (float)Math.Min(min, max), (float)Math.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = value;
            }
        }
    }

    /// <summary>
    /// Draws float properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeFloatDrawer : OdinAttributeDrawer<PropertyRangeAttribute, float>
    {
        private InspectorPropertyValueGetter<float> getterMinValue;
        private InspectorPropertyValueGetter<float> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            float min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (float)this.Attribute.Min;
            float max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (float)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            float value = SirenixEditorFields.RangeFloatField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = value;
            }
        }
    }

    /// <summary>
    /// Draws decimal properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    public sealed class PropertyRangeAttributeDecimalDrawer : OdinAttributeDrawer<PropertyRangeAttribute, decimal>
    {
        private InspectorPropertyValueGetter<decimal> getterMinValue;
        private InspectorPropertyValueGetter<decimal> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<decimal>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<decimal>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            decimal min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (decimal)this.Attribute.Min;
            decimal max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (decimal)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            float value = SirenixEditorFields.RangeFloatField(label, (float)this.ValueEntry.SmartValue, (float)Math.Min(min, max), (float)Math.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = (decimal)value;
            }
        }
    }

    /// <summary>
    /// Draws short properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeInt16Drawer : OdinAttributeDrawer<PropertyRangeAttribute, short>
    {
        private InspectorPropertyValueGetter<short> getterMinValue;
        private InspectorPropertyValueGetter<short> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<short>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<short>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            short min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (short)this.Attribute.Min;
            short max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (short)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < short.MinValue)
                {
                    value = short.MinValue;
                }
                else if (value > short.MaxValue)
                {
                    value = short.MaxValue;
                }

                this.ValueEntry.SmartValue = (short)value;
            }
        }
    }

    /// <summary>
    /// Draws int properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeInt32Drawer : OdinAttributeDrawer<PropertyRangeAttribute, int>
    {
        private InspectorPropertyValueGetter<int> getterMinValue;
        private InspectorPropertyValueGetter<int> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            int min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (int)this.Attribute.Min;
            int max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (int)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < int.MinValue)
                {
                    value = int.MinValue;
                }
                else if (value > int.MaxValue)
                {
                    value = int.MaxValue;
                }

                this.ValueEntry.SmartValue = value;
            }
        }
    }

    /// <summary>
    /// Draws long properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeInt64Drawer : OdinAttributeDrawer<PropertyRangeAttribute, long>
    {
        private InspectorPropertyValueGetter<long> getterMinValue;
        private InspectorPropertyValueGetter<long> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<long>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<long>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            long min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (long)this.Attribute.Min;
            long max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (long)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, (int)this.ValueEntry.SmartValue, (int)Math.Min(min, max), (int)Math.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = value;
            }
        }
    }

    /// <summary>
    /// Draws sbyte properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeSByteDrawer : OdinAttributeDrawer<PropertyRangeAttribute, sbyte>
    {
        private InspectorPropertyValueGetter<sbyte> getterMinValue;
        private InspectorPropertyValueGetter<sbyte> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<sbyte>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<sbyte>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            sbyte min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (sbyte)this.Attribute.Min;
            sbyte max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (sbyte)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < sbyte.MinValue)
                {
                    value = sbyte.MinValue;
                }
                else if (value > sbyte.MaxValue)
                {
                    value = sbyte.MaxValue;
                }

                this.ValueEntry.SmartValue = (sbyte)value;
            }
        }
    }

    /// <summary>
    /// Draws ushort properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeUInt16Drawer : OdinAttributeDrawer<PropertyRangeAttribute, ushort>
    {
        private InspectorPropertyValueGetter<ushort> getterMinValue;
        private InspectorPropertyValueGetter<ushort> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<ushort>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<ushort>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ushort min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (ushort)this.Attribute.Min;
            ushort max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (ushort)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, this.ValueEntry.SmartValue, Mathf.Min(min, max), Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < ushort.MinValue)
                {
                    value = ushort.MinValue;
                }
                else if (value > ushort.MaxValue)
                {
                    value = ushort.MaxValue;
                }

                this.ValueEntry.SmartValue = (ushort)value;
            }
        }
    }

    /// <summary>
    /// Draws uint properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeUInt32Drawer : OdinAttributeDrawer<PropertyRangeAttribute, uint>
    {
        private InspectorPropertyValueGetter<uint> getterMinValue;
        private InspectorPropertyValueGetter<uint> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<uint>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<uint>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            uint min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (uint)this.Attribute.Min;
            uint max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (uint)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, (int)this.ValueEntry.SmartValue, (int)Mathf.Min(min, max), (int)Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < uint.MinValue)
                {
                    value = (int)uint.MinValue;
                }
                else
                {
                    this.ValueEntry.SmartValue = (uint)value;
                }

                this.ValueEntry.SmartValue = (uint)value;
            }
        }
    }

    /// <summary>
    /// Draws ulong properties marked with <see cref="PropertyRangeAttribute"/>.
    /// </summary>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>
    public sealed class PropertyRangeAttributeUInt64Drawer : OdinAttributeDrawer<PropertyRangeAttribute, ulong>
    {
        private InspectorPropertyValueGetter<ulong> getterMinValue;
        private InspectorPropertyValueGetter<ulong> getterMaxValue;

        /// <summary>
        /// Initialized the drawer.
        /// </summary>
        protected override void Initialize()
        {
            if (this.Attribute.MinMember != null)
            {
                this.getterMinValue = new InspectorPropertyValueGetter<ulong>(this.Property, this.Attribute.MinMember);
            }
            if (this.Attribute.MaxMember != null)
            {
                this.getterMaxValue = new InspectorPropertyValueGetter<ulong>(this.Property, this.Attribute.MaxMember);
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            ulong min = this.getterMinValue != null ? this.getterMinValue.GetValue() : (ulong)this.Attribute.Min;
            ulong max = this.getterMaxValue != null ? this.getterMaxValue.GetValue() : (ulong)this.Attribute.Max;

            if (this.getterMinValue != null && this.getterMinValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMinValue.ErrorMessage);
            }
            if (this.getterMaxValue != null && this.getterMaxValue.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.getterMaxValue.ErrorMessage);
            }

            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.RangeIntField(label, (int)this.ValueEntry.SmartValue, (int)Mathf.Min(min, max), (int)Mathf.Max(min, max));
            if (EditorGUI.EndChangeCheck())
            {
                if (value < (int)ulong.MinValue)
                {
                    value = (int)ulong.MinValue;
                }
                else
                {
                    this.ValueEntry.SmartValue = (ulong)value;
                }

                this.ValueEntry.SmartValue = (ulong)value;
            }
        }
    }
}
#endif