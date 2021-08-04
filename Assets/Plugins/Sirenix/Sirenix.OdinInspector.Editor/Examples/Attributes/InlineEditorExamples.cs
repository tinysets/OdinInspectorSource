#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InlineEditorExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using Sirenix.OdinInspector;

    [AttributeExample(typeof(InlineEditorAttribute))]
    internal class InlineEditorExamples
    {
        [DisableInInlineEditors]
        public Vector3 DisabledInInlineEditors;

        [HideInInlineEditors]
        public Vector3 HiddenInInlineEditors;

        [InlineEditor]
        public InlineEditorExamples Self;

        [InlineEditor]
        public Transform InlineComponent;

        [InlineEditor(InlineEditorModes.FullEditor)]
        public Material FullInlineEditor;

        [InlineEditor(InlineEditorModes.GUIAndHeader)]
        public Material InlineMaterial;

        [InlineEditor(InlineEditorModes.SmallPreview)]
        public Material[] InlineMaterialList;

        [InlineEditor(InlineEditorModes.LargePreview)]
        public Mesh InlineMeshPreview;

        [Header("Boxed / Default")]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public Transform Boxed;

        [Header("Foldout")]
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public MinMaxSliderExamples Foldout;

        [Header("Hide ObjectField")]
        [InlineEditor(InlineEditorObjectFieldModes.CompletelyHidden)]
        public Transform CompletelyHidden;

        [Header("Show ObjectField if null")]
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        public Transform OnlyHiddenWhenNotNull;
    }
}
#endif