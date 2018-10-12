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
            WasHandled = false;
            OldValue = oldValue;
            NewValue = newValue;
            Children = new List<Discrepancy>();
        }

        public readonly ISQLObject OldValue;

        public readonly ISQLObject NewValue;

        public SqlAction Action { get; set; }

        public ISQLObject Value
        {
            get { return (NewValue != null ? NewValue : OldValue); }
        }

        public List<Discrepancy> Children { get; set; }

        internal bool WasHandled { get; set; }

        private string ToDebuggerDisplay()
        {
            return $"{Action} | {NewValue?.GetName()}[{Children?.Count}]";
        }
    }
}