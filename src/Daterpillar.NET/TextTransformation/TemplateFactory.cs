using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.TextTransformation
{
    public class TemplateFactory
    {
        public TemplateFactory()
        {
            LoadAssemblyTypes();
        }

        public IScriptBuilder CreateInstance(string name, bool partialMatch = true)
        {
            try
            {
                if (partialMatch) name = name.EndsWith(_targetInterface) ? name : (name + _targetInterface);
                return (IScriptBuilder)Activator.CreateInstance(_templateTypes[name]);
            }
            catch (KeyNotFoundException) { return new NullScriptBuilder(); }
        }

        public IScriptBuilder CreateInstance(SupportedDatabase databaseType)
        {
            string key = string.Concat(databaseType, _targetInterface);
            return CreateInstance(key);
        }

        #region Private Members

        private static readonly string _targetInterface = (nameof(IScriptBuilder).Substring(1));
        private IDictionary<string, Type> _templateTypes;

        private void LoadAssemblyTypes()
        {
            _templateTypes = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetAssembly(typeof(IScriptBuilder)).GetTypes())
            {
                if (!type.IsAbstract && !type.IsInterface && type.IsSubclassOf(typeof(IScriptBuilder)))
                {
                    _templateTypes.Add(type.Name, type);
                }
            }
        }

        #endregion Private Members
    }
}