#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="AttributeExampleUtilities.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="AttributeExampleInfo.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    internal static class AttributeExampleUtilities
    {
        private static readonly Type[] AttributeTypes;
        private static readonly Dictionary<Type, OdinRegisterAttributeAttribute> AttributeRegisterMap;
        private static readonly Dictionary<Type, Texture> AttributeIconMap;

        static AttributeExampleUtilities()
        {
            AttributeRegisterMap = AssemblyUtilities.GetAllAssemblies()
                .SelectMany(a => a.GetCustomAttributes(typeof(OdinRegisterAttributeAttribute), true))
                .Cast<OdinRegisterAttributeAttribute>()
                .ToDictionary(x => x.AttributeType);

            AttributeTypes = AttributeRegisterMap.Keys.ToArray();
            AttributeIconMap = new Dictionary<Type, Texture>();
        }

        public static IEnumerable<Type> GetAllOdinAttributes()
        {
            return AttributeTypes;
        }

        public static Texture GetAttributeIcon(Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            Texture icon;
            if (AttributeIconMap.TryGetValue(attributeType, out icon) == false)
            {
                OdinRegisterAttributeAttribute registration;
                if (AttributeRegisterMap.TryGetValue(attributeType, out registration))
                {
                    if (registration.PngEncodedIcon != null)
                    {
                        icon = TextureUtilities.LoadImage(0, 0, System.Convert.FromBase64String(registration.PngEncodedIcon));
                    }
                    else if (registration.IconPath != null)
                    {
                        Debug.LogWarning("OdinRegisterAttribute.IconPath is not yet supported.");
                        icon = null;
                    }
                }

                AttributeIconMap.Add(attributeType, icon);
            }

            return icon;
        }

        public static IEnumerable<string> GetAttributeCategories(Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            OdinRegisterAttributeAttribute registration;
            if (AttributeRegisterMap.TryGetValue(attributeType, out registration) && registration.Categories != null)
            {
                // TODO: Cache this?
                return registration.Categories.Split(',').Select(x => x.Trim());
            }
            else
            {
                return new string[] { "Uncategorized" };
            }
        }

        public static string GetAttributeDescription(Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            OdinRegisterAttributeAttribute registration;
            if (AttributeRegisterMap.TryGetValue(attributeType, out registration))
            {
                return registration.Description;
            }
            else
            {
                return null;
            }
        }

        public static AttributeExampleInfo[] GetAttributeExamples(Type attributeType)
        {
            if (attributeType == null)  
            {
                throw new ArgumentNullException("attributeType");
            }

            AttributeExampleInfo[] examples;
            if (InternalAttributeExampleInfoMap.Map.TryGetValue(attributeType, out examples) == false)
            {
                examples = new AttributeExampleInfo[0];
            }

            return examples;
        }

        public static string GetOnlineDocumentationUrl(Type attributeType)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            OdinRegisterAttributeAttribute registration;
            if (AttributeRegisterMap.TryGetValue(attributeType, out registration))
            {
                return registration.DocumentationUrl;
            }

            return null;
        }

        public static void BuildMenuTree(OdinMenuTree tree)
        {
            foreach (var a in GetAllOdinAttributes())
            {
                string search = a.Name; // TODO: tags?

                foreach (var c in GetAttributeCategories(a))
                {
                    var item = new OdinMenuItem(tree, a.GetNiceName().Replace("Attribute", "").SplitPascalCase(), a)
                    {
                        Value = a,
                        Icon = GetAttributeIcon(a),
                        SearchString = search,
                    };
                    search = null; // Only allow the user to find the first item of an attribute by search.

                    tree.AddMenuItemAtPath(c, item);
                }
            }

            tree.SortMenuItemsByName();
        }

        public static OdinAttributeExample GetExample<T>() where T : Attribute
        {
            return GetExample(typeof(T));
        }

        public static OdinAttributeExample GetExample(Type attributeType)
        {
            OdinRegisterAttributeAttribute registration;
            AttributeRegisterMap.TryGetValue(attributeType, out registration);
            return new OdinAttributeExample(attributeType, registration);
        }
    }

    internal class OdinAttributeExample
    {
        private Type attributeType;
        private OdinRegisterAttributeAttribute registration;
        private AttributeExample[] examples;
        private GUITabGroup tabGroup;

        public readonly string Name;
        public readonly Texture Icon;

        public bool DrawCodeExample { get; set; }

        public OdinAttributeExample(Type attributeType, OdinRegisterAttributeAttribute registration)
        {
            if (attributeType == null)
            {
                throw new ArgumentNullException("attributeType");
            }

            this.attributeType = attributeType;
            this.registration = registration;
            this.Name = this.attributeType.GetNiceName().SplitPascalCase();
            this.Icon = AttributeExampleUtilities.GetAttributeIcon(this.registration.AttributeType);
            this.DrawCodeExample = true;

            var exampleInfos = AttributeExampleUtilities.GetAttributeExamples(attributeType);
            this.examples = new AttributeExample[exampleInfos.Length];
            for (int i = 0; i < exampleInfos.Length; i++)
            {
                this.examples[i] = new AttributeExample(exampleInfos[i]);
            }

            this.tabGroup = new GUITabGroup();
            for (int i = 0; i < exampleInfos.Length; i++)
            {
                this.tabGroup.RegisterTab(exampleInfos[i].Name);
            }
        }

        [OnInspectorGUI]
        public void Draw()
        {
            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            {
                Rect rect = GUILayoutUtility.GetRect(100, 500, 20, 20).AddY(2);
                if (this.Icon != null)
                {
                    EditorIcons.X.Draw(rect.AlignLeft(16).SetHeight(16).AddX(2), this.Icon);
                    rect.xMin += 20;
                }

                GUI.Label(rect, this.Name);

                if (string.IsNullOrEmpty(this.registration.DocumentationUrl) == false)
                {
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Documentation", SirenixGUIStyles.MiniButton))
                    {
                        Help.BrowseURL(this.registration.DocumentationUrl);
                    }
                }
            }
            SirenixEditorGUI.EndBoxHeader();

            if (string.IsNullOrEmpty(this.registration.Description) == false)
            {
                GUILayout.Label(this.registration.Description);
            }

            SirenixEditorGUI.EndBox();
            GUILayout.Space(15);

            if (this.examples.Length > 0)
            {
                this.tabGroup.BeginGroup();
                foreach (var example in this.examples)
                {
                    var tab = this.tabGroup.RegisterTab(example.ExampleInfo.Name);
                    if (tab.BeginPage())
                    {
                        example.Draw(this.DrawCodeExample);
                    }
                    tab.EndPage();
                }
                this.tabGroup.EndGroup();
            }
            else
            {
                GUILayout.Label("No examples available.");
            }
        }
    }

    internal class AttributeExample
    {
        private static GUIStyle codeStyle;

        public AttributeExampleInfo ExampleInfo;
        private PropertyTree tree;
        private string highlightedCode = null;
        private Vector2 scrollPosition;
        private bool showRaw;

        public AttributeExample(AttributeExampleInfo exampleInfo)
        {
            this.ExampleInfo = exampleInfo;

            try
            {
                this.highlightedCode = SyntaxHighlighter.Parse(this.ExampleInfo.Code);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                this.highlightedCode = this.ExampleInfo.Code;
                this.showRaw = true;
            }
        }

        public void Draw(bool drawCodeExample)
        {
            if (this.ExampleInfo.Description != null)
            {
                SirenixEditorGUI.BeginBox();
                GUILayout.Label(this.ExampleInfo.Description, SirenixGUIStyles.MultiLineLabel);
                SirenixEditorGUI.EndBox();
            }

            SirenixEditorGUI.BeginBox();
            {
                this.tree = this.tree ?? PropertyTree.Create(this.ExampleInfo.PreviewObject);
                this.tree.Draw(false);
            }
            SirenixEditorGUI.EndBox();

            if (drawCodeExample && this.ExampleInfo.Code != null)
            {
                GUILayout.Space(15);

                Rect rect = SirenixEditorGUI.BeginToolbarBox();
                SirenixEditorGUI.DrawSolidRect(rect.HorizontalPadding(1), SyntaxHighlighter.BackgroundColor);

                SirenixEditorGUI.BeginToolbarBoxHeader();
                {
                    if (SirenixEditorGUI.ToolbarButton(this.showRaw ? "Highlighted" : "Raw"))
                    {
                        this.showRaw = !this.showRaw;
                    }

                    GUILayout.FlexibleSpace();
                    if (SirenixEditorGUI.ToolbarButton("Copy"))
                    {
                        Clipboard.Copy(this.ExampleInfo.Code);
                    }
                }
                SirenixEditorGUI.EndToolbarBoxHeader();

                if (codeStyle == null)
                {
                    codeStyle = new GUIStyle(SirenixGUIStyles.MultiLineLabel);
                    codeStyle.normal.textColor = SyntaxHighlighter.TextColor;
                    codeStyle.active.textColor = SyntaxHighlighter.TextColor;
                    codeStyle.focused.textColor = SyntaxHighlighter.TextColor;
                    codeStyle.wordWrap = false;
                }

                GUIContent codeContent = GUIHelper.TempContent(this.showRaw ? this.ExampleInfo.Code : this.highlightedCode);
                Vector2 size = codeStyle.CalcSize(codeContent);

                this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, true, false, GUI.skin.horizontalScrollbar, GUIStyle.none, GUILayoutOptions.MinHeight(size.y + 20));
                var codeRect = GUILayoutUtility.GetRect(size.x + 50, size.y);

                if (this.showRaw)
                {
                    EditorGUI.SelectableLabel(codeRect, this.ExampleInfo.Code, codeStyle);
                }
                else
                {
                    GUI.Label(codeRect, codeContent, codeStyle);
                }

                GUILayout.EndScrollView();

                SirenixEditorGUI.EndToolbarBox();
            }
        }
    }
}
#endif