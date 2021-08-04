#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="BoxGroupExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;

    [AttributeExample(typeof(BoxGroupAttribute))]
    internal class BoxGroupExamples
    {
        // Box with a title.
        [BoxGroup("Some Title")]
        public string A;

        [BoxGroup("Some Title")]
        public string B;

        // Box with a centered title.
        [BoxGroup("Centered Title", centerLabel: true)]
        public string E;

        [BoxGroup("Centered Title")]
        public string F;

        // Box with a title received from a field.
        [BoxGroup("$G")]
        public string G = "Dynamic box title 2";

        [BoxGroup("$G")]
        public string H;

        // No title
        [BoxGroup]
        public string C;

        [BoxGroup]
        public string D;

        // A named box group without a title.
        [BoxGroup("NoTitle", false)]
        public string I;

        [BoxGroup("NoTitle")]
        public string J;

        [BoxGroup("A Struct In A Box"), HideLabel]
        public SomeStruct BoxedStruct;

        public SomeStruct DefaultStruct;

        [Serializable]
        public struct SomeStruct
        {
            public int One;
            public int Two;
            public int Three;
        }
    }

}
#endif