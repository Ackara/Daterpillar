﻿namespace Ackara.Daterpillar.TypeResolvers
{
    /// <summary>
    /// Defines a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd" TypeName to another language's type name.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        /// Maps the specified <see cref="DataType"/> to another language's type name.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        string GetTypeName(DataType dataType);
    }
}