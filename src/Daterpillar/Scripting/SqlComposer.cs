using Acklann.Daterpillar.Scripting.Writers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    public static class SqlComposer
    {
        public static string Create(object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICRUDWriter writer = GetBestWriter(recordType);
            return writer.Create(record, dialect);
        }

        public static IEnumerable<object> Read(Type recordType, IDataReader resultSet)
        {
            if (resultSet == null) throw new ArgumentNullException(nameof(resultSet));

            ICRUDWriter handler = GetBestWriter(recordType);
            while (resultSet.Read())
            {
                yield return handler.Read(resultSet, recordType);
            }
        }

        public static IEnumerable<TRecord> Read<TRecord>(IDataReader resultSet) => Read(typeof(TRecord), resultSet).Cast<TRecord>();

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

        #region Backing Members

        private static readonly ICRUDWriter _defaultWriter = new CRUDWriterBase();

        private static readonly IList<ICRUDWriter> _writers = new List<ICRUDWriter>();

        #endregion Backing Members
    }
}