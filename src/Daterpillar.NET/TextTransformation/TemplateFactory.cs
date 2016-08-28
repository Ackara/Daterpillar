﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class TemplateFactory
    {
        public TemplateFactory()
        {
            LoadAssemblyTypes();
        }

        public ITemplate CreateInstance(string name, bool partialMatch = true)
        {
            try
            {
                if (partialMatch) name = name.EndsWith(_targetInterface) ? name : (name + _targetInterface);
                return (ITemplate)Activator.CreateInstance(_templateTypes[name]);
            }
            catch (KeyNotFoundException) { return new NullTemplate(); }
        }

        public ITemplate CreateInstance(SupportedDatabase databaseType)
        {
            string key = string.Concat(databaseType, _targetInterface);
            return CreateInstance(key);
        }

        #region Private Members

        private static readonly string _targetInterface = (nameof(ITemplate).Substring(1));
        private IDictionary<string, Type> _templateTypes;

        private void LoadAssemblyTypes()
        {
            _templateTypes = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetAssembly(typeof(ITemplate)).GetTypes())
            {
                if (!type.IsAbstract && !type.IsInterface && type.IsSubclassOf(typeof(ITemplate)))
                {
                    _templateTypes.Add(type.Name, type);
                }
            }
        }

        #endregion Private Members
    }
}