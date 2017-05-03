using System;
using System.Reflection;

namespace Ackara.Daterpillar.TypeResolvers
{
    /// <summary>
    /// Provide methods that maps a <see cref="http://static.acklann.com/schema/v2/daterpillar.xsd"/> TypeName to a clr type name.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class CSharpTypeResolver : TypeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTypeResolver"/> class.
        /// </summary>
        public CSharpTypeResolver() : base()
        {
            TypeMap[BOOL.ToLower()] = "bool";
            TypeMap[BLOB.ToLower()] = "string";
            TypeMap[CHAR.ToLower()] = "string";
            TypeMap[TEXT.ToLower()] = "string";
            TypeMap[VARCHAR.ToLower()] = "string";
            TypeMap[INT.ToLower()] = "int";
            TypeMap[BIGINT.ToLower()] = "long";
            TypeMap[MEDIUMINT.ToLower()] = "int";
            TypeMap[SMALLINT.ToLower()] = "int";
            TypeMap[TINYINT.ToLower()] = "int";
            TypeMap[FLOAT.ToLower()] = "float";
            TypeMap[DOUBLE.ToLower()] = "double";
            TypeMap[DECIMAL.ToLower()] = "decimal";
            TypeMap[DATE.ToLower()] = "DateTime";
            TypeMap[TIME.ToLower()] = "DateTime";
            TypeMap[DATETIME.ToLower()] = "DateTime";
        }

        /// <summary>
        /// Maps the specified <see cref="Type" /> to a <see cref="DataType"/>.
        /// </summary>
        /// <param name="clrType">The clr type.</param>
        /// <returns>DataType.</returns>
        /// <exception cref="System.NullReferenceException">clrType</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public static DataType GetDataType(Type clrType)
        {
            if (clrType == null) throw new NullReferenceException($"The {nameof(clrType)} parameter is null. You cannot pass a null arg to this method.");
            else if (clrType.GetTypeInfo().IsEnum) return new DataType(INT);
            else switch (clrType.Name)
                {
                    case nameof(Boolean):
                        return new DataType(BOOL);

                    case nameof(SByte):
                        return new DataType(SMALLINT);

                    case nameof(Int16):
                        return new DataType(MEDIUMINT);

                    case nameof(Int32):
                        return new DataType(INT);

                    case nameof(Int64):
                        return new DataType(BIGINT);

                    case nameof(Single):
                        return new DataType(FLOAT);

                    case nameof(Double):
                        return new DataType(DOUBLE);

                    case nameof(Decimal):
                        return new DataType(DECIMAL, 10, 2);

                    case nameof(Char):
                        return new DataType(CHAR);

                    case nameof(String):
                        return new DataType(VARCHAR, 64);

                    case nameof(TimeSpan):
                    case nameof(DateTime):
                        return new DataType(DATETIME);

                    default:
                        throw new ArgumentException($"'{clrType.Name}' is not mapped to a {nameof(DataType)}.", paramName: nameof(clrType));
                }
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a clr type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        public override string GetTypeName(DataType dataType)
        {
            return TypeMap[dataType.Name.ToLower()];
        }
    }
}