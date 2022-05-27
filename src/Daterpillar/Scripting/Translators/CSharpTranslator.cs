using Acklann.Daterpillar.Modeling;
using System;
using System.Reflection;

namespace Acklann.Daterpillar.Scripting.Translators
{
    /// <summary>
    /// Provide methods that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to a clr type name.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class CSharpTranslator : TranslatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpTranslator"/> class.
        /// </summary>
        public CSharpTranslator() : base()
        {
            TypeMap[BOOL.ToLowerInvariant()] = "bool";
            TypeMap[BLOB.ToLowerInvariant()] = "string";
            TypeMap[CHAR.ToLowerInvariant()] = "string";
            TypeMap[TEXT.ToLowerInvariant()] = "string";
            TypeMap[VARCHAR.ToLowerInvariant()] = "string";
            TypeMap[INT.ToLowerInvariant()] = "int";
            TypeMap[BIGINT.ToLowerInvariant()] = "long";
            TypeMap[MEDIUMINT.ToLowerInvariant()] = "int";
            TypeMap[SMALLINT.ToLowerInvariant()] = "int";
            TypeMap[TINYINT.ToLowerInvariant()] = "int";
            TypeMap[FLOAT.ToLowerInvariant()] = "float";
            TypeMap[DOUBLE.ToLowerInvariant()] = "double";
            TypeMap[DECIMAL.ToLowerInvariant()] = "decimal";
            TypeMap[DATE.ToLowerInvariant()] = "DateTime";
            TypeMap[TIME.ToLowerInvariant()] = "DateTime";
            TypeMap[DATETIME.ToLowerInvariant()] = "DateTime";
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
            if (clrType.IsGenericType && Nullable.GetUnderlyingType(clrType) != null) clrType = clrType.GetGenericArguments()[0];

            if (clrType.GetTypeInfo().IsEnum) return new DataType(INT);
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
                        return new DataType(DECIMAL, 8, 2);

                    case nameof(Char):
                        return new DataType(CHAR, 1);

                    case nameof(String):
                        return new DataType(VARCHAR);

                    case nameof(TimeSpan):
                    case nameof(DateTime):
                    case nameof(DateTimeOffset):
                        return new DataType(DATETIME);

                    default:
                        throw new ArgumentException($"'{clrType.Name}' is not mapped to a {nameof(DataType)}.", paramName: nameof(clrType));
                }
        }

        /// <summary>
        /// Maps the specified <see cref="Type"/> to a <see cref="DataType"/>
        /// </summary>
        /// <param name="clrType">The clr type.</param>
        /// <param name="dataType">The resulting <see cref="DataType"/>.</param>
        /// <returns><c>true</c> if the conversion was successul; <c>false</c> otherwise.</returns>
        public static bool TryGetDataType(Type clrType, out DataType dataType)
        {
            try
            {
                dataType = GetDataType(clrType);
                return true;
            }
            catch { }

            dataType = new DataType();
            return false;
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a clr type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        public override string ConvertToString(DataType dataType)
        {
            return TypeMap[dataType.Name.ToLowerInvariant()];
        }
    }
}