using Acklann.Daterpillar.Modeling.Attributes;
using System.ComponentModel;

namespace Acklann.Daterpillar.Prototyping
{
    [Table]
    public enum Genre
    {
        [EnumValue("Hip Hop")]
        HipHip,

        Pop,

        Country,

        [EnumValue("Rock n Roll")]
        Rock
    }
}