#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ShowInInspectorExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(ShowInInspectorAttribute),
        "ShowInInspector is used to display properties that otherwise wouldn't be shown in the inspector. Such as non-serialized fields or properties.")]
    internal class ShowInInspectorExamples
    {
        [ShowInInspector]
        private int myPrivateInt;

        [ShowInInspector]
        public int MyPropertyInt { get; set; }

        [ShowInInspector]
        public int ReadOnlyProperty
        {
            get { return this.myPrivateInt; }
        }

        [ShowInInspector]
        public static bool StaticProperty { get; set; }
    }
}
#endif