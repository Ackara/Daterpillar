using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acklann.Daterpillar.Scripting.Writers
{
	public struct ColumnValuePair
	{
        public string ColumnName { get; set; }

        public bool Value { get; set; }
    }
}