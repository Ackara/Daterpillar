using Acklann.Daterpillar.Annotations;
using System;

namespace Acklann.Daterpillar.Prototyping
{
    [Table(TABLE)]
    public class Contact
    {
        public const string TABLE = "contact";

        [Key, Column]
        public int Id { get; set; }

        [Column("first_name")]
        [Column("last_name")]
        public FullName Name { get; set; }

        public string Email { get; set; }

        public DayOfWeek DayBorn { get; set; }

        public TimeSpan TimeBorn { get; set; }

        public DateTime BirthDay { get; set; }
    }
}