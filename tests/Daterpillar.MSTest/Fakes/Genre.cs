using System.ComponentModel;

namespace Acklann.Daterpillar.Fakes
{
    [Table, DisplayName("genre")]
    public enum Genre
    {
        [DisplayName("Hip Hop")]
        HipHip,

        Pop,

        Country,

        [EnumValue("Rock n Roll")]
        Rock
    }
}