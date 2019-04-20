using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Translators
{
    /// <summary>
    /// Provides methods for translating SQL name/type to another SQL dialect.
    /// </summary>
    public interface ITranslator
    {
        /// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns></returns>
        string Escape(string objectName);

        /// <summary>
        /// Replaces the name of each placeholder variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">A string containing the names of zero or more environment variables.</param>
        /// <returns></returns>
        string ExpandVariables(string name);

        /// <summary>
        /// Converts the <see cref="DataType"/> value to its equivalent SQL representation.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        string ConvertToString(DataType dataType);

        /// <summary>
        /// Converts the <see cref="ReferentialAction"/> value to its equivalent SQL representation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        string ConvertToString(ReferentialAction action);
    }
}