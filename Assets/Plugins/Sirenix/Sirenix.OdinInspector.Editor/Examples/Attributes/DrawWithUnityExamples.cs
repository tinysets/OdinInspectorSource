#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DrawWithUnityExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;

    [AttributeExample(typeof(DrawWithUnityAttribute))]
    internal class DrawWithUnityExamples
    {
        [InfoBox("If you ever experience trouble with one of Odin's attributes, there is a good chance that the DrawWithUnity will come in handy.")]
        public GameObject ObjectDrawnWithOdin;

        [DrawWithUnity]
        public GameObject ObjectDrawnWithUnity;
    }
}
#endif