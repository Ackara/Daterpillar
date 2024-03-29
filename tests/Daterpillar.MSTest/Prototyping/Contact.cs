using Acklann.Daterpillar.Annotations;
using System;

namespace Acklann.Daterpillar.Prototyping
{
    [Table("contact")]
    public class Contact
    {
        [Key, Column]
        public int Id { get; set; }

        [Column("first_name")]
        [Column("last_name")]
        public FullName Name { get; set; }

        [Column("user_alias")]
        [Column("user_id")]
        public Username UserId { get; set; }

        public string Email { get; set; }

        public DayOfWeek DayBorn { get; set; }

        public TimeSpan TimeBorn { get; set; }

        public DateTime BirthDay { get; set; }

        [Column("secret")]
        private string _secret;
    }
}