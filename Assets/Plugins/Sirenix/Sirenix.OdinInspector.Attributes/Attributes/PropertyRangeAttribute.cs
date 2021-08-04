//-----------------------------------------------------------------------// <copyright file="PropertyRangeAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyRangeAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
	using System;

    /// <summary>
    /// <para>PropertyRange attribute creates a slider control to set the value of a property to between the specified range.</para>
    /// <para>This is equivalent to Unity's Range attribute, but this attribute can be applied to both fields and property.</para>
    /// </summary>
    /// <example>The following example demonstrates how PropertyRange is used.</example>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    /// 	[PropertyRange(0, 100)]
    ///		public int MyInt;
    ///		
    ///		[PropertyRange(-100, 100)]
    ///		public float MyFloat;
    ///		
    ///		[PropertyRange(-100, -50)]
    ///		public decimal MyDouble;
    ///		
    ///     // This attribute also supports dynamically referencing members by name to assign the min and max values for the range field.
    ///     [PropertyRange("DynamicMin", "DynamicMax"]
    ///     public float MyDynamicValue;
    ///     
    ///     public float DynamicMin, DynamicMax;
    ///	}
    /// </code>
    /// <seealso cref="ShowInInspectorAttribute"/>
    /// <seealso cref="PropertySpaceAttribute"/>
    /// <seealso cref="PropertyTooltipAttribute"/>
    /// <seealso cref="PropertyOrderAttribute"/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
	public sealed class PropertyRangeAttribute : Attribute
	{
		/// <summary>
		/// The minimum value.
		/// </summary>
		public double Min;

		/// <summary>
		/// The maximum value.
		/// </summary>
		public double Max;

        /// <summary>
        /// The name of a field, property or method to get the min value from.
        /// </summary>
        public string MinMember;

        /// <summary>
        /// The name of a field, property or method to get the max value from.
        /// </summary>
        public string MaxMember;

		/// <summary>
		/// Creates a slider control to set the value of the property to between the specified range..
		/// </summary>
		/// <param name="min">The minimum value.</param>
		/// <param name="max">The maximum value.</param>
		public PropertyRangeAttribute(double min, double max)
		{
			this.Min = min < max ? min : max;
			this.Max = max > min ? max : min;
		}

        /// <summary>
        /// Creates a slider control to set the value of the property to between the specified range..
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min value from.</param>
        /// <param name="max">The maximum value.</param>
        public PropertyRangeAttribute(string minMember, double max)
        {
            this.MinMember = minMember;
            this.Max = max;
        }

        /// <summary>
        /// Creates a slider control to set the value of the property to between the specified range..
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max value from.</param>
        public PropertyRangeAttribute(double min, string maxMember)
        {
            this.Min = min;
            this.MaxMember = maxMember;
        }

        /// <summary>
        /// Creates a slider control to set the value of the property to between the specified range..
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min value from.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max value from.</param>
        public PropertyRangeAttribute(string minMember, string maxMember)
        {
            this.MinMember = minMember;
            this.MaxMember = maxMember;
        }
	}
}