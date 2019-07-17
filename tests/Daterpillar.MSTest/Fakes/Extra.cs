using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Fakes
{
    [Table]
    public class Extra
    {
        [Key]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Index, Column]
        public string PlanId { get; set; }

        [Index, Column]
        public string OwnerId { get; set; }
    }
}