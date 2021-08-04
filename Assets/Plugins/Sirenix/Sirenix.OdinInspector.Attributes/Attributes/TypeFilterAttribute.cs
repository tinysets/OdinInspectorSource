//-----------------------------------------------------------------------// <copyright file="TypeFilterAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ValueDropdownAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class TypeFilterAttribute : Attribute
    {
        /// <summary>
        /// Name of any field, property or method member that implements IList. E.g. arrays or Lists.
        /// </summary>
        public string MemberName;

        /// <summary>
        /// Gets or sets the title for the dropdown. Null by default.
        /// </summary>
        public string DropdownTitle;

        /// <summary>
        /// Creates a dropdown menu for a property.
        /// </summary>
        /// <param name="memberName">Name of any field, property or method member that implements IList. E.g. arrays or Lists.</param>
        public TypeFilterAttribute(string memberName)
        {
            this.MemberName = memberName;
        }
    }
}