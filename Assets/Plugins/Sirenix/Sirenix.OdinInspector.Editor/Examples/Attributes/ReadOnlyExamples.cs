#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ReadOnlyExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(ReadOnlyAttribute), "ReadOnly disables properties in the inspector.")]
    internal class ReadOnlyExamples
    {
        [ReadOnly]
        public string MyString = "This is displayed as text";

        [ReadOnly]
        public int MyInt = 9001;

        [ReadOnly]
        public int[] MyIntList = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    }
}
#endif