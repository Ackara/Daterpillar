namespace Gigobyte.Daterpillar.TextTransformation
{
    public interface ITypeNameResolver
    {
        string GetName(DataType dataType);
    }
}