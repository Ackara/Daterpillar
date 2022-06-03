using System;
using System.Data;

namespace Acklann.Daterpillar.Scripting.Translators
{
    public static class DbTypeConverter
    {
        public static DbType ToDbType(this object value)
        {
            switch (value)
            {
                case long: return DbType.Int64;
                case ulong: return DbType.UInt64;
                case uint: return DbType.UInt32;
                case int: return DbType.Int32;
                case ushort: return DbType.UInt16;
                case short: return DbType.Int16;
                case sbyte: return DbType.SByte;
                case byte: return DbType.Byte;
                case byte[]: return DbType.Binary;
                case decimal: return DbType.Decimal;
                case double: return DbType.Double;
                case float: return DbType.Single;
                case bool: return DbType.Boolean;

                case char: return DbType.StringFixedLength;
                case string: return DbType.String;
                case Guid: return DbType.Guid;

                case DateTime: return DbType.DateTime;
                case DateTimeOffset: return DbType.DateTimeOffset;

                default: return DbType.AnsiString;
            }
        }
    }
}