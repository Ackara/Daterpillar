using System;
using System.Reflection;

namespace Acklann.Daterpillar.Serialization
{
    internal static class Helper
    {
        public static string GetTableName(this Type type)
        {
            if (type.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)) is System.ComponentModel.DataAnnotations.Schema.TableAttribute attr1 && !string.IsNullOrEmpty(attr1.Name))
            {
                return attr1.Name;
            }
            else if (type.GetCustomAttribute(typeof(Modeling.Attributes.TableAttribute)) is Modeling.Attributes.TableAttribute attr2 && !string.IsNullOrEmpty(attr2.Name))
            {
                return attr2.Name;
            }
            else
            {
                return type.Name;
            }
        }

        public static string GetColumnName(this MemberInfo member)
        {
            if (member.GetCustomAttribute(typeof(Modeling.Attributes.ColumnAttribute)) is Modeling.Attributes.ColumnAttribute attribute1 && !string.IsNullOrEmpty(attribute1.Name))
            {
                return attribute1.Name;
            }
            else if (member.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) is System.ComponentModel.DataAnnotations.Schema.ColumnAttribute attribute2 && !string.IsNullOrEmpty(attribute2.Name))
            {
                return attribute2.Name;
            }
            else
            {
                return member.Name;
            }
        }

        public static int GetMaxLength(this MemberInfo member)
        {
            return member.GetCustomAttribute<System.ComponentModel.DataAnnotations.StringLengthAttribute>()?.MaximumLength ?? member.GetCustomAttribute<System.ComponentModel.DataAnnotations.MaxLengthAttribute>()?.Length ?? 0;
        }
    }
}