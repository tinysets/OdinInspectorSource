//-----------------------------------------------------------------------// <copyright file="CustomContextMenuAttribute.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="CustomContextMenuAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
	/// <para>CustomContextMenu is used on any property, and adds a custom options to the context menu for the property.</para>
	/// <para>Use this for when you want to add custom actions to the context menu of a property.</para>
    /// </summary>
	/// <remarks>
	/// <note type="note">CustomContextMenu currently does not support static functions.</note>
	/// </remarks>
	/// <example>
	/// <para>The following example shows how CustomContextMenu is used to add a custom option to a property.</para>
    /// <code>
	///	public class MyComponent : MonoBehaviour
	///	{
	///		[CustomContextMenu("My custom option", "MyAction")]
	///		public Vector3 MyVector;
	///
	///		private void MyAction()
	///		{
	///			MyVector = Random.onUnitSphere;
	///		}
	///	}
	/// </code>
    /// </example>
	/// <seealso cref="DisableContextMenuAttribute"/>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = true)]
    public sealed class CustomContextMenuAttribute : Attribute
    {
        /// <summary>
        /// The name of the menu item.
        /// </summary>
        public string MenuItem;

        /// <summary>
        /// The name of the callback method.
        /// </summary>
        public string MethodName;

        /// <summary>
        /// Adds a custom option to the context menu of the property.
        /// </summary>
        /// <param name="menuItem">The name of the menu item.</param>
        /// <param name="methodName">The name of the callback method.</param>
        public CustomContextMenuAttribute(string menuItem, string methodName)
        {
            this.MenuItem = menuItem;
            this.MethodName = methodName;
        }
    }
}