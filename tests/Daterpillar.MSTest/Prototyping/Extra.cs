using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;

namespace Acklann.Daterpillar.Prototyping
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