﻿namespace Gigobyte.Daterpillar.Commands
{
    public class NullCommand : ICommand
    {
        public int Execute(object args) => ExitCode.Success;
    }
}