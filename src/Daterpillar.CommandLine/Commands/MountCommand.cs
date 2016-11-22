using System;

namespace Acklann.Daterpillar.Commands
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