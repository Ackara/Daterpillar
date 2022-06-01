using Acklann.Daterpillar.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    internal static class TypeInfoExtensions
    {
        public static string GetTableName(this Type type)
        {
            if (type.GetCustomAttribute(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)) is System.ComponentModel.DataAnnotations.Schema.TableAttribute attr1 && !string.IsNullOrEmpty(attr1.Name))
            {
                return attr1.Name;
            }
            else if (type.GetCustomAttribute(typeof(TableAttribute)) is TableAttribute attr2 && !string.IsNullOrEmpty(attr2.Name))
            {
                return attr2.Name;
            }
            else
            {
                return type.Name;
            }
        }

        public static IEnumerable<MemberInfo> GetColumns(this Type type)
        {
            if (type.GetCustomAttribute<TableAttribute>()?.OptInProperties ?? false)
            {
                return (from property in type.GetProperties()
                        let not_explictly_defined = property.IsDefined(typeof(ColumnAttribute)) == false && property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) == false
                        where
                           /*not ignored*/property.IsDefined(typeof(SqlIgnoreAttribute)) == false && property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) == false
                           &&
                           (not_explictly_defined && property.CanWrite == false) == false
                        select (MemberInfo)property)

                     .Concat

                     (from field in type.GetRuntimeFields()
                      let isExplict = field.IsDefined(typeof(ColumnAttribute)) || field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute))
                      where
                         field.IsDefined(typeof(SqlIgnoreAttribute)) == false && field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) == false
                         &&
                         isExplict
                      select field);
            }
            else
            {
                return (from property in type.GetProperties()
                        where
                            (property.IsDefined(typeof(ColumnAttribute)) || property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)))
                            &&
                            !property.IsDefined(typeof(SqlIgnoreAttribute))
                            &&
                            !property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute))
                        select (MemberInfo)property)

                        .Concat

                        (from field in type.GetRuntimeFields()
                         where
                            (field.IsDefined(typeof(ColumnAttribute)) || field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)))
                            &&
                            !field.IsDefined(typeof(SqlIgnoreAttribute))
                            &&
                            !field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute))
                         select field
                    );
            }
        }

        public static string GetColumnName(this MemberInfo member)
        {
            if (member.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute attribute1 && !string.IsNullOrEmpty(attribute1.Name))
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

        public static IEnumerable<string> GetColumnNames(this MemberInfo member)
        {
            if (member.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)))
                foreach (var attribute in member.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>())
                {
                    yield return attribute.Name ?? member.Name;
                }
            else if (member.IsDefined(typeof(ColumnAttribute)))
                foreach (var attribute in member.GetCustomAttributes<ColumnAttribute>())
                {
                    yield return attribute.Name ?? member.Name;
                }
            else
            {
                yield return member.Name;
            }
        }

        public static bool IsAutoColumn(this MemberInfo member)
        {
            foreach (var attribute in member.GetCustomAttributes<ColumnAttribute>())
                if (attribute.AutoIncrement)
                {
                    return true;
                }

            return false;
        }

        public static bool IsKey(this MemberInfo member)
        {
            if (member.IsDefined(typeof(KeyAttribute))) return true;

            foreach (var item in member.GetCustomAttributes<IndexAttribute>())
                if (item.Type == IndexType.PrimaryKey)
                {
                    return true;
                }

            return false;
        }

        public static int GetMaxLength(this MemberInfo member)
        {
            return member.GetCustomAttribute<System.ComponentModel.DataAnnotations.StringLengthAttribute>()?.MaximumLength ?? member.GetCustomAttribute<System.ComponentModel.DataAnnotations.MaxLengthAttribute>()?.Length ?? 0;
        }
    }
}