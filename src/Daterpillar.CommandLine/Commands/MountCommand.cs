using System;

namespace Gigobyte.Daterpillar.Commands
{
    [VerbLink(MountVerb.Name)]
    public class MountCommand : ICommand
    {
        public int Execute(object args)
        {
            throw new NotImplementedException();
        }
    }
}