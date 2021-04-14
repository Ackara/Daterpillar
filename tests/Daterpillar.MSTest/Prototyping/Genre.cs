using Acklann.Daterpillar.Modeling.Attributes;
using System.ComponentModel;

namespace Acklann.Daterpillar.Prototyping
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