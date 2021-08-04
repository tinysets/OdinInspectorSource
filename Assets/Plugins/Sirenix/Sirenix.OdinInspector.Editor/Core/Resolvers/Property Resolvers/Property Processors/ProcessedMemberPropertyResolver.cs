#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ProcessedMemberPropertyResolver.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ProcessedMemberPropertyResolver.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using System.Collections.Generic;

    [ResolverPriority(-5)]
    public class ProcessedMemberPropertyResolver<T> : BaseMemberPropertyResolver<T>
    {
        private List<OdinPropertyProcessor> processors;

        protected override InspectorPropertyInfo[] GetPropertyInfos()
        {
            if (this.processors == null)
            {
                this.processors = OdinPropertyProcessorLocator.GetMemberProcessors(this.Property);
            }

            var includeSpeciallySerializedMembers = this.Property.ValueEntry.SerializationBackend != SerializationBackend.Unity;
            var infos = InspectorPropertyInfoUtility.CreateMemberProperties(this.Property, typeof(T), includeSpeciallySerializedMembers);

            for (int i = 0; i < this.processors.Count; i++)
            {
                ProcessedMemberPropertyResolverExtensions.ProcessingOwnerType = typeof(T);
                this.processors[i].ProcessMemberProperties(infos);
            }

            return InspectorPropertyInfoUtility.BuildPropertyGroupsAndFinalize(this.Property, typeof(T), infos, includeSpeciallySerializedMembers);
        }
    }
}
#endif