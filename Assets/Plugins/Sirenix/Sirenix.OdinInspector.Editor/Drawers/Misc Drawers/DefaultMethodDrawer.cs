#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="DefaultMethodDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DefaultMethodDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utilities;
    using Utilities.Editor;

    /// <summary>
    /// The default method drawer, that drawers most buttons and also runs OnInspectorGUI attributes.
    /// </summary>
    [DrawerPriority(0, 0, 0.1)]
    public sealed class DefaultMethodDrawer : MethodDrawer
    {
        internal static bool DontDrawMethodParamaters;

        private bool drawParameters;
        private string name;
        private LocalPersistentContext<bool> toggle;
        private ButtonAttribute buttonAttribute;
        private int buttonHeight;
        private GUIStyle style;
        private GUIStyle toggleBtnStyle;
        private StringMemberHelper mh;
        private GUIContent label;
        private ButtonStyle btnStyle;
        private bool expanded;
        private Color btnColor;
        private bool hasGUIColorAttribute;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected override void Initialize()
        {
            this.expanded = false;
            this.buttonAttribute = this.Property.GetAttribute<ButtonAttribute>();
            this.buttonHeight = this.Property.Context.GetGlobal("ButtonHeight", 0).Value;
            this.style = this.Property.Context.GetGlobal("ButtonStyle", (GUIStyle)null).Value;
            this.toggle = this.GetPersistentValue<bool>("toggle", false);
            this.hasGUIColorAttribute = this.Property.GetAttribute<GUIColorAttribute>() != null;
            this.drawParameters = this.Property.Children.Count > 0 && !DontDrawMethodParamaters;
            this.name = this.Property.NiceName;
            this.label = new GUIContent(name);

            if (buttonAttribute != null)
            {
                this.btnStyle = this.buttonAttribute.Style;
                this.expanded = buttonAttribute.Expanded;

                if (!string.IsNullOrEmpty(this.buttonAttribute.Name))
                {
                    this.mh = new StringMemberHelper(this.Property, this.buttonAttribute.Name);
                }

                if (this.buttonHeight == 0 && buttonAttribute.ButtonHeight > 0)
                {
                    this.buttonHeight = buttonAttribute.ButtonHeight;
                }
            }

            if (this.style == null)
            {
                if (this.buttonHeight > 20) this.style = SirenixGUIStyles.Button;
                else this.style = EditorStyles.miniButton;
            }

            if (this.drawParameters && this.btnStyle == ButtonStyle.FoldoutButton && !this.expanded)
            {
                if (this.buttonHeight > 20)
                {
                    this.style = SirenixGUIStyles.ButtonLeft;
                    this.toggleBtnStyle = SirenixGUIStyles.ButtonRight;
                }
                else
                {
                    this.style = EditorStyles.miniButtonLeft;
                    this.toggleBtnStyle = EditorStyles.miniButtonRight;
                }
            }
        }

        /// <summary>
        /// Draws the property layout.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent lbl)
        {
            if (this.mh == null)
            {
                this.label = lbl;
            }

            this.btnColor = GUI.color;
            var contentColor = this.hasGUIColorAttribute ? GUIColorAttributeDrawer.CurrentOuterColor : this.btnColor;
            GUIHelper.PushColor(contentColor);

            var h = this.Property.Context.GetGlobal("ButtonHeight", 0).Value;
            var s = this.Property.Context.GetGlobal("ButtonStyle", (GUIStyle)null).Value;
            if (this.buttonHeight != h && h != 0 || s != null && this.style != s)
            {
                this.Initialize();
            }

            if (this.mh != null && this.label != null)
            {
                var tmp = this.label.text;

                try
                {
                    this.label.text = this.mh.ForceGetString(this.Property.ParentValues[0]);
                }
                catch
                {
                    this.label.text = tmp;
                }
            }

            this.Property.Label = this.label;

            if (!this.drawParameters)
            {
                DrawNormalButton();
            }
            else if (this.btnStyle == ButtonStyle.FoldoutButton)
            {
                if (this.expanded)
                {
                    this.DrawNormalButton();
                    EditorGUI.indentLevel++;
                    this.DrawParameters(false);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    this.DrawFoldoutButton();
                }
            }
            else if (this.btnStyle == ButtonStyle.CompactBox)
            {
                this.DrawCompactBoxButton();
            }
            else if (this.btnStyle == ButtonStyle.Box)
            {
                this.DrawBoxButton();
            }

            GUIHelper.PopColor();
        }

        private void DrawBoxButton()
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginToolbarBoxHeader();

            if (this.expanded)
            {
                EditorGUILayout.LabelField(this.label);
            }
            else
            {
                this.toggle.Value = SirenixEditorGUI.Foldout(this.toggle.Value, this.label);
            }

            SirenixEditorGUI.EndToolbarBoxHeader();
            this.DrawParameters(true);
            SirenixEditorGUI.EndToolbarBox();
        }

        private void DrawCompactBoxButton()
        {
            SirenixEditorGUI.BeginBox();
            var rect = SirenixEditorGUI.BeginToolbarBoxHeader().AlignRight(70).Padding(1);
            rect.height -= 1;

            GUIHelper.PushColor(this.btnColor);

            if (GUI.Button(rect, "Invoke"))
            {
                this.InvokeButton();
            }

            GUIHelper.PopColor();

            if (this.expanded)
            {
                EditorGUILayout.LabelField(this.label);
            }
            else
            {
                this.toggle.Value = SirenixEditorGUI.Foldout(this.toggle.Value, this.label);
            }

            SirenixEditorGUI.EndToolbarBoxHeader();
            this.DrawParameters(false);
            SirenixEditorGUI.EndToolbarBox();
        }

        private void DrawNormalButton()
        {
            Rect btnRect = this.buttonHeight > 0 ?
                GUILayoutUtility.GetRect(GUIContent.none, style, GUILayoutOptions.Height(this.buttonHeight)) :
                GUILayoutUtility.GetRect(GUIContent.none, style);

            btnRect = EditorGUI.IndentedRect(btnRect);

            var tmp = GUI.color;
            GUI.color = this.btnColor;
            if (GUI.Button(btnRect, this.label != null ? this.label : GUIHelper.TempContent(string.Empty), this.style))
            {
                InvokeButton();
            }
            GUI.color = tmp;
        }

        private void DrawFoldoutButton()
        {
            Rect btnRect = this.buttonHeight > 0 ?
                GUILayoutUtility.GetRect(GUIContent.none, style, GUILayoutOptions.Height(this.buttonHeight)) :
                GUILayoutUtility.GetRect(GUIContent.none, style);

            btnRect = EditorGUI.IndentedRect(btnRect);

            GUIHelper.PushColor(this.btnColor);

            var foldoutRect = btnRect.AlignRight(20);
            if (GUI.Button(foldoutRect, GUIContent.none, toggleBtnStyle))
            {
                this.toggle.Value = !this.toggle.Value;
            }

            btnRect.width -= foldoutRect.width;
            if (!this.toggle.Value)
            {
                foldoutRect.x -= 1;
                foldoutRect.yMin -= 1;
            }

            if (this.toggle.Value) EditorIcons.TriangleDown.Draw(foldoutRect, 16);
            else EditorIcons.TriangleLeft.Draw(foldoutRect, 16);

            if (GUI.Button(btnRect, this.label, this.style))
            {
                this.InvokeButton();
            }

            GUIHelper.PopColor();

            EditorGUI.indentLevel++;
            this.DrawParameters(false);
            EditorGUI.indentLevel--;
        }

        private void DrawParameters(bool appendButton)
        {
            if (SirenixEditorGUI.BeginFadeGroup(this, this.toggle.Value || this.expanded))
            {
                GUILayout.Space(0);
                for (int i = 0; i < this.Property.Children.Count; i++)
                {
                    this.Property.Children[i].Draw();
                }

                if (appendButton)
                {
                    var rect = EditorGUILayout.BeginVertical(SirenixGUIStyles.BottomBoxPadding).Expand(3);
                    SirenixEditorGUI.DrawHorizontalLineSeperator(rect.x, rect.y, rect.width);
                    this.DrawNormalButton();
                    EditorGUILayout.EndVertical();
                }
            }
            SirenixEditorGUI.EndFadeGroup();
        }

        private void InvokeButton()
        {
            GUIHelper.RemoveFocusControl();
            GUIHelper.RequestRepaint();

            foreach (var target in this.Property.SerializationRoot.ValueEntry.WeakValues.OfType<UnityEngine.Object>())
            {
                InspectorUtilities.RegisterUnityObjectDirty(target);
                Undo.RecordObject(target, "Button click " + this.Property.NiceName + " on " + target.name);
            }

            var serializationRoot = this.Property.SerializationRoot;

            for (int j = 0; j < serializationRoot.ValueEntry.ValueCount; j++)
            {
                UnityEngine.Object unityObj = serializationRoot.ValueEntry.WeakValues[j] as UnityEngine.Object;

                if (unityObj != null)
                {
                }
            }

            var methodInfo = (MethodInfo)this.Property.Info.GetMemberInfo();
            if (methodInfo != null)
            {
                var parentValueProperty = this.Property.ParentValueProperty;
                var targets = this.Property.ParentValues;

                for (int i = 0; i < targets.Count; i++)
                {
                    object value = targets[i];

                    if (object.ReferenceEquals(value, null) == false || methodInfo.IsStatic)
                    {
                        try
                        {
                            var arguments = new object[this.Property.Children.Count];
                            for (int j = 0; j < arguments.Length; j++)
                            {
                                arguments[j] = this.Property.Children[j].ValueEntry.WeakSmartValue;
                            }

                            if (methodInfo.IsStatic)
                            {
                                methodInfo.Invoke(null, arguments);
                            }
                            else
                            {
                                methodInfo.Invoke(value, arguments);
                            }

                            for (int j = 0; j < arguments.Length; j++)
                            {
                                this.Property.Children[j].ValueEntry.WeakSmartValue = arguments[j];
                            }
                        }
                        catch (ExitGUIException ex)
                        {
                            throw ex;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }

                        if (parentValueProperty != null && value.GetType().IsValueType)
                        {
                            // If it's a struct, it will have been boxed and the invoke call might
                            // have changed the struct and this won't be reflected in the original,
                            // unboxed source struct.

                            // Therefore, set the source value to the boxed struct that we just invoked on.
                            parentValueProperty.ValueEntry.WeakValues[i] = value;
                        }
                    }
                }
            }
            else
            {
                try
                {
                    var arguments = new object[this.Property.Children.Count];
                    for (int j = 0; j < arguments.Length; j++)
                    {
                        arguments[j] = this.Property.Children[j].ValueEntry.WeakSmartValue;
                    }
                    this.Property.Info.GetMethodDelegate().DynamicInvoke(arguments);
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
    }
}
#endif