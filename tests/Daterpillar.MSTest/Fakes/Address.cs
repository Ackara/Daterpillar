using System;

namespace Acklann.Daterpillar.Fakes
{
    [Table]
    public class Address
    {
        [Column("address_id", AutoIncrement = true)]
        public int Id { get; set; }

        [Column("address", scale: 50)]
        public string Street1 { get; set; }

        [Column("address2", scale: 50, Nullable = true)]
        public string Street2 { get; set; }

        [Column("district", 20)]
        public string District { get; set; }

        [Column("city_id")]
        [ForeignKey(typeof(City), nameof(City.Id))]
        public int CityId { get; set; }

        [Column("postal_code", 10, DefaultValue = "00818")]
        public string PostalCode { get; set; }

        [Column("phone")]
        [DataType(SchemaType.VARCHAR, 20)]
        public string Phone { get; set; }

        [Column("last_update")]
        public DateTime LastUpdate { get; set; }
    }
}