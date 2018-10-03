using System;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Represents a SQL object (<see cref="Table"/>, <see cref="ForeignKey"/> and <see cref="Index"/>).
    /// </summary>
    /// <seealso cref="System.ICloneable" />
    public interface ISQLObject : ICloneable
    {
        /// <summary>
        /// Gets or sets the object's name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }
    }
}