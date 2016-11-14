using Gigobyte.Daterpillar.TextTransformation;
using System.Text;

namespace Gigobyte.Daterpillar.Migration
{
    public class SqlComparer
    {
        public SqlDiff Compare(IScriptBuilder builder, Schema source, Schema target)
        {
            _changes = new SqlDiff();
            
            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            SummarizeReport(source, target);

            return _changes;
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

        protected virtual void FindDiscrepanciesBetween(Table[] left, Table[] right)
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
                    _script.Drop(target);
                    _changes.Discrepancies++;
                }
                else if (source != null && target == null)
                {
                    // TODO: Add table from the left
                    _script.Create(source);
                    _changes.Discrepancies++;
                }
                else if (source.Name != target.Name)
                {
                    // TODO: Drop table from the right
                    // TODO: Add table from the left
                    _script.Drop(target);
                    _script.Create(source);
                    _changes.Discrepancies++;
                }
                else FindDiscrepanciesBetween(left[i].Columns.ToArray(), target.Columns.ToArray());
            }
        }

        protected virtual void FindDiscrepanciesBetween(Column[] left, Column[] right)
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
                    _script.Drop(target);
                }
                else if (source != null && target == null)
                {
                    // Add the column on the right
                    _script.Create(source);
                }
                else if (source.Name != target.Name)
                {
                    // Replace the right with the left
                    _script.Drop(target);
                    _script.Create(source);
                }
                else if (equalityChecker.Equals(source, target))
                {
                    // Change the right column to the left
                    _script.AlterTable(source, target);
                }
            }
        }

        #region Private Members

        private SqlDiff _changes;
        private IScriptBuilder _script;

        private static void EnsureBothArraysAreTheSameSize<T>(ref T[] left, ref T[] right)
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

        private static void SortTheItemsOfBothArraysByName<T>(ref T[] left, ref T[] right)
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

        private void SummarizeReport(Schema source, Schema target)
        {
        }

        #endregion Private Members
    }
}