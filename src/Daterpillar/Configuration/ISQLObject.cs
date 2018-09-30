using System;

namespace Acklann.Daterpillar.Configuration
{
    /// <summary>
    /// Defines the minimal properties of the SQL Object.
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

        /// <summary>
        /// Gets or sets the parent object.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        //ISQLObject Parent { get; set; }
    }
}