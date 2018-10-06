using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
    public interface ITypeResolver
    {
        string Escape(string objectName);

        /// <summary>
        /// Maps the specified <see cref="DataType"/> to another language's type name.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        string GetTypeName(DataType dataType);

        string GetActionName(ReferentialAction action);
    }
}