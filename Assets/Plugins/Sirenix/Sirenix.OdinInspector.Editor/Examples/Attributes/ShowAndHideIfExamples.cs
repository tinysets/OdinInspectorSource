#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ShowAndHideIfExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    // Note that you can also reference methods and properties. You are not limited to fields.
    [AttributeExample(typeof(ShowIfAttribute))]
    [AttributeExample(typeof(HideIfAttribute))]
    internal class ShowAndHideIfExamples
    {
        public UnityEngine.Object SomeObject;

        [EnumToggleButtons]
        public InfoMessageType SomeEnum;

        public bool IsToggled;

        [ShowIf("SomeEnum", InfoMessageType.Info)]
        public Vector3 Info;

        [ShowIf("SomeEnum", InfoMessageType.Warning)]
        public Vector2 Warning;

        [ShowIf("SomeEnum", InfoMessageType.Error)]
        public Vector3 Error;

        [ShowIf("IsToggled")]
        public Vector2 VisibleWhenToggled;

        [HideIf("IsToggled")]
        public Vector3 HiddenWhenToggled;

        [HideIf("SomeObject")]
        public Vector3 ShowWhenNull;

        [ShowIf("SomeObject")]
        public Vector3 HideWhenNull;
    }
}
#endif