#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinPropertyProcessorLocator.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="OdinPropertyProcessorLocator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    public static class OdinPropertyProcessorLocator
    {
        private static readonly Dictionary<Type, OdinPropertyProcessor> EmptyInstances = new Dictionary<Type, OdinPropertyProcessor>();
        public static readonly TypeSearchIndex SearchIndex = new TypeSearchIndex() { MatchedTypeLogName = "member property processor" };
        private static readonly List<TypeSearchResult[]> CachedQueryList = new List<TypeSearchResult[]>();

        static OdinPropertyProcessorLocator()
        {
            SearchIndex.AddIndexedTypes(
                ResolverUtilities.GetResolverAssemblies()
                    .SelectMany(a => a.SafeGetTypes())
                    .Where(type => !type.IsAbstract
                                && typeof(OdinPropertyProcessor).IsAssignableFrom(type)
                                && !type.IsDefined<OdinDontRegisterAttribute>(false))
                    .Select(type =>
                    {
                        if (type.ImplementsOpenGenericClass(typeof(OdinPropertyProcessor<>)))
                        {
                            if (type.ImplementsOpenGenericClass(typeof(OdinPropertyProcessor<,>)))
                            {
                                // Value/attribute targeted resolver
                                return new TypeSearchInfo()
                                {
                                    MatchType = type,
                                    Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinPropertyProcessor<,>)),
                                    Priority = ResolverUtilities.GetResolverPriority(type)
                                };
                            }

                            // Value targeted resolver
                            return new TypeSearchInfo()
                            {
                                MatchType = type,
                                Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinPropertyProcessor<>)),
                                Priority = ResolverUtilities.GetResolverPriority(type)
                            };
                        }

                        // No target constraints resolver (only CanResolveForProperty)
                        return new TypeSearchInfo()
                        {
                            MatchType = type,
                            Targets = Type.EmptyTypes,
                            Priority = ResolverUtilities.GetResolverPriority(type)
                        };
                    }));
        }

        public static List<OdinPropertyProcessor> GetMemberProcessors(InspectorProperty property)
        {
            var queries = CachedQueryList;
            queries.Clear();

            //var results = CachedResultList;
            //results.Clear();

            queries.Add(SearchIndex.GetMatches(Type.EmptyTypes));

            if (property.ValueEntry != null)
            {
                var valueType = property.ValueEntry.TypeOfValue;

                queries.Add(SearchIndex.GetMatches(valueType));

                for (int i = 0; i < property.Attributes.Count; i++)
                {
                    queries.Add(SearchIndex.GetMatches(valueType, property.Attributes[i].GetType()));
                }
            }

            var results = TypeSearchIndex.GetCachedMergedQueryResults(queries);

            List<OdinPropertyProcessor> processors = new List<OdinPropertyProcessor>();

            for (int i = 0; i < results.Length; i++)
            {
                var result = results[i];
                if (GetEmptyInstance(result.MatchedType).CanProcessForProperty(property))
                {
                    processors.Add(OdinPropertyProcessor.Create(result.MatchedType, property));
                }
            }

            return processors;
        }

        private static OdinPropertyProcessor GetEmptyInstance(Type type)
        {
            OdinPropertyProcessor result;
            if (!EmptyInstances.TryGetValue(type, out result))
            {
                result = (OdinPropertyProcessor)FormatterServices.GetUninitializedObject(type);
                EmptyInstances[type] = result;
            }
            return result;
        }
    }
}
#endif