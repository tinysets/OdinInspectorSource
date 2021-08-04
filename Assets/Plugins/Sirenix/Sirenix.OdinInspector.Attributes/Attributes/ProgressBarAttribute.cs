//-----------------------------------------------------------------------// <copyright file="ProgressBarAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ProgressBarAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;
    using UnityEngine;

    /// <summary>
    /// <para>Draws a horizontal progress bar based on the value of the property.</para>
    /// <para>Use it for displaying a meter to indicate how full an inventory is, or to make a visual indication of a health bar.</para>
    /// </summary>
    /// <example>
    /// <para>The following example shows how ProgressBar can be used.</para>
    /// <code>
    /// public class ProgressBarExample : MonoBehaviour
    /// {
    ///		// Default progress bar.
    ///		[ProgressBar(0, 100)]
    ///		public int ProgressBar;
    ///
    ///		// Health bar.
    ///		[ProgressBar(0, 100, ColorMember = "GetHealthBarColor")]
    ///		public float HealthBar = 50;
    ///
    ///		private Color GetHealthBarColor(float value)
    ///		{
    ///			// Blends between red, and yellow color for when the health is below 30,
    ///			// and blends between yellow and green color for when the health is above 30.
    ///			return Color.Lerp(Color.Lerp(
    ///				Color.red, Color.yellow, MathUtilities.LinearStep(0f, 30f, value)),
    ///				Color.green, MathUtilities.LinearStep(0f, 100f, value));
    ///		}
    ///
    ///		// Stacked health bar.
    ///		// The ProgressBar attribute is placed on property, without a set method, so it can't be edited directly.
    ///		// So instead we have this Range attribute on a float to change the value.
    ///		[Range(0, 300)]
    ///		public float StackedHealth;
    ///
    ///		[ProgressBar(0, 100, ColorMember = "GetStackedHealthColor", BackgroundColorMember = "GetStackHealthBackgroundColor")]
    ///		private float StackedHealthProgressBar
    ///		{
    ///			// Loops the stacked health value between 0, and 100.
    ///			get { return this.StackedHealth - 100 * (int)((this.StackedHealth - 1) / 100); }
    ///		}
    ///
    ///		private Color GetStackedHealthColor()
    ///		{
    ///			return
    ///				this.StackedHealth > 200 ? Color.cyan :
    ///				this.StackedHealth > 100 ? Color.green :
    ///				Color.red;
    ///		}
    ///
    ///		private Color GetStackHealthBackgroundColor()
    ///		{
    ///			return
    ///				this.StackedHealth > 200 ? Color.green :
    ///				this.StackedHealth > 100 ? Color.red :
    ///				new Color(0.16f, 0.16f, 0.16f, 1f);
    ///		}
    ///
    ///		// Custom color and height.
    ///		[ProgressBar(-100, 100, r: 1, g: 1, b: 1, Height = 30)]
    ///		public short BigProgressBar = 50;
    ///		
    ///     // You can also reference members by name to dynamically assign the min and max progress bar values.
    ///     [ProgressBar("DynamicMin", "DynamicMax")]
    ///     public float DynamicProgressBar;
    ///     
    ///     public float DynamicMin, DynamicMax;
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="HideLabelAttribute"/>
    /// <seealso cref="PropertyRangeAttribute"/>
    /// <seealso cref="MinMaxSliderAttribute"/>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public sealed class ProgressBarAttribute : Attribute
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
        /// The name of a field, property or method to get the min values from.
        /// </summary>
        public string MinMember;

        /// <summary>
        /// The name of a field, property or method to get the max values from.
        /// </summary>
        public string MaxMember;

        /// <summary>
        /// The red channel of the color of the progress bar.
        /// </summary>
        public float R;

        /// <summary>
        /// The green channel of the color of the progress bar.
        /// </summary>
        public float G;

        /// <summary>
        /// The blue channel of the color of the progress bar.
        /// </summary>
        public float B;

        /// <summary>
        /// The height of the progress bar in pixels. Defaults to 12 pixels.
        /// </summary>
        public int Height;

        /// <summary>
        /// Optional reference to a Color field, property or method, to dynamically change the color of the progress bar.
        /// </summary>
        public string ColorMember;

        /// <summary>
        /// Optional reference to a Color field, property or method, to dynamically change the background color of the progress bar.
        /// Default background color is (0.16, 0.16, 0.16, 1).
        /// </summary>
        public string BackgroundColorMember;

        /// <summary>
        /// If <c>true</c> then the progress bar will be drawn in tiles.
        /// </summary>
        public bool Segmented;

        /// <summary>
        /// References a member by name to get a custom value label string from.
        /// </summary>
        public string CustomValueStringMember;

        private bool drawValueLabel;

        private TextAlignment valueLabelAlignment;

        /// <summary>
        /// Draws a progress bar for the value.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="r">The red channel of the color of the progress bar.</param>
        /// <param name="g">The green channel of the color of the progress bar.</param>
        /// <param name="b">The blue channel of the color of the progress bar.</param>
        public ProgressBarAttribute(double min, double max, float r = 0.15f, float g = 0.47f, float b = 0.74f)
        {
            this.Min = min;
            this.Max = max;
            this.R = r;
            this.G = g;
            this.B = b;
            this.Height = 12;
            this.Segmented = false;
            this.drawValueLabel = true;
            this.DrawValueLabelHasValue = false;
            this.valueLabelAlignment = TextAlignment.Center;
            this.ValueLabelAlignmentHasValue = false;
        }

        /// <summary>
        /// Draws a progress bar for the value.
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min values from.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="r">The red channel of the color of the progress bar.</param>
        /// <param name="g">The green channel of the color of the progress bar.</param>
        /// <param name="b">The blue channel of the color of the progress bar.</param>
        public ProgressBarAttribute(string minMember, double max, float r = 0.15f, float g = 0.47f, float b = 0.74f)
        {
            this.MinMember = minMember;
            this.Max = max;
            this.R = r;
            this.G = g;
            this.B = b;
            this.Height = 12;
            this.Segmented = false;
            this.drawValueLabel = true;
            this.DrawValueLabelHasValue = false;
            this.valueLabelAlignment = TextAlignment.Center;
            this.ValueLabelAlignmentHasValue = false;
        }

        /// <summary>
        /// Draws a progress bar for the value.
        /// </summary>
        /// <param name="min">The minimum value.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max values from.</param>
        /// <param name="r">The red channel of the color of the progress bar.</param>
        /// <param name="g">The green channel of the color of the progress bar.</param>
        /// <param name="b">The blue channel of the color of the progress bar.</param>
        public ProgressBarAttribute(double min, string maxMember, float r = 0.15f, float g = 0.47f, float b = 0.74f)
        {
            this.Min = min;
            this.MaxMember = maxMember;
            this.R = r;
            this.G = g;
            this.B = b;
            this.Height = 12;
            this.Segmented = false;
            this.drawValueLabel = true;
            this.DrawValueLabelHasValue = false;
            this.valueLabelAlignment = TextAlignment.Center;
            this.ValueLabelAlignmentHasValue = false;
        }

        /// <summary>
        /// Draws a progress bar for the value.
        /// </summary>
        /// <param name="minMember">The name of a field, property or method to get the min values from.</param>
        /// <param name="maxMember">The name of a field, property or method to get the max values from.</param>
        /// <param name="r">The red channel of the color of the progress bar.</param>
        /// <param name="g">The green channel of the color of the progress bar.</param>
        /// <param name="b">The blue channel of the color of the progress bar.</param>
        public ProgressBarAttribute(string minMember, string maxMember, float r = 0.15f, float g = 0.47f, float b = 0.74f)
        {
            this.MinMember = minMember;
            this.MaxMember = maxMember;
            this.R = r;
            this.G = g;
            this.B = b;
            this.Height = 12;
            this.Segmented = false;
            this.drawValueLabel = true;
            this.DrawValueLabelHasValue = false;
            this.valueLabelAlignment = TextAlignment.Center;
            this.ValueLabelAlignmentHasValue = false;
        }

        /// <summary>
        /// If <c>true</c> then there will be drawn a value label on top of the progress bar.
        /// </summary>
        public bool DrawValueLabel
        {
            get
            {
                return this.drawValueLabel;
            }

            set
            {
                this.drawValueLabel = value;
                this.DrawValueLabelHasValue = true;
            }
        }

        /// <summary>
        /// Gets a value indicating if the user has set a custom DrawValueLabel value.
        /// </summary>
        public bool DrawValueLabelHasValue { get; private set; }

        /// <summary>
        /// The alignment of the value label on top of the progress bar. Defaults to center.
        /// </summary>
        public TextAlignment ValueLabelAlignment
        {
            get
            {
                return this.valueLabelAlignment;
            }

            set
            {
                this.valueLabelAlignment = value;
                this.ValueLabelAlignmentHasValue = true;
            }
        }

        /// <summary>
        /// Gets a value indicating if the user has set a custom ValueLabelAlignment value.
        /// </summary>
        public bool ValueLabelAlignmentHasValue { get; private set; }
    }
}