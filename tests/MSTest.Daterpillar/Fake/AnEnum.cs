using Ackara.Daterpillar;

namespace MSTest.Daterpillar.Fake
{
    [Table("color")]
    public enum AnEnum
    {
        Red,
        Blue,
        Green,

        [EnumValue("Dark Gray")]
        DarkGray
    }
}