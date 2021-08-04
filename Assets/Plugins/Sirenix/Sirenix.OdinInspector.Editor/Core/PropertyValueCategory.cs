#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyValueCategory.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyValueCategory.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    /// <summary>
    /// Enumeration describing the different kinds of value categories a property can be, IE, where does the property get its value from?
    /// <para/>
    /// NOTE: The PropertyValueCategory enum is obsolete since patch 1.1, as the notion of a value category is no longer robust in the new property system.
    /// </summary>
    [System.Obsolete("The PropertyValueCategory enum is obsolete, as the notion of a value category is no longer robust in the new property system.", true)]
    public enum PropertyValueCategory
    {
        /// <summary>
        /// Property represents a member.
        /// </summary>
        Member,

        /// <summary>
        /// Property represents an element in a strongly typed list (<see cref="System.Collections.Generic.IList{T}"/>).
        /// </summary>
        StrongListElement,

        /// <summary>
        /// Property represents an element in a weakly typed list (<see cref="System.Collections.IList"/>).
        /// </summary>
        WeakListElement,

        /// <summary>
        /// Property represents a key value pair in a dictionary (<see cref="System.Collections.Generic.IDictionary{TKey, TValue}"/>).
        /// </summary>
        DictionaryElement
    }
}
#endif