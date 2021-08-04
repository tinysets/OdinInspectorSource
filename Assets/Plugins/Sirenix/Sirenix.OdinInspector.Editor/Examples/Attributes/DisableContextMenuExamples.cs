#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DisableContextMenuExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    [AttributeExample(typeof(DisableContextMenuAttribute))]
    internal class DisableContextMenuExamples
    {
        [InfoBox("DisableContextMenu disables all right-click context menus provided by Odin. It does not disable Unity's context menu.", InfoMessageType.Warning)]
        [DisableContextMenu]
        public int[] NoRightClickList;

        [DisableContextMenu(disableForMember: false, disableCollectionElements: true)]
        public int[] NoRightClickListOnListElements;

        [DisableContextMenu(disableForMember: true, disableCollectionElements: true)]
        public int[] DisableRightClickCompletely;

        [DisableContextMenu]
        public int NoRightClickField;
    }
}
#endif