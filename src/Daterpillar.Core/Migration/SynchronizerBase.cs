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
            FindDiscrepanciesBetween(source.Tables, source.Tables);

            return _scriptBuilder.GetContentAsBytes();
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

        private static void GetTheItemsOfBothCollectionsAlignedByNameInAnArray<T>(IList<T> source, IList<T> target)
        {
            int maxLength = ((source.Count >= target.Count) ? source.Count : target.Count);
            var leftArray = new T[maxLength];
            var rightArray = new T[maxLength];

            source.CopyTo(leftArray, 0);
            target.CopyTo(rightArray, 0);
            
            dynamic lItem, rItem, temp;

            for (int i = 0; i < maxLength; i++)
            {
                lItem = leftArray[i];
                if (lItem == null) continue;

                for (int n = i; n < maxLength; n++)
                {
                    rItem = rightArray[i];
                    if(lItem.Name == rItem?.Name)
                    {
                        temp = lItem;
                        lItem = rItem;
                        rItem = temp;
                    }
                }
            }

            source = new List<T>(leftArray);
            target = new List<T>(rightArray);
        }

        private void FindDiscrepanciesBetween(IList<Table> source, IList<Table> target)
        {
            GetTheItemsOfBothCollectionsAlignedByNameInAnArray(source, target);
            
            string srcName, tgtName;
            for (int i = 0; i < source.Count; i++)
            {
                srcName = source[i]?.Name;
                tgtName = target[i]?.Name;

                if (srcName == tgtName)
                {
                    // TODO: Compare columns
                    // TODO: Compare indexes
                    // TODO: Compare foreign keys
                }
                else if (srcName == null && tgtName != null)
                {
                    // TODO: Drop table on the right
                }
                else if (srcName != null && tgtName == null)
                {
                    // TODO: Add table on the left
                }
                else if (srcName != null && tgtName != null)
                {
                    // TODO: Drop table on the right
                    // TODO: Add table on the left
                }
            }
        }

        #endregion Private Members
    }
}
