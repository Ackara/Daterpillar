using System;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class MSSQLScriptBuilder : IScriptBuilder
    {
        public MSSQLScriptBuilder() : this(MSSQLScriptBuilderSettings.Default, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLScriptBuilder(MSSQLScriptBuilderSettings settings) : this(settings, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLScriptBuilder(MSSQLScriptBuilderSettings settings, ITypeNameResolver typeResolver)
        {
            _typeResolver = typeResolver;
        }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            throw new NotImplementedException();
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            throw new NotImplementedException();
        }

        public void Append(string text)
        {
            _text.Append(text);
        }

        public void AppendLine(string text)
        {
            _text.AppendLine(text);
        }

        public void Create(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public void Create(Index index)
        {
            throw new NotImplementedException();
        }

        public void Create(Column column)
        {
            throw new NotImplementedException();
        }

        public void Create(Schema schema)
        {
            throw new System.NotImplementedException();
        }

        public void Create(Table table)
        {
            
        }

        public void Drop(Index index)
        {
            throw new NotImplementedException();
        }

        public void Drop(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public void Drop(Column column)
        {
            throw new NotImplementedException();
        }

        public void Drop(Schema schema)
        {
            throw new System.NotImplementedException();
        }

        public void Drop(Table table)
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            return _text.ToString();
        }

        #region Private Members

        private int _seed = 0;
        private readonly ITypeNameResolver _typeResolver;
        private readonly StringBuilder _text = new StringBuilder();

        #endregion Private Members
    }
}