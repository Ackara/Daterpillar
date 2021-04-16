using Acklann.Daterpillar.Modeling.Attributes;
using System;

namespace Acklann.Daterpillar.Prototyping
{
    [Table("artist")]
    public class Artist: Modeling.DataRecord
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