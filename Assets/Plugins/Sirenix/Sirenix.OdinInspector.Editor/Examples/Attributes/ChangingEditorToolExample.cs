#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ChangingEditorToolExample.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using Sirenix.OdinInspector;

    [AttributeExampleDescription("Example of using EnumPaging together with OnValueChanged.")]
    [AttributeExample(typeof(EnumPagingAttribute), Order = 10)]
    [AttributeExample(typeof(OnValueChangedAttribute), Order = 10)]
    internal class ChangingEditorToolExample
    {
        [ShowInInspector]
        [EnumPaging, OnValueChanged("SetCurrentTool")]
        [InfoBox("Changing this property will change the current selected tool in the Unity editor.")]
        private UnityEditor.Tool sceneTool;

        private void SetCurrentTool()
        {
            UnityEditor.Tools.current = this.sceneTool;
        }
    }
}
#endif