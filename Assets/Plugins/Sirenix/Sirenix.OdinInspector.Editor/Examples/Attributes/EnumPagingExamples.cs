#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="EnumPagingExamples.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
#pragma warning disable
namespace Sirenix.OdinInspector.Editor.Examples
{
    using Sirenix.OdinInspector;

    [AttributeExample(typeof(EnumPagingAttribute))]
    internal class EnumPagingExamples
    {
        [EnumPaging]
        public SomeEnum SomeEnumField;
        
        public enum SomeEnum
        {
            A, B, C
        }
    }
}
#endif