#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyOrderExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(PropertyOrderAttribute))]
    internal class PropertyOrderExamples
    {
		[PropertyOrder(1)]
		public int Second;

		[InfoBox("PropertyOrder is used to change the order of properties in the inspector.")]
		[PropertyOrder(-1)]
		public int First;
	}
}
#endif