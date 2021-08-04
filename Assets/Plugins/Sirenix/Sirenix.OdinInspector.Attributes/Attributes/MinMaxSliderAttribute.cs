//-----------------------------------------------------------------------// <copyright file="MinMaxSliderAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="MinMaxSliderAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>Draw a special slider the user can use to specify a range between a min and a max value.</para>
    /// <para>Uses a Vector2 where x is min and y is max.</para>
    /// </summary>
    /// <example>
	/// <para>The following example shows how MinMaxSlider is used.</para>
    /// <code>
    /// public class Player : MonoBehaviour
    /// {
    ///		[MinMaxSlider(4, 5)]
    ///		public Vector2 SpawnRadius;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class MinMaxSliderAttribute : Attribute
    {
        /// <summary>
        /// The min value for the slider.
        /// </summary>
        public float MinValue;

        /// <summary>
        /// The max value for the slider.
        /// </summary>
        public float MaxValue;

        /// <summary>
        /// The name of a field, property or method to get the min value from.
        /// </summary>
        public string MinMember;

        /// <summary>
        /// The name of a field, property or method to get the max value from.
        /// </summary>
        public string MaxMember;

        /// <summary>
        /// The name of a Vector2 field, property or method to get the min max values from.
        /// </summary>
        public string MinMaxMember;

        /// <summary>
        /// Draw float fields for min and max value.
        /// </summary>
        public bool ShowFields;

        /// <summary>
        /// Draws a min-max slider in the inspector. X will be set to min, and Y will be set to max.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="showFields">If <c>true</c> number fields will drawn next to the MinMaxSlider.</param>
        public MinMaxSliderAttribute(float minValue, float maxValue, bool showFields = false)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;
            this.ShowFields = showFields;
        }

        /// <summary>
        /// Draws a min-max slider in the inspector. X will be set to min, and Y will be set to max.
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min value from.</param>
        /// <param name="maxValue">The max value.</param>
        /// <param name="showFields">If <c>true</c> number fields will drawn next to the MinMaxSlider.</param>
        public MinMaxSliderAttribute(string minMember, float maxValue, bool showFields = false)
        {
            this.MinMember = minMember;
            this.MaxValue = maxValue;
            this.ShowFields = showFields;
        }

        /// <summary>
        /// Draws a min-max slider in the inspector. X will be set to min, and Y will be set to max.
        /// </summary>
        /// <param name="minValue">The min value.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max value from.</param>
        /// <param name="showFields">If <c>true</c> number fields will drawn next to the MinMaxSlider.</param>
        public MinMaxSliderAttribute(float minValue, string maxMember, bool showFields = false)
        {
            this.MinValue = minValue;
            this.MaxMember = maxMember;
            this.ShowFields = showFields;
        }

        /// <summary>
        /// Draws a min-max slider in the inspector. X will be set to min, and Y will be set to max.
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min value from.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max value from.</param>
        /// <param name="showFields">If <c>true</c> number fields will drawn next to the MinMaxSlider.</param>
        public MinMaxSliderAttribute(string minMember, string maxMember, bool showFields = false)
        {
            this.MinMember = minMember;
            this.MaxMember = maxMember;
            this.ShowFields = showFields;
        }

        /// <summary>
        /// Draws a min-max slider in the inspector. X will be set to min, and Y will be set to max.
        /// </summary>
        /// <param name="minMaxMember">The name of a Vector2 field, property or method to get the min max values from.</param>
        /// <param name="showFields">If <c>true</c> number fields will drawn next to the MinMaxSlider.</param>
        public MinMaxSliderAttribute(string minMaxMember, bool showFields = false)
        {
            this.MinMaxMember = minMaxMember;
            this.ShowFields = showFields;
        }
    }
}