#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PreviewFieldsExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;

    [AttributeExample(typeof(PreviewFieldAttribute))]
    internal class PreviewFieldsExamples
    {
        [PreviewField]
        public Object RegularPreviewField;

        [VerticalGroup("row1/left")]
        public string A, B, C;

        [HideLabel]
        [PreviewField(50, ObjectFieldAlignment.Right)]
        [HorizontalGroup("row1", 50), VerticalGroup("row1/right")]
        public Object D;

        [HideLabel]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        [HorizontalGroup("row2", 50), VerticalGroup("row2/left")]
        public Object E;

        [VerticalGroup("row2/right"), LabelWidth(-54)]
        public string F, G, H;

        [InfoBox(
            "These object fields can also be selectively enabled and customized globally " +
            "from the Odin preferences window.\n\n" +
            " - Hold Ctrl + Click = Delete Instance\n" +
            " - Drag and drop = Move / Swap.\n" +
            " - Ctrl + Drag = Replace.\n" +
            " - Ctrl + drag and drop = Move and override.")]
        [PropertyOrder(-1)]
        [Button(ButtonSizes.Large)]
        private void ConfigureGlobalPreviewFieldSettings()
        {
            Sirenix.OdinInspector.Editor.GeneralDrawerConfig.Instance.OpenInEditor();   
        }
    }
}
#endif