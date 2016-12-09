﻿namespace Acklann.Daterpillar.TextTransformation
{
    public sealed class CSharpTypeNameResolver : TypeNameResolverBase
    {
        public CSharpTypeNameResolver()
        {
            TypeNames[BOOL] = "bool";
            TypeNames[BLOB] = "string";
            TypeNames[CHAR] = "string";
            TypeNames[TEXT] = "string";
            TypeNames[VARCHAR] = "string";
            TypeNames[INT] = "int";
            TypeNames[BIGINT] = "long";
            TypeNames[MEDIUMINT] = "int";
            TypeNames[SMALLINT] = "int";
            TypeNames[TINYINT] = "int";
            TypeNames[FLOAT] = "float";
            TypeNames[DOUBLE] = "double";
            TypeNames[DECIMAL] = "decimal";
            TypeNames[DATE] = "DateTime";
            TypeNames[TIME] = "DateTime";
            TypeNames[DATETIME] = "DateTime";
        }

        public override string GetName(DataType dataType)
        {
            return TypeNames[dataType.Name.ToLower()];
        }
    }
}