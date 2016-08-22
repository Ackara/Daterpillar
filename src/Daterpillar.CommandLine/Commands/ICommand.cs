namespace Gigobyte.Daterpillar.Commands
{
    public interface ICommand
    {
        int Execute(object args);
    }
}