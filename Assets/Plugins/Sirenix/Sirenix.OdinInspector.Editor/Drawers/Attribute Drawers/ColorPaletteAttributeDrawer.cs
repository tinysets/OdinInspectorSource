#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="ColorPaletteAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="ColorPaletteAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Linq;

    /// <summary>
    /// Odin drawer for <see cref="ColorPaletteAttribute"/>.
    /// </summary>
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public sealed class ColorPaletteAttributeDrawer : OdinAttributeDrawer<ColorPaletteAttribute, Color>
    {
        private int PaletteIndex;
        private string CurrentName;
        private LocalPersistentContext<string> PersistentName;
        private bool ShowAlpha;
        private string[] Names;
        private StringMemberHelper NameGetter;

        /// <summary>
        /// Initializes the drawer.
        /// </summary>
        protected override void Initialize()
        {
            this.PaletteIndex = 0;
            this.CurrentName = this.Attribute.PaletteName;
            this.ShowAlpha = this.Attribute.ShowAlpha;
            this.Names = ColorPaletteManager.Instance.ColorPalettes.Select(x => x.Name).ToArray();

            if (this.Attribute.PaletteName == null)
            {
                this.PersistentName = this.ValueEntry.Context.GetPersistent<string>(this, "ColorPaletteName", null);
                var list = this.Names.ToList();
                this.CurrentName = this.PersistentName.Value;

                if (this.CurrentName != null && list.Contains(this.CurrentName))
                {
                    this.PaletteIndex = list.IndexOf(this.CurrentName);
                }
            }
            else
            {
                this.NameGetter = new StringMemberHelper(this.Property, this.Attribute.PaletteName);
            }

        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var entry = this.ValueEntry;
            var attribute = this.Attribute;

            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                var hideLabel = label == null;
                if (hideLabel == false)
                {
                    GUILayout.Label(label, GUILayoutOptions.Width(GUIHelper.BetterLabelWidth - 4).ExpandWidth(false));
                }
                else
                {
                    GUILayout.Space(5);
                }

                //var colorPaletDropDown = entry.Context.Get(this, "colorPalette", 0);
                //var currentName = entry.Context.Get(this, "currentName", attribute.PaletteName);
                //var showAlpha = entry.Context.Get(this, "showAlpha", attribute.ShowAlpha);
                //var names = ColorPaletteManager.Instance.GetColorPaletteNames();

                ColorPalette colorPalette;
                var rect = EditorGUILayout.BeginHorizontal();
                {
                    rect.x -= 3;
                    rect.width = 25;

                    entry.SmartValue = SirenixEditorGUI.DrawColorField(rect, entry.SmartValue, false, this.ShowAlpha);
                    bool openInEditorShown = false;
                    GUILayout.Space(28);
                    SirenixEditorGUI.BeginInlineBox();
                    {
                        if (attribute.PaletteName == null || ColorPaletteManager.Instance.ShowPaletteName)
                        {
                            SirenixEditorGUI.BeginToolbarBoxHeader();
                            {
                                if (attribute.PaletteName == null)
                                {
                                    var newValue = EditorGUILayout.Popup(this.PaletteIndex, this.Names, GUILayoutOptions.ExpandWidth(true));
                                    if (this.PaletteIndex != newValue)
                                    {
                                        this.PaletteIndex = newValue;
                                        this.CurrentName = this.Names[newValue];
                                        this.PersistentName.Value = this.CurrentName;
                                        GUIHelper.RemoveFocusControl();
                                    }
                                }
                                else
                                {
                                    GUILayout.Label(this.CurrentName);
                                    GUILayout.FlexibleSpace();
                                }
                                openInEditorShown = true;
                                if (SirenixEditorGUI.IconButton(EditorIcons.SettingsCog))
                                {
                                    ColorPaletteManager.Instance.OpenInEditor();
                                }
                            }
                            SirenixEditorGUI.EndToolbarBoxHeader();
                        }

                        if (attribute.PaletteName == null)
                        {
                            colorPalette = ColorPaletteManager.Instance.ColorPalettes.FirstOrDefault(x => x.Name == this.Names[this.PaletteIndex]);
                        }
                        else
                        {
                            colorPalette = ColorPaletteManager.Instance.ColorPalettes.FirstOrDefault(x => x.Name == this.NameGetter.GetString(entry));
                        }

                        if (colorPalette == null)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (attribute.PaletteName != null)
                                {
                                    if (GUILayout.Button("Create color palette: " + this.NameGetter.GetString(entry)))
                                    {
                                        ColorPaletteManager.Instance.ColorPalettes.Add(new ColorPalette() { Name = this.NameGetter.GetString(entry) });
                                        ColorPaletteManager.Instance.OpenInEditor();
                                    }
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            this.CurrentName = colorPalette.Name;
                            this.ShowAlpha = attribute.ShowAlpha && colorPalette.ShowAlpha;
                            if (openInEditorShown == false)
                            {
                                GUILayout.BeginHorizontal();
                            }
                            var color = entry.SmartValue;
                            var stretch = ColorPaletteManager.Instance.StretchPalette;
                            var size = ColorPaletteManager.Instance.SwatchSize;
                            var margin = ColorPaletteManager.Instance.SwatchSpacing;
                            if (DrawColorPaletteColorPicker(entry, colorPalette, ref color, colorPalette.ShowAlpha, stretch, size, 20, margin))
                            {
                                entry.SmartValue = color;
                                //entry.ApplyChanges();
                            }
                            if (openInEditorShown == false)
                            {
                                GUILayout.Space(4);
                                if (SirenixEditorGUI.IconButton(EditorIcons.SettingsCog))
                                {
                                    ColorPaletteManager.Instance.OpenInEditor();
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    SirenixEditorGUI.EndInlineBox();
                }
                EditorGUILayout.EndHorizontal();
            }

            SirenixEditorGUI.EndIndentedHorizontal();
        }

        internal static bool DrawColorPaletteColorPicker(object key, ColorPalette colorPalette, ref Color color, bool drawAlpha, bool stretchPalette, float width = 20, float height = 20, float margin = 0)
        {
            bool result = false;

            var rect = SirenixEditorGUI.BeginHorizontalAutoScrollBox(key, GUILayoutOptions.ExpandWidth(true).ExpandHeight(false));
            {
                if (stretchPalette)
                {
                    rect.width -= margin * colorPalette.Colors.Count - margin;
                    width = Mathf.Max(width, rect.width / colorPalette.Colors.Count);
                }

                bool isMouseDown = Event.current.type == EventType.MouseDown;
                var innerRect = GUILayoutUtility.GetRect((width + margin) * colorPalette.Colors.Count, height, GUIStyle.none);
                float spacing = width + margin;
                var cellRect = innerRect;
                cellRect.width = width;

                for (int i = 0; i < colorPalette.Colors.Count; i++)
                {
                    cellRect.x = spacing * i;

                    if (drawAlpha)
                    {
                        EditorGUIUtility.DrawColorSwatch(cellRect, colorPalette.Colors[i]);
                    }
                    else
                    {
                        var c = colorPalette.Colors[i];
                        c.a = 1;
                        SirenixEditorGUI.DrawSolidRect(cellRect, c);
                    }

                    if (isMouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        color = colorPalette.Colors[i];
                        result = true;
                        GUI.changed = true;
                        Event.current.Use();
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalAutoScrollBox();
            return result;
        }
    }
}
#endif