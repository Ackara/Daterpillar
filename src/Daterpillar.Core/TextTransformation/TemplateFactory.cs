using System;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class TemplateFactory
    {
        public ITemplate CreateInstance(string name)
        {
            try { return (ITemplate)Activator.CreateInstance(_templateTypes[name]); }
            catch (KeyNotFoundException) { return new NullTemplate(); }
        }

        public ITemplate CreateInstance(SupportedDatabase databaseType)
        {
            string key = $"{databaseType.ToString()}Template";
            return CreateInstance(key);
        }

        #region Private Members

        private IDictionary<string, Type> _templateTypes;

        private void LoadAssemblyTypes()
        {
        }

        #endregion Private Members
    }
}