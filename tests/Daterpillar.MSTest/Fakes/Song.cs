using Acklann.Daterpillar.Attributes;
using System.ComponentModel;

namespace Acklann.Daterpillar.Fakes
{
    [StaticId(100)]
    [Table, DisplayName("song")]
    public class Song
    {
        [Column("Id", AutoIncrement = true)]
        public int Id { get; set; }

        [StaticId(201), Column("Title")]
        public string Title { get; set; }

        [Column("Length")]
        public int Length { get; set; }

        [Column("Artists")]
        public string ContributingArtists { get; set; }

        [Column("Album")]
        public string Album { get; set; }

        [StaticId(205), DisplayName("Track"), DefaultValue(1)]
        public int TrackNo { get; set; }

        [Column("Disc", DefaultValue = 1)]
        public int DiscNo { get; set; }

        [Column("Genre")]
        [ForeignKey(typeof(Genre))]
        public Genre Genre { get; set; }
    }
}