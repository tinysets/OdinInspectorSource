#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="PropertyContextMenuDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="PropertyContextMenuDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Serialization;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public static class PropertyContextMenuDrawer
    {
        /// <summary>
        /// Adds the right click area.
        /// </summary>
        public static void AddRightClickArea(InspectorProperty property, Rect rect)
        {
            var id = GUIUtility.GetControlID(FocusType.Passive);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && rect.Contains(Event.current.mousePosition))
            {
                // This check should be unnecessary now because the PropertyContextMenuDrawer should now be skipped.
                // Do not eat events or change hot ID if the context menu is disabled.
                //var disableAttr = property.GetAttribute<DisableContextMenuAttribute>();
                //if (disableAttr != null)
                //{
                //    if (property.ChildResolver is ICollectionResolver)
                //    {
                //        if (disableAttr.DisableForCollectionElements)
                //        {
                //            return;
                //        }
                //    }
                //    else if (disableAttr.DisableForMember)
                //    {
                //        return;
                //    }
                //}

                GUIUtility.hotControl = id;
                Event.current.Use();
                GUIHelper.RequestRepaint();
            }

            if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition) && id == GUIUtility.hotControl)
            {
                GUIHelper.RemoveFocusControl();
                Event.current.Use();

                var enableContextMenu = true;
                // This check should be unnecessary now because the PropertyContextMenuDrawer should now be skipped.
                //var disableAttr = property.GetAttribute<DisableContextMenuAttribute>();
                //if (disableAttr != null)
                //{
                //    if (property.ChildResolver is ICollectionResolver)
                //    {
                //        enableContextMenu = !disableAttr.DisableForCollectionElements;
                //    }
                //    else
                //    {
                //        enableContextMenu = !disableAttr.DisableForMember;
                //    }
                //}

                if (enableContextMenu)
                {
                    var menu = new GenericMenu();
                    GUIHelper.RemoveFocusControl();
                    PopulateGenericMenu(property, menu);
                    property.PopulateGenericMenu(menu);
                    if (menu.GetItemCount() == 0)
                    {
                        menu = null;
                    }
                    else { menu.ShowAsContext(); }
                }
            }

            if (GUIUtility.hotControl == id && Event.current.type == EventType.Repaint)
            {
                rect.width = 3;
                rect.x -= 4;
                SirenixEditorGUI.DrawSolidRect(rect, SirenixGUIStyles.HighlightedTextColor);
            }
        }

        private static void PopulateChangedFromPrefabContext(InspectorProperty property, GenericMenu genericMenu)
        {
            var entry = property.ValueEntry;

            if (entry != null)
            {
                InspectorProperty prefabProperty = null;

                if (property.Tree.PrefabModificationHandler.PrefabPropertyTree != null)
                {
                    prefabProperty = property.Tree.PrefabModificationHandler.PrefabPropertyTree.GetPropertyAtPath(property.Path);
                }

                bool active = prefabProperty != null;

                int moddedChildren = property.Children.Recurse().Count(c => c.ValueEntry != null && c.ValueEntry.ValueChangedFromPrefab);

                if (entry.ValueChangedFromPrefab || moddedChildren > 0)
                {
                    if (active)
                    {
                        genericMenu.AddItem(new GUIContent("Revert to prefab value" + (moddedChildren > 0 ? " (" + moddedChildren + " child modifications to revert)" : "")), false, () =>
                        {
                            for (int i = 0; i < entry.ValueCount; i++)
                            {
                                property.Tree.PrefabModificationHandler.RemovePrefabModification(property, i, PrefabModificationType.Value);
                            }

                            if (property.Tree.UnitySerializedObject != null)
                            {
                                property.Tree.UnitySerializedObject.Update();
                            }
                        });
                    }
                    else
                    {
                        genericMenu.AddDisabledItem(new GUIContent("Revert to prefab value (Does not exist on prefab)"));
                    }
                }

                if (entry.ListLengthChangedFromPrefab)
                {
                    if (active)
                    {
                        genericMenu.AddItem(new GUIContent("Revert to prefab list length"), false, () =>
                        {
                            for (int i = 0; i < entry.ValueCount; i++)
                            {
                                property.Tree.PrefabModificationHandler.RemovePrefabModification(property, i, PrefabModificationType.ListLength);
                            }

                            property.Children.Update();

                            if (property.Tree.UnitySerializedObject != null)
                            {
                                property.Tree.UnitySerializedObject.Update();
                            }
                        });
                    }
                    else
                    {
                        genericMenu.AddDisabledItem(new GUIContent("Revert to prefab list length (Does not exist on prefab)"));
                    }
                }

                if (entry.DictionaryChangedFromPrefab)
                {
                    if (active)
                    {
                        genericMenu.AddItem(new GUIContent("Revert dictionary changes to prefab value"), false, () =>
                        {
                            for (int i = 0; i < entry.ValueCount; i++)
                            {
                                property.Tree.PrefabModificationHandler.RemovePrefabModification(property, i, PrefabModificationType.Dictionary);
                            }

                            property.Children.Update();

                            if (property.Tree.UnitySerializedObject != null)
                            {
                                property.Tree.UnitySerializedObject.Update();
                            }
                        });
                    }
                    else
                    {
                        genericMenu.AddDisabledItem(new GUIContent("Revert dictionary changes to prefab value (Does not exist on prefab)"));
                    }
                }
            }
        }

        private static void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            PopulateChangedFromPrefabContext(property, genericMenu);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }
            var objs = property.ValueEntry.WeakValues.FilterCast<object>().Where(x => x != null).ToArray();
            var valueToCopy = (objs == null || objs.Length == 0) ? null : (objs.Length == 1 ? objs[0] : objs);
            bool isUnityObject = property.ValueEntry.BaseValueType.InheritsFrom(typeof(UnityEngine.Object));
            bool hasValue = valueToCopy != null;
            bool canPaste = Clipboard.CanPaste(property.ValueEntry.BaseValueType);
            bool isEditable = property.ValueEntry.IsEditable;
            bool isNullable =
                (property.ValueEntry.BaseValueType.IsClass || property.ValueEntry.BaseValueType.IsInterface) &&
                !property.Info.TypeOfValue.IsValueType &&
                (property.ValueEntry.SerializationBackend != SerializationBackend.Unity || isUnityObject);

            //if (canPaste && property.ValueEntry.SerializationBackend != SerializationBackend.Unity && Clipboard.CurrentCopyMode == CopyModes.CopyReference)
            //{
            //    canPaste = false;
            //}

            if (canPaste && isEditable)
            {
                genericMenu.AddItem(new GUIContent("Paste"), false, () =>
                {
                    property.Tree.DelayActionUntilRepaint(() =>
                    {
                        for (int i = 0; i < property.ValueEntry.ValueCount; i++)
                        {
                            property.ValueEntry.WeakValues[i] = Clipboard.Paste();
                        }
                        // Apply happens after the action is invoked in repaint
                        //property.ValueEntry.ApplyChanges();
                        GUIHelper.RequestRepaint();
                    });
                });
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Paste"));
            }

            if (hasValue)
            {
                if (isUnityObject)
                {
                    genericMenu.AddItem(new GUIContent("Copy"), false, () => Clipboard.Copy(valueToCopy, CopyModes.CopyReference));
                }
                else if (property.ValueEntry.TypeOfValue.IsNullableType() == false)
                {
                    genericMenu.AddItem(new GUIContent("Copy"), false, () => Clipboard.Copy(valueToCopy, CopyModes.CopyReference));
                }
                else if (property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
                {
                    genericMenu.AddItem(new GUIContent("Copy"), false, () => Clipboard.Copy(valueToCopy, CopyModes.DeepCopy));
                }
                else
                {
                    genericMenu.AddItem(new GUIContent("Copy"), false, () => Clipboard.Copy(valueToCopy, CopyModes.DeepCopy));
                    genericMenu.AddItem(new GUIContent("Copy Special/Deep Copy (default)"), false, () => Clipboard.Copy(valueToCopy, CopyModes.DeepCopy));
                    genericMenu.AddItem(new GUIContent("Copy Special/Shallow Copy"), false, () => Clipboard.Copy(valueToCopy, CopyModes.ShallowCopy));
                    genericMenu.AddItem(new GUIContent("Copy Special/Copy Reference"), false, () => Clipboard.Copy(valueToCopy, CopyModes.CopyReference));
                }
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Copy"));
            }

            if (isNullable)
            {
                genericMenu.AddSeparator("");

                if (hasValue && isEditable)
                {
                    genericMenu.AddItem(new GUIContent("Set To Null"), false, () =>
                    {
                        property.Tree.DelayActionUntilRepaint(() =>
                        {
                            for (int i = 0; i < property.ValueEntry.ValueCount; i++)
                            {
                                property.ValueEntry.WeakValues[i] = null;
                            }
                            // Apply happens after the action is invoked in repaint
                            //property.ValueEntry.ApplyChanges();
                            GUIHelper.RequestRepaint();
                        });
                    });
                }
                else
                {
                    genericMenu.AddDisabledItem(new GUIContent("Set To Null"));
                }
            }
        }
    }

    /// <summary>
    /// Opens a context menu for any given property on right click. The context menu is populated by all relevant drawers that implements <see cref="IDefinesGenericMenuItems"/>.
    /// </summary>
    /// <seealso cref="IDefinesGenericMenuItems"/>
    [DrawerPriority(95, 0, 0)]
    public sealed class PropertyContextMenuDrawer<T> : OdinValueDrawer<T>
    {
        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
            var disableAttr = this.Property.GetAttribute<DisableContextMenuAttribute>();

            if (disableAttr != null && disableAttr.DisableForMember)
            {
                this.SkipWhenDrawing = true;
            }
            else if (this.Property.Parent != null && this.Property.Parent.ChildResolver is ICollectionResolver)
            {
                disableAttr = this.Property.Parent.GetAttribute<DisableContextMenuAttribute>();
                this.SkipWhenDrawing = disableAttr != null && disableAttr.DisableForCollectionElements;
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.CallNextDrawer(label);

            if (Event.current.type == EventType.Layout)
            {
                return;
            }

            Rect rect;

            if (this.Property.Parent != null && this.Property.Parent.ChildResolver is ICollectionResolver)
            {
                rect = GUIHelper.GetCurrentLayoutRect();
            }
            else
            {
                rect = this.Property.LastDrawnValueRect;
            }

            GUIHelper.PushGUIEnabled(true);
            PropertyContextMenuDrawer.AddRightClickArea(this.Property, rect);
            GUIHelper.PopGUIEnabled();
        }
    }
}
#endif