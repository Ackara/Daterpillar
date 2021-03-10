using Acklann.Daterpillar.Attributes;

namespace Acklann.Daterpillar.Prototyping
{
    [Table]
    public class Album
    {
        public const string album = "album";

        [SqlIgnore]
        public int TotalTracks;

        public string Name { get; set; }

        [Key]
        [ForeignKey("artist", nameof(Artist.Id))]
        public int ArtistId { get; set; }

        [Key]
        [ForeignKey(typeof(Song), nameof(Song.Id))]
        public int SongId { get; set; }

        public int Year { get; set; }
    }
}