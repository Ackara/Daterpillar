using Acklann.Daterpillar.Linq;
using System;
using System.Data;

namespace Acklann.Daterpillar.Prototyping
{
    public class Contact : IEntity
    {
        public const string TABLE = "contact";

        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DayOfWeek DayBorn { get; set; }

        public TimeSpan TimeBorn { get; set; }

        public DateTime BirthDay { get; set; }

        public string GetTableName()
        {
            return nameof(Contact);
        }

        public string[] GetColumnList()
        {
            return new string[] { nameof(Id), nameof(Name), nameof(Email), nameof(DayBorn), nameof(TimeBorn), nameof(BirthDay) };
        }

        public object[] GetValueList()
        {
            return new object[] { Id, $"'{Name}'", $"'{Email}'", (int)DayBorn, $"'{TimeBorn.ToString(@"hh\:mm\:ss")}'", $"'{BirthDay.ToString("yyyy-MM-dd HH:mm:ss")}'" };
        }

        public void Load(IDataRecord record)
        {
            Id = (int)record[0];
            Name = (string)record[1];
            Email = Convert.ToString(record[nameof(Email)]);
            DayBorn = (DayOfWeek)record[3];
            TimeBorn = Convert.ToDateTime(record[4]).TimeOfDay;
        }
    }
}