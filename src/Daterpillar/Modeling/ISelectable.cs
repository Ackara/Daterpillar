using System.Data;

namespace Acklann.Daterpillar.Modeling
{
    public interface ISelectable
    {
        void Load(IDataRecord record);
    }
}