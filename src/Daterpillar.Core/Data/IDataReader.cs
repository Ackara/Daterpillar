using System;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Provide methods to create an object mapped to an database table.
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        object CreateInstance(Type returnType, object data);
    }
}