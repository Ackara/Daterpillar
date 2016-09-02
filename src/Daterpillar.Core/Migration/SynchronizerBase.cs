using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;
using System;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Migration
{
    public abstract class SynchronizerBase : ISynchronizer
    {
        public byte[] GenerateScript(Schema source, Schema target)
        {
            a(source.Tables, source.Tables);
            throw new NotImplementedException();
        }

        public byte[] GenerateScript(ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source) { using (target) { return GenerateScript(source.FetchSchema(), target.FetchSchema()); } }
        }

        #region Private Members

        private IScriptBuilder _scriptBuilder;

        private static void IncreaseLengthOfArray<T>(ref T[] array, int capacity)
        {
            var newArray = new T[capacity];
            for (int i = 0; i < array.Length; i++)
            {
                newArray[i] = array[i];
            }

            array = newArray;
        }

        private static void EnsureBothArraysAreOfTheSameSize<T>(ref T[] left, ref T[] right)
        {
            if (left.Length > right.Length)
                IncreaseLengthOfArray(ref right, left.Length);
            else if (left.Length < right.Length)
                IncreaseLengthOfArray(ref left, right.Length);
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

        private static void GetTheItemsOfBothCollectionsAlignedByNameInAnArray<T>(ICollection<T> source, ICollection<T> target, out T[] leftArray, out T[] rightArray)
        {
            int maxLength = ((source.Count >= target.Count) ? source.Count : target.Count);
            leftArray = new T[maxLength];
            rightArray = new T[maxLength];

            source.CopyTo(leftArray, 0);
            target.CopyTo(rightArray, 0);

            dynamic lItem, rItem;
            string lName, rName;
            for (int i = 0; i < maxLength; i++)
            {
                lItem = leftArray[i];
                rItem = rightArray[i];

                for (int n = 0; n < (maxLength - i); n++)
                {

                }
            }
            throw new System.NotImplementedException();
        }

        private void a(ICollection<Table> source, ICollection<Table> target)
        {
            Table[] left, right;
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target, out left, out right);

            string lName, rName;
            for (int i = 0; i < left.Length; i++)
            {
                lName = left[i]?.Name;
                rName = right[i]?.Name;

                if (lName == rName)
                {
                    // TODO: Compare columns
                    // TODO: Compare indexes
                    // TODO: Compare foreign keys
                }
                else if (lName == null && rName != null)
                {
                    // TODO: Drop table on the right
                }
                else if (lName != null && rName == null)
                {
                    // TODO: Add table on the left
                }
                else if (lName != null && rName != null)
                {
                    // TODO: Drop table on the right
                    // TODO: Add table on the left
                }
            }
        }

        #endregion Private Members
    }
}