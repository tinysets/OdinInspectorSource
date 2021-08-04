#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="TabGroupAttributeDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="TabGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System.Collections.Generic;
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="TabGroupAttribute"/>
    /// </summary>
    /// <seealso cref="TabGroupAttribute"/>
    public class TabGroupAttributeDrawer : OdinGroupDrawer<TabGroupAttribute>
    {
        private class Tab
        {
            public string TabName;
            public List<InspectorProperty> InspectorProperties = new List<InspectorProperty>();
            public StringMemberHelper Title;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var attribute = this.Attribute;

            var tabGroup = SirenixEditorGUI.CreateAnimatedTabGroup(property);
            tabGroup.AnimationSpeed = 1 / SirenixEditorGUI.TabPageSlideAnimationDuration;
            tabGroup.FixedHeight = attribute.UseFixedHeight;

            LocalPersistentContext<int> persistentPageContext = property.Context.GetPersistent(this, "CurrentPage", 0);

            bool setPersistentPage = false;
            PropertyContext<List<Tab>> tabs;
            if (property.Context.Get(this, "Tabs", out tabs))
            {
                setPersistentPage = true;
                tabs.Value = new List<Tab>();
                var addLastTabs = new List<Tab>();

                for (int j = 0; j < property.Children.Count; j++)
                {
                    var child = property.Children[j];

                    bool added = false;

                    if (child.Info.PropertyType == PropertyType.Group)
                    {
                        var attrType = child.GetAttribute<PropertyGroupAttribute>().GetType();

                        if (attrType.IsNested && attrType.DeclaringType == typeof(TabGroupAttribute))
                        {
                            // This is a tab subgroup; add all its children to a tab for that subgroup

                            Tab tab = new Tab();

                            tab.TabName = child.NiceName;
                            tab.Title = new StringMemberHelper(property, child.Name.TrimStart('#'));
                            for (int i = 0; i < child.Children.Count; i++)
                            {
                                tab.InspectorProperties.Add(child.Children[i]);
                            }

                            tabs.Value.Add(tab);
                            added = true;
                        }
                    }

                    if (!added)
                    {
                        // This is a group member of the tab group itself, so it gets its own tab

                        Tab tab = new Tab();

                        tab.TabName = child.NiceName;
                        tab.Title = new StringMemberHelper(property, child.Name.TrimStart('#'));
                        tab.InspectorProperties.Add(child);

                        addLastTabs.Add(tab);
                    }
                }

                foreach (var tab in addLastTabs)
                {
                    tabs.Value.Add(tab);
                }
            }

            for (int i = 0; i < tabs.Value.Count; i++)
            {
                tabGroup.RegisterTab(tabs.Value[i].TabName);
            }

            if (setPersistentPage)
            {
                if (persistentPageContext.Value >= tabs.Value.Count || persistentPageContext.Value < 0)
                {
                    persistentPageContext.Value = 0;
                }

                tabGroup.SetCurrentPage(tabGroup.RegisterTab(tabs.Value[persistentPageContext.Value].TabName));
            }

            SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            tabGroup.BeginGroup(true, attribute.Paddingless ? GUIStyle.none : null);

            for (int i = 0; i < tabs.Value.Count; i++)
            {
                var page = tabGroup.RegisterTab(tabs.Value[i].TabName);
                page.Title = tabs.Value[i].Title.GetString(property);
                if (page.BeginPage())
                {
                    persistentPageContext.Value = i;
                    int pageCount = tabs.Value[i].InspectorProperties.Count;
                    for (int j = 0; j < pageCount; j++)
                    {
                        var child = tabs.Value[i].InspectorProperties[j];
                        child.Update(); // Since the property is not fetched through the property system, ensure it's updated before drawing it.
                        child.Draw(child.Label);
                    }
                }
                page.EndPage();
            }

            tabGroup.EndGroup();
            SirenixEditorGUI.EndIndentedVertical();
        }
    }
}
#endif