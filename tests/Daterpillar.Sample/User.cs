namespace Acklann.Daterpillar
{
    [Table]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Column(DefaultValue = "anon")]
        public string Name { get; set; }
    }
}