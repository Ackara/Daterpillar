using Acklann.Daterpillar.Annotations;

namespace Acklann.Daterpillar.Prototyping
{
    [Table("contact")]
    public class Complex
    {
        [Key, Column("id")]
        public string Id { get; set; }

        [Column("first_name")]
        [Column("last_name")]
        public FullName Name { get; set; }
    }
}