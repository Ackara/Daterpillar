using System;

namespace Gigobyte.Daterpillar.Data
{
    public interface IEntityConstructor
    {
        object CreateInstance(Type returnType, object data);
    }
}