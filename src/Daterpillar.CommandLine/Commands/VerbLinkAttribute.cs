using System;

namespace Gigobyte.Daterpillar.Commands
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class VerbLinkAttribute : Attribute
    {
        public VerbLinkAttribute(string name)
        {
            Name = name;
        }

        public readonly string Name;
    }
}