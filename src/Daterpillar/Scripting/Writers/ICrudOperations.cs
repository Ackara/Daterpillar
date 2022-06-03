using System;
using System.Data;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface ICrudOperations
    {
        string Create(object record, Language dialect);
        void Create2(IDbCommand command, object record, Language dialect);

        object Read(IDataRecord data, Type recordType);

        string Update(object record, Language dialect);

        string Delete(object record, Language dialect);

        bool CanAccept(Type type);
    }
}