using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
    public partial class TypeResolverBase
    {
        public virtual string ExpandVariables(string name) => name;

        /// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>The escaped name.</returns>
		public virtual string Escape(string objectName) => objectName;

        public virtual string GetActionName(ReferentialAction action)
        {
            switch (action)
            {
                default:
                case ReferentialAction.Cascade:
                    return "CASCADE";

                case ReferentialAction.NoAction:
                    return "NO ACTION";

                case ReferentialAction.Restrict:
                    return "RESTRICT";

                case ReferentialAction.SetNull:
                    return "SET NULL";

                case ReferentialAction.SetDefault:
                    return "SET DEFAULT";
            }
        }

        /// <summary>
        /// Maps the specified <see cref="DataType"/>.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        public abstract string GetTypeName(DataType dataType);

        internal struct Placeholder
        {
            public const string NOW = @"(?i)\$\(now\)";
        }
    }
}