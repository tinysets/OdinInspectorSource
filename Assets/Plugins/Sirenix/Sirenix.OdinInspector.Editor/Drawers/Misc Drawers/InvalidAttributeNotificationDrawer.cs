#if UNITY_EDITOR
//-----------------------------------------------------------------------// <copyright file="InvalidAttributeNotificationDrawer.cs" company="Sirenix IVS"> // Copyright (c) Sirenix IVS. All rights reserved.// </copyright>//-----------------------------------------------------------------------
//-----------------------------------------------------------------------
// <copyright file="DrawerChain.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Text;
    using UnityEditor;
    using UnityEngine;

    [DrawerPriority(-1, -1, -1), OdinDontRegisterAttribute]
    public class InvalidAttributeNotificationDrawer<TInvalidAttribute> : OdinDrawer
    {
        private class Context
        {
            public string ErrorMessage;
            public string ValidTypeMessage;
            public bool IsFolded = true;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            var property = this.Property;
            var context = property.Context.Get(this, "context", (Context)null);

            if (context.Value == null)
            {
                context.Value = new Context();

                var sb = new StringBuilder("Attribute '")
                                .Append(typeof(TInvalidAttribute).GetNiceName())
                                .Append("' cannot be put on property '")
                                .Append(property.Name)
                                .Append("'");

                if (property.ValueEntry != null)
                {
                    sb.Append(" of base type '")
                      .Append(property.ValueEntry.BaseValueType.GetNiceName())
                      .Append("'");
                }
                sb.Append('.');

                context.Value.ErrorMessage = sb.ToString();

                sb.Length = 0;

                var validTypes = DrawerUtilities.InvalidAttributeTargetUtility.GetValidTargets(typeof(TInvalidAttribute));
                sb.AppendLine("The following types are valid:");
                sb.AppendLine();

                for (int i = 0; i < validTypes.Count; i++)
                {
                    var type = validTypes[i];
                    sb.Append(type.GetNiceName());

                    if (type.IsGenericParameter)
                    {
                        sb.Append(" ")
                          .Append(type.GetGenericParameterConstraintsString(useFullTypeNames: true));
                    }

                    sb.AppendLine();
                }

                sb.Append("Supported collections where the element type is any of the above types");

                context.Value.ValidTypeMessage = sb.ToString();
            }

            context.Value.IsFolded = SirenixEditorGUI.DetailedMessageBox(context.Value.ErrorMessage, context.Value.ValidTypeMessage, MessageType.Error, context.Value.IsFolded);

            this.CallNextDrawer(label);
        }
    }
}
#endif