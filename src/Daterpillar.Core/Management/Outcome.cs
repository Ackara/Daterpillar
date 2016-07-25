using System;

namespace Gigobyte.Daterpillar.Management
{
    [Flags]
    public enum Outcome
    {
        Equal = 2,
        NotEqual = 4,
        SourceEmpty = 8,
        TargetEmpty = 16
    }
}