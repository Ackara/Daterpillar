﻿namespace Acklann.Daterpillar.Migration
{
    [System.Flags]
    public enum SqlDiffSummary
    {
        None = 0x0,
        Equal = 0x1,
        NotEqual = 0x2,
        SourceEmpty = 0x4,
        TargetEmpty = 0x8
    }
}