#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="InspectorPropertyDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using Utilities;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>
    /// Base class for all Odin drawers. In order to create your own custom drawers you need to derive from one of the following drawers:
    /// </para>
    /// <list type="bullet">
    /// <item><see cref="OdinAttributeDrawer{TAttribute}"/></item>
    /// <item><see cref="OdinAttributeDrawer{TAttribute, TValue}"/></item>
    /// <item><see cref="OdinValueDrawer{T}"/></item>
    /// <item><see cref="OdinGroupDrawer{TGroupAttribute}"/></item>
    /// </list>
    /// <para>Remember to provide your custom drawer with an <see cref="Sirenix.OdinInspector.Editor.OdinDrawerAttribute"/> in order for it to be located by the <see cref="DrawerLocator"/>.</para>
    /// <para>Drawers require a <see cref="PropertyTree"/> context, and are instantiated automatically by the <see cref="DrawerLocator"/>.</para>
    /// <para>Odin supports the use of GUILayout and takes care of undo for you. It also takes care of multi-selection in many simple cases. Checkout the manual for more information.</para>
    /// </summary>
    /// <seealso cref="OdinAttributeDrawer{TAttribute}"/>
    /// <seealso cref="OdinAttributeDrawer{TAttribute, TValue}"/>
    /// <seealso cref="OdinValueDrawer{T}"/>
    /// <seealso cref="OdinGroupDrawer{TGroupAttribute}"/>
    /// <seealso cref="InspectorProperty"/>
    /// <seealso cref="Sirenix.OdinInspector.Editor.OdinDrawerAttribute"/>
    /// <seealso cref="DrawerPriorityAttribute"/>
    /// <seealso cref="DrawerLocator"/>
    /// <seealso cref="InspectorUtilities"/>
    /// <seealso cref="PropertyTree"/>
    /// <seealso cref="Sirenix.Utilities.Editor.GUIHelper"/>
    /// <seealso cref="Sirenix.Utilities.Editor.SirenixEditorGUI"/>
    public abstract class OdinDrawer
    {
        private bool initializedGuiEnabledForReadOnly;
        private bool guiEnabledForReadOnly;
        private bool initialized;
        private InspectorProperty property;

        /// <summary>
        /// If <c>true</c> then this drawer will be skipped in the draw chain. Otherwise the drawer will be called as normal in the draw chain.
        /// </summary>
        public bool SkipWhenDrawing { get; set; }

        /// <summary>
        /// Gets a value indicating if the drawer has been initialized yet.
        /// </summary>
        public bool Initialized { get { return this.initialized; } }

        /// <summary>
        /// Gets the property this drawer draws for.
        /// </summary>
        public InspectorProperty Property { get { return this.property; } }

        /// <summary>
        /// If true, not-editable properties will not have its GUI being disabled as otherwise would be the case.
        /// This is useful if you want some GUI to be enabled regardless of whether a property is read-only or not.
        /// This value is true when an <see cref="AllowGUIEnabledForReadonlyAttribute"/> is defined on the drawer class itself.
        /// </summary>
        protected bool AutoSetGUIEnabled
        {
            get
            {
                if (!this.initializedGuiEnabledForReadOnly)
                {
                    // TODO: Tor, get rid of the AutoSetGUIEnabled and AllowGUIEnabledForReadonlyAttribute entirely!
                    // It really fuck things up with the EnableGUI attribute, since it just gets overriden with each draw property call.
                    // This should just be handled by each drawer. We think.
                    // We added this ugly hack.
                    // Signed, Bjarke & Mikkel.
                    if (this.property.Attributes.HasAttribute<EnableGUIAttribute>())
                    {
                        this.guiEnabledForReadOnly = true;
                    }
                    else
                    {
                        this.guiEnabledForReadOnly = this.GetType().IsDefined<AllowGUIEnabledForReadonlyAttribute>(inherit: true);
                    }

                    this.initializedGuiEnabledForReadOnly = guiEnabledForReadOnly;
                }

                return this.guiEnabledForReadOnly;
            }
        }

        /// <summary>
        /// Gets the OdinDrawerAttribute defined on the class. This returns null, if no <see cref="OdinInspector.Editor.OdinDrawerAttribute"/> is defined.
        /// </summary>
        [Obsolete("The OdinDrawerAttribute attribute is now obsolete, and serves no purpose any longer.", false)]
        public OdinDrawerAttribute OdinDrawerAttribute
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// <para>Override this method in order to define custom type constraints to specify whether or not a type should be drawn by the drawer.</para>
        /// <para>Note that Odin's <see cref="DrawerLocator" /> has full support for generic class constraints, so most often you can get away with not overriding CanDrawTypeFilter.</para>
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// Returns true by default, unless overridden.
        /// </returns>
        public virtual bool CanDrawTypeFilter(Type type)
        {
            return true;
        }

        /// <summary>
        /// Initializes the drawer instance.
        /// </summary>
        /// <param name="property"></param>
        public void Initialize(InspectorProperty property)
        {
            if (this.initialized) return;

            this.property = property;

            try
            {
                this.Initialize();
            }
            finally
            {
                this.initialized = true;
            }
        }

        /// <summary>
        /// Initializes the drawer instance. Override this to implement your own initialization logic.
        /// </summary>
        protected virtual void Initialize()
        {
        }

        /// <summary>
        /// Draws the property using the default label found in <see cref="InspectorProperty" />
        /// This method also disables the GUI if the property is read-only and <see cref="AutoSetGUIEnabled" /> is false.
        /// </summary>
        /// <param name="property">The property.</param>
#if SIRENIX_INTERNAL
        [Obsolete("Manually call the other overload to reduce call-stack madness.", true)]
#endif
        public void DrawProperty(InspectorProperty property)
        {
            this.DrawProperty(property.Label);
        }

        /// <summary>
        /// Draws the property with a custom label.
        /// This method also disables the GUI if the property is read-only and <see cref="AutoSetGUIEnabled" /> is false.
        /// </summary>
        /// <param name="label">The label. Null is allow if you wish no label should be drawn.</param>
        public void DrawProperty(GUIContent label)
        {
            if (!this.initialized) throw new InvalidOperationException("Cannot call DrawProperty on a drawer before it has been initialized!");

            //bool enabledForReadOnly = property.ValueEntry != null ? this.AutoSetGUIEnabled : true;

            //if (!enabledForReadOnly)
            //{
            //    GUIHelper.PushGUIEnabled(GUI.enabled && property.ValueEntry.IsEditable);
            //}

            this.DrawPropertyLayout(label);

            //if (!enabledForReadOnly)
            //{
            //    GUIHelper.PopGUIEnabled();
            //}
        }

        /// <summary>
        /// Draws the property with a custom label.
        /// This method also disables the GUI if the property is read-only and <see cref="AutoSetGUIEnabled" /> is false.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label. Null is allow if you wish no label should be drawn.</param>
        [Obsolete("Call the overload of DrawProperty that only takes a label instead. The property is now located in this.Property.", false)]
        public void DrawProperty(InspectorProperty property, GUIContent label)
        {
            if (!object.ReferenceEquals(property, this.property)) throw new ArgumentException("Invalid property passed as argument! A drawer can no longer draw any property but the property it has been assigned to.");
            if (!this.initialized) throw new InvalidOperationException("Cannot call DrawProperty on a drawer before it has been initialized!");

            //bool enabledForReadOnly = property.ValueEntry != null ? this.AutoSetGUIEnabled : true;

            //if (!enabledForReadOnly)
            //{
            //    GUIHelper.PushGUIEnabled(GUI.enabled && property.ValueEntry.IsEditable);
            //}

            this.DrawPropertyLayout(label);

            //if (!enabledForReadOnly)
            //{
            //    GUIHelper.PopGUIEnabled();
            //}
        }

        /// <summary>
        /// Draws the actual property. This method is called by this.DrawProperty(...)
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label. This can be null, so make sure your drawer supports that.</param>
        [Obsolete("Implement DrawPropertyLayout instead.", false)]
        protected virtual void DrawPropertyImplementation(InspectorProperty property, GUIContent label)
        {
            if (label != null)
            {
                EditorGUILayout.LabelField(label, new GUIContent("The DrawPropertyLayout method has not been implemented for the drawer of type '" + this.GetType().GetNiceName() + "'."));
            }
            else
            {
                EditorGUILayout.LabelField(new GUIContent("The DrawPropertyLayout method has not been implemented for the drawer of type '" + this.GetType().GetNiceName() + "'."));
            }
        }

        /// <summary>
        /// Draws the property with GUILayout support.
        /// </summary>
        /// <param name="label">The label. This can be null, so make sure your drawer supports that.</param>
        protected virtual void DrawPropertyLayout(GUIContent label)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            this.DrawPropertyImplementation(this.Property, label);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// <para>Calls the next drawer in the drawer chain.</para>
        /// <para>
        /// In Odin, multiple drawers are used to draw any given property. This method calls
        /// the next drawer in the drawer chain provided by the <see cref="DrawerLocator" />.
        /// The order of the drawer chain is defined using the <see cref="DrawerPriorityAttribute" />.
        /// </para>
        /// </summary>
        /// <param name="entry">The entry with the property to draw.</param>
        /// <param name="label">The label. Null is allow if you wish no label should be drawn.</param>
        /// <returns>Returns true, if a next drawer was called, otherwise a warning message is shown in the inspector and false is returned.</returns>
#if SIRENIX_INTERNAL

        [Obsolete("Manually call the other overload to reduce call-stack madness.", true)]
#else
        [Obsolete("Call the overload of CallNextDrawer that only takes a label instead. The property is now located in this.Property.", false)]
#endif
        protected bool CallNextDrawer(IPropertyValueEntry entry, GUIContent label)
        {
            if (!object.ReferenceEquals(entry.Property, this.property)) throw new ArgumentException("Invalid property passed as argument! A drawer can no longer draw any property but the property it has been assigned to.");
            return this.CallNextDrawer(label);
        }

        /// <summary>
        /// <para>Calls the next drawer in the drawer chain.</para>
        /// <para>
        /// Odin supports multiple drawers being used to draw any given property. This method calls
        /// the next drawer in the drawer chain provided by the <see cref="DrawerLocator" />.
        /// The order of the drawer chain is defined using the <see cref="DrawerPriorityAttribute" />.
        /// </para>
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="label">The label. Null is allowed if you wish no label to be drawn.</param>
        /// <returns>Returns true, if a next drawer was called, otherwise a warning message is shown in the inspector and false is returned.</returns>
#if SIRENIX_INTERNAL

        [Obsolete("Manually call the other overload to reduce call-stack madness.", true)]
#else
        [Obsolete("Call the overload of CallNextDrawer that only takes a label instead. The property is now located in this.Property.", false)]
#endif
        protected bool CallNextDrawer(InspectorProperty property, GUIContent label)
        {
            if (!object.ReferenceEquals(property, this.property)) throw new ArgumentException("Invalid property passed as argument! A drawer can no longer draw any property but the property it has been assigned to.");
            return this.CallNextDrawer(label);
        }

        /// <summary>
        /// Calls the next drawer in the draw chain.
        /// </summary>
        /// <param name="label">The label to pass on to the next drawer.</param>
        protected bool CallNextDrawer(GUIContent label)
        {
            //var nextDrawer = DrawerLocator.GetNextDrawer(this, property);

            OdinDrawer nextDrawer = null;

            var chain = this.property.GetActiveDrawerChain();

            if (chain.MoveNext())
            {
                nextDrawer = chain.Current;
            }

            if (nextDrawer != null)
            {
                //nextDrawer.DrawProperty(label);
                nextDrawer.DrawPropertyLayout(label);
                return true;
            }
            else if (property.ValueEntry != null)
            {
                var rect = EditorGUILayout.GetControlRect();
                if (label == null)
                {
                    GUI.Label(rect, this.Property.NiceName);
                }
                else
                {
                    GUI.Label(rect, label);
                }

                //GUILayout.BeginHorizontal();
                //{
                //    if (label != null)
                //    {
                //        EditorGUILayout.PrefixLabel(label);
                //    }
                //    SirenixEditorGUI.WarningMessageBox("There is no custom drawer defined for type '" + property.ValueEntry.TypeOfValue.GetNiceName() + "', and the type has no members to draw.");
                //}
                //GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                {
                    if (label != null)
                    {
                        EditorGUILayout.PrefixLabel(label);
                    }
                    SirenixEditorGUI.WarningMessageBox("There is no drawer defined for property " + property.NiceName + " of type " + property.Info.PropertyType + ".");
                }
                GUILayout.EndHorizontal();
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating if the drawer can draw for the specified property.
        /// Override this to implement a custom property filter for your drawer.
        /// </summary>
        /// <param name="property">The property to test.</param>
        /// <returns><c>true</c> if the drawer can draw for the property. Otherwise <c>false</c>.</returns>
        public virtual bool CanDrawProperty(InspectorProperty property)
        {
            return true;
        }
    }
}
#endif