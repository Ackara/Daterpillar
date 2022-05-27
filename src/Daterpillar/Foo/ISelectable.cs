using System.Data;

namespace Acklann.Daterpillar.Foo
{
    public interface ISelectable
    {
        void Load(IDataRecord record);
    }
}