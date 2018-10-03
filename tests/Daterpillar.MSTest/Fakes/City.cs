using System;

namespace Acklann.Daterpillar.Fakes
{
    [Table("city")]
    public class City
    {
        [Column("city_id", AutoIncrement = true)]
        public int Id { get; set; }

        [Column("city", 50)]
        public string Name { get; set; }

        [Column("country_id")]
        [ForeignKey("country", "country_id")]
        public int CountryId { get; set; }

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }
    }
}