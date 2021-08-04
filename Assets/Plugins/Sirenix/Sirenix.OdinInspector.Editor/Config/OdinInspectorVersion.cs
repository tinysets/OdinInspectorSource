#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="OdinInspectorVersion.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="Odin Version.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using System.Linq;

    /// <summary>S
    /// Installed Odin Inspector Version Info.
    /// </summary>
    public static class OdinInspectorVersion
    {
        private static string version;

        /// <summary>
        /// Gets the current running version of Odin Inspector.
        /// </summary>
        public static string Version
        {
            get
            {
                if (version == null)
                {
                    var attribute = typeof(InspectorConfig).Assembly.GetCustomAttributes(typeof(SirenixBuildNameAttribute), true).FirstOrDefault() as SirenixBuildNameAttribute;
                    if (attribute == null)
                    {
                        return "Source Code Mode";
                    }
                    version = attribute.BuildName;
                }

                return version;
            }
        }
    }
}
#endif