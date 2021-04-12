using System.Reflection;

namespace Acklann.Daterpillar.Serialization
{
    internal static class Helper
    {
        static Helper()
        {
            System.ComponentModel.DataAnnotations.Schema.TableAttribute ta;
            System.ComponentModel.DataAnnotations.Schema.ColumnAttribute f;
            System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute fka;
            System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute nma;
            System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedAttribute dga;
            System.ComponentModel.DataAnnotations.DataTypeAttribute dta;
            System.ComponentModel.DataAnnotations.DisplayAttribute da;
        }

        public static int GetMaxLength(this MemberInfo member)
        {
            return member.GetCustomAttribute<System.ComponentModel.DataAnnotations.MaxLengthAttribute>()?.Length ?? 0;
        }

    }
}