using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Migration
{
    public class Synchronizer
    {
        public Synchronizer(IScriptBuilder scriptBuilder) : this(scriptBuilder, SynchronizerSettings.Default)
        {
        }

        public Synchronizer(IScriptBuilder scriptBuilder, SynchronizerSettings settings)
        {
            _settings = settings;
            _scriptBuilder = scriptBuilder;
        }

        public string GenerateScript(Schema source, Schema target)
        {
            _source = source; _target = target;
            GetChangesBetween(source.Tables, target.Tables);

            return _scriptBuilder.GetContent();
        }

        public string GenerateScript(ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source) { using (target) { return GenerateScript(source.FetchSchema(), target.FetchSchema()); } }
        }

        #region Private Members

        private readonly IScriptBuilder _scriptBuilder;
        private readonly SynchronizerSettings _settings;

        private Schema _source, _target;

        private static void GetTheItemsOfBothCollectionsAlignedByNameInAnArray<T>(ICollection<T> source, ICollection<T> target, out T[] leftArray, out T[] rightArray)
        {
            int maxLength = ((source.Count >= target.Count) ? source.Count : target.Count);
            leftArray = new T[maxLength];
            rightArray = new T[maxLength];

            source.CopyTo(leftArray, 0);
            target.CopyTo(rightArray, 0);

            dynamic lItem, rItem, temp;

            for (int i = 0; i < maxLength; i++)
            {
                lItem = leftArray[i];
                if (lItem == null) continue;

                for (int n = i; n < maxLength; n++)
                {
                    rItem = rightArray[n];
                    if (lItem.Name == rItem?.Name)
                    {
                        temp = rightArray[i];
                        rightArray[i] = rightArray[n];
                        rightArray[n] = temp;
                    }
                }
            }
        }

        private void GetChangesBetween(IList<Table> source, IList<Table> target)
        {
            Table[] left, right;
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target, out left, out right);

            string lTable, rTable;
            for (int i = 0; i < left.Length; i++)
            {
                lTable = left[i]?.Name;
                rTable = right[i]?.Name;

                if (lTable == rTable)
                {
                    // TODO: Compare columns
                    // TODO: Compare indexes
                    // TODO: Compare foreign keys
                    GetChangesBetween(left[i].Columns, right[i].Columns);
                    GetChangesBetween(left[i].Indexes, right[i].Indexes);
                    GetChangesBetween(left[i].ForeignKeys, right[i].ForeignKeys);
                }
                else if (lTable == null && rTable != null)
                {
                    _scriptBuilder.Drop(right[i]);
                }
                else if (lTable != null && rTable == null)
                {
                    // TODO: Add table on the left
                    _scriptBuilder.Create(left[i]);
                }
                else if (lTable != null && rTable != null)
                {
                    // TODO: Drop table on the right
                    // TODO: Add table on the left
                    _scriptBuilder.Drop(right[i]);
                    _scriptBuilder.Create(left[i]);
                }
            }
        }

        private void GetChangesBetween(ICollection<Column> source, ICollection<Column> target)
        {
            Column[] left, right;
            var comparer = new Equality.ColumnEqualityComparer();
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target, out left, out right);

            Column lColumn, rColumn;

            for (int i = 0; i < left.Length; i++)
            {
                lColumn = left[i];
                rColumn = right[i];

                if (lColumn == null && rColumn != null)
                {
                    _scriptBuilder.Drop(_target, rColumn);
                }
                else if (lColumn != null && rColumn == null)
                {
                    _scriptBuilder.Create(lColumn);
                }
                else if (!comparer.Equals(lColumn, rColumn))
                {
                    if (lColumn.Name == rColumn.Name) _scriptBuilder.AlterTable(lColumn, rColumn);
                    else
                    {
                        _scriptBuilder.Drop(_target, rColumn);
                        _scriptBuilder.Create(lColumn);
                    }
                }
            }
        }

        private void GetChangesBetween(ICollection<Index> source, ICollection<Index> target)
        {
            Index[] left, right;
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target, out left, out right);

            for (int i = 0; i < left.Length; i++)
            {
                _scriptBuilder.Drop(right[i]);
                _scriptBuilder.Create(left[i]);
            }
        }

        private void GetChangesBetween(ICollection<ForeignKey> source, ICollection<ForeignKey> target)
        {
            ForeignKey[] left, right;
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target, out left, out right);

            for (int i = 0; i < left.Length; i++)
            {
                _scriptBuilder.Drop(right[i]);
                _scriptBuilder.Create(left[i]);
            }
        }

        #endregion Private Members
    }
}