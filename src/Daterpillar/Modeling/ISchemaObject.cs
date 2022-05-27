using System;

namespace Acklann.Daterpillar.Modeling
{
    /// <summary>
    /// Represents a SQL object (<see cref="Table"/>, <see cref="ForeignKey"/> and <see cref="Index"/>).
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    public interface ISchemaObject : ICloneable
    {
        /// <summary>
        /// Gets the object's name.
        /// </summary>
        /// <returns></returns>
        string GetName();
    }
}