using System;

namespace Ackara.Daterpillar.Annotation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TableAttribute : Attribute
    {
        public TableAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}