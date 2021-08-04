#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="HideDuplicateReferenceBoxExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using UnityEngine;
    using System.Collections.Generic;
    using Sirenix.Utilities.Editor;

    public class HideDuplicateReferenceBoxExamples
    {
        [HideDuplicateReferenceBox, PropertyOrder(1)]
        public ReferenceTypeClass firstObject;

        [PropertyOrder(3)]
        public ReferenceTypeClass withReferenceBox;

        [HideDuplicateReferenceBox, PropertyOrder(5)]
        public ReferenceTypeClass withoutReferenceBox;


        [OnInspectorGUI, PropertyOrder(0)]
        private void MessageBox1()
        {
            SirenixEditorGUI.MessageBox("The first reference will always be drawn normally.");
        }

        [OnInspectorGUI, PropertyOrder(2)]
        private void MessageBox2()
        {
            SirenixEditorGUI.MessageBox("All subsequent references will be wrapped in a reference box.");
        }

        [OnInspectorGUI, PropertyOrder(4)]
        private void MessageBox3()
        {
            SirenixEditorGUI.MessageBox("With the [HideDuplicateReferenceBox] attribute, however, this behaviour can be suppressed, and the reference box can be hidden.");
        }

        public class ReferenceTypeClass
        {
            [OnInspectorGUI, PropertyOrder(-1)]
            private void MessageBox()
            {
                SirenixEditorGUI.WarningMessageBox("Recursively drawn references will always show the reference box regardless, to prevent infinite depth draw loops.");
            }

            [HideDuplicateReferenceBox]
            public ReferenceTypeClass recursiveReference;
        }

        public HideDuplicateReferenceBoxExamples()
        {
            var obj = new ReferenceTypeClass();
            this.firstObject = obj;
            this.withReferenceBox = obj;
            this.withoutReferenceBox = obj;
            obj.recursiveReference = obj;
        }
    }
}
#endif