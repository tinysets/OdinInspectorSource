#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TypeInfoBoxAttributePropertyProcessorProcessor.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TypeInfoBoxAttributePropertyProcessorProcessor.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Sirenix.OdinInspector;
    using Sirenix.Utilities;

    [ResolverPriority(-10)]
    public class TypeInfoBoxAttributePropertyProcessorProcessor : OdinPropertyProcessor
    {
        private static readonly MemberInfo InjectedMemberInfo = typeof(TypeInfoBoxAttributePropertyProcessorProcessor).GetMember("InjectedMember", Flags.AllMembers).First();

        private static void InjectedMember()
        {
        }

        private TypeInfoBoxAttribute GetClassDefinedTypeInfoBox(InspectorProperty property)
        {
            Type parentType;
            if (property.Tree.SecretRootProperty == property)
            {
                parentType = property.Tree.WeakTargets[0].GetType();
            }
            else
            {
                parentType = property.ValueEntry.TypeOfValue;
            }

            return parentType.GetCustomAttribute<TypeInfoBoxAttribute>(true);
        }

        public override bool CanProcessForProperty(InspectorProperty property)
        {
            return this.GetClassDefinedTypeInfoBox(property) != null;
        }

        public override void ProcessMemberProperties(List<InspectorPropertyInfo> memberInfos)
        {
            var attr = this.GetClassDefinedTypeInfoBox(this.Property);
            var p = InspectorPropertyInfo.CreateForMember(InjectedMemberInfo, false, SerializationBackend.None, new Attribute[] { new OnInspectorGUIAttribute(), new InfoBoxAttribute(attr.Message), new PropertyOrderAttribute(-100000) });
            memberInfos.Insert(0, p);
        }
    }
}
#endif