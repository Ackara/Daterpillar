using Acklann.Daterpillar.Annotations;

namespace Acklann.Daterpillar.Prototyping
{
    [View("playlist", "select {1}, {2} from {0};", nameof(Song), nameof(Song.Title), nameof(Song.Length))]
    public class Playlist
    {
        public string Title { get; set; }

        public int Length { get; set; }
    }
}