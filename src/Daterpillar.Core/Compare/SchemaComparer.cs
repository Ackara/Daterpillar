using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Compare
{
    public class SchemaComparer : ISchemaComparer
    {
        public ComparisonReport GenerateReport(ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source)
            {
                using (target)
                {
                    return GenerateReport(source.FetchSchema(), target.FetchSchema());
                }
            }
        }

        public ComparisonReport GenerateReport(Schema source, Schema target)
        {
            _report = new ComparisonReport();
            _report.TotalSourceTables = source.Tables.Count;
            _report.TotalTargetTables = target.Tables.Count;

            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            _report.Summarize();
            return _report;
        }

        public int Compare(Schema source, Schema target)
        {
            GenerateReport(source, target);

            throw new System.NotImplementedException();
        }

        #region Private Members

        private ComparisonReport _report;

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
                FindDiscrepanciesBetween(left[i].Columns.ToArray(), right[i].Columns.ToArray());
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
                    _report.Discrepancies.Add(new Discrepancy()
                    {
                    });
                }

                if (left[i].AutoIncrement != right[i].AutoIncrement)
                {
                    _report.Discrepancies.Add(new Discrepancy()
                    {
                    });
                }

                if (left[i].DataType != right[i].DataType)
                {
                    _report.Discrepancies.Add(new Discrepancy()
                    {
                    });
                }
            }
        }

        #endregion Private Members
    }
}