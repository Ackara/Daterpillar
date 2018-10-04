using Acklann.Daterpillar.Configuration;
using System;
using System.IO;

namespace Acklann.Daterpillar.Compilation
{
    public class SqlMigrator
    {
        public SqlMigrator()
        {
        }

        public SqlMigrator(object writer)
        {
        }

        public void GenerateMigrationScript(Stream stream, Schema from, Schema to, Syntax syntax = Syntax.Generic)
        {
            // ownsStream: If true, the output stream is closed by the writer when done; otherwise false. The default value is true.

            Table[] left = from.Clone().Tables.ToArray();
            Table[] right = to.Clone().Tables.ToArray();

            // Step 1: Mark all the tables that need to be added, removed or updated.
            MarkTablesThatNeedToBeAddedOrAltered(left, right);

            // Step 2: Sort the tables by dependency.

            // Step 3: Write the SQL statements that will edit the schema to the stream.
        }

        // ==================== INTERNAL METHODS ==================== //

        private void MarkTablesThatNeedToBeAddedOrAltered(Table[] left, Table[] right)
        {
            foreach (Table r in right)
            {
                foreach (Table l in left)
                {
                    if (r.Name.Equals(l.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        // alter left
                        break;
                    }
                }
                // add right
            }
        }

        #region Private Members

        private readonly object _writer;

        #endregion Private Members
    }
}