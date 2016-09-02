using System;

namespace Gigobyte.Daterpillar.Commands
{
    [VerbLink(SyncVerb.Name)]
    public class SyncCommand : ICommand
    {
        public int Execute(object args)
        {
            throw new NotImplementedException();
        }
    }
}