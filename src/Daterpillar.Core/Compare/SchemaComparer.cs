using Gigobyte.Daterpillar.Transformation;
using System;

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
            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            return _report;
        }

        public int Compare(Schema source, Schema target)
        {
            var report = GenerateReport(source, target);

            throw new System.NotImplementedException();
        }

        #region Private Members

        private ComparisonReport _report;

        private static void EnsureBothArraysAreOfTheSameSize<T>(ref T[] left, ref T[] right)
        {
            if (left.Length > right.Length)
                IncreaseLengthOfArray(right, left.Length);
            else if (left.Length < right.Length)
                IncreaseLengthOfArray(left, right.Length);
        }

        private static void IncreaseLengthOfArray<T>(T[] array, int capacity)
        {
            throw new NotImplementedException();
        }

        private void FindDiscrepanciesBetween(Table[] left, Table[] right)
        {
            // TODO: Fill the lesser array with null values.
            EnsureBothArraysAreOfTheSameSize(ref left, ref right);


            // TODO: Align the arrays
            // TODO: compare columns
        }

        private void FindDiscrepanciesBetween(Column[] left, Column[] right)
        {
            // TODO: Fill the lesser array with null values.
            // TODO: Align the arrays.
            // TODO: compare each column.
        }

        #endregion Private Members
    }
}