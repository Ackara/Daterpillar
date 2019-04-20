using System;

namespace Acklann.Daterpillar
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SqlIgnoreAttribute : Attribute { }
}