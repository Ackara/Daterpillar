using System;

namespace Acklann.Daterpillar.Fakes
{
    [Table("country")]
    public class Country
    {
        [Column("country_id", TypeName = "smallInt")]
        public int Id { get; set; }

        [Column("country", 50)]
        public string Name { get; set; }

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }
    }
}