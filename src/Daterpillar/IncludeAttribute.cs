using System;

namespace Acklann.Daterpillar
{
    [AttributeUsage((AttributeTargets.Assembly), AllowMultiple = false, Inherited = false)]
    public sealed class IncludeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeAttribute"/> class.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        public IncludeAttribute(string relativePath)
        {
            Path = relativePath;
        }

        public readonly string Path;
    }
}