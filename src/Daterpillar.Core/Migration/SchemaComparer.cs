using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;

namespace Gigobyte.Daterpillar.Migration
{
    public class SchemaComparer : ISynchronizer
    {
        public ChangeLog GenerateScript(Schema source, Schema target)
        {
            _changes = new ChangeLog();

            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            SummarizeReport(source, target);

            return _changes;
        }

        public ChangeLog GenerateScript(ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source)
            {
                using (target)
                {
                    return GenerateScript(source.FetchSchema(), target.FetchSchema());
                }
            }
        }

        #region Private Members

        private ChangeLog _changes;

        private static void EnsureBothArraysAreOfTheSameSize<T>(ref T[] left, ref T[] right)
        {
            if (left.Length > right.Length)
                IncreaseLengthOfArray(ref right, left.Length);
            else if (left.Length < right.Length)
                IncreaseLengthOfArray(ref left, right.Length);
        }

        private static void IncreaseLengthOfArray<T>(ref T[] array, int capacity)
        {
            var newArray = new T[capacity];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            array = newArray;
        }

        private static void AlignTheItemsOfBothArraysByName<T>(ref T[] left, ref T[] right)
        {
            dynamic l;
            for (int i = 0; i < left.Length; i++)
            {
                l = left[i];
                SwapMatchingItems(ref right, (l?.Name), i);
            }
        }

        private static void SwapMatchingItems<T>(ref T[] right, string name, int targetIdx)
        {
            dynamic r; T temp;
            for (int i = targetIdx; i < right.Length; i++)
            {
                r = right[i];
                if (name == r?.Name)
                {
                    temp = right[targetIdx];
                    right[targetIdx] = right[i];
                    right[i] = temp;
                    break;
                }
            }
        }

        private void FindDiscrepanciesBetween(Table[] left, Table[] right)
        {
            EnsureBothArraysAreOfTheSameSize(ref left, ref right);
            AlignTheItemsOfBothArraysByName(ref left, ref right);

            for (int i = 0; i < left.Length; i++)
            {
                if (left[i] == null && right[i] != null)
                {
                    // TODO: Drop table from the right
                    _changes.Discrepancies.Add(new Modification());
                }
                else if (left[i] != null && right[i] == null)
                {
                    // TODO: Add table from the left
                    _changes.Discrepancies.Add(new Modification());
                }
                else if(left[i].Name != right[i].Name)
                {
                    // TODO: Drop table from the right
                    // TODO: Add table from the left
                }
                else FindDiscrepanciesBetween(left[i].Columns.ToArray(), right[i].Columns.ToArray());
                
            }
        }

        private void FindDiscrepanciesBetween(Column[] left, Column[] right)
        {
            EnsureBothArraysAreOfTheSameSize(ref left, ref right);
            AlignTheItemsOfBothArraysByName(ref left, ref right);

            // TODO: compare each column.
            for (int i = 0; i < left.Length; i++)
            {
                if (left[i].Name != right[i].Name)
                {
                    _changes.Discrepancies.Add(new Modification()
                    {
                    });
                }

                if (left[i].AutoIncrement != right[i].AutoIncrement)
                {
                    _changes.Discrepancies.Add(new Modification()
                    {
                    });
                }

                if (left[i].DataType != right[i].DataType)
                {
                    _changes.Discrepancies.Add(new Modification()
                    {
                    });
                }
            }
        }

        private void SummarizeReport(Schema source, Schema target)
        {
            _changes.Source.TotalTables = ((source?.Tables?.Count) ?? 0);
            _changes.Target.TotalTables = ((target?.Tables?.Count) ?? 0);

            foreach (var table in source.Tables)
            {
                _changes.Source.TotalColumns += ((table?.Columns?.Count) ?? 0);
                _changes.Source.TotalIndexes += ((table?.Indexes?.Count) ?? 0);
                _changes.Source.TotalForeignKeys += ((table?.ForeignKeys?.Count) ?? 0);
            }

            foreach (var table in target.Tables)
            {
                _changes.Target.TotalColumns += ((table?.Columns?.Count) ?? 0);
                _changes.Target.TotalIndexes += ((table?.Indexes?.Count) ?? 0);
                _changes.Target.TotalForeignKeys += ((table?.ForeignKeys?.Count) ?? 0);
            }

            _changes.Summarize();
        }

        #endregion Private Members
    }
}