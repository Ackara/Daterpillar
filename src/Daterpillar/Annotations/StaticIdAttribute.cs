using System;

namespace Acklann.Daterpillar.Annotations
{
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = false, Inherited = true)]
    public class StaticIdAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticIdAttribute"/> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        public StaticIdAttribute(object id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            Id = id.ToString();
        }

        /// <summary>
        /// The unique identifier
        /// </summary>
        public readonly string Id;

        internal const string XName = "suid";
    }
}