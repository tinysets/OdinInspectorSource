#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyTree.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyTree.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using UnityEditor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// <para>Represents a set of values of the same type as a tree of properties that can be drawn in the inspector, and provides an array of utilities for querying the tree of properties.</para>
    /// <para>This class also handles management of prefab modifications.</para>
    /// </summary>
    public abstract class PropertyTree : IDisposable
    {
        /// <summary>
        /// Delegate for on property value changed callback.
        /// </summary>
        public delegate void OnPropertyValueChangedDelegate(InspectorProperty property, int selectionIndex);

        private MethodInfo onValidateMethod;
        private OdinAttributeProcessorLocator attributeProcessorLocator;
        private OdinPropertyResolverLocator propertyResolverLocator;
        private DrawerChainResolver drawerChainResolver;
        internal float ContextWidth;
        internal bool WillUndo;

        /// <summary>
        /// The <see cref="SerializedObject"/> that this tree represents, if the tree was created for a <see cref="SerializedObject"/>.
        /// </summary>
        public abstract SerializedObject UnitySerializedObject { get; }

        /// <summary>
        /// The current update ID of the tree. This is incremented once, each update, and is used by <see cref="InspectorProperty.Update(bool)"/> to avoid updating multiple times in the same update round.
        /// </summary>
        public abstract int UpdateID { get; }

        /// <summary>
        /// The type of the values that the property tree represents.
        /// </summary>
        public abstract Type TargetType { get; }

        /// <summary>
        /// The actual values that the property tree represents.
        /// </summary>
        public abstract ImmutableList<object> WeakTargets { get; }

        /// <summary>
        /// The number of root properties in the tree.
        /// </summary>
        public abstract int RootPropertyCount { get; }

        /// <summary>
        /// The prefab modification handler of the tree.
        /// </summary>
        public abstract PrefabModificationHandler PrefabModificationHandler { get; }

        /// <summary>
        /// Whether this property tree also represents members that are specially serialized by Odin.
        /// </summary>
        [Obsolete("This value is no longer guaranteed to be correct, as it may have different answers for different properties in the tree. Instead look at InspectorProperty.SerializationRoot to determine whether specially serialized members might be included.", true)]
        public abstract bool IncludesSpeciallySerializedMembers { get; }

        /// <summary>
        /// Gets a value indicating whether or not to draw the mono script object field at the top of the property tree.
        /// </summary>
        public bool DrawMonoScriptObjectField { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not the PropertyTree is inspecting a static type.
        /// </summary>
        public bool IsStatic { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="OdinAttributeProcessorLocator"/> for the PropertyTree.
        /// </summary>
        public OdinAttributeProcessorLocator AttributeProcessorLocator
        {
            get
            {
                if (this.attributeProcessorLocator == null)
                {
                    this.attributeProcessorLocator = DefaultOdinAttributeProcessorLocator.Instance;
                }

                return this.attributeProcessorLocator;
            }
            set
            {
                if (!object.ReferenceEquals(this.attributeProcessorLocator, value))
                {
                    this.attributeProcessorLocator = value;
                    this.SecretRootProperty.RefreshSetup();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="OdinPropertyResolverLocator"/> for the PropertyTree.
        /// </summary>
        public OdinPropertyResolverLocator PropertyResolverLocator
        {
            get
            {
                if (this.propertyResolverLocator == null)
                {
                    this.propertyResolverLocator = DefaultOdinPropertyResolverLocator.Instance;
                }

                return this.propertyResolverLocator;
            }
            set
            {
                if (!object.ReferenceEquals(this.propertyResolverLocator, value))
                {
                    this.propertyResolverLocator = value;
                    this.SecretRootProperty.RefreshSetup();
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Sirenix.OdinInspector.Editor.DrawerChainResolver"/> for the PropertyTree.
        /// </summary>
        public DrawerChainResolver DrawerChainResolver
        {
            get
            {
                if (this.drawerChainResolver == null)
                {
                    this.drawerChainResolver = DefaultDrawerChainResolver.Instance;
                }

                return this.drawerChainResolver;
            }
            set
            {
                if (!object.ReferenceEquals(this.drawerChainResolver, value))
                {
                    this.drawerChainResolver = value;
                    this.SecretRootProperty.RefreshSetup();
                }
            }
        }

        /// <summary>
        /// An event that is invoked whenever an undo or a redo is performed in the inspector.
        /// The advantage of using this event on a property tree instance instead of
        /// <see cref="Undo.undoRedoPerformed"/> is that this event will be desubscribed from
        /// <see cref="Undo.undoRedoPerformed"/> when the selection changes and the property
        /// tree is no longer being used, allowing the GC to collect the property tree.
        /// </summary>
        public event Action OnUndoRedoPerformed;

        /// <summary>
        /// This event is invoked whenever the value of any property in the entire property tree is changed through the property system.
        /// </summary>
        public event OnPropertyValueChangedDelegate OnPropertyValueChanged;

        /// <summary>
        /// Creates a new <see cref="PropertyTree" /> for all target values of a <see cref="SerializedObject" />.
        /// </summary>
        public PropertyTree()
        {
            if (typeof(UnityEngine.Object).IsAssignableFrom(this.TargetType))
            {
                this.onValidateMethod = this.TargetType.FindMember()
                                                       .IsNamed("OnValidate")
                                                       .IsMethod()
                                                       .IsInstance()
                                                       .HasNoParameters()
                                                       .GetMember<MethodInfo>();

                Undo.undoRedoPerformed += this.InvokeOnUndoRedoPerformed;
                Selection.selectionChanged += this.OnSelectionChanged;
            }
        }

        internal void InvokeOnPropertyValueChanged(InspectorProperty property, int selectionIndex)
        {
            if (this.OnPropertyValueChanged != null)
            {
                try
                {
                    this.OnPropertyValueChanged(property, selectionIndex);
                }
                catch (ExitGUIException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }
        }

        /// <summary>
        /// Gets the secret root property of the tree, which hosts the property resolver used to resolve the "actual" root properties of the tree.
        /// </summary>
        public abstract InspectorProperty SecretRootProperty { get; }

        internal abstract void NotifyPropertyCreated(InspectorProperty property);
        internal abstract void NotifyPropertyDisposed(InspectorProperty property);
        internal abstract void ClearPathCaches();

        /// <summary>
        /// Registers that a given property is dirty and needs its changes to be applied at the end of the current frame.
        /// </summary>
        public abstract void RegisterPropertyDirty(InspectorProperty property);

        /// <summary>
        /// Schedules a delegate to be invoked at the end of the current GUI frame.
        /// </summary>
        /// <param name="action">The action delegate to be delayed.</param>
        public abstract void DelayAction(Action action);

        /// <summary>
        /// Schedules a delegate to be invoked at the end of the next Repaint GUI frame.
        /// </summary>
        /// <param name="action">The action to be delayed.</param>
        public abstract void DelayActionUntilRepaint(Action action);

        /// <summary>
        /// Enumerates over the properties of the tree.
        /// </summary>
        /// <param name="includeChildren">Whether to include children of the root properties or not. If set to true, every property in the entire tree will be enumerated.</param>
        /// <returns></returns>
        public abstract IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren);

        /// <summary>
        /// Gets the property at the given path. Note that this is the path found in <see cref="InspectorProperty.Path" />, not the Unity path.
        /// </summary>
        /// <param name="path">The path of the property to get.</param>
        public abstract InspectorProperty GetPropertyAtPath(string path);

        /// <summary>
        /// Gets the property at the given path. Note that this is the path found in <see cref="InspectorProperty.Path" />, not the Unity path.
        /// </summary>
        /// <param name="path">The path of the property to get.</param>
        /// <param name="closestProperty"></param>
        public abstract InspectorProperty GetPropertyAtPath(string path, out InspectorProperty closestProperty);

        /// <summary>
        /// Gets the property at the given Unity path.
        /// </summary>
        /// <param name="path">The Unity path of the property to get.</param>
        public abstract InspectorProperty GetPropertyAtUnityPath(string path);

        /// <summary>
        /// Gets the property at the given Unity path.
        /// </summary>
        /// <param name="path">The Unity path of the property to get.</param>
        /// <param name="closestProperty"></param>
        public abstract InspectorProperty GetPropertyAtUnityPath(string path, out InspectorProperty closestProperty);

        /// <summary>
        /// Gets the property at the given deep reflection path.
        /// </summary>
        /// <param name="path">The deep reflection path of the property to get.</param>
        [Obsolete("Use GetPropertyAtPrefabModificationPath instead.", false)]
        public InspectorProperty GetPropertyAtDeepReflectionPath(string path)
        {
            return this.GetPropertyAtPrefabModificationPath(path);
        }

        /// <summary>
        /// Gets the property at the given Odin prefab modification path.
        /// </summary>
        /// <param name="path">The prefab modification path of the property to get.</param>
        public abstract InspectorProperty GetPropertyAtPrefabModificationPath(string path);

        /// <summary>
        /// Gets the property at the given Odin prefab modification path.
        /// </summary>
        /// <param name="path">The prefab modification path of the property to get.</param>
        /// <param name="closestProperty"></param>
        public abstract InspectorProperty GetPropertyAtPrefabModificationPath(string path, out InspectorProperty closestProperty);

        /// <summary>
        /// <para>Draw the property tree, and handles management of undo, as well as marking scenes and drawn assets dirty.</para>
        /// <para>
        /// This is a shorthand for calling
        /// <see cref="InspectorUtilities.BeginDrawPropertyTree(PropertyTree, bool)"/>,
        /// <see cref="InspectorUtilities.DrawPropertiesInTree(PropertyTree)"/> and .
        /// <see cref="InspectorUtilities.EndDrawPropertyTree(PropertyTree)"/>.
        /// </para>
        /// </summary>
        public void Draw(bool applyUndo = true)
        {
            InspectorUtilities.BeginDrawPropertyTree(this, applyUndo);
            InspectorUtilities.DrawPropertiesInTree(this);
            InspectorUtilities.EndDrawPropertyTree(this);
        }

        /// <summary>
        /// Gets a Unity property for the given Odin or Unity path. If there is no <see cref="SerializedObject" /> for this property tree, or no such property is found in the <see cref="SerializedObject" />, a property will be emitted using <see cref="UnityPropertyEmitter" />.
        /// </summary>
        /// <param name="path">The Odin or Unity path to the property to get.</param>
        public SerializedProperty GetUnityPropertyForPath(string path)
        {
            FieldInfo fieldInfo;
            return this.GetUnityPropertyForPath(path, out fieldInfo);
        }

        /// <summary>
        /// Gets a Unity property for the given Odin or Unity path. If there is no <see cref="SerializedObject" /> for this property tree, or no such property is found in the <see cref="SerializedObject" />, a property will be emitted using <see cref="UnityPropertyEmitter" />.
        /// </summary>
        /// <param name="path">The Odin or Unity path to the property to get.</param>
        /// <param name="backingField">The backing field of the Unity property.</param>
        public abstract SerializedProperty GetUnityPropertyForPath(string path, out FieldInfo backingField);

        /// <summary>
        /// Checks whether a given object instance is referenced anywhere in the tree, and if it is, gives the path of the first time the object reference was encountered as an out parameter.
        /// </summary>
        /// <param name="value">The reference value to check.</param>
        /// <param name="referencePath">The first found path of the object.</param>
        public abstract bool ObjectIsReferenced(object value, out string referencePath);

        /// <summary>
        /// Gets the number of references to a given object instance in this tree.
        /// </summary>
        public abstract int GetReferenceCount(object reference);

        /// <summary>
        /// Updates all properties in the entire tree, and validates the prefab state of the tree, if applicable.
        /// </summary>
        public abstract void UpdateTree();

        /// <summary>
        /// Replaces all occurrences of a value with another value, in the entire tree.
        /// </summary>
        /// <param name="from">The value to find all instances of.</param>
        /// <param name="to">The value to replace the found values with.</param>
        public abstract void ReplaceAllReferences(object from, object to);

        /// <summary>
        /// Gets the root tree property at a given index.
        /// </summary>
        /// <param name="index">The index of the property to get.</param>
        public abstract InspectorProperty GetRootProperty(int index);

        /// <summary>
        /// Invokes the actions that have been delayed using <see cref="DelayAction(Action)"/> and <see cref="DelayActionUntilRepaint(Action)"/>.
        /// </summary>
        public abstract void InvokeDelayedActions();

        /// <summary>
        /// Applies all changes made with properties to the inspected target tree values, and marks all changed Unity objects dirty.
        /// </summary>
        /// <returns>true if any values were changed, otherwise false</returns>
        public abstract bool ApplyChanges();

        /// <summary>
        /// Invokes the OnValidate method on the property tree's targets if they are derived from <see cref="UnityEngine.Object"/> and have the method defined.
        /// </summary>
        public void InvokeOnValidate()
        {
            if (this.onValidateMethod != null)
            {
                for (int i = 0; i < this.WeakTargets.Count; i++)
                {
                    try
                    {
                        this.onValidateMethod.Invoke(this.WeakTargets[i], null);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
            }
        }

        /// <summary>
        /// Registers an object reference to a given path; this is used to ensure that objects are always registered after having been encountered once.
        /// </summary>
        /// <param name="reference">The referenced object.</param>
        /// <param name="property">The property that contains the reference.</param>
        internal abstract void ForceRegisterObjectReference(object reference, InspectorProperty property);

        /// <summary>
        /// Creates a PropertyTree to inspect the static values of the given type.
        /// </summary>
        /// <param name="type">The type to inspect.</param>
        /// <returns>A PropertyTree instance for inspecting the type.</returns>
        public static PropertyTree CreateStatic(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return (PropertyTree)Activator.CreateInstance(typeof(PropertyTree<>).MakeGenericType(type));
        }

        /// <summary>
        /// Creates a new <see cref="PropertyTree" /> for a given target value.
        /// </summary>
        /// <param name="target">The target to create a tree for.</param>
        /// <exception cref="System.ArgumentNullException">target is null</exception>
        public static PropertyTree Create(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            return Create((IList)new object[] { target }, null);
        }

        /// <summary>
        /// <para>Creates a new <see cref="PropertyTree" /> for a set of given target values.</para>
        /// <para>Note that the targets all need to be of the same type.</para>
        /// </summary>
        /// <param name="targets">The targets to create a tree for.</param>
        /// <exception cref="System.ArgumentNullException">targets is null</exception>
        public static PropertyTree Create(params object[] targets)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }

            return Create((IList)targets);
        }

        /// <summary>
        /// Creates a new <see cref="PropertyTree" /> for all target values of a <see cref="SerializedObject" />.
        /// </summary>
        /// <param name="serializedObject">The serialized object to create a tree for.</param>
        /// <exception cref="System.ArgumentNullException">serializedObject is null</exception>
        public static PropertyTree Create(SerializedObject serializedObject)
        {
            if (serializedObject == null)
            {
                throw new ArgumentNullException("serializedObject");
            }

            return Create(serializedObject.targetObjects, serializedObject);
        }

        /// <summary>
        /// <para>Creates a new <see cref="PropertyTree"/> for a set of given target values.</para>
        /// <para>Note that the targets all need to be of the same type.</para>
        /// </summary>
        /// <param name="targets">The targets to create a tree for.</param>
        public static PropertyTree Create(IList targets)
        {
            return Create(targets, null);
        }

        /// <summary>
        /// <para>Creates a new <see cref="PropertyTree"/> for a set of given target values, represented by a given <see cref="SerializedObject"/>.</para>
        /// <para>Note that the targets all need to be of the same type.</para>
        /// </summary>
        /// <param name="targets">The targets to create a tree for.</param>
        /// <param name="serializedObject">The serialized object to create a tree for. Note that the target values of the given <see cref="SerializedObject"/> must be the same values given in the targets parameter.</param>
        public static PropertyTree Create(IList targets, SerializedObject serializedObject)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }

            if (targets.Count == 0)
            {
                throw new ArgumentException("There must be at least one target.");
            }

            if (serializedObject != null)
            {
                bool valid = true;
                var targetObjects = serializedObject.targetObjects;

                if (targets.Count != targetObjects.Length)
                {
                    valid = false;
                }
                else
                {
                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (!object.ReferenceEquals(targets[i], targetObjects[i]))
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    throw new ArgumentException("Given target array must be identical in length and content to the target objects array in the given serializedObject.");
                }
            }

            Type targetType = null;

            for (int i = 0; i < targets.Count; i++)
            {
                Type otherType;
                object target = targets[i];

                if (object.ReferenceEquals(target, null))
                {
                    throw new ArgumentException("Target at index " + i + " was null.");
                }

                if (i == 0)
                {
                    targetType = target.GetType();
                }
                else if (targetType != (otherType = target.GetType()))
                {
                    if (targetType.IsAssignableFrom(otherType))
                    {
                        continue;
                    }
                    else if (otherType.IsAssignableFrom(targetType))
                    {
                        targetType = otherType;
                        continue;
                    }

                    throw new ArgumentException("Expected targets of type " + targetType.Name + ", but got an incompatible target of type " + otherType.Name + " at index " + i + ".");
                }
            }

            Type treeType = typeof(PropertyTree<>).MakeGenericType(targetType);
            Array targetArray;

            if (targets.GetType().IsArray && targets.GetType().GetElementType() == targetType)
            {
                targetArray = (Array)targets;
            }
            else
            {
                targetArray = Array.CreateInstance(targetType, targets.Count);
                targets.CopyTo(targetArray, 0);
            }

            if (serializedObject == null && targetType.IsAssignableFrom(typeof(UnityEngine.Object)))
            {
                UnityEngine.Object[] objs = new UnityEngine.Object[targets.Count];
                targets.CopyTo(objs, 0);

                serializedObject = new SerializedObject(objs);
            }

            return (PropertyTree)Activator.CreateInstance(treeType, targetArray, serializedObject);
        }

        private void InvokeOnUndoRedoPerformed()
        {
            if (this.OnUndoRedoPerformed != null)
            {
                this.OnUndoRedoPerformed();
            }
        }

        private void OnSelectionChanged()
        {
            Undo.undoRedoPerformed -= this.InvokeOnUndoRedoPerformed;
            Selection.selectionChanged -= this.OnSelectionChanged;
        }

        #region IDisposable Support
        private volatile bool disposedValue = false;

        protected virtual void Dispose(bool finalizer)
        {
            if (!this.disposedValue)
            {
                if (finalizer)
                {
                    // The tree is being garbage collected, but has not yet been disposed.
                    // 
                    // This will "resurrect" the property tree once and put it back on the reachable heap
                    // while it is waiting to be disposed, through the subscribed action, which will be
                    // cleared after it runs. The second time the property tree is collected by the GC,
                    // it will have been disposed properly already, and will not be resurrected again.
                    UnityEditorEventUtility.DelayActionThreadSafe(this.ActuallyDispose);
                }
                else
                {
                    this.ActuallyDispose();
                }

            }
        }

        ~PropertyTree()
        {
            Dispose(finalizer: true);
        }

        public void Dispose()
        {
            Dispose(finalizer: false);
        }

        private void ActuallyDispose()
        {
            foreach (var child in this.SecretRootProperty.Children.GetExistingChildren())
            {
                child.Dispose();
            }

            if (this.drawerChainResolver is IDisposable)
            {
                (this.drawerChainResolver as IDisposable).Dispose();
            }

            if (this.attributeProcessorLocator is IDisposable)
            {
                (this.attributeProcessorLocator as IDisposable).Dispose();
            }

            if (this.propertyResolverLocator is IDisposable)
            {
                (this.propertyResolverLocator as IDisposable).Dispose();
            }

            this.OnUndoRedoPerformed = null;
            this.OnPropertyValueChanged = null;
            this.disposedValue = true;
        }
        #endregion

    }

    /// <summary>
    /// <para>Represents a set of strongly typed values as a tree of properties that can be drawn in the inspector, and provides an array of utilities for querying the tree of properties.</para>
    /// <para>This class also handles management of prefab modifications.</para>
    /// </summary>
    public sealed class PropertyTree<T> : PropertyTree
    {
        private struct PropertyPathResult
        {
            public InspectorProperty Property;
            public InspectorProperty ClosestProperty;
        }

        private static readonly bool TargetIsValueType = typeof(T).IsValueType;
        private static readonly bool TargetIsUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(typeof(T));

        private Dictionary<object, int> objectReferenceCounts = new Dictionary<object, int>(ReferenceEqualityComparer<object>.Default);
        private Dictionary<object, string> objectReferences = new Dictionary<object, string>(ReferenceEqualityComparer<object>.Default);

        private Dictionary<string, PropertyPathResult> propertiesPathCache = new Dictionary<string, PropertyPathResult>();
        private Dictionary<string, PropertyPathResult> propertiesUnityPathCache = new Dictionary<string, PropertyPathResult>();
        private Dictionary<string, PropertyPathResult> propertiesPrefabModificationPathCache = new Dictionary<string, PropertyPathResult>();

        private Dictionary<string, Dictionary<Type, SerializedProperty>> emittedUnityPropertyCache = new Dictionary<string, Dictionary<Type, SerializedProperty>>();
        //private Dictionary<string, Dictionary<Type, UnityPropertyEmitter.Handle>> emittedUnityGameObjectPropertyCache = new Dictionary<string, Dictionary<Type, UnityPropertyEmitter.Handle>>();

        private List<Action> delayedActions = new List<Action>();
        private List<Action> delayedRepaintActions = new List<Action>();

        private List<InspectorProperty> dirtyProperties = new List<InspectorProperty>();

        private T[] targets;

        private InspectorProperty secretRootProperty;

        private int updateID = 1;
        private SerializedObject serializedObject;
        private object[] weakTargets;
        private ImmutableList<T> immutableTargets;
        private ImmutableList<object> immutableWeakTargets;
        private bool includesSpeciallySerializedMembers;
        private PrefabModificationHandler prefabModificationHandler;

        /// <summary>
        /// Gets the secret root property of the PropertyTree.
        /// </summary>
        public override InspectorProperty SecretRootProperty { get { return this.secretRootProperty; } }

        /// <summary>
        /// Gets the <see cref="prefabModificationHandler"/> for the PropertyTree.
        /// </summary>
        public override PrefabModificationHandler PrefabModificationHandler { get { return this.prefabModificationHandler; } }

        /// <summary>
        /// The current update ID of the tree. This is incremented once, each update, and is used by <see cref="InspectorProperty.Update(bool)" /> to avoid updating multiple times in the same update round.
        /// </summary>
        public override int UpdateID { get { return this.updateID; } }

        /// <summary>
        /// The <see cref="SerializedObject" /> that this tree represents, if the tree was created for a <see cref="SerializedObject" />.
        /// </summary>
        public override SerializedObject UnitySerializedObject { get { return this.serializedObject; } }

        /// <summary>
        /// The type of the values that the property tree represents.
        /// </summary>
        public override Type TargetType { get { return typeof(T); } }

        /// <summary>
        /// The strongly types actual values that the property tree represents.
        /// </summary>
        public ImmutableList<T> Targets
        {
            get
            {
                if (this.immutableTargets == null)
                {
                    this.immutableTargets = new ImmutableList<T>(this.targets);
                }

                return this.immutableTargets;
            }
        }

        /// <summary>
        /// The weakly types actual values that the property tree represents.
        /// </summary>
        public override ImmutableList<object> WeakTargets
        {
            get
            {
                if (this.immutableWeakTargets == null)
                {
                    if (this.weakTargets == null)
                    {
                        this.weakTargets = new object[this.targets.Length];
                        this.targets.CopyTo(this.weakTargets, 0);
                    }

                    this.immutableWeakTargets = new ImmutableList<object>(this.weakTargets);
                }
                else if (TargetIsValueType)
                {
                    this.targets.CopyTo(this.weakTargets, 0);
                }

                return this.immutableWeakTargets;
            }
        }

        /// <summary>
        /// The number of root properties in the tree.
        /// </summary>
        public override int RootPropertyCount { get { return this.secretRootProperty.Children.Count; } }

        /// <summary>
        /// Whether this property tree also represents members that are specially serialized by Odin.
        /// </summary>
        [Obsolete("This value is no longer guaranteed to be correct, as it may have different answers for different properties in the tree. Instead look at InspectorProperty.SerializationRoot to determine whether specially serialized members might be included.", true)]
        public override bool IncludesSpeciallySerializedMembers { get { throw new NotSupportedException(); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTree{T}"/> class, inspecting only the target (<see cref="T"/>) type's static members.
        /// </summary>
        public PropertyTree()
        {
            this.IsStatic = true;
            this.targets = new T[1];

            var rootPropertyInfo = InspectorPropertyInfo.CreateValue(
                name: "ROOT",
                order: 0,
                serializationBackend: this.includesSpeciallySerializedMembers ? SerializationBackend.Odin : SerializationBackend.Unity,
                getterSetter: new GetterSetter<int, T>(
                    getter: (ref int index) => this.targets[index],
                    setter: null),
                attributes: null);

            this.prefabModificationHandler = new PrefabModificationHandler(this);

            this.secretRootProperty = InspectorProperty.Create(this, null, rootPropertyInfo, 0, true);
            this.secretRootProperty.Update(true);

            this.prefabModificationHandler.Update();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTree{T}"/> class.
        /// </summary>
        /// <param name="serializedObject">The serialized object to represent.</param>
        public PropertyTree(SerializedObject serializedObject)
            : this(serializedObject.targetObjects.Cast<T>().ToArray(), serializedObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTree{T}"/> class.
        /// </summary>
        /// <param name="targets">The targets to represent.</param>
        public PropertyTree(T[] targets)
            : this(targets, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyTree{T}"/> class.
        /// </summary>
        /// <param name="targets">The targets to represent.</param>
        /// <param name="serializedObject">The serialized object to represent. Note that the target values of the given <see cref="SerializedObject"/> must be the same values given in the targets parameter.</param>
        /// <exception cref="System.ArgumentNullException">targets is null</exception>
        /// <exception cref="System.ArgumentException">
        /// There must be at least one target.
        /// or
        /// A given target is a null value.
        /// </exception>
        public PropertyTree(T[] targets, SerializedObject serializedObject)
        {
            if (targets == null)
            {
                throw new ArgumentNullException("targets");
            }

            if (targets.Length == 0)
            {
                throw new ArgumentException("There must be at least one target.");
            }

            for (int i = 0; i < targets.Length; i++)
            {
                if (object.ReferenceEquals(targets[i], null))
                {
                    throw new ArgumentException("A target at index '" + i + "' is a null value.");
                }
            }

            this.includesSpeciallySerializedMembers = typeof(T).IsDefined<ShowOdinSerializedPropertiesInInspectorAttribute>(inherit: true);
            this.serializedObject = serializedObject;
            this.targets = targets;

            var rootPropertyInfo = InspectorPropertyInfo.CreateValue(
                name: "ROOT",
                order: 0,
                serializationBackend: this.includesSpeciallySerializedMembers ? SerializationBackend.Odin : SerializationBackend.Unity,
                getterSetter: new GetterSetter<int, T>(
                    getter: (ref int index) => this.targets[index],
                    setter: null),
                attributes: null);

            this.prefabModificationHandler = new PrefabModificationHandler(this);

            this.secretRootProperty = InspectorProperty.Create(this, null, rootPropertyInfo, 0, true);
            this.secretRootProperty.Update(true);

            this.prefabModificationHandler.Update();
        }

        /// <summary>
        /// Applies all changes made with properties to the inspected target tree values.
        /// </summary>
        /// <returns>
        /// true if any values were changed, otherwise false
        /// </returns>
        public override bool ApplyChanges()
        {
            bool changed = false;

            // Apply changes for dirty properties
            {
                for (int i = 0; i < this.dirtyProperties.Count; i++)
                {
                    var property = this.dirtyProperties[i];

                    IApplyableResolver resolver = property.ChildResolver as IApplyableResolver;

                    if (resolver != null && resolver.ApplyChanges())
                    {
                        changed = true;

                        if (property.BaseValueEntry != null)
                        {
                            for (int j = 0; j < property.BaseValueEntry.ValueCount; j++)
                            {
                                property.BaseValueEntry.TriggerOnValueChanged(j);
                            }

                            if (property.BaseValueEntry.ValueChangedFromPrefab)
                            {
                                for (int k = 0; k < this.Targets.Count; k++)
                                {
                                    this.PrefabModificationHandler.RegisterPrefabValueModification(property, k);
                                }
                            }
                        }
                    }

                    if (property.ValueEntry != null)
                    {
                        if (property.ValueEntry.ApplyChanges())
                        {
                            changed = true;
                        }
                    }

                    if (changed)
                    {
                        var serializationRoot = property.SerializationRoot;

                        for (int j = 0; j < serializationRoot.ValueEntry.ValueCount; j++)
                        {
                            UnityEngine.Object unityObj = serializationRoot.ValueEntry.WeakValues[j] as UnityEngine.Object;

                            if (unityObj != null)
                            {
                                InspectorUtilities.RegisterUnityObjectDirty(unityObj);
                            }
                        }
                    }
                }

                this.dirtyProperties.Clear();
            }

            if (changed && this.PrefabModificationHandler != null && this.PrefabModificationHandler.HasPrefabs && this.UnitySerializedObject != null)
            {
                this.DelayActionUntilRepaint(() =>
                {
                    // We make ABSOLUTELY SURE that this code runs at the *very end* of Repaint, after *all* other delayed Repaint invokes.

                    this.DelayActionUntilRepaint(() =>
                    {
                        for (int i = 0; i < this.WeakTargets.Count; i++)
                        {
                            // Before we ever call PrefabUtility.RecordPrefabInstancePropertyModifications, we MUST
                            // make sure that prefab modifications are registered and applied on the object.
                            //
                            // If we don't, there is a chance that Unity will crash, for unknown reasons.

                            var receiver = this.WeakTargets[i] as ISerializationCallbackReceiver;

                            if (receiver != null)
                            {
                                receiver.OnBeforeSerialize();
                            }

                            PrefabUtility.RecordPrefabInstancePropertyModifications((UnityEngine.Object)this.WeakTargets[i]);
                        }
                    });
                });
            }

            return changed;
        }

        /// <summary>
        /// Registers that a given property is dirty and needs its changes to be applied at the end of the current frame.
        /// </summary>
        /// <param name="property"></param>
        /// <exception cref="NotImplementedException"></exception>
        public override void RegisterPropertyDirty(InspectorProperty property)
        {
            this.dirtyProperties.Add(property);
        }

        /// <summary>
        /// Updates all properties in the entire tree, and validates the prefab state of the tree, if applicable.
        /// </summary>
        public override void UpdateTree()
        {
            // Changes might have been set since the last frame, during event calls that occurred outside of IMGUI.
            // Those changes should be applied before we do anything else, so they don't get lost.
            this.ApplyChanges();

            unchecked
            {
                this.updateID++;
            }

            this.objectReferences.Clear();
            this.objectReferenceCounts.Clear();

            if (TargetIsUnityObject)
            {
                this.prefabModificationHandler.Update();
            }

            this.secretRootProperty.Update();
        }

        internal override void NotifyPropertyCreated(InspectorProperty property)
        {
            var result = new PropertyPathResult { Property = property, ClosestProperty = property };

            this.propertiesPathCache[property.Path] = result;
            this.propertiesUnityPathCache[property.UnityPropertyPath] = result;
            this.propertiesPrefabModificationPathCache[property.PrefabModificationPath] = result;
        }

        internal override void NotifyPropertyDisposed(InspectorProperty property)
        {
            this.ClearPathCaches();
        }

        internal override void ClearPathCaches()
        {
            this.propertiesPathCache.Clear();
            this.propertiesUnityPathCache.Clear();
            this.propertiesPrefabModificationPathCache.Clear();
        }

        /// <summary>
        /// Checks whether a given object instance is referenced anywhere in the tree, and if it is, gives the path of the first time the object reference was encountered as an out parameter.
        /// </summary>
        /// <param name="value">The reference value to check.</param>
        /// <param name="referencePath">The first found path of the object.</param>
        public override bool ObjectIsReferenced(object value, out string referencePath)
        {
            return this.objectReferences.TryGetValue(value, out referencePath);
        }

        /// <summary>
        /// Gets the number of references to a given object instance in this tree.
        /// </summary>
        /// <param name="reference"></param>
        public override int GetReferenceCount(object reference)
        {
            int count;
            this.objectReferenceCounts.TryGetValue(reference, out count);
            return count;
        }

        /// <summary>
        /// Gets the property at the given path. Note that this is the path found in <see cref="InspectorProperty.Path" />, not the Unity path.
        /// </summary>
        /// <param name="path">The path of the property to get.</param>
        public override InspectorProperty GetPropertyAtPath(string path)
        {
            InspectorProperty closest;
            return this.GetPropertyAtPath(path, out closest);
        }

        /// <summary>
        /// Gets the property at the given path. Note that this is the path found in <see cref="InspectorProperty.Path" />, not the Unity path.
        /// </summary>
        /// <param name="path">The path of the property to get.</param>
        /// <param name="closestProperty"></param>
        public override InspectorProperty GetPropertyAtPath(string path, out InspectorProperty closestProperty)
        {
            closestProperty = null;
            PropertyPathResult result;
            if (!this.propertiesPathCache.TryGetValue(path, out result))
            {
                using (var sbCache = Cache<StringBuilder>.Claim())
                {
                    var sb = sbCache.Value;
                    sb.Length = 0;

                    var steps = path.Split('.');
                    var current = this.secretRootProperty;

                    for (int i = 0; i < steps.Length; i++)
                    {
                        var step = steps[i];

                        if (i != 0) sb.Append('.');
                        sb.Append(step);

                        result.ClosestProperty = current;
                        current = current.Children[step];

                        var currentPath = sb.ToString();
                        if (!this.propertiesPathCache.ContainsKey(currentPath))
                            this.propertiesPathCache[currentPath] = new PropertyPathResult() { Property = current, ClosestProperty = current != null ? current : result.ClosestProperty };

                        if (current == null) break;
                    }

                    result.Property = current;
                    this.propertiesPathCache[path] = result;
                }
            }

            closestProperty = result.ClosestProperty;

            if (closestProperty != null) closestProperty.Update();
            if (result.Property != null) result.Property.Update();

            return result.Property;
        }

        /// <summary>
        /// Finds the property at the specified unity path.
        /// </summary>
        /// <param name="path">The unity path for the property.</param>
        /// <returns>The property found at the path.</returns>
        public override InspectorProperty GetPropertyAtUnityPath(string path)
        {
            InspectorProperty closest;
            return this.GetPropertyAtUnityPath(path, out closest);
        }

        /// <summary>
        /// Finds the property at the specified unity path.
        /// </summary>
        /// <param name="path">The unity path for the property.</param>
        /// <param name="closestProperty"></param>
        /// <returns>The property found at the path.</returns>
        public override InspectorProperty GetPropertyAtUnityPath(string path, out InspectorProperty closestProperty)
        {
            closestProperty = null;
            PropertyPathResult result;
            if (!this.propertiesUnityPathCache.TryGetValue(path, out result))
            {
                var steps = path.Split('.');
                var current = this.secretRootProperty;

                using (var sbCache = Cache<StringBuilder>.Claim())
                {
                    var sb = sbCache.Value;
                    sb.Length = 0;

                    for (int i = 0; i < steps.Length; i++)
                    {
                        var step = steps[i];
                        var next = current.Children[step];

                        if (i != 0)
                            sb.Append('.');

                        sb.Append(step);

                        // Copy with Unity's annoying array syntax
                        if (next == null && i + 1 < steps.Length && step == "Array" && steps[i + 1].StartsWith("data["))
                        {
                            var indexStr = steps[i + 1];
                            indexStr = indexStr.Substring(5, indexStr.Length - 6);

                            int index;
                            if (!int.TryParse(indexStr, out index)) continue;

                            // The standard for prefab collection supporting collections is "$index" for naming elements.
                            step = CollectionResolverUtilities.DefaultIndexToChildName(index);
                            i++; // Consume an extra "step"

                            sb.Append('.');
                            sb.Append(steps[i]);
                            next = current.Children[step];
                        }
                        else if (next == null && !(current.ChildResolver is ICollectionResolver))
                        {
                            // If the above lookup failed, perhaps due to the concrete member being hidden in a group somewhere,
                            // recursively look through all groups in this property for concrete members with a matching name.

                            next = this.TryFindChildMemberPropertyWithNameFromGroups(step, current);
                        }

                        var currentPath = sb.ToString();

                        if (!this.propertiesUnityPathCache.ContainsKey(currentPath))
                            this.propertiesUnityPathCache[currentPath] = new PropertyPathResult() { Property = next, ClosestProperty = next != null ? next : result.ClosestProperty };

                        current = next;
                        if (next == null) break;
                        result.ClosestProperty = current;
                    }
                }

                result.Property = current;
                this.propertiesUnityPathCache[path] = result;
            }
            
            closestProperty = result.ClosestProperty;

            if (closestProperty != null) closestProperty.Update();
            if (result.Property != null) result.Property.Update();

            return result.Property;
        }

        /// <summary>
        /// Finds the property at the specified modification path.
        /// </summary>
        /// <param name="path">The prefab modification path for the property.</param>
        /// <returns>The property found at the path.</returns>
        public override InspectorProperty GetPropertyAtPrefabModificationPath(string path)
        {
            InspectorProperty closest;
            return this.GetPropertyAtPrefabModificationPath(path, out closest);
        }

        /// <summary>
        /// Finds the property at the specified modification path.
        /// </summary>
        /// <param name="path">The prefab modification path for the property.</param>
        /// <param name="closestProperty"></param>
        /// <returns>The property found at the path.</returns>
        public override InspectorProperty GetPropertyAtPrefabModificationPath(string path, out InspectorProperty closestProperty)
        {
            closestProperty = null;
            PropertyPathResult result;
            if (!this.propertiesPrefabModificationPathCache.TryGetValue(path, out result))
            {
                using (var sbCache = Cache<StringBuilder>.Claim())
                {
                    var sb = sbCache.Value;
                    sb.Length = 0;

                    var steps = path.Split('.');
                    var current = this.secretRootProperty;

                    for (int i = 0; i < steps.Length; i++)
                    {
                        var step = steps[i];

                        if (i != 0) sb.Append('.');
                        sb.Append(step);

                        var next = current.Children[step];

                        if (next == null && !(current.ChildResolver is ICollectionResolver))
                        {
                            // If the above lookup failed, perhaps due to the concrete member being hidden in a group somewhere,
                            // recursively look through all groups in this property for concrete members with a matching name.

                            next = this.TryFindChildMemberPropertyWithNameFromGroups(step, current);
                        }

                        var currentPath = sb.ToString();
                        if (!this.propertiesPrefabModificationPathCache.ContainsKey(currentPath))
                            this.propertiesPrefabModificationPathCache[currentPath] = new PropertyPathResult() { Property = next, ClosestProperty = next != null ? next : result.ClosestProperty };

                        current = next;
                        if (next == null) break;
                        result.ClosestProperty = current;
                    }

                    result.Property = current;
                    this.propertiesPrefabModificationPathCache[path] = result;
                }
            }

            closestProperty = result.ClosestProperty;

            if (closestProperty != null) closestProperty.Update();
            if (result.Property != null) result.Property.Update();

            return result.Property;
        }

        private InspectorProperty TryFindChildMemberPropertyWithNameFromGroups(string name, InspectorProperty property)
        {
            // Optimization - this shouldn't ever happen for collections, and will just be a performance hog
            if (property.ChildResolver is ICollectionResolver) return null;

            for (int i = 0; i < property.Children.Count; i++)
            {
                var child = property.Children[i];

                switch (child.Info.PropertyType)
                {
                    case PropertyType.Value:
                        {
                            if (child.Info.HasSingleBackingMember && child.Name == name) return child;
                        }
                        break;

                    case PropertyType.Method:
                        continue;

                    case PropertyType.Group:
                        {
                            var found = this.TryFindChildMemberPropertyWithNameFromGroups(name, child);
                            if (found != null) return found;
                        }
                        break;

                    default:
                        throw new NotImplementedException(child.Info.PropertyType.ToString());
                }
            }

            return null;
        }

        /// <summary>
        /// Gets a Unity property for the given Odin or Unity path. If there is no <see cref="SerializedObject" /> for this property tree, or no such property is found in the <see cref="SerializedObject" />, a property will be emitted using <see cref="UnityPropertyEmitter" />.
        /// </summary>
        /// <param name="path">The Odin or Unity path to the property to get.</param>
        /// <param name="backingField">The backing field of the Unity property.</param>
        public override SerializedProperty GetUnityPropertyForPath(string path, out FieldInfo backingField)
        {
            backingField = null;
            string unityPath;
            InspectorProperty prop = this.GetPropertyAtPath(path);

            if (prop == null)
            {
                unityPath = InspectorUtilities.ConvertToUnityPropertyPath(path);
            }
            else
            {
                unityPath = prop.UnityPropertyPath;
            }

            SerializedProperty result = null;

            if (this.serializedObject != null)
            {
                result = this.serializedObject.FindProperty(unityPath);

                if (prop != null)
                {
                    // There is both a Unity property and a Sirenix property and the backing member is a field
                    // We can assign the FieldInfo now, as it's stored in the Sirenix property (or its parent, in the case of a collection).
                    backingField = prop.Info.GetMemberInfo() as FieldInfo;

                    if (backingField == null && prop.Parent != null && prop.Parent.ChildResolver is ICollectionResolver)
                    {
                        backingField = prop.Parent.Info.GetMemberInfo() as FieldInfo;
                    }
                }
            }

            if (result == null && prop != null && prop.Info.PropertyType == PropertyType.Value)
            {
                Dictionary<Type, SerializedProperty> innerDict;

                if (!this.emittedUnityPropertyCache.TryGetValue(path, out innerDict))
                {
                    innerDict = new Dictionary<Type, SerializedProperty>(FastTypeComparer.Instance);
                    this.emittedUnityPropertyCache.Add(path, innerDict);
                }

                if (!innerDict.TryGetValue(prop.ValueEntry.TypeOfValue, out result))
                {
                    result = UnityPropertyEmitter.CreateEmittedScriptableObjectProperty(prop.Info.PropertyName, prop.ValueEntry.TypeOfValue, this.targets.Length);
                    innerDict.Add(prop.ValueEntry.TypeOfValue, result);
                }
                // TargetObject is sometimes destroyed or the serialized object is disposed, often when the profiler is toggled. Not sure why, but we need to handle the case.
                else if (result != null && result.serializedObject.targetObject == null)
                {
                    result = UnityPropertyEmitter.CreateEmittedScriptableObjectProperty(prop.Info.PropertyName, prop.ValueEntry.TypeOfValue, this.targets.Length);
                    innerDict[prop.ValueEntry.TypeOfValue] = result;
                }
                //else if (result == null)
                //{
                //    Dictionary<Type, UnityPropertyEmitter.Handle> innerGoDict;
                //    GameObject go = null;

                //    if (!this.emittedUnityGameObjectPropertyCache.TryGetValue(path, out innerGoDict))
                //    {
                //        innerGoDict = new Dictionary<Type, UnityPropertyEmitter.Handle>(FastTypeComparer.Instance);
                //        this.emittedUnityGameObjectPropertyCache.Add(path, innerGoDict);
                //    }

                //    UnityPropertyEmitter.Handle handle;

                //    if (!innerGoDict.TryGetValue(prop.ValueEntry.TypeOfValue, out handle))
                //    {
                //        handle = UnityPropertyEmitter.CreateEmittedMonoBehaviourProperty(prop.Info.PropertyName, prop.ValueEntry.TypeOfValue, this.targets.Length, ref go);
                //        innerGoDict.Add(prop.ValueEntry.TypeOfValue, handle);
                //    }
                //    else if (handle != null && handle.UnityProperty.serializedObject.targetObject == null)
                //    {
                //        handle.Dispose();
                //        handle = UnityPropertyEmitter.CreateEmittedMonoBehaviourProperty(prop.Info.PropertyName, prop.ValueEntry.TypeOfValue, this.targets.Length, ref go);
                //        innerGoDict[prop.ValueEntry.TypeOfValue] = handle;
                //    }

                //    if (handle != null && handle.UnityProperty != null)
                //        result = handle.UnityProperty;
                //}

                if (result != null)
                {
                    result.serializedObject.Update();
                }
            }

            return result;
        }

        /// <summary>
        /// Enumerates over the properties of the tree. WARNING: For tree that have large targets with lots of data, this may involve massive amounts of work as the full tree structure is resolved. USE THIS METHOD SPARINGLY AND ONLY WHEN ABSOLUTELY NECESSARY!
        /// </summary>
        /// <param name="includeChildren">Whether to include children of the root properties or not. If set to true, every property in the entire tree will be enumerated.</param>
        public override IEnumerable<InspectorProperty> EnumerateTree(bool includeChildren)
        {
            for (int i = 0; i < this.secretRootProperty.Children.Count; i++)
            {
                InspectorProperty root = this.secretRootProperty.Children[i];

                yield return root;

                if (includeChildren)
                {
                    foreach (var child in root.Children.Recurse())
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Replaces all occurrences of a value with another value, in the entire tree.
        /// </summary>
        /// <param name="from">The value to find all instances of.</param>
        /// <param name="to">The value to replace the found values with.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">The value to replace with must either be null or be the same type as the value to replace (" + from.GetType().Name + ").</exception>
        public override void ReplaceAllReferences(object from, object to)
        {
            if (from == null)
            {
                throw new ArgumentNullException();
            }

            if (to != null)
            {
                if (from.GetType() != to.GetType())
                {
                    throw new ArgumentException("The value to replace with must either be null or be the same type as the value to replace (" + from.GetType().Name + ").");
                }
            }

            foreach (var prop in this.EnumerateTree(true))
            {
                if (prop.Info.PropertyType == PropertyType.Value && !prop.Info.TypeOfValue.IsValueType)
                {
                    var valueEntry = prop.ValueEntry;

                    for (int i = 0; i < valueEntry.ValueCount; i++)
                    {
                        object obj = valueEntry.WeakValues[i];

                        if (object.ReferenceEquals(from, obj))
                        {
                            valueEntry.WeakValues[i] = to;
                        }
                    }
                }
            }
        }

        internal override void ForceRegisterObjectReference(object reference, InspectorProperty property)
        {
            var type = reference.GetType();

            if (type.IsValueType || type == typeof(string) || type.IsEnum)
            {
                // We do not allow references to boxed value types or primitives
                return;
            }

            this.objectReferences[reference] = property.Path;
        }
        
        /// <summary>
        /// Gets the root tree property at a given index.
        /// </summary>
        /// <param name="index">The index of the property to get.</param>
        public override InspectorProperty GetRootProperty(int index)
        {
            return this.secretRootProperty.Children[index];
        }

        /// <summary>
        /// Schedules a delegate to be invoked at the end of the current GUI frame.
        /// </summary>
        /// <param name="action">The action delegate to be delayed.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public override void DelayAction(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.delayedActions.Add(action);
        }

        /// <summary>
        /// Schedules a delegate to be invoked at the end of the next Repaint GUI frame.
        /// </summary>
        /// <param name="action">The action to be delayed.</param>
        /// <exception cref="System.ArgumentNullException">action</exception>
        public override void DelayActionUntilRepaint(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            this.delayedRepaintActions.Add(action);
        }

        /// <summary>
        /// Invokes the actions that have been delayed using <see cref="DelayAction(Action)" /> and <see cref="DelayActionUntilRepaint(Action)" />.
        /// </summary>
        public override void InvokeDelayedActions()
        {
            for (int i = 0; i < this.delayedActions.Count; i++)
            {
                try
                {
                    this.delayedActions[i]();
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            this.delayedActions.Clear();

            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < this.delayedRepaintActions.Count; i++)
                {
                    try
                    {
                        this.delayedRepaintActions[i]();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }

                this.delayedRepaintActions.Clear();
            }
        }
    }
}
#endif