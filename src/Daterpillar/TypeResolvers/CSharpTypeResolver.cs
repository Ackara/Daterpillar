namespace Ackara.Daterpillar.TypeResolvers
{
    public class CSharpTypeResolver : TypeResolverBase
    {
        public CSharpTypeResolver() : base()
        {
            TypeMap[BOOL] = "bool";
            TypeMap[BLOB] = "string";
            TypeMap[CHAR] = "string";
            TypeMap[TEXT] = "string";
            TypeMap[VARCHAR] = "string";
            TypeMap[INT] = "int";
            TypeMap[BIGINT] = "long";
            TypeMap[MEDIUMINT] = "int";
            TypeMap[SMALLINT] = "int";
            TypeMap[TINYINT] = "int";
            TypeMap[FLOAT] = "float";
            TypeMap[DOUBLE] = "double";
            TypeMap[DECIMAL] = "decimal";
            TypeMap[DATE] = "DateTime";
            TypeMap[TIME] = "DateTime";
            TypeMap[DATETIME] = "DateTime";
        }

        public override string GetTypeName(DataType dataType)
        {
            return TypeMap[dataType.Name.ToLower()];
        }
    }
}