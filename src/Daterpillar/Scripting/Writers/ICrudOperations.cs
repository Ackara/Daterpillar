using System;
using System.Data;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface ICrudOperations
    {
        void Create(IDbCommand command, object record, Language dialect);

        object Read(IDataRecord data, Type recordType);

        void Update(IDbCommand command, object record, Language dialect);

        void Delete(IDbCommand command, object record, Language dialect);

        bool CanAccept(Type type);
    }
}