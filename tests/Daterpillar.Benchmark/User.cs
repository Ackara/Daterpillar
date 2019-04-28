using Acklann.Daterpillar.Linq;
using System;
using System.Data;

namespace Acklann.Daterpillar
{
    public class User : IEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string GetTableName()
        {
            return nameof(User);
        }

        public string[] GetColumnList()
        {
            return new string[] { nameof(Id), nameof(Name), nameof(Email) };
        }

        public object[] GetValueList()
        {
            return new object[] { Id, $"'{Name}'", $"'{Email}'" };
        }

        public void Load(IDataRecord record)
        {
            Id = (int)(record[nameof(Id)]);
            Name = (string)(record[nameof(Name)]);
            Email = (string)(record[nameof(Email)]);
        }
    }
}