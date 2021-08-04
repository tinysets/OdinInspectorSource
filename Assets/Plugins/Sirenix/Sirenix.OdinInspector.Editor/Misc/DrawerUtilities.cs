#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DrawerUtilities.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DrawerUtilities.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.OdinInspector.Editor.Drawers;
    using Sirenix.OdinInspector.Editor.TypeSearch;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Serialization;
    using UnityEditor;
    using UnityEngine;

    public static class DrawerUtilities
    {
        private static class Null { }

        public static readonly TypeSearchIndex SearchIndex = new TypeSearchIndex() { MatchedTypeLogName = "drawer" };

        private static List<Type> AllDrawerTypes = new List<Type>();

        private static readonly FieldInfo CustomPropertyDrawerTypeField = typeof(CustomPropertyDrawer).GetField("m_Type", Flags.InstanceAnyVisibility);
        private static readonly FieldInfo CustomPropertyDrawerUseForChildrenField = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", Flags.InstanceAnyVisibility);

        private static readonly Dictionary<Type, DrawerPriority> DrawerTypePriorityLookup = new Dictionary<Type, DrawerPriority>(FastTypeComparer.Instance);
        private static readonly Dictionary<Type, OdinDrawer> UninitializedDrawers = new Dictionary<Type, OdinDrawer>(FastTypeComparer.Instance);

        private static readonly List<TypeSearchResult[]> CachedQueryResultList = new List<TypeSearchResult[]>(10);

        private static readonly TypeMatchRule InvalidAttributeRule = new TypeMatchRule(
            "Invalid Attribute Notification Dummy Rule (This is never matched against, but only serves to be a result rule for invalid attribute type search results)",
            (info, target) => null);

        /// <summary>
        /// Odin has its own implementations for these attribute drawers; never use Unity's.
        /// </summary>
        private static readonly HashSet<string> ExcludeUnityDrawers = new HashSet<string>()
        {
            "HeaderDrawer",
            "DelayedDrawer",
            "MultilineDrawer",
            "RangeDrawer",
            "SpaceDrawer",
            "TextAreaDrawer",
            "ColorUsageDrawer"
        };

        static DrawerUtilities()
        {
            //Profiler.BeginSample("DrawerUtilities.cctor");
            // This method is *very* expensive performance-wise and generates lots of garbage due to liberal use of LINQ for readability.
            // This is acceptable, as it only runs once per AppDomain reload, and only ever in the editor.

            //
            // First, get all relevant types
            //

            var typesToSearch = AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes | AssemblyTypeFlags.UnityEditorTypes)
                            .Where(type => !type.IsAbstract && type.IsClass && !type.IsDefined(typeof(OdinDontRegisterAttribute), false) && (typeof(OdinDrawer).IsAssignableFrom(type) || (typeof(GUIDrawer).IsAssignableFrom(type) && (!(type.Namespace ?? "").StartsWith("Unity", StringComparison.InvariantCulture) || !ExcludeUnityDrawers.Contains(type.Name)))))
                            .ToList();

            //
            // Find all regular Unity property and decorator drawers and create alias drawers for them
            //

            IEnumerable<Type> unityPropertyDrawers;
            IEnumerable<Type> unityPropertyAttributeDrawers;
            IEnumerable<Type> unityDecoratorDrawers;

            if (DrawerUtilities.CustomPropertyDrawerTypeField != null && DrawerUtilities.CustomPropertyDrawerUseForChildrenField != null)
            {
                unityPropertyDrawers =
                    typesToSearch.Where(type => type.IsGenericTypeDefinition == false && typeof(PropertyDrawer).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
                            .SelectMany(type => type.GetCustomAttributes<CustomPropertyDrawer>(true).Select(attr => new { Type = type, Attribute = attr }))
                            .Where(n =>
                            {
                                if (n.Attribute != null)
                                {
                                    var drawnType = CustomPropertyDrawerTypeField.GetValue(n.Attribute) as Type;

                                    if (drawnType != null && !typeof(PropertyAttribute).IsAssignableFrom(drawnType))
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            })
                            .Select(n =>
                            {
                                var drawnType = (Type)CustomPropertyDrawerTypeField.GetValue(n.Attribute);

                                if (drawnType.IsAbstract || (bool)DrawerUtilities.CustomPropertyDrawerUseForChildrenField.GetValue(n.Attribute))
                                {
                                    var tArg = typeof(AbstractTypeUnityPropertyDrawer<,,>).GetGenericArguments()[2];
                                    return typeof(AbstractTypeUnityPropertyDrawer<,,>).MakeGenericType(n.Type, drawnType, tArg);
                                }
                                else
                                {
                                    return typeof(UnityPropertyDrawer<,>).MakeGenericType(n.Type, drawnType);
                                }
                            });

                unityPropertyAttributeDrawers =
                    typesToSearch.Where(type => type.IsGenericTypeDefinition == false && typeof(PropertyDrawer).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
                            .SelectMany(type => type.GetCustomAttributes<CustomPropertyDrawer>(true).Select(attr => new { Type = type, Attribute = attr }))
                            .Where(n =>
                            {
                                if (n.Attribute != null)
                                {
                                    var drawnType = CustomPropertyDrawerTypeField.GetValue(n.Attribute) as Type;

                                    if (drawnType != null && typeof(PropertyAttribute).IsAssignableFrom(drawnType))
                                    {
                                        return true;
                                    }
                                }

                                return false;
                            })
                            .Select(n =>
                            {
                                Type drawnType = (Type)CustomPropertyDrawerTypeField.GetValue(n.Attribute);

                                if ((bool)DrawerUtilities.CustomPropertyDrawerUseForChildrenField.GetValue(n.Attribute))
                                {
                                    var tAttributeArgParam = typeof(UnityPropertyAttributeDrawer<,,>).GetGenericArguments()[1];
                                    return typeof(UnityPropertyAttributeDrawer<,,>).MakeGenericType(n.Type, tAttributeArgParam, drawnType);
                                }
                                else
                                {
                                    return typeof(UnityPropertyAttributeDrawer<,,>).MakeGenericType(n.Type, drawnType, typeof(PropertyAttribute));
                                }
                            });

                unityDecoratorDrawers =
                    typesToSearch.Where(type => type.IsGenericTypeDefinition == false && typeof(DecoratorDrawer).IsAssignableFrom(type) && type.GetConstructor(Type.EmptyTypes) != null)
                            .Select(type => new { Type = type, Attribute = type.GetCustomAttribute<CustomPropertyDrawer>(true) })
                            .Where(n => n.Attribute != null)
                            .Select(n => new { Type = n.Type, Attribute = n.Attribute, DrawnType = CustomPropertyDrawerTypeField.GetValue(n.Attribute) as Type })
                            .Where(n => n.DrawnType != null && typeof(PropertyAttribute).IsAssignableFrom(n.DrawnType))
                            .Select(n =>
                            {
                                if ((bool)DrawerUtilities.CustomPropertyDrawerUseForChildrenField.GetValue(n.Attribute))
                                {
                                    var tAttributeArgParam = typeof(UnityDecoratorAttributeDrawer<,,>).GetGenericArguments()[1];
                                    return typeof(UnityDecoratorAttributeDrawer<,,>).MakeGenericType(n.Type, tAttributeArgParam, n.DrawnType);
                                }
                                else
                                {
                                    return typeof(UnityDecoratorAttributeDrawer<,,>).MakeGenericType(n.Type, n.DrawnType, typeof(PropertyAttribute));
                                }
                            });
            }
            else
            {
                Debug.LogWarning("Could not find internal fields 'm_Type' and/or 'm_UseForChildren' in type CustomPropertyDrawer in this version of Unity; support for legacy Unity PropertyDrawers and DecoratorDrawers have been disabled in Odin's inspector. Please report this on Odin's issue tracker.");
                unityPropertyDrawers = Enumerable.Empty<Type>();
                unityPropertyAttributeDrawers = Enumerable.Empty<Type>();
                unityDecoratorDrawers = Enumerable.Empty<Type>();
            }

            DrawerUtilities.AllDrawerTypes = typesToSearch
                    .Where(type => typeof(OdinDrawer).IsAssignableFrom(type))
                    .AppendWith(unityPropertyDrawers)
                    .AppendWith(unityPropertyAttributeDrawers)
                    .AppendWith(unityDecoratorDrawers)
                    .OrderByDescending(type => GetDrawerPriority(type))
                    .ToList();

            //DrawerUtilities.SearchIndex.IndexingRules.Add(new TypeMatchIndexingRule(
            //    "DEBUG",
            //    (ref TypeSearchInfo info, ref string error) =>
            //    {
            //        Debug.Log("Indexed drawer: " + info.MatchType.GetNiceFullName());
            //        return true;
            //    }));

            // Unity drawers have a peculiar method of generic target selection,
            // where you pass in the generic type definition that you wish to draw.
            //
            // We need to support this, for Unity's own legacy drawers.
            DrawerUtilities.SearchIndex.MatchRules.Add(new TypeSearch.TypeMatchRule(
                "Unity Drawer Generic Target Matcher",
                (info, targets) =>
                {
                    if (targets.Length != 1) return null;
                    if (!info.Targets[0].IsGenericTypeDefinition) return null;

                    var baseDef = info.MatchType.GetGenericTypeDefinition();

                    bool abstractUnityValueDrawer = baseDef == typeof(AbstractTypeUnityPropertyDrawer<,,>);
                    bool plainUnityValueDrawer = baseDef == typeof(UnityPropertyDrawer<,>);

                    if (!(abstractUnityValueDrawer || plainUnityValueDrawer)) return null;

                    if (abstractUnityValueDrawer)
                    {
                        if (targets[0].ImplementsOpenGenericType(info.Targets[0]))
                        {
                            var args = info.MatchType.GetGenericArguments();
                            return info.MatchType.GetGenericTypeDefinition().MakeGenericType(args[0], targets[0], targets[0]);
                        }
                    }
                    else
                    {
                        if (!targets[0].IsGenericType) return null;

                        if (targets[0].GetGenericTypeDefinition() == info.Targets[0])
                        {
                            var args = info.MatchType.GetGenericArguments();
                            return info.MatchType.GetGenericTypeDefinition().MakeGenericType(args[0], targets[0]);
                        }
                    }

                    return null;
                })
            );

            DrawerUtilities.SearchIndex.AddIndexedTypes(
                DrawerUtilities.AllDrawerTypes
                    .Select((type, i) =>
                    {
                        var info = new TypeSearchInfo()
                        {
                            MatchType = type,
                            Priority = DrawerUtilities.AllDrawerTypes.Count - i,
                            Targets = null
                        };

                        if (type.ImplementsOpenGenericClass(typeof(OdinValueDrawer<>)))
                        {
                            info.Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinValueDrawer<>));
                        }
                        else if (type.ImplementsOpenGenericClass(typeof(OdinAttributeDrawer<>)))
                        {
                            if (type.ImplementsOpenGenericClass(typeof(OdinAttributeDrawer<,>)))
                            {
                                info.Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinAttributeDrawer<,>));
                                InvalidAttributeTargetUtility.RegisterValidAttributeTarget(info.Targets[0], info.Targets[1]);
                            }
                            else
                            {
                                info.Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinAttributeDrawer<>));
                            }
                        }
                        else if (type.ImplementsOpenGenericClass(typeof(OdinGroupDrawer<>)))
                        {
                            info.Targets = type.GetArgumentsOfInheritedOpenGenericClass(typeof(OdinGroupDrawer<>));
                        }
                        else if (!type.IsFullyConstructedGenericType())
                        {
                            info.Targets = type.GetGenericArguments();
                        }

                        info.Targets = info.Targets ?? Type.EmptyTypes;
                        return info;
                    })
            );

            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetAttributes<StaticInitializeBeforeDrawingAttribute>()).SelectMany(a => a.Types ?? Type.EmptyTypes).Where(t => t != null))
            {
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            }

            //Profiler.EndSample();
        }

        public static void GetDefaultPropertyDrawers(InspectorProperty property, List<TypeSearchResult> resultList)
        {
            resultList.Clear();

            var queries = CachedQueryResultList;
            queries.Clear();

            // First we make and gather lots of small queries, which are essentially instant, as they are
            //  all cached by the search index.
            {
                // Empty query (for all drawers with no type constraints at all)
                queries.Add(SearchIndex.GetMatches(Type.EmptyTypes));

                // Value query
                if (property.ValueEntry != null)
                {
                    queries.Add(SearchIndex.GetMatches(property.ValueEntry.TypeOfValue));
                }

                // Attribute queries
                for (int i = 0; i < property.Attributes.Count; i++)
                {
                    var attr = property.Attributes[i].GetType();
                    queries.Add(SearchIndex.GetMatches(attr));

                    // Attribute and value query
                    if (property.ValueEntry != null)
                    {
                        queries.Add(SearchIndex.GetMatches(attr, property.ValueEntry.TypeOfValue));

                        if (InvalidAttributeTargetUtility.ShowInvalidAttributeErrorFor(property, attr))
                        {
                            queries.Add(GetInvalidAttributeTypeSearchResult(attr));
                        }
                    }
                }
            }

            var finalResults = TypeSearchIndex.GetCachedMergedQueryResults(queries);

            // Build up the final result list, filtering invalid drawer types away
            //  as we go.
            for (int i = 0; i < finalResults.Length; i++)
            {
                var result = finalResults[i];

                if (DrawerTypeCanDrawProperty(result.MatchedType, property))
                {
                    resultList.Add(finalResults[i]);
                }
            }
        }

        private static readonly Dictionary<Type, TypeSearchResult[]> InvalidAttributeTypeSearchResults = new Dictionary<Type, TypeSearchResult[]>(FastTypeComparer.Instance);

        private static TypeSearchResult[] GetInvalidAttributeTypeSearchResult(Type attr)
        {
            TypeSearchResult[] result;

            if (!InvalidAttributeTypeSearchResults.TryGetValue(attr, out result))
            {
                result = new TypeSearchResult[]
                {
                    new TypeSearchResult()
                    {
                        MatchedInfo = new TypeSearchInfo()
                        {
                            MatchType = typeof(InvalidAttributeNotificationDrawer<>),
                            Priority = double.MaxValue,
                            Targets = Type.EmptyTypes
                        },
                        MatchedRule = InvalidAttributeRule,
                        MatchedTargets = Type.EmptyTypes,
                        MatchedType = typeof(InvalidAttributeNotificationDrawer<>).MakeGenericType(attr)
                    }
                };

                InvalidAttributeTypeSearchResults.Add(attr, result);
            }

            return result;
        }

        public static IEnumerable<TypeSearchResult> GetDefaultPropertyDrawers(InspectorProperty property)
        {
            var queries = CachedQueryResultList;
            queries.Clear();

            // First we make and gather lots of small queries, which are essentially instant, as they are
            //  all cached by the search index.
            {
                // Empty query (for all drawers with no type constraints at all)
                queries.Add(SearchIndex.GetMatches(Type.EmptyTypes));

                // Value query
                if (property.ValueEntry != null)
                {
                    queries.Add(SearchIndex.GetMatches(property.ValueEntry.TypeOfValue));
                }

                // Attribute queries
                for (int i = 0; i < property.Attributes.Count; i++)
                {
                    var attr = property.Attributes[i].GetType();
                    queries.Add(SearchIndex.GetMatches(attr));

                    // Attribute and value query
                    if (property.ValueEntry != null)
                    {
                        queries.Add(SearchIndex.GetMatches(attr, property.ValueEntry.TypeOfValue));

                        if (InvalidAttributeTargetUtility.ShowInvalidAttributeErrorFor(property, attr))
                        {
                            queries.Add(new TypeSearchResult[]
                            {
                                new TypeSearchResult()
                                {
                                    MatchedInfo = new TypeSearchInfo()
                                    {
                                        MatchType = typeof(InvalidAttributeNotificationDrawer<>),
                                        Priority = double.MaxValue,
                                        Targets = Type.EmptyTypes
                                    },
                                    MatchedRule = InvalidAttributeRule,
                                    MatchedTargets = Type.EmptyTypes,
                                    MatchedType = typeof(InvalidAttributeNotificationDrawer<>).MakeGenericType(attr)
                                }
                            });
                        }
                    }
                }
            }

            var finalResults = TypeSearchIndex.GetCachedMergedQueryResults(queries);

            // Yield the final result array, filtering invalid drawer types away
            //  as we go.
            for (int i = 0; i < finalResults.Length; i++)
            {
                var result = finalResults[i];

                if (DrawerTypeCanDrawProperty(result.MatchedType, property))
                {
                    yield return finalResults[i];
                }
            }
        }

        /// <summary>
        /// Gets the priority of a given drawer type.
        /// </summary>
        public static DrawerPriority GetDrawerPriority(Type drawerType)
        {
            DrawerPriority result;

            if (!DrawerTypePriorityLookup.TryGetValue(drawerType, out result))
            {
                result = CalculateDrawerPriority(drawerType);
                DrawerTypePriorityLookup[drawerType] = result;
            }

            return result;
        }

        private static DrawerPriority CalculateDrawerPriority(Type drawerType)
        {
            DrawerPriority priority = DrawerPriority.AutoPriority;

            // Find a DrawerPriorityAttribute if there is one anywhere and use the priority from that
            {
                DrawerPriorityAttribute priorityAttribute = null;

                if (DrawerIsUnityAlias(drawerType))
                {
                    // Special case for Unity property alias drawers;
                    // We should check if their assigned Unity drawer type
                    // itself declares a DrawerPriorityAttribute.

                    priorityAttribute = drawerType.GetGenericArguments()[0].GetCustomAttribute<DrawerPriorityAttribute>();
                }

                if (priorityAttribute == null)
                {
                    priorityAttribute = drawerType.GetCustomAttribute<DrawerPriorityAttribute>();
                }

                if (priorityAttribute != null)
                {
                    priority = priorityAttribute.Priority;
                }
            }

            // Figure out the drawer's actual priority if it's auto priority
            if (priority == DrawerPriority.AutoPriority)
            {
                if (drawerType.ImplementsOpenGenericClass(typeof(OdinAttributeDrawer<>)))
                {
                    priority = DrawerPriority.AttributePriority;
                }
                else
                {
                    priority = DrawerPriority.ValuePriority;
                }

                // All Odin drawers are slightly lower priority, so
                // that user-defined default-priority drawers always
                // override default-priority Odin drawers.
                if (drawerType.Assembly == typeof(OdinEditor).Assembly)
                {
                    priority.Value -= 0.001;
                }
            }

            return priority;
        }

        private static bool DrawerIsUnityAlias(Type drawerType)
        {
            if (!drawerType.IsGenericType || drawerType.IsGenericTypeDefinition)
                return false;

            var definition = drawerType.GetGenericTypeDefinition();

            return definition == typeof(UnityPropertyDrawer<,>)
                || definition == typeof(UnityPropertyAttributeDrawer<,,>)
                || definition == typeof(UnityDecoratorAttributeDrawer<,,>)
                || definition == typeof(AbstractTypeUnityPropertyDrawer<,,>);
        }

        public static bool DrawerTypeCanDrawProperty(Type drawerType, InspectorProperty property)
        {
            var drawer = GetCachedUninitializedDrawer(drawerType);
            return drawer.CanDrawProperty(property);
        }

        public static OdinDrawer GetCachedUninitializedDrawer(Type drawerType)
        {
            OdinDrawer result;
            if (!UninitializedDrawers.TryGetValue(drawerType, out result))
            {
                result = (OdinDrawer)FormatterServices.GetUninitializedObject(drawerType);
                UninitializedDrawers[drawerType] = result;
            }
            return result;
        }

        public static bool HasAttributeDrawer(Type attributeType)
        {
            return AllDrawerTypes
                .Select(d => d.GetBaseClasses(false)
                    .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(OdinAttributeDrawer<>)))
                .Where(d => d != null)
                .Any(d => d.GetGenericArguments()[0] == attributeType);
        }

        public static class InvalidAttributeTargetUtility
        {
            private static readonly Dictionary<Type, List<Type>> ConcreteAttributeTargets = new Dictionary<Type, List<Type>>(FastTypeComparer.Instance);
            private static readonly Dictionary<Type, List<Type>> GenericParameterAttributeTargets = new Dictionary<Type, List<Type>>(FastTypeComparer.Instance);

            // Attribute, value, result
            private static readonly DoubleLookupDictionary<Type, Type, bool> ShowErrorCache = new DoubleLookupDictionary<Type, Type, bool>(FastTypeComparer.Instance, FastTypeComparer.Instance);

            private static readonly List<Type> EmptyList = new List<Type>();

            public static void RegisterValidAttributeTarget(Type attribute, Type target)
            {
                List<Type> list;

                if (attribute.IsGenericParameter)
                {
                    if (!GenericParameterAttributeTargets.TryGetValue(attribute, out list))
                    {
                        list = new List<Type>();
                        GenericParameterAttributeTargets[attribute] = list;
                    }
                }
                else
                {
                    if (!ConcreteAttributeTargets.TryGetValue(attribute, out list))
                    {
                        list = new List<Type>();
                        ConcreteAttributeTargets[attribute] = list;
                    }
                }

                list.Add(target);
            }

            public static List<Type> GetValidTargets(Type attribute)
            {
                List<Type> result;
                if (!ConcreteAttributeTargets.TryGetValue(attribute, out result) && !GenericParameterAttributeTargets.TryGetValue(attribute, out result))
                {
                    bool foundAnyValids = false;

                    foreach (var entry in GenericParameterAttributeTargets)
                    {
                        var param = entry.Key;
                        var targets = entry.Value;

                        if (param.GenericParameterIsFulfilledBy(attribute))
                        {
                            result = targets.ToList();
                            ConcreteAttributeTargets[attribute] = result;
                            foundAnyValids = true;
                        }
                    }

                    if (!foundAnyValids)
                    {
                        ConcreteAttributeTargets[attribute] = null;
                    }
                }
                return result ?? EmptyList;
            }

            public static bool ShowInvalidAttributeErrorFor(InspectorProperty property, Type attribute)
            {
                if (property.ValueEntry == null) return false;
                if (property.ValueEntry.BaseValueType == typeof(object)) return false;
                if (property.Parent != null && property.Parent.ChildResolver is ICollectionResolver) return false;
                if (property.GetAttribute<SuppressInvalidAttributeErrorAttribute>() != null) return false;
                if (property.Info.TypeOfValue.IsInterface) return false;

                var collectionResolver = property.ChildResolver as ICollectionResolver;
                if (collectionResolver != null)
                {
                    if (collectionResolver.ElementType == typeof(object)) return false;
                    if (collectionResolver.ElementType.IsInterface) return false;

                    return ShowInvalidAttributeErrorFor(attribute, property.ValueEntry.BaseValueType)
                        && ShowInvalidAttributeErrorFor(attribute, collectionResolver.ElementType);
                }

                return ShowInvalidAttributeErrorFor(attribute, property.ValueEntry.BaseValueType);
            }

            public static bool ShowInvalidAttributeErrorFor(Type attribute, Type value)
            {
                bool result;
                if (!ShowErrorCache.TryGetInnerValue(attribute, value, out result))
                {
                    result = CalculateShowInvalidAttributeErrorFor(attribute, value);
                    ShowErrorCache[attribute][value] = result;
                }
                return result;
            }

            private static bool CalculateShowInvalidAttributeErrorFor(Type attribute, Type value)
            {
                if (attribute == typeof(DelayedAttribute) || attribute == typeof(DelayedPropertyAttribute))
                {
                    return false;
                }

                var validTargets = GetValidTargets(attribute);

                if (validTargets.Count == 0) return false;
                if (value == typeof(object)) return false;

                for (int i = 0; i < validTargets.Count; i++)
                {
                    var valid = validTargets[i];

                    if (valid == value)
                    {
                        return false;
                    }
                    else if (valid.IsGenericParameter)
                    {
                        if (valid.GenericParameterIsFulfilledBy(value))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
#endif