using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public class SqlWriterFactory
    {
        public SqlWriterFactory()
        {
            var assemblyTypes = (from t in typeof(SqlWriter).Assembly.ExportedTypes
                                 where
                                    !t.IsAbstract &&
                                    !t.IsInterface &&
                                    typeof(SqlWriter).IsAssignableFrom(t)
                                 select t);

            foreach (Type type in assemblyTypes)
            {
                if (type.GetCustomAttribute(typeof(System.ComponentModel.CategoryAttribute)) is System.ComponentModel.CategoryAttribute attr)
                {
                    _returnTypes.Add(attr.Category, type);
                }
            }
        }

        public SqlWriter CreateInstance(Language syntax, Stream stream)
        {
            return CreateInstance(syntax, new StreamWriter(stream));
        }

        public SqlWriter CreateInstance(Language syntax, TextWriter writer)
        {
            if (_returnTypes.ContainsKey(syntax.ToString()))
            {
                return (SqlWriter)Activator.CreateInstance(_returnTypes[syntax.ToString()], writer);
            }

            throw new ArgumentOutOfRangeException(nameof(syntax), $"{GetType().Assembly.FullName} do not support '{syntax}' at this time. Visit 'https://github.com/Ackara/Daterpillar' to request support.");
        }

        public IEnumerable<Type> GetWriterTypes()
        {
            return _returnTypes.Values;
        }

        #region Private Members

        private readonly IDictionary<string, Type> _returnTypes = new Dictionary<string, Type>();

        #endregion Private Members
    }
}