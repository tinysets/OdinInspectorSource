#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="MinMaxSliderExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;

    [AttributeExample(typeof(MinMaxSliderAttribute), "Uses a Vector2 where x is the min knob and y is the max knob.")]
    internal class MinMaxSliderExamples
    {
        [MinMaxSlider(-10, 10)]
        public Vector2 MinMaxValueSlider;

        [MinMaxSlider(-10, 10, true)]
		public Vector2 WithFields;

        [InfoBox("You can also assign the min max values dynamically by refering to members.")]
        [MinMaxSlider("DynamicRange", true)]
        public Vector2 DynamicMinMax;

        [MinMaxSlider("Min", 10f, true)]
        public Vector2 DynamicMin;

        [MinMaxSlider(0f, "Max", true)]
        public Vector2 DynamicMax;

        public Vector2 DynamicRange;

        public float Min { get { return this.DynamicRange.x; } }

        public float Max { get { return this.DynamicRange.y; } }
    }
}
#endif