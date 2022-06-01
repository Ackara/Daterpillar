using Acklann.Daterpillar.Annotations;

namespace Acklann.Daterpillar.Prototyping
{
    [Table]
    public class TrackSequence
    {
        [Key, Column(AutoIncrement = true)]
        public int Id { get; set; }

        [Column]
        public string SongId { get; set; }

        [Column]
        public string Disk { get; set; }
    }
}