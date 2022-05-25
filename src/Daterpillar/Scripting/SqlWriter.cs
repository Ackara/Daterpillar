using Acklann.Daterpillar.Scripting.Writers;
using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlWriter
    {
        public static string Create(object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICRUDWriter writer = GetBestWriter(recordType);
            return writer.Create(record, dialect);
        }

        public static void Register(ICRUDWriter writer)
        {
            _writers.Add(writer);
        }

        private static ICRUDWriter GetBestWriter(Type type)
        {
            for (int i = 0; i < _writers.Count; i++)
                if (_writers[i].CanAccept(type))
                {
                    return _writers[i];
                }

            return _defaultWriter;
        }

        private static ICRUDWriter GetDefaultWriter() => _writers[0];

        #region Backing Members

        private static readonly ICRUDWriter _defaultWriter = new CRUDWriterBase();

        private static readonly IList<ICRUDWriter> _writers = new List<ICRUDWriter>
        {
        };

        #endregion Backing Members
    }
}