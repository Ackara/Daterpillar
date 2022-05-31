using System;
using System.Data;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface ICrudOperations
    {
        string Create(object model, Language dialect);

        object Read(IDataRecord data, Type recordType);

        string Update(object model, Language dialect);

        string Delete(object model, Language dialect);

        bool CanAccept(Type type);
    }
}