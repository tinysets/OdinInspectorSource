#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InheritAttributeAttributesAttributeProcessor.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="InheritAttributeAttributesAttributeProcessor.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Resolvers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;

    /// <summary>
    /// This attribute processor will take any attribute already applied to the property with the <see cref="IncludeMyAttributesAttribute"/> applied to,
    /// and take all attributes applied to the attribute (except any <see cref="AttributeUsageAttribute"/>) and add to them to the property.
    /// This allows for adding attributes to attributes in the property system.
    /// </summary>
    [ResolverPriority(-100000)]
    public class InheritAttributeAttributesAttributeProcessor : OdinAttributeProcessor
    {
        /// <summary>
        /// Looks for attributes in the attributes list with a <see cref="IncludeMyAttributesAttribute"/> applied, and adds the attribute from those attributes to the property.
        /// </summary>
        /// <param name="parentProperty">The parent of the member.</param>
        /// <param name="member">The member that is being processed.</param>
        /// <param name="attributes">The list of attributes currently applied to the property.</param>
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes)
        {
            for (int i = attributes.Count - 1; i >= 0; i--)
            {
                if (attributes[i].GetType().IsDefined<IncludeMyAttributesAttribute>())
                {
                    attributes.AddRange(attributes[i].GetType().GetCustomAttributes<Attribute>(true).Where(a => a is AttributeUsageAttribute == false));
                }
            }
        }
    }
}
#endif