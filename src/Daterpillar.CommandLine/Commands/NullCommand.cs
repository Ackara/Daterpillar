namespace Gigobyte.Daterpillar.Commands
{
    public class NullCommand : ICommand
    {
        public int Execute(object args)
        {
            return 0;
        }
    }
}