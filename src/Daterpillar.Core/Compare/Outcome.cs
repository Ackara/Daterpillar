using System;

namespace Gigobyte.Daterpillar.Compare
{
    [Flags]
    public enum Outcomes
    {
        None = 0,
        Equal = 1,
        NotEqual = 2,
        SourceEmpty = 4,
        TargetEmpty = 8
    }
}