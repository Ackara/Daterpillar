using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Equality
{
    internal class TableNameComparer : IEqualityComparer<Table>
    {
        public bool Equals(Table x, Table y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Table obj)
        {
            return obj.GetHashCode();
        }
    }
}