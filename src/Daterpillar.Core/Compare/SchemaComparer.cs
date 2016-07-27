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

        private static void AlignTheElementsOfBothArraysByName<T>(ref T[] left, ref T[] right)
        {
            dynamic l, r;
            for (int i = 0; i < left.Length; i++)
            {
                l = left[i]; r = right[i];
            }
        }

        private void FindDiscrepanciesBetween(Table[] left, Table[] right)
        {
            EnsureBothArraysAreOfTheSameSize(ref left, ref right);
            AlignTheElementsOfBothArraysByName(ref left, ref right);

            for (int i = 0; i < left.Length; i++)
            {
                FindDiscrepanciesBetween(left[i].Columns.ToArray(), right[i].Columns.ToArray());
            }
        }

        private void FindDiscrepanciesBetween(Column[] left, Column[] right)
        {
            EnsureBothArraysAreOfTheSameSize(ref left, ref right);
            AlignTheElementsOfBothArraysByName(ref left, ref right);

            // TODO: compare each column.
            for (int i = 0; i < left.Length; i++)
            {
            }
        }

        #endregion Private Members
    }
}