using Acklann.Daterpillar.Linq;
using System;
using System.Data;

namespace Acklann.Daterpillar
{
    public class User : IEntity
    {
        private EntityConstructor _ctor = null;

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

        public EntityConstructor GetConstructor()
        {
            if (_ctor == null) _ctor = EntityBase.CreateConstructor(GetType());
            return _ctor;
        }

        public void Load(IDataRecord record)
        {
            Id = Convert.ToInt32(record[nameof(Id)]);
            Name = Convert.ToString(record[nameof(Name)]);
            Email = Convert.ToString(record[nameof(Email)]);
        }
    }
}