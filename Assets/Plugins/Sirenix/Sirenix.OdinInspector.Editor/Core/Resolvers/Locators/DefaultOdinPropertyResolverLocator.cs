#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DefaultOdinPropertyResolverLocator.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DefaultPropertyResolverLocator.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using Sirenix.Utilities;

    /// <summary>
    /// Default implementation and the version that will be used by <see cref="PropertyTree"/> if no other <see cref="OdinPropertyResolver"/> instance have been specified.
    /// </summary>
    public class DefaultOdinPropertyResolverLocator : OdinPropertyResolverLocator
    {
        /// <summary>
        /// Singleton instance of <see cref="DefaultOdinPropertyResolverLocator"/>.
        /// </summary>
        public static readonly DefaultOdinPropertyResolverLocator Instance = new DefaultOdinPropertyResolverLocator();

        private static readonly TypeSearchIndex SearchIndex = new TypeSearchIndex();
        
        private static Dictionary<Type, OdinPropertyResolver> resolverEmptyInstanceMap = new Dictionary<Type, OdinPropertyResolver>(FastTypeComparer.Instance);

        private static readonly List<TypeSearchResult[]> QueryResultsList = new List<TypeSearchResult[]>();
        private static readonly List<TypeSearchResult> MergedSearchResultsList = new List<TypeSearchResult>();

        static DefaultOdinPropertyResolverLocator()
        {
            SearchIndex.AddIndexedTypes(ResolverUtilities.GetResolverAssemblies()
                .SelectMany(a => a.SafeGetTypes())
                .Where(type => type.IsAbstract == false
                    && typeof(OdinPropertyResolver).IsAssignableFrom(type)
                    && !type.IsDefined<OdinDontRegisterAttribute>())
                .Select(type =>
                {
                    var result = new TypeSearchInfo()
                    {
                        MatchType = type,
                        Priority = ResolverUtilities.GetResolverPriority(type),
                    };

                    if (type.ImplementsOpenGenericType(typeof(OdinPropertyResolver<>)))
                    {
                        if (type.ImplementsOpenGenericType(typeof(OdinPropertyResolver<,>)))
                        {
                            result.Targets = type.GetArgumentsOfInheritedOpenGenericType(typeof(OdinPropertyResolver<,>));
                        }
                        else
                        {
                            result.Targets = type.GetArgumentsOfInheritedOpenGenericType(typeof(OdinPropertyResolver<>));
                        }
                    }
                    else
                    {
                        result.Targets = Type.EmptyTypes;
                    }

                    return result;
                }));
        }

        /// <summary>
        /// Gets an <see cref="OdinPropertyResolver"/> instance for the specified property.
        /// </summary>
        /// <param name="property">The property to get an <see cref="OdinPropertyResolver"/> instance for.</param>
        /// <returns>An instance of <see cref="OdinPropertyResolver"/> to resolver the specified property.</returns>
        public override OdinPropertyResolver GetResolver(InspectorProperty property)
        {
            if (property.Tree.IsStatic && property == property.Tree.SecretRootProperty)
            {
                return OdinPropertyResolver.Create(typeof(StaticRootPropertyResolver<>).MakeGenericType(property.ValueEntry.TypeOfValue), property);
            }

            var queries = QueryResultsList;
            queries.Clear();

            queries.Add(SearchIndex.GetMatches(Type.EmptyTypes));

            Type typeOfValue = property.ValueEntry != null ? property.ValueEntry.TypeOfValue : null;

            if (typeOfValue != null)
            {
                queries.Add(SearchIndex.GetMatches(typeOfValue));

                for (int i = 0; i < property.Attributes.Count; i++)
                {
                    queries.Add(SearchIndex.GetMatches(typeOfValue, property.Attributes[i].GetType()));
                }
            }

            TypeSearchIndex.MergeQueryResultsIntoList(queries, MergedSearchResultsList);

            for (int i = 0; i < MergedSearchResultsList.Count; i++)
            {
                var info = MergedSearchResultsList[i];

                if (GetEmptyResolverInstance(info.MatchedType).CanResolveForPropertyFilter(property))
                {
                    return OdinPropertyResolver.Create(info.MatchedType, property);
                }
            }

            return OdinPropertyResolver.Create<EmptyPropertyResolver>(property);
        }

        private OdinPropertyResolver GetEmptyResolverInstance(Type resolverType)
        {
            OdinPropertyResolver result;
            if (!resolverEmptyInstanceMap.TryGetValue(resolverType, out result))
            {
                result = (OdinPropertyResolver)FormatterServices.GetUninitializedObject(resolverType);
                resolverEmptyInstanceMap[resolverType] = result;
            }
            return result;
        }
    }
}
#endif