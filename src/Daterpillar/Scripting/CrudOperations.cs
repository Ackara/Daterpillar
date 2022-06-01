using Acklann.Daterpillar.Scripting.Writers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Scripting
{
    public static class CrudOperations
    {
        public static void Configure(Action<IDefaultCrudOperationsConfigBuilder> configuration)
        {
            var builder = new DefaultCrudOperationsConfigBuilder();
            configuration?.Invoke(builder);
            _defaultOperations = (DefaultCrudOperations)builder.Build();
        }

        public static string Create(object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICrudOperations writer = GetBestOperations(recordType);
            return writer.Create(record, dialect);
        }

        public static IEnumerable<object> Read(Type recordType, IDataReader resultSet)
        {
            if (resultSet == null) throw new ArgumentNullException(nameof(resultSet));

            ICrudOperations handler = GetBestOperations(recordType);
            while (resultSet.Read())
            {
                yield return handler.Read(resultSet, recordType);
            }
        }

        public static IEnumerable<TRecord> Read<TRecord>(IDataReader resultSet)
            => Read(typeof(TRecord), resultSet).Cast<TRecord>();

        public static long Update(object record, Language dialect)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));

            Type recordType = record.GetType();
            ICrudOperations handler = GetBestOperations(recordType);
            return handler.Update(record, dialect);
        }

        private static ICrudOperations GetBestOperations(Type type)
        {
            for (int i = 0; i < _alternateOperations.Count; i++)
                if (_alternateOperations[i].CanAccept(type))
                {
                    return _alternateOperations[i];
                }

            return _defaultOperations;
        }

        #region Backing Members

        private static DefaultCrudOperations _defaultOperations = new DefaultCrudOperations();

        private static readonly IList<ICrudOperations> _alternateOperations = new List<ICrudOperations>();

        #endregion Backing Members
    }
}