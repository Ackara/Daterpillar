using System.Data;

namespace Acklann.Daterpillar.Modeling
{
    public interface IReadable
    {
        void Load(IDataRecord record);
    }
}