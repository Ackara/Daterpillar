using System;

namespace Gigobyte.Daterpillar.Annotation
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public readonly string Name;

        public bool IsKey { get; set; }

        public bool AutoIncrement { get; set; }
    }
}