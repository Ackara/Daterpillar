using Acklann.Daterpillar.Linq;
using System;
using System.Data;

namespace Acklann.Daterpillar.Fakes
{
    public class Contact : EntityBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public override string GetTableName()
        {
            return nameof(Contact);
        }

        public override string[] GetColumnList()
        {
            return new string[] { nameof(Id), nameof(Name), nameof(Email) };
        }

        public override object[] GetValueList()
        {
            return new object[] { Id, $"'{Name}'", $"'{Email}'" };
        }

        public override void Load(IDataRecord record)
        {
            Id = record.GetInt32(0);
            Name = record.GetString(1);
            Email = Convert.ToString(record[nameof(Email)]);
        }
    }
}