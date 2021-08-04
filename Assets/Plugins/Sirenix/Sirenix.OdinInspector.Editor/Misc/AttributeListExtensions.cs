#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AttributeListExtensions.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AttributeListExtensions.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension method for List&lt;Attribute&gt;
    /// </summary>
    public static class AttributeListExtensions
    {
        /// <summary>
        /// Determines whether the list contains a specific attribute.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns>
        ///   <c>true</c> if the specified attribute list has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute<T>(this IList<Attribute> attributeList)
            where T : Attribute
        {
            return attributeList.OfType<T>().Any();
        }

        /// <summary>
        /// Adds the attribute if not exist.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns></returns>
        public static T GetOrAddAttribute<T>(this List<Attribute> attributeList)
            where T : Attribute, new()
        {
            var attr = attributeList.OfType<T>().FirstOrDefault();

            if (attr == null)
            {
                attr = new T();
                attributeList.Add(attr);
            }

            return attr;
        }

        /// <summary>
        /// Adds the attribute if not exist.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns></returns>
        public static T GetAttribute<T>(this IList<Attribute> attributeList)
            where T : Attribute
        {
            for (int i = 0; i < attributeList.Count; i++)
            {
                var attr = attributeList[i] as T;
                if (attr != null)
                {
                    return attr;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds the attribute if not exist.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns></returns>
        public static T Add<T>(this List<Attribute> attributeList)
            where T : Attribute, new()
        {
            var attr = new T();
            attributeList.Add(attr);
            return attr;
        }

        /// <summary>
        /// Adds the attribute if not exist.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <param name="attr">The attribute.</param>
        /// <returns></returns>
        public static bool GetOrAddAttribute<T>(this List<Attribute> attributeList, T attr)
            where T : Attribute
        {
            if (!attributeList.OfType<T>().Any())
            {
                attributeList.Add(attr);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes the type of the attribute of.
        /// </summary>
        /// <typeparam name="T">The type of attribute.</typeparam>
        /// <param name="attributeList">The attribute list.</param>
        /// <returns></returns>
        public static bool RemoveAttributeOfType<T>(this List<Attribute> attributeList)
            where T : Attribute
        {
            var toRemove = attributeList.OfType<T>().ToList();

            if (toRemove.Count == 0)
            {
                return false;
            }

            foreach (var item in toRemove)
            {
                attributeList.Remove(item);
            }

            return true;
        }
    }
}
#endif