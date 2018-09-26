using Acklann.Daterpillar;
using Acklann.Daterpillar.Configuration;
using System;

namespace MSTest.Daterpillar.Fake
{
    [Table(Table)]
    public class SimpleTable
    {
        public const string
            Table = "table_with_no_constraints",
            CreatedOn = "Created_On";

        [Column(AutoIncrement = true)]
        public int Id { get; set; }

        [Column]
        [Index(IndexType.Index)]
        public string Name { get; set; }

        [Column("varchar", 1)]
        public string Sex { get; set; }

        [Column("body", "text")]
        public string Body { get; set; }

        [Column(CreatedOn)]
        public DateTime Date { get; set; }

        [Column("amount", "decimal", 10, 2)]
        public decimal Amount { get; set; }

        [Column]
        public float Rate { get; set; }

        [Column]
        public DayOfWeek Day { get; set; }

        [Column]
        public bool Exist { get; set; }
    }
}