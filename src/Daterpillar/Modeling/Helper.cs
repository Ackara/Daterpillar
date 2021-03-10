using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    internal static class Helper
    {
        public static IEnumerable<PropertyInfo> GetColumns(this Type model)
        {
            return from t in model.GetProperties()
                   where t.IsDefined(typeof(Attributes.ColumnAttribute))
                   select t;
        }
    }
}