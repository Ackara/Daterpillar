using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataRecord : DataViewRecord, IInsertable
    {
        public DataRecord()
        {
            ColumnMap.Register(GetType());
        }

        public string GetTableName() => TableName;

        public virtual string[] GetColumns()
        {
            return ColumnMap.GetColumns(GetTableName());
        }

        public virtual object[] GetValues()
        {
            MemberInfo[] members = ColumnMap.GetMembers(GetTableName());
            object[] results = new object[members.Length];

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = WriteValue(members[i]);
            }

            return results;
        }

        protected virtual object WriteValue(PropertyInfo member)
        {
            return Linq.SqlComposer.Serialize(member.GetValue(this));
        }

        protected virtual object WriteValue(FieldInfo member)
        {
            return Linq.SqlComposer.Serialize(member.GetValue(this));
        }

        protected object WriteValue(MemberInfo member)
        {
            if (member is PropertyInfo property)
                return WriteValue(property);
            else if (member is FieldInfo field)
                return WriteValue(field);
            else
                return null;
        }

        #region Backing Members

        private string GetKey(string item) => string.Concat(GetTableName(), '.', item);

        #endregion Backing Members
    }
}