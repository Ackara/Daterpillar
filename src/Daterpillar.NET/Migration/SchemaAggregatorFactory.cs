using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Acklann.Daterpillar.Migration
{
    public class SchemaAggregatorFactory
    {
        public SchemaAggregatorFactory()
        {
            LoadAggregatorTypes();
        }

        public ISchemaAggregator CreateInstance(string name, IDbConnection connection)
        {
            try { return (ISchemaAggregator)Activator.CreateInstance(_aggregatorTypes[name.ToLower()], new object[1] { connection }); }
            catch (KeyNotFoundException) { return new NullSchemaAggregator(); }
        }

        public ISchemaAggregator CreateInstance(ConnectionType dbType, IDbConnection connection)
        {
            return CreateInstance(string.Concat(dbType, _targetInterface), connection);
        }

        #region Private Members

        private readonly string _targetInterface = (nameof(ISchemaAggregator).Substring(1));

        private IDictionary<string, Type> _aggregatorTypes;

        private void LoadAggregatorTypes()
        {
            _aggregatorTypes = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetAssembly(typeof(SchemaAggregatorFactory)).GetTypes())
            {
                if (!type.IsAbstract && !type.IsInterface && (type.GetInterface(typeof(ISchemaAggregator).Name) != null))
                {
                    _aggregatorTypes.Add(type.Name.ToLower(), type);
                }
            }
        }

        #endregion Private Members
    }
}