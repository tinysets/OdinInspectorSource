#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="NullableReferenceDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="NullReferenceDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Sirenix.Utilities;
    using System.Collections;
    using Sirenix.Serialization;

    /// <summary>
    /// Draws all nullable reference types, with an object field.
    /// </summary>
    [AllowGUIEnabledForReadonly]
    [DrawerPriority(0, 0, 2000)]
    public sealed class NullableReferenceDrawer<T> : OdinValueDrawer<T>, IDefinesGenericMenuItems
    {
        private bool shouldDrawReferencePicker;
        private bool drawUnityObject;
        private bool allowSceneObjects;
        private LocalPersistentContext<bool> isToggled;
        private OdinDrawer[] bakedDrawerArray;
        private InlinePropertyAttribute inlinePropertyAttr;
        private bool drawChildren;

        protected override void Initialize()
        {
            this.isToggled = LocalPersistentContext<bool>.Create(PersistentContext.Get(this.Property.Path, SirenixEditorGUI.ExpandFoldoutByDefault));
            this.drawUnityObject = typeof(UnityEngine.Object).IsAssignableFrom(this.ValueEntry.TypeOfValue);
            this.allowSceneObjects = this.Property.GetAttribute<AssetsOnlyAttribute>() == null;
            this.bakedDrawerArray = this.Property.GetActiveDrawerChain().BakedDrawerArray;
            this.inlinePropertyAttr = this.Property.Attributes.GetAttribute<InlinePropertyAttribute>();
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;

            if (Event.current.type == EventType.Layout)
            {
                this.shouldDrawReferencePicker = ShouldDrawReferenceObjectPicker(this.ValueEntry);

                if (this.Property.Children.Count > 0)
                {
                    this.drawChildren = true;
                }
                else if (this.ValueEntry.ValueState != PropertyValueState.None)
                {
                    this.drawChildren = false;
                }
                else
                {
                    // Weird case: This prevents a foldout from being drawn that expands nothing.
                    // If we're the second last drawer, then the next drawer is most likely 
                    // the composite drawer. And since we don't have any children in this
                    // else statement, we don't have anything else to draw.
                    this.drawChildren = this.bakedDrawerArray[this.bakedDrawerArray.Length - 2] != this;
                }
            }

            if (entry.ValueState == PropertyValueState.NullReference)
            {
                if (this.drawUnityObject)
                {
                    this.CallNextDrawer(label);
                }
                else
                {
                    if (entry.SerializationBackend == SerializationBackend.Unity && entry.IsEditable && Event.current.type == EventType.Layout)
                    {
                        Debug.LogError("Unity-backed value is null. This should already be fixed by the FixUnityNullDrawer!");
                    }
                    else
                    {
                        this.DrawField(label);
                    }
                }
            }
            else
            {
                if (this.shouldDrawReferencePicker)
                {
                    this.DrawField(label);
                }
                else
                {
                    this.CallNextDrawer(label);
                }
            }

            var objectPicker = ObjectPicker.GetObjectPicker(entry, entry.BaseValueType);
            if (objectPicker.IsReadyToClaim)
            {
                var obj = objectPicker.ClaimObject();
                entry.Property.Tree.DelayActionUntilRepaint(() =>
                {
                    entry.WeakValues[0] = obj;
                    for (int j = 1; j < entry.ValueCount; j++)
                    {
                        entry.WeakValues[j] = SerializationUtility.CreateCopy(obj);
                    }
                });
            }
        }

        private void DrawField(GUIContent label)
        {
            if (this.inlinePropertyAttr != null)
            {
                var pushLabelWidth = this.inlinePropertyAttr.LabelWidth > 0;
                if (label == null)
                {
                    if (pushLabelWidth) GUIHelper.PushLabelWidth(this.inlinePropertyAttr.LabelWidth);
                    this.DrawInlinePropertyReferencePicker();
                    this.CallNextDrawer(null);
                    if (pushLabelWidth) GUIHelper.PopLabelWidth();
                }
                else
                {
                    SirenixEditorGUI.BeginVerticalPropertyLayout(label);
                    this.DrawInlinePropertyReferencePicker();
                    if (pushLabelWidth) GUIHelper.PushLabelWidth(this.inlinePropertyAttr.LabelWidth);
                    for (int i = 0; i < this.Property.Children.Count; i++)
                    {
                        var child = this.Property.Children[i];
                        child.Draw(child.Label);
                    }
                    if (pushLabelWidth) GUIHelper.PopLabelWidth();
                    SirenixEditorGUI.EndVerticalPropertyLayout();
                }
            }
            else
            {

                Rect valueRect;
                bool hasKeyboadFocus;
                int id;
                var rect = SirenixEditorGUI.GetFeatureRichControlRect(null, out id, out hasKeyboadFocus, out valueRect);

                if (label != null)
                {
                    rect.width = GUIHelper.BetterLabelWidth;
                    valueRect.xMin = rect.xMax;

                    if (this.drawChildren)
                    {
                        this.isToggled.Value = SirenixEditorGUI.Foldout(rect, this.isToggled.Value, label);
                    }
                    else if (Event.current.type == EventType.Repaint)
                    {
                        rect = EditorGUI.IndentedRect(rect);
                        GUI.Label(rect, label);
                    }
                }
                else if (this.drawChildren)
                {
                    if (EditorGUIUtility.hierarchyMode)
                    {
                        rect.width = 18;
                        var preev = EditorGUIUtility.hierarchyMode;
                        this.isToggled.Value = SirenixEditorGUI.Foldout(rect, this.isToggled.Value, GUIContent.none);
                    }
                    else
                    {
                        rect.width = 18;
                        valueRect.xMin = rect.xMax;
                        var preev = EditorGUIUtility.hierarchyMode;
                        EditorGUIUtility.hierarchyMode = false;
                        this.isToggled.Value = SirenixEditorGUI.Foldout(rect, this.isToggled.Value, GUIContent.none);
                        EditorGUIUtility.hierarchyMode = preev;
                    }
                }

                EditorGUI.BeginChangeCheck();
                var prev = EditorGUI.showMixedValue;
                if (this.ValueEntry.ValueState == PropertyValueState.ReferenceValueConflict)
                {
                    EditorGUI.showMixedValue = true;
                }
                var newValue = SirenixEditorFields.PolymorphicObjectField(valueRect, this.ValueEntry.WeakSmartValue, this.ValueEntry.BaseValueType, this.allowSceneObjects, hasKeyboadFocus, id);
                EditorGUI.showMixedValue = prev;

                if (EditorGUI.EndChangeCheck())
                {
                    this.ValueEntry.Property.Tree.DelayActionUntilRepaint(() =>
                    {
                        this.ValueEntry.WeakValues[0] = newValue;
                        for (int j = 1; j < this.ValueEntry.ValueCount; j++)
                        {
                            this.ValueEntry.WeakValues[j] = SerializationUtility.CreateCopy(newValue);
                        }
                    });
                }

                if (this.drawChildren)
                {
                    var toggle = this.ValueEntry.ValueState == PropertyValueState.NullReference ? false : this.isToggled.Value;
                    if (SirenixEditorGUI.BeginFadeGroup(this, toggle))
                    {
                        EditorGUI.indentLevel++;
                        this.CallNextDrawer(null);
                        EditorGUI.indentLevel--;
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
            }
        }

        private void DrawInlinePropertyReferencePicker()
        {
            EditorGUI.BeginChangeCheck();
            var prev = EditorGUI.showMixedValue;
            if (this.ValueEntry.ValueState == PropertyValueState.ReferenceValueConflict)
            {
                EditorGUI.showMixedValue = true;
            }
            var newValue = SirenixEditorFields.PolymorphicObjectField(this.ValueEntry.WeakSmartValue, this.ValueEntry.BaseValueType, this.allowSceneObjects);
            EditorGUI.showMixedValue = prev;

            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.Property.Tree.DelayActionUntilRepaint(() =>
                {
                    this.ValueEntry.WeakValues[0] = newValue;
                    for (int j = 1; j < this.ValueEntry.ValueCount; j++)
                    {
                        this.ValueEntry.WeakValues[j] = SerializationUtility.CreateCopy(newValue);
                    }
                });
            }
        }

        private static bool ShouldDrawReferenceObjectPicker(IPropertyValueEntry<T> entry)
        {
            return entry.SerializationBackend != SerializationBackend.Unity
                && !entry.BaseValueType.IsValueType
                && entry.BaseValueType != typeof(string)
                && !(entry.Property.ChildResolver is ICollectionResolver)
                && !entry.BaseValueType.IsArray
                && entry.IsEditable
                && (!(typeof(UnityEngine.Object).IsAssignableFrom(entry.TypeOfValue) && !entry.BaseValueType.IsInterface))
                && !entry.BaseValueType.InheritsFrom(typeof(System.Collections.IDictionary))
                && !(entry.WeakSmartValue as UnityEngine.Object)
                && entry.Property.GetAttribute<HideReferenceObjectPickerAttribute>() == null;
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            var entry = property.ValueEntry as IPropertyValueEntry<T>;
            bool isChangeable = property.ValueEntry.SerializationBackend != SerializationBackend.Unity
                && !entry.BaseValueType.IsValueType
                && entry.BaseValueType != typeof(string);

            if (isChangeable)
            {
                if (entry.IsEditable)
                {
                    var objectPicker = ObjectPicker.GetObjectPicker(entry, entry.BaseValueType);
                    var rect = entry.Property.LastDrawnValueRect;
                    rect.position = GUIUtility.GUIToScreenPoint(rect.position);
                    rect.height = 20;
                    genericMenu.AddItem(new GUIContent("Change Type"), false, () =>
                    {
                        objectPicker.ShowObjectPicker(entry.WeakSmartValue, false, rect);
                    });
                }
                else
                {
                    genericMenu.AddDisabledItem(new GUIContent("Change Type"));
                }
            }
        }

        /// <summary>
        /// Returns a value that indicates if this drawer can be used for the given property.
        /// </summary>
        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            var type = property.ValueEntry.BaseValueType;
            return (type.IsClass || type.IsInterface) && type != typeof(string) && !typeof(UnityEngine.Object).IsAssignableFrom(type);
        }
    }
}
#endif