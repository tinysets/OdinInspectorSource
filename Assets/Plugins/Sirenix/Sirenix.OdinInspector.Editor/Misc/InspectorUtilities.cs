#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InspectorUtilities.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="InspectorUtilities.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Linq;
    using System.Text;
    using Utilities;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Provides a variety of miscellaneous utilities widely used in the inspector.
    /// </summary>
    public static class InspectorUtilities
    {
        static InspectorUtilities()
        {
            string nativeObjectPtrName = UnityVersion.IsVersionOrGreater(2018, 3) ? "m_NativeObjectPtr" : "m_Property";

            var nativeObjectPtrField = typeof(SerializedObject).GetField(nativeObjectPtrName, Flags.InstanceAnyVisibility);

            if (nativeObjectPtrField != null)
            {
                SerializedObject_nativeObjectPtrGetter = EmitUtilities.CreateInstanceFieldGetter<SerializedObject, IntPtr>(nativeObjectPtrField);
            }
            else
            {
                Debug.LogWarning("The internal Unity field SerializedObject.m_Property (< 2018.3)/SerializedObject.m_NativeObjectPtr (>= 2018.3) has been renamed in this version of Unity!");
            }
        }

        private static ValueGetter<SerializedObject, IntPtr> SerializedObject_nativeObjectPtrGetter;
        private static GUIFrameCounter frameCounter = new GUIFrameCounter();
        private static int drawnInspectorDepthCount = 0;

        /// <summary>
        /// Converts an Odin property path to a deep reflection path.
        /// </summary>
        public static string ConvertToDeepReflectionPath(string odinPropertyPath)
        {
            return ConvertOdinPath(odinPropertyPath, isUnity: false);
        }

        /// <summary>
        /// Converts an Odin property path (without groups included) into a Unity property path.
        /// </summary>
        public static string ConvertToUnityPropertyPath(string odinPropertyPath)
        {
            return ConvertOdinPath(odinPropertyPath, isUnity: true);
        }

        private static string ConvertOdinPath(string odinPropertyPath, bool isUnity)
        {
            bool hasSpecialCharacters = false;

            for (int i = 0; i < odinPropertyPath.Length; i++)
            {
                if (odinPropertyPath[i] == '$' || odinPropertyPath[i] == '#')
                {
                    hasSpecialCharacters = true;
                    break;
                }
            }

            if (hasSpecialCharacters)
            {
                using (var sbCache = Cache<StringBuilder>.Claim())
                {
                    StringBuilder sb = sbCache.Value;
                    sb.Length = 0;

                    bool skipUntilNextDot = false;

                    for (int i = 0; i < odinPropertyPath.Length; i++)
                    {
                        var c = odinPropertyPath[i];

                        if (c == '.') skipUntilNextDot = false;
                        else if (skipUntilNextDot) continue;

                        if (c == '$')
                        {
                            sb.Append(isUnity ? "Array.data[" : "[");
                            i++;

                            while (i < odinPropertyPath.Length && char.IsNumber(odinPropertyPath[i]))
                            {
                                sb.Append(odinPropertyPath[i]);
                                i++;
                            }

                            // Insert ']' char after array number
                            sb.Append(']');

                            // Make sure we don't skip the next char
                            i--;
                        }
                        else if (c == '#')
                        {
                            skipUntilNextDot = true;
                            continue;
                        }
                        else if (c == '.')
                        {
                            if (sb.Length > 0 && sb[sb.Length - 1] != '.') // Never add a dot at the start, or just after another dot
                            {
                                sb.Append('.');
                            }
                        }
                        else
                        {
                            sb.Append(c);
                        }
                    }

                    while (sb.Length > 0 && sb[0] == '.')
                    {
                        sb.Remove(0, 1);
                    }

                    while (sb.Length > 0 && sb[sb.Length - 1] == '.')
                    {
                        sb.Remove(sb.Length - 1, 1);
                    }

                    return sb.ToString();
                }
            }
            else
            {
                return odinPropertyPath;
            }
        }

        /// <summary>
        /// Prepares a property tree for drawing, and handles management of undo, as well as marking scenes and drawn assets dirty.
        /// </summary>
        /// <param name="tree">The tree to be drawn.</param>
        /// <param name="withUndo">Whether to register undo commands for the changes made to the tree. This can only be set to true if the tree has a <see cref="SerializedObject"/> to represent.</param>
        /// <exception cref="System.ArgumentNullException">tree is null</exception>
        public static void BeginDrawPropertyTree(PropertyTree tree, bool withUndo)
        {
            // This provides GUIHelper with a more reliable context-width, so that Unity
            // can better figure out what the label width is non-repaint events.
            // - Bjarke
            if (Event.current.type == EventType.Repaint)
            {
                tree.ContextWidth = GUIHelper.ContextWidth;
            }
            GUIHelper.BetterContextWidth = tree.ContextWidth;
            
            if (frameCounter.Update().IsNewFrame)
            {
                drawnInspectorDepthCount = 0;
            }
            drawnInspectorDepthCount++;

            if (tree == null)
            {
                throw new ArgumentNullException("tree");
            }

            if (!tree.IsStatic)
            {
                for (int i = 0; i < tree.WeakTargets.Count; i++)
                {
                    if (tree.WeakTargets[i] == null)
                    {
                        GUILayout.Label("An inspected object has been destroyed; please refresh the inspector.");
                        return;
                    }
                }
            }

            if (tree.UnitySerializedObject != null)
            {
                tree.UnitySerializedObject.Update();
            }

            tree.UpdateTree();

            if (tree.PrefabModificationHandler.HasNestedOdinPrefabData)
            {
                SirenixEditorGUI.ErrorMessageBox("A selected object is serialized by Odin, is a prefab, and contains nested prefab data (IE, more than one possible layer of prefab modifications). This is NOT CURRENTLY SUPPORTED by Odin - therefore, modification of all Odin-serialized values has been disabled for this object.\n\nThere is a strong likelihood that Odin-serialized values will be corrupt and/or wrong in other ways, as well as a very real risk that your computer may spontaneously combust and turn into a flaming wheel of cheese.");
            }

            tree.WillUndo = false;

            if (withUndo)
            {
                if (tree.TargetType.ImplementsOrInherits(typeof(UnityEngine.Object)) == false)
                {
                    Debug.LogError("Automatic inspector undo only works when you're inspecting a type derived from UnityEngine.Object, and you are inspecting '" + tree.TargetType.GetNiceName() + "'.");
                }
                else
                {
                    tree.WillUndo = true;
                }
            }

            if (tree.WillUndo)
            {
                for (int i = 0; i < tree.WeakTargets.Count; i++)
                {
                    Undo.RecordObject((UnityEngine.Object)tree.WeakTargets[i], "Sirenix Inspector value change");
                }
            }

            if (tree.DrawMonoScriptObjectField)
            {
                var scriptProp = tree.UnitySerializedObject.FindProperty("m_Script");

                if (scriptProp != null)
                {
                    GUIHelper.PushGUIEnabled(false);
                    EditorGUILayout.PropertyField(scriptProp);
                    GUIHelper.PopGUIEnabled();
                }
            }
        }

        /// <summary>
        /// Ends drawing a property tree, and handles management of undo, as well as marking scenes and drawn assets dirty.
        /// </summary>
        /// <param name="tree">The tree.</param>
        public static void EndDrawPropertyTree(PropertyTree tree)
        {
            tree.InvokeDelayedActions();

            if (tree.UnitySerializedObject != null)
            {
                if (SerializedObject_nativeObjectPtrGetter != null)
                {
                    var obj = tree.UnitySerializedObject;
                    IntPtr ptr = SerializedObject_nativeObjectPtrGetter(ref obj);

                    if (ptr == IntPtr.Zero)
                    {
                        // SerializedObject has been disposed, likely due to a scene change invoked from GUI code.
                        // BAIL THE FUCK OUT! :D Crashes will happen.
                        return;
                    }
                }

                if (tree.WillUndo)
                {
                    tree.UnitySerializedObject.ApplyModifiedProperties();
                }
                else
                {
                    tree.UnitySerializedObject.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            bool appliedOdinChanges = false;

            if (tree.ApplyChanges())
            {
                appliedOdinChanges = true;
                GUIHelper.RequestRepaint();
            }

            // This is very important, as applying changes may cause more actions to be delayed
            tree.InvokeDelayedActions();

            if (appliedOdinChanges)
            {
                tree.InvokeOnValidate();

                if (tree.PrefabModificationHandler.HasPrefabs)
                {
                    var targets = tree.WeakTargets;

                    for (int i = 0; i < targets.Count; i++)
                    {
                        if (tree.PrefabModificationHandler.TargetPrefabs[i] == null) continue;

                        var target = (UnityEngine.Object)targets[i];
                        PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                    }
                }
            }

            if (tree.WillUndo)
            {
                Undo.FlushUndoRecordObjects();
            }

            drawnInspectorDepthCount--;

#if ODIN_TRIAL_VERSION
            if (drawnInspectorDepthCount == 0)
            {
                float height = OdinTrialVersionInfo.IsExpired ? 22 : 17;
                var rect = GUILayoutUtility.GetRect(16, height, GUILayoutOptions.ExpandWidth().Height(height));

                var bgRect = rect;
                bgRect.xMin -= 20;
                bgRect.xMax += 20;
                bgRect.y += 2;
                SirenixEditorGUI.DrawBorders(bgRect, 0, 0, 1, 0, SirenixGUIStyles.LightBorderColor);

                rect.y += 2;
                if (OdinTrialVersionInfo.IsExpired)
                {
                    EditorGUI.DrawRect(bgRect, Color.black);
                    GUIHelper.PushContentColor(Color.red);

                    GUI.Label(rect.AddY(3), "Odin Inspector Trial expired!", SirenixGUIStyles.Label);
                    GUIHelper.PopContentColor();
                    var btnRect = rect.AlignRight(EditorStyles.miniButton.CalcSize(new GUIContent("Purchase Odin Inspector")).x);
                    btnRect.yMin += 2;
                    btnRect.yMax -= 2;
                    GUIHelper.PushColor(Color.green);
                    if (GUI.Button(btnRect, "Purchase Odin Inspector", EditorStyles.miniButton))
                    {
                        UnityEditorInternal.AssetStore.Open("content/89041");
                    }
                    GUIHelper.PopColor();
                }
                else
                {
                    GUI.Label(rect, "Odin Inspector Trial Version", SirenixGUIStyles.LeftAlignedGreyMiniLabel);
                    GUI.Label(rect, "Expires " + OdinTrialVersionInfo.ExpirationDate.ToShortDateString(), EditorStyles.centeredGreyMiniLabel);
                    GUI.Label(rect, OdinTrialVersionInfo.ExpirationDate.Subtract(System.DateTime.Now).TotalHours.ToString("F2") + " hours remaining.", SirenixGUIStyles.RightAlignedGreyMiniLabel);
                }
            }
#endif
        }

        public static void RegisterUnityObjectDirty(UnityEngine.Object unityObj)
        {
            //var component = unityObj as Component;

            if (AssetDatabase.Contains(unityObj) /*|| (component != null && AssetDatabase.Contains(component.gameObject))*/)
            {
                EditorUtility.SetDirty(unityObj);
                //if (component != null)
                //{
                //    EditorUtility.SetDirty(component.gameObject);
                //}
            }
            else if (Application.isPlaying == false)
            {
                if (unityObj is Component)
                {
                    Component component = (Component)unityObj;
                    EditorUtility.SetDirty(component);
                    EditorUtility.SetDirty(component.gameObject);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(component.gameObject.scene);
                }
                else if (unityObj is EditorWindow || unityObj is ScriptableObject)
                {
                    EditorUtility.SetDirty(unityObj);
                }
                else
                {
                    // We can't find out where this thing is from
                    // It is probably a "temporary" UnityObject created from a script somewhere
                    // Just to be safe, mark it as dirty, and mark all scenes as dirty

                    EditorUtility.SetDirty(unityObj);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
                }
            }
        }

        /// <summary>
        /// Draws all properties in a given property tree; must be wrapped by a <see cref="BeginDrawPropertyTree(PropertyTree, bool)"/> and <see cref="EndDrawPropertyTree(PropertyTree)"/>.
        /// </summary>
        /// <param name="tree">The tree to be drawn.</param>
        public static void DrawPropertiesInTree(PropertyTree tree)
        {
            foreach (var property in tree.EnumerateTree(false))
            {
                try
                {
                    property.Draw(property.Label);
                }
                catch (Exception ex)
                {
                    if (ex is ExitGUIException || ex.InnerException is ExitGUIException)
                    {
                        throw ex;
                    }
                    else
                    {
                        var msg =
                            "This error occurred while being drawn by Odin. \n" +
                            "Odin Property Path: " + property.Path + "\n" +
                            "Odin Drawer Chain: " + string.Join(", ", property.GetActiveDrawerChain().BakedDrawerArray.Select(n => n.GetType().GetNiceName()).ToArray()) + ".";

                        Debug.LogException(new OdinPropertyException(msg, ex));
                    }
                }
            }
        }

        /// <summary>
        /// Draws a property in the inspector using a given label.
        /// </summary>
#if SIRENIX_INTERNAL
        [Obsolete("Use InspectorProperty.Draw(label) instead.", true)]
#else
        [Obsolete("Use InspectorProperty.Draw(label) instead.", false)]
#endif
        public static void DrawProperty(InspectorProperty property, GUIContent label)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            property.Draw(label);
        }
    }

    /// <summary>
    /// Odin property system exception.
    /// </summary>
    public class OdinPropertyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of OdinPropertyException.
        /// </summary>
        /// <param name="message">The message for the exception.</param>
        /// <param name="innerException">An inner exception.</param>
        public OdinPropertyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
#endif