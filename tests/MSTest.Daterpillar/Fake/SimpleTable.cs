using Ackara.Daterpillar;
using System;

namespace MSTest.Daterpillar.Fake
{
    [Table(Table)]
    public class SimpleTable
    {
        public const string Table = "table_with_no_constraints";

        [Column("Id", AutoIncrement = true)]
        public int Id { get; set; }

        [Column]
        [Index(IndexType.Index)]
        public string Name { get; set; }

        [Column("Sex", "varchar", 1)]
        public string Sex { get; set; }

        [Column("body", "text")]
        public string Body { get; set; }

        [Column("Created_On")]
        public DateTime Date { get; set; }

        [Column("amount", "decimal", 10, 2)]
        public decimal Amount { get; set; }

        [Column]
        public float Rate { get; set; }

        [Column]
        public DayOfWeek Day { get; set; }
    }
}