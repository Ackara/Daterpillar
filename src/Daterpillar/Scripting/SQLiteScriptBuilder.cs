using System;
using System.Collections.Generic;
using System.Text;
using Ackara.Daterpillar.TypeResolvers;

namespace Ackara.Daterpillar.Scripting
{
    public class SQLiteScriptBuilder : SqlScriptBuilderBase
    {
        public SQLiteScriptBuilder() : base(new SqlScriptBuilderSettings()
        {
            ShowHeader = true,
            IgnoreScripts = false,
            IgnoreComments = false,
            AppendCreateSchemaCommand = false
        }, new SQLiteTypeResolver())
        { }

        public SQLiteScriptBuilder(SqlScriptBuilderSettings settings) : base(settings, new SQLiteTypeResolver())
        {
        }

        public override IScriptBuilder Append(Schema schema)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Append(Table table)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Append(Column column)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Append(Index index)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Append(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Remove(Schema schema)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Remove(Table table)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Remove(Column column)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Remove(Index index)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Remove(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            throw new NotImplementedException();
        }

        public override IScriptBuilder Update(Table oldTable, Table newTable)
        {
            throw new NotImplementedException();
        }
    }
}
