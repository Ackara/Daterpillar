using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface ICRUDWriter
    {
        string Create(object model, Language dialect);

        IEnumerable<object> Read(string query, Language dialect);

        string Update(object model, Language dialect);

        string Delete(object model, Language dialect);

        bool CanAccept(Type type);
    }
}