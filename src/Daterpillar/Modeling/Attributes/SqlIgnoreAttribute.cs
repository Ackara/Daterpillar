using System;

namespace Acklann.Daterpillar.Modeling.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class SqlIgnoreAttribute : Attribute { }
}