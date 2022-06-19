namespace Acklann.Daterpillar.Prototyping
{
    public readonly struct Username
    {
        public Username(string name, string value)
        {
            Name = name;
            Email = value;
        }

        public string Name { get; }

        public string Email { get; }
    }
}