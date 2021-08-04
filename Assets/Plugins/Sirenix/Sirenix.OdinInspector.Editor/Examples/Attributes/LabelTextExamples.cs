#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="LabelTextExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(LabelTextAttribute), "Specify a different label text for your properties.")]
    internal class LabelTextExamples
    {
        [LabelText("1")]
        public int MyInt1;

        [LabelText("2")]
        public int MyInt2;

        [LabelText("3")]
        public int MyInt3;

		[InfoBox("Use $ to refer to a member string.")]
		[LabelText("$MyInt3")]
		public string LabelText = "Dynamic label text";
    }
}
#endif