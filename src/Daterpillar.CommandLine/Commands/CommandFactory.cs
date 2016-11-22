using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.Commands
{
    public class CommandFactory
    {
        public CommandFactory()
        {
            LoadCommandTypes();
        }

        public ICommand CrateInstance(string verb)
        {
            try { return (ICommand)Activator.CreateInstance(_commandTypes[verb]); }
            catch (KeyNotFoundException) { return new NullCommand(); }
        }

        #region Private Members

        private IDictionary<string, Type> _commandTypes;

        private void LoadCommandTypes()
        {
            _commandTypes = new Dictionary<string, Type>();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.IsAbstract && !type.IsInterface && (type.GetInterface(typeof(ICommand).Name) != null))
                {
                    var verb = type.GetCustomAttribute<VerbLinkAttribute>();
                    if (verb != null)
                    {
                        _commandTypes.Add(verb.Name, type);
                    }
                }
            }
        }

        #endregion Private Members
    }
}