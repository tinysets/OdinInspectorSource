#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinUnityContextMenuItems.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor.Windows
{
    internal class OdinUnityContextMenuItems
    {
        const int Group0 = -1000;
        const int Group1 = 10000;
        const int Group2 = 100000;
        const int Group3 = 1000000;

        [MenuItem("Tools/Odin Inspector/Getting Started", priority = Group0 + 1)]
        private static void OpenGettingStarted()
        {
            OdinGettingStartedWindow.ShowWindow();
        }

        [MenuItem("Tools/Odin Inspector/Static Inspector", priority = Group1 + 1)]
        private static void OpenStaticInspector()
        {
            StaticInspectorWindow.ShowWindow();
        }

        [MenuItem("Tools/Odin Inspector/Scene Validator", priority = Group1 + 2)]
        private static void OpenOdinSceneValidator()
        {
            OdinSceneValidatorWindow.OpenWindow();
        }

        [MenuItem("Tools/Odin Inspector/Serialization Debugger", priority = Group1 + 3)]
        public static void ShowSerializationDebugger()
        {
            SerializationDebuggerWindow.ShowWindow();
        }

        [MenuItem("Tools/Odin Inspector/Preferences", priority = Group2 + 1)]
        public static void OpenSirenixPreferences()
        {
            SirenixPreferencesWindow.OpenSirenixPreferences();
        }

        [MenuItem("Tools/Odin Inspector/Help/Discord", priority = Group3 + 1)]
        private static void Discord()
        {
            Application.OpenURL("https://discord.gg/WTYJEra");
        }

        [MenuItem("Tools/Odin Inspector/Help/Report An Issue", priority = Group3 + 2)]
        private static void ReportAnIssue()
        {
            Application.OpenURL("https://bitbucket.org/sirenix/odin-inspector/issues");
        }

        [MenuItem("Tools/Odin Inspector/Help/Contact", priority = Group3 + 3)]
        private static void Contact()
        {
            Application.OpenURL("http://sirenix.net/support");
        }

        [MenuItem("Tools/Odin Inspector/Release Notes", priority = Group3 + 4)]
        private static void OpenReleaseNotes()
        {
            Application.OpenURL("http://sirenix.net/odininspector/releasenotes");
        }

        [MenuItem("Tools/Odin Inspector/About", priority = Group3 + 5)]
        private static void ShowAboutOdinInspector()
        {
#if ODIN_TRIAL_VERSION
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(465f).AlignMiddle(175f);
#else
            var rect = GUIHelper.GetEditorWindowRect().AlignCenter(465f).AlignMiddle(135f);
#endif
            var w = OdinInspectorAboutWindow.GetWindowWithRect<OdinInspectorAboutWindow>(rect, true, "Odin Inspector & Serializer");
            w.ShowUtility();
        }

#if ODIN_TRIAL_VERSION
		[MenuItem("Tools/Odin Inspector/Get it here", priority = -100000)]
		private static void OpenStoreLink()
		{
			Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/89041");
		}
#endif

        [MenuItem("CONTEXT/MonoBehaviour/Debug Serialization")]
        private static void ComponentContextMenuItem(MenuCommand menuCommand)
        {
            SerializationDebuggerWindow.ShowWindow(menuCommand.context.GetType());
        }
    }
}
#endif