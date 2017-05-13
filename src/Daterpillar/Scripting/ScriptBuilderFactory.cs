﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Provides methods to create <see cref="IScriptBuilder"/> instances.
    /// </summary>
    public class ScriptBuilderFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBuilderFactory"/> class.
        /// </summary>
        public ScriptBuilderFactory()
        {
            _scriptBuilderTypes = new Dictionary<string, Type>()
            {
                { "cs", typeof(CSharpScriptBuilder) },
                { "c#", typeof(CSharpScriptBuilder) },
                { nameof(Syntax.CSharp).ToLower(), typeof(CSharpScriptBuilder) },
                { nameof(Syntax.MSSQL ).ToLower(), typeof(MSSQLScriptBuilder) },
                { nameof(Syntax.MySQL ).ToLower(), typeof(MySQLScriptBuilder) },
                { nameof(Syntax.SQLite).ToLower(), typeof(SQLiteScriptBuilder) }
            };

            var assembly = Assembly.Load(new AssemblyName(GetType().AssemblyQualifiedName.Remove(0, GetType().FullName.Length).Trim(' ', ',')));
            var scriptTypes = from type in assembly.ExportedTypes
                              let info = type.GetTypeInfo()
                              where info.IsInterface == false && info.IsAbstract == false && (typeof(IScriptBuilder).GetTypeInfo().IsAssignableFrom(info))
                              select type;

            foreach (var type in scriptTypes)
            {
                _scriptBuilderTypes.Add(type.Name.ToLower(), type);
            }
        }

        /// <summary>
        /// Creates a <see cref="IScriptBuilder"/> instance.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="IScriptBuilder"/> instance.</returns>
        public IScriptBuilder Create(string name)
        {
            try
            {
                Type type = _scriptBuilderTypes[name?.ToLower()];
                return (IScriptBuilder)Activator.CreateInstance(type);
            }
            catch (KeyNotFoundException)
            {
                return new NullScriptBuilder();
            }
        }

        /// <summary>
        /// Creates a <see cref="IScriptBuilder"/> instance.
        /// </summary>
        /// <param name="syntax">The syntax.</param>
        /// <returns>A <see cref="IScriptBuilder"/> instance.</returns>
        public IScriptBuilder Create(Syntax syntax)
        {
            return Create(syntax.ToString());
        }

        #region Private Members

        private IDictionary<string, Type> _scriptBuilderTypes;

        #endregion Private Members
    }
}