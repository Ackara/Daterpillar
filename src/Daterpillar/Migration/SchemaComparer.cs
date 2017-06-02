﻿using Acklann.Daterpillar.Equality;
using Acklann.Daterpillar.Scripting;
using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Provides methods to compare and synchronize two <see cref="Schema"/>.
    /// </summary>
    public class SchemaComparer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaComparer"/> class.
        /// </summary>
        public SchemaComparer()
        {
        }

        /// <summary>
        /// Compares the specified schemas.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="builder">The builder.</param>
        /// <param name="modifications">The modifications.</param>
        /// <returns>MigrationState.</returns>
        public MigrationState Compare(Schema source, Schema target, IScriptBuilder builder, out string modifications)
        {
            _script = builder;
            _modifications = new List<string>();

            FindDiscrepanciesBetween(source.Tables.ToArray(), target.Tables.ToArray());
            ComputeResult(source, target, out MigrationState state);
            modifications = string.Join(Environment.NewLine, _modifications);

            return state;
        }

        /// <summary>
        /// Compares the specified schemas.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="builder">The builder.</param>
        /// <returns>MigrationState.</returns>
        public MigrationState Compare(Schema source, Schema target, IScriptBuilder builder)
        {
            return Compare(source, target, builder, out string modifications);
        }

        #region Private Members

        private IScriptBuilder _script;
        private IList<string> _modifications;

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
                    // Drop table from the right
                    _modifications.Add($"Remove: [{target.Name}] table.");
                    _script.Remove(target);
                }
                else if (source != null && target == null)
                {
                    // Add table from the left
                    _modifications.Add($"Add: [{source.Name}] table.");
                    _script.Append(source);
                }
                else if (source.Name != target.Name)
                {
                    // Drop table from the right
                    _modifications.Add($"Remove: [{target.Name}] table.");
                    _script.Remove(target);

                    // Add table from the left
                    _modifications.Add($"Add: [{source.Name}] table.");
                    _script.Append(source);
                }
                else
                {
                    FindDiscrepanciesBetween(left[i].Columns.ToArray(), target.Columns.ToArray());
                    FindDiscrepanciesBetween(left[i].ForeignKeys.ToArray(), target.ForeignKeys.ToArray());
                    FindDiscrepanciesBetween(left[i].Indexes.ToArray(), target.Indexes.ToArray());
                }
            }
        }

        private void FindDiscrepanciesBetween(Column[] left, Column[] right)
        {
            EnsureBothArraysAreTheSameSize(ref left, ref right);
            SortTheItemsOfBothArraysByName(ref left, ref right);

            var equalityChecker = new ColumnEqualityComparer();

            for (int i = 0; i < left.Length; i++)
            {
                Column source = left[i];
                Column target = right[i];

                if (source == null && target != null)
                {
                    // Drop the column on the right
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] column.");
                    _script.Remove(target);
                }
                else if (source != null && target == null)
                {
                    // Add the column on the right
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] column.");
                    _script.Append(source);
                }
                else if (source.Name != target.Name)
                {
                    // Replace the right with the left
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] column.");
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] column.");
                    _script.Remove(target);
                    _script.Append(source);
                }
                else if (!equalityChecker.Equals(source, target))
                {
                    // Change the right column to the left
                    _modifications.Add($"Alter: [{target.Table.Name}].[{target.Name}] column.");
                    _script.Update(target, source);
                }
            }
        }

        private void FindDiscrepanciesBetween(ForeignKey[] left, ForeignKey[] right)
        {
            EnsureBothArraysAreTheSameSize(ref left, ref right);
            SortTheItemsOfBothArraysByName(ref left, ref right);

            var equalityChecker = new ForeignKeyEqualityComparer();

            for (int i = 0; i < left.Length; i++)
            {
                ForeignKey source = left[i];
                ForeignKey target = right[i];

                if (source == null && target != null)
                {
                    // Drop the foreign key on the right
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] foreign key.");
                    _script.Remove(target);
                }
                else if (source != null && target == null)
                {
                    // Add the foreign key on the right
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] foreign key.");
                    _script.Append(source);
                }
                else if (!equalityChecker.Equals(source, target))
                {
                    // Replace the right with the left
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] foreign key.");
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] foreign key.");
                    _script.Remove(target);
                    _script.Append(source);
                }
            }
        }

        private void FindDiscrepanciesBetween(Index[] left, Index[] right)
        {
            EnsureBothArraysAreTheSameSize(ref left, ref right);
            SortTheItemsOfBothArraysByName(ref left, ref right);

            var equalityChecker = new IndexEqualityComparer();

            for (int i = 0; i < left.Length; i++)
            {
                Index source = left[i];
                Index target = right[i];

                if (source == null && target != null)
                {
                    // Drop the index on the right
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] index.");
                    _script.Remove(target);
                }
                else if (source != null && target == null)
                {
                    // Add the index on the right
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] index.");
                    _script.Append(source);
                }
                else if (!equalityChecker.Equals(source, target))
                {
                    // Replace the right with the left
                    _modifications.Add($"Remove: [{target.Table.Name}].[{target.Name}] index.");
                    _modifications.Add($"Add: [{source.Table.Name}].[{source.Name}] index.");
                    _script.Remove(target);
                    _script.Append(source);
                }
            }
        }

        private void ComputeResult(Schema source, Schema target, out MigrationState state)
        {
            state = MigrationState.NoChanges;
            int sourceObjectCount = source.Tables.Count;
            int targetObjectCount = target.Tables.Count;

            if (_modifications.Count == 0)
            {
                state = MigrationState.NoChanges;
            }
            else if (sourceObjectCount == 0 && targetObjectCount == 0)
            {
                state = MigrationState.SourceIsEmpty | MigrationState.TargetIsEmpty;
            }
            else if (sourceObjectCount == 0 && targetObjectCount > 0)
            {
                state = MigrationState.SourceIsEmpty | MigrationState.PendingChanges;
            }
            else if (sourceObjectCount > 0 && targetObjectCount == 0)
            {
                state = MigrationState.TargetIsEmpty | MigrationState.PendingChanges;
            }
            else if (sourceObjectCount != targetObjectCount)
            {
                state = MigrationState.PendingChanges;
            }
            else if (_modifications.Count > 0)
            {
                state = MigrationState.PendingChanges;
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