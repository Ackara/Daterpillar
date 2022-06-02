using System.Data;

namespace Acklann.Daterpillar.Foo
{

    [System.Obsolete]
    public interface ISelectable
    {
        void Load(IDataRecord record);
    }
}