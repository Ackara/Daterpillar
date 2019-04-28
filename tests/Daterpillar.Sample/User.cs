using Acklann.Daterpillar.Attributes;
using System;

namespace Acklann.Daterpillar
{
    [Table("user")]
    public partial class User
    {
        [Key]
        public int Id { get; set; }

        [Column("title", DefaultValue = "anon")]
        public string Name { get; set; }

        public DayOfWeek BirthDay { get; set; }

        public DateTime Dob { get; set; }

        public bool IsVerified { get; set; }

        [SqlIgnore]
        public string Danger { get; set; }
    }
}