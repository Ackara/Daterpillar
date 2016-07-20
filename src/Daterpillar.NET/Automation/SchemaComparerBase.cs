using Gigobyte.Daterpillar.Transformation;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Gigobyte.Daterpillar.Automation
{
    public abstract class SchemaComparerBase : ISchemaComparer
    {
        public SchemaDiscrepancy Compare(ISchemaAggregator source, ISchemaAggregator target)
        {
            _source = source;
            _target = target;
            _discrepancy = new SchemaDiscrepancy();

            CompareSchemas(source.FetchSchema(), target.FetchSchema());
            SummaryDiscrepancies(_discrepancy);

            return _discrepancy;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _source?.Dispose();
                _target?.Dispose();
            }
        }

        protected virtual void CompareSchemas(Schema source, Schema target)
        {
            _discrepancy.Counters.SourceTables = source.Tables.Count;
            _discrepancy.Counters.DestTables = target.Tables.Count;


            // TODO: Normalize dataset

            int targetIndex, sourceIndex;
            for (targetIndex = 0; targetIndex < source.Tables.Count; targetIndex++)
            {
                bool sourceHasTarget = CheckIfSourceContainsTarget(source.Tables, target.Tables[targetIndex], out sourceIndex);
                if (sourceHasTarget)
                {
                    // TODO: Check for column discrepancy
                    CheckColumnsForDiscrepancies(source.Tables[sourceIndex], target.Tables[targetIndex]);

                    // TODO: Check for index discrepancy

                    // TODO: Check for foreign key discrepancy
                }
                else
                {
                    // TODO: Create discrepancy.
                }
            }

            if (source.Tables.Count < target.Tables.Count)
            {
                // TODO: Create discrepancies;
            }
        }

        protected virtual void SummaryDiscrepancies(SchemaDiscrepancy report)
        {
        }

        #region Private Members

        private ISchemaAggregator _source, _target;
        private SchemaDiscrepancy _discrepancy;

        private bool CheckIfSourceContainsTarget(IList<Table> source, Table target, out int sourceIndex)
        {
            sourceIndex = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Name.Equals(target.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceIndex = i;
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfSourceContainsTarget(IList<Column> source, Column target, out int sourceIndex)
        {
            sourceIndex = -1;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i].Name.Equals(target.Name, StringComparison.CurrentCultureIgnoreCase))
                {
                    sourceIndex = i;
                    return true;
                }
            }

            return false;
        }

        private void CheckColumnsForDiscrepancies(Table source, Table target)
        {
            // TODO: Normalize dataset

            int sourceIndex, targetIndex;
            for (targetIndex = 0; targetIndex < source.Columns.Count; targetIndex++)
            {
                bool sourceHasTarget = CheckIfSourceContainsTarget(source.Columns, target.Columns[targetIndex], out sourceIndex);
                if (sourceHasTarget)
                {
                    EvaluateColumns(source.Columns[sourceIndex], target.Columns[targetIndex]);
                }
                else
                {
                    EvaluateColumns(new Column(), target.Columns[targetIndex]);
                }
            }
        }

        private void EvaluateColumns(Column source, Column target)
        {
            throw new NotImplementedException();
        }

        #endregion Private Members
    }
}