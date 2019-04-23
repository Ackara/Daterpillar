using Acklann.Daterpillar.Attributes;
using System;

namespace Acklann.Daterpillar.Fakes
{
    [Table("artist")]
    public class Artist
    {
        [Column(AutoIncrement = true)]
        public int Id { get; set; }

        [Index, Column]
        public string Name { get; set; }

        [Column]
        public string Bio { get; set; }

        [Column]
        public DateTime DOB { get; set; }
    }
}