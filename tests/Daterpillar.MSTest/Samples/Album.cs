namespace Acklann.Daterpillar.Samples
{
    [Table]
    public class Album
    {
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