#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DisableIfExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;

    [AttributeExample(typeof(DisableIfExamples))]
    internal class DisableIfExamples
    {
        public UnityEngine.Object SomeObject;

        [EnumToggleButtons]
        public InfoMessageType SomeEnum;

        public bool IsToggled;

        [DisableIf("SomeEnum", InfoMessageType.Info)]
        public Vector2 Info;

        [DisableIf("SomeEnum", InfoMessageType.Error)]
        public Vector2 Error;

        [DisableIf("SomeEnum", InfoMessageType.Warning)]
        public Vector2 Warning;

        [DisableIf("IsToggled")]
        public int DisableIfToggled;

        [DisableIf("SomeObject")]
        public Vector3 EnabledWhenNull;
    }
}
#endif