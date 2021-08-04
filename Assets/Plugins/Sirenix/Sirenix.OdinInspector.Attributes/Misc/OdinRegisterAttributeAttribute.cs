//-----------------------------------------------------------------------// <copyright file="OdinRegisterAttributeAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class OdinRegisterAttributeAttribute : Attribute
    {
        public Type AttributeType;
        public string Categories;
        public string Description;
        public string PngEncodedIcon;
        public string IconPath;
        public string DocumentationUrl;
        
        public OdinRegisterAttributeAttribute(Type attributeType, string category, string description)
        {
            this.AttributeType = attributeType;
            this.Categories = category;
            this.Description = description;
        }
    }
}