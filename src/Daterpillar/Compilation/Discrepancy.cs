using Acklann.Daterpillar.Configuration;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Compilation
{
    [System.Diagnostics.DebuggerDisplay("{ToDebuggerDisplay()}")]
    public class Discrepancy
    {
        public Discrepancy(ISQLObject oldValue, ISQLObject newValue) : this(SqlAction.None, oldValue, newValue)
        {
            if (oldValue == null && newValue == null)
                Action = SqlAction.None;
            else if (oldValue == null)
                Action = SqlAction.Create;
            else if (newValue == null)
                Action = SqlAction.Drop;
            else
                Action = SqlAction.Alter;
        }

        public Discrepancy(SqlAction action, ISQLObject oldValue, ISQLObject newValue)
        {
            Action = action;
            OldValue = oldValue;
            NewValue = newValue;
            Children = new List<Discrepancy>();
        }

        public readonly ISQLObject OldValue;

        public readonly ISQLObject NewValue;

        public SqlAction Action { get; set; }

        public ISQLObject Value
        {
            get
            {
                if (OldValue == null && NewValue == null)
                    return null;
                else if (NewValue == null || Action == SqlAction.Drop)
                    return OldValue;
                else
                    return NewValue;
            }
        }

        public List<Discrepancy> Children { get; set; }

        public void Add(SqlAction action, ISQLObject oldValue, ISQLObject newValue)
        {
            Children.Add(new Discrepancy(action, oldValue, newValue));
        }

        internal int GetWeight()
        {
            int weight = 0;

            if (Action == SqlAction.Create) weight += 1;
            else if (Action == SqlAction.Drop) weight += 2;

            if (Value is Index) weight += 2;
            else if (Value is ForeignKey) weight += 1;
            else if ((Action == SqlAction.Create || Action == SqlAction.Alter) && Value is Column) weight = 5;

            return weight;
        }

        internal void Sort()
        {
            Children.Sort(compare);

            int compare(Discrepancy x, Discrepancy y)
            {
                if (x.GetWeight() > y.GetWeight())
                    return -1;
                else if (x.GetWeight() < y.GetWeight())
                    return 1;
                else
                    return 0;
            }
        }

        private string ToDebuggerDisplay()
        {
            return $"{Action} {Value.GetType().Name} | {Value.GetName()}[{Children?.Count}]";
        }
    }
}