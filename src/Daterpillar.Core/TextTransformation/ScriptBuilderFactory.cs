using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.TextTransformation
{
    public class ScriptBuilderFactory
    {
        public ScriptBuilderFactory()
        {
            LoadAssemblyTypes();
        }

        public IScriptBuilder CreateInstance(string name)
        {
            try { return (IScriptBuilder)Activator.CreateInstance(Type.GetType(_scriptBuilderTypes[name.ToLower()])); }
            catch (KeyNotFoundException) { return new NullScriptBuilder(); }
        }

        public IScriptBuilder CreateInstance(ConnectionType connectionType)
        {
            return CreateInstance(string.Concat(connectionType, _targetInterface));
        }

        #region Private Members

        private string _targetInterface = nameof(IScriptBuilder).Substring(1);
        private IDictionary<string, string> _scriptBuilderTypes = new Dictionary<string, string>();

        private void LoadAssemblyTypes()
        {
            Assembly thisAssembly = Assembly.Load(new AssemblyName(typeof(IScriptBuilder).GetTypeInfo().Assembly.FullName));
            foreach (var typeInfo in thisAssembly.DefinedTypes)
                if (typeInfo.IsPublic && !typeInfo.IsInterface && !typeInfo.IsAbstract && typeof(IScriptBuilder).GetTypeInfo().IsAssignableFrom(typeInfo))
                {
                    _scriptBuilderTypes.Add(typeInfo.Name.ToLower(), typeInfo.FullName);
                }
        }

        #endregion Private Members
    }
}