#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="MinValueAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="MinValueAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    /// <summary>
    /// Draws uint properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeUint32Drawer : OdinAttributeDrawer<MinValueAttribute, uint>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            uint v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (uint)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws ulong properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeUint64Drawer : OdinAttributeDrawer<MinValueAttribute, ulong>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            ulong v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (ulong)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws ushort properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeUint16Drawer : OdinAttributeDrawer<MinValueAttribute, ushort>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            ushort v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (ushort)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws sbyte properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeSbyteDrawer : OdinAttributeDrawer<MinValueAttribute, sbyte>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            sbyte v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (sbyte)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws long properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeInt64Drawer : OdinAttributeDrawer<MinValueAttribute, long>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            long v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (long)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws int properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeInt32Drawer : OdinAttributeDrawer<MinValueAttribute, int>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            int v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (int)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws short properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeInt16Drawer : OdinAttributeDrawer<MinValueAttribute, short>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            short v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (short)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws float properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeFloatDrawer : OdinAttributeDrawer<MinValueAttribute, float>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            float v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (float)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws double properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeDoubleDrawer : OdinAttributeDrawer<MinValueAttribute, double>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            double v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws byte properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeByteDrawer : OdinAttributeDrawer<MinValueAttribute, byte>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            byte v = entry.SmartValue;
            if (attribute.MinValue > v)
            {
                entry.SmartValue = (byte)attribute.MinValue;
            }
        }
    }

    /// <summary>
    /// Draws Vector2 properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeVector2Drawer : OdinAttributeDrawer<MinValueAttribute, Vector2>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            Vector2 v = entry.SmartValue;
            if (attribute.MinValue > v.x ||
                attribute.MinValue > v.y)
            {
                v.x = attribute.MinValue > v.x ? (float)attribute.MinValue : v.x;
                v.y = attribute.MinValue > v.y ? (float)attribute.MinValue : v.y;
                entry.SmartValue = v;
            }
        }
    }

    /// <summary>
    /// Draws Vector3 properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeVector3Drawer : OdinAttributeDrawer<MinValueAttribute, Vector3>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            Vector3 v = entry.SmartValue;
            if (attribute.MinValue > v.x ||
                attribute.MinValue > v.y ||
                attribute.MinValue > v.z)
            {
                v.x = attribute.MinValue > v.x ? (float)attribute.MinValue : v.x;
                v.y = attribute.MinValue > v.y ? (float)attribute.MinValue : v.y;
                v.z = attribute.MinValue > v.z ? (float)attribute.MinValue : v.z;
                entry.SmartValue = v;
            }
        }
    }

    /// <summary>
    /// Draws Vector4 properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeVector4Drawer : OdinAttributeDrawer<MinValueAttribute, Vector4>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            Vector4 v = entry.SmartValue;
            if (attribute.MinValue > v.x ||
                attribute.MinValue > v.y ||
                attribute.MinValue > v.z ||
                attribute.MinValue > v.w)
            {
                v.x = attribute.MinValue > v.x ? (float)attribute.MinValue : v.x;
                v.y = attribute.MinValue > v.y ? (float)attribute.MinValue : v.y;
                v.z = attribute.MinValue > v.z ? (float)attribute.MinValue : v.z;
                v.w = attribute.MinValue > v.w ? (float)attribute.MinValue : v.w;
                entry.SmartValue = v;
            }
        }
    }

    /// <summary>
    /// Draws decimal properties marked with <see cref="MinValueAttribute"/>.
    /// </summary>
    /// <seealso cref="MinValueAttribute"/>
    /// <seealso cref="MaxValueAttribute"/>
    /// <seealso cref="RangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    /// <seealso cref="DelayedAttribute"/>
    /// <seealso cref="WrapAttribute"/>

    [DrawerPriority(0.3)]
    public sealed class MinValueAttributeDecimalDrawer : OdinAttributeDrawer<MinValueAttribute, decimal>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            this.CallNextDrawer(label);

            if ((decimal)attribute.MinValue > entry.SmartValue)
            {
                entry.SmartValue = (decimal)attribute.MinValue;
            }
        }
    }
}
#endif