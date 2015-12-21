using System;

namespace Ackara.Daterpillar.Annotation
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ViewAttribute : Attribute
    {
    }
}