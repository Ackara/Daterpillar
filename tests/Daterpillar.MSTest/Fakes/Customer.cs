using System;

namespace Acklann.Daterpillar.Fakes
{
    [Table("customer")]
    public class Customer
    {
        [Column("customer_id", AutoIncrement = true)]
        public int CustomerId { get; set; }

        [Column("store_id")]
        [DataType(SchemaType.TINYINT)]
        public int StoreId { get; set; }

        [Column("first_name", 45)]
        public string FirstName { get; set; }

        [Column("last_name", 45)]
        public string LastName { get; set; }

        [Column("email", 50, Nullable = true)]
        public string Email { get; set; }

        [Column("address_id")]
        [ForeignKey(typeof(Address), nameof(Address.Id))]
        public int AddressId { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("create_date")]
        public DateTime CreateDate { get; set; }

        [Column("last_update")]
        public DateTime? LastUpdate { get; set; }
    }
}