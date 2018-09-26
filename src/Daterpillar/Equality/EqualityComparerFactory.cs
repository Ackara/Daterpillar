using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Equality
{
    internal class EqualityComparerFactory
    {
        public EqualityComparerFactory()
        {
            _comparerTypes = new Dictionary<string, Type>();
            var assembly = Assembly.Load(new AssemblyName(GetType().AssemblyQualifiedName.Remove(0, GetType().FullName.Length).Trim(' ', ',')));
            var comparerTypes = from type in assembly.DefinedTypes
                                where
                                  type.IsInterface == false
                                  && type.IsAbstract == false
                                  &&
                                  (
                                      typeof(IEqualityComparer<Column>).GetTypeInfo().IsAssignableFrom(type)
                                      || typeof(IEqualityComparer<Index>).GetTypeInfo().IsAssignableFrom(type)
                                      || typeof(IEqualityComparer<ForeignKey>).GetTypeInfo().IsAssignableFrom(type)
                                  )
                                select type.AsType();

            foreach (var type in comparerTypes)
            {
                _comparerTypes.Add(type.Name, type);
            }
        }

        public IEqualityComparer<Column> CreateColumnComparer(string name)
        {
            try
            {
                Type target = _comparerTypes[name];
                return (IEqualityComparer<Column>)Activator.CreateInstance(target);
            }
            catch (KeyNotFoundException)
            {
                return new ColumnEqualityComparer();
            }
        }

        public IEqualityComparer<Index> CreateIndexComparer(string name)
        {
            try
            {
                Type target = _comparerTypes[name];
                return (IEqualityComparer<Index>)Activator.CreateInstance(target);
            }
            catch (KeyNotFoundException)
            {
                return new IndexEqualityComparer();
            }
        }

        public IEqualityComparer<ForeignKey> CreateForeignKeyComparer(string name)
        {
            try
            {
                Type target = _comparerTypes[name];
                return (IEqualityComparer<ForeignKey>)Activator.CreateInstance(target);
            }
            catch (KeyNotFoundException)
            {
                return new ForeignKeyEqualityComparer();
            }
        }

        #region Private Members

        private IDictionary<string, Type> _comparerTypes;

        #endregion Private Members
    }
}