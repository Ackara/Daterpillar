using Acklann.Daterpillar.Scripting.Writers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    public static class CrudBuilder
    {
        public static void Configure(Action<IDefaultCrudOperationsConfigBuilder> configuration)
        {
            var builder = new DefaultCrudOperationsConfigBuilder();
            configuration?.Invoke(builder);
            _defaultOperations = (DefaultCrudBuilder)builder.Build();
            foreach (ICrudBuilder item in builder.GetCrudOperations()) _alternateOperations.Add(item);
        }

        public static void Create(IDbCommand command, object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICrudBuilder writer = GetBestOperations(recordType);
            writer.Create(command, record, dialect);
        }

        public static IEnumerable<object> Read(Type recordType, IDataReader resultSet)
        {
            if (resultSet == null) throw new ArgumentNullException(nameof(resultSet));

            var records = new LinkedList<object>();
            ICrudBuilder handler = GetBestOperations(recordType);
            while (resultSet.Read())
            {
                records.AddLast(handler.Read(resultSet, recordType));
            }

            return records;
        }

        public static IEnumerable<TRecord> Read<TRecord>(IDataReader resultSet)
            => Read(typeof(TRecord), resultSet).Cast<TRecord>();

        public static void Update(IDbCommand command, object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICrudBuilder handler = GetBestOperations(recordType);
            handler.Update(command, record, dialect);
        }

        public static void Delete(IDbCommand command, object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICrudBuilder handler = GetBestOperations(recordType);
            handler.Delete(command, record, dialect);
        }

        private static ICrudBuilder GetBestOperations(Type type)
        {
            for (int i = 0; i < _alternateOperations.Count; i++)
                if (_alternateOperations[i].CanAccept(type))
                {
                    return _alternateOperations[i];
                }

            return _defaultOperations;
        }

        #region Backing Members

        private static DefaultCrudBuilder _defaultOperations = new DefaultCrudBuilder();

        private static readonly IList<ICrudBuilder> _alternateOperations = new List<ICrudBuilder>();

        #endregion Backing Members
    }
}