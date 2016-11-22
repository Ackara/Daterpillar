using Acklann.Daterpillar.TextTransformation;

namespace Acklann.Daterpillar.Migration
{
    public class SqlComparer
    {
        public SqlDiff Compare(IScriptBuilder builder, Schema source, Schema target)
        {
            _script = builder;
            _diff = new SqlDiff();

            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            SummarizeChanges(source, target);

            return _diff;
        }

        public SqlDiff Compare(IScriptBuilder builder, ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source)
            {
                using (target)
                {
                    return Compare(builder, source.FetchSchema(), target.FetchSchema());
                }
            }
        }

        #region Private Members

        private SqlDiff _diff;
        private IScriptBuilder _script;

        private void FindDiscrepanciesBetween(Table[] left, Table[] right)
        {
            EnsureBothArraysAreTheSameSize(ref left, ref right);
            SortTheItemsOfBothArraysByName(ref left, ref right);

            for (int i = 0; i < left.Length; i++)
            {
                Table source = left[i];
                Table target = right[i];

                if (source == null && target != null)
                {
                    // TODO: Drop table from the right
                    _diff.Changes++;
                    _script.Drop(target);
                }
                else if (source != null && target == null)
                {
                    // TODO: Add table from the left
                    _diff.Changes++;
                    _script.Create(source);
                }
                else if (source.Name != target.Name)
                {
                    // TODO: Drop table from the right
                    // TODO: Add table from the left
                    _diff.Changes++;
                    _script.Drop(target);
                    _script.Create(source);
                }
                else FindDiscrepanciesBetween(left[i].Columns.ToArray(), target.Columns.ToArray());
            }
        }

        private void FindDiscrepanciesBetween(Column[] left, Column[] right)
        {
            EnsureBothArraysAreTheSameSize(ref left, ref right);
            SortTheItemsOfBothArraysByName(ref left, ref right);

            var equalityChecker = new Equality.ColumnEqualityComparer();

            for (int i = 0; i < left.Length; i++)
            {
                Column source = left[i];
                Column target = right[i];

                if (source == null && target != null)
                {
                    // Drop the column on the right
                    _diff.Changes++;
                    _script.Drop(target);
                }
                else if (source != null && target == null)
                {
                    // Add the column on the right
                    _diff.Changes++;
                    _script.Create(source);
                }
                else if (source.Name != target.Name)
                {
                    // Replace the right with the left
                    _diff.Changes += 2;
                    _script.Drop(target);
                    _script.Create(source);
                }
                else if (equalityChecker.Equals(source, target))
                {
                    // Change the right column to the left
                    _diff.Changes++;
                    _script.AlterTable(source, target);
                }
            }
        }

        private void SummarizeChanges(Schema source, Schema target)
        {
            int sourceObjectCount = source.Tables.Count;
            int targetObjectCount = target.Tables.Count;

            if (_diff.Changes == 0)
            {
                _diff.Summary = SqlDiffSummary.Equal;
            }
            else if (sourceObjectCount == 0 && targetObjectCount > 0)
            {
                _diff.Summary = SqlDiffSummary.SourceEmpty | SqlDiffSummary.NotEqual;
            }
            else if (sourceObjectCount > 0 && targetObjectCount == 0)
            {
                _diff.Summary = SqlDiffSummary.TargetEmpty | SqlDiffSummary.NotEqual;
            }
            else if (sourceObjectCount != targetObjectCount)
            {
                _diff.Summary = SqlDiffSummary.NotEqual;
            }
            else if (_diff.Changes > 0)
            {
                _diff.Summary = SqlDiffSummary.NotEqual;
            }
        }

        // Helper Methods

        private void EnsureBothArraysAreTheSameSize<T>(ref T[] left, ref T[] right)
        {
            if (left.Length > right.Length)
                IncreaseLengthOfArray(ref right, left.Length);
            else if (left.Length < right.Length)
                IncreaseLengthOfArray(ref left, right.Length);
        }

        private void SortTheItemsOfBothArraysByName<T>(ref T[] left, ref T[] right)
        {
            dynamic l;
            for (int i = 0; i < left.Length; i++)
            {
                l = left[i];
                SwapMatchingItems(ref right, (l?.Name), i);
            }
        }

        private void IncreaseLengthOfArray<T>(ref T[] array, int capacity)
        {
            var newArray = new T[capacity];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            array = newArray;
        }

        private void SwapMatchingItems<T>(ref T[] right, string name, int targetIdx)
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

        #endregion Private Members
    }
}