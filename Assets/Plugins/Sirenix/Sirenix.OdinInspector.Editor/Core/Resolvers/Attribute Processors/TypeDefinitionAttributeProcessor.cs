#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TypeDefinitionAttributeProcessor.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TypeDefinitionAttributeProcessor.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sirenix.Utilities;

    /// <summary>
    /// Find attributes attached to the type definition of a property and adds to them to attribute list.
    /// </summary>
    [ResolverPriority(1000)]
    public class TypeDefinitionAttributeProcessor : OdinAttributeProcessor
    {
        /// <summary>
        /// This attribute processor can only process for properties.
        /// </summary>
        /// <param name="parentProperty">The parent of the specified member.</param>
        /// <param name="member">The member to process.</param>
        /// <returns><c>false</c>.</returns>
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return false;
        }

        /// <summary>
        /// This attribute processor can only process for properties with an attached value entry.
        /// </summary>
        /// <param name="property">The property to process.</param>
        /// <returns><c>true</c> if the specified property has a value entry. Otherwise <c>false</c>.</returns>
        public override bool CanProcessSelfAttributes(InspectorProperty property)
        {
            return property.ValueEntry != null;
        }

        /// <summary>
        /// Finds all attributes attached to the type and base types of the specified property value and adds them to the attribute list.
        /// </summary>
        /// <param name="property">The property to process.</param>
        /// <param name="attributes">The list of attributes for the property.</param>
        public override void ProcessSelfAttributes(InspectorProperty property, List<Attribute> attributes)
        {
            if (property.ValueEntry == null) return;

            var current = property.ValueEntry.TypeOfValue;

            while (ContinueAddingAttributesFor(current))
            {
                attributes.AddRange(current.GetAttributes());
                current = current.BaseType;
            }
        }

        private static bool ContinueAddingAttributesFor(Type type)
        {
            if (type == null) return false;

            var flag = AssemblyUtilities.GetAssemblyTypeFlag(type.Assembly);

            if (flag == AssemblyTypeFlags.OtherTypes) return false;
            if (flag == AssemblyTypeFlags.UnityTypes) return false;

            return true;
        }
    }


    [ResolverPriority(1000)]
    public class TypeDefinitionGroupAttributeProcessor : OdinAttributeProcessor
    {
        public override bool CanProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member)
        {
            return member.GetReturnType() != null;
        }

        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            var current = member.GetReturnType();

            while (ContinueAddingAttributesFor(current))
            {
                attributes.AddRange(current.GetAttributes().Where(x => x is PropertyGroupAttribute));
                current = current.BaseType;
            }
        }

        private static bool ContinueAddingAttributesFor(Type type)
        {
            if (type == null) return false;
            var flag = AssemblyUtilities.GetAssemblyTypeFlag(type.Assembly);
            if (flag == AssemblyTypeFlags.OtherTypes) return false;
            if (flag == AssemblyTypeFlags.UnityTypes) return false;
            return true;
        }
    }
}
#endif