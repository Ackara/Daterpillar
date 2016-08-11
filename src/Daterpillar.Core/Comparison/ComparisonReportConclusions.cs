namespace Gigobyte.Daterpillar.Compare
{
    [System.Flags]
    public enum ComparisonReportConclusions
    {
        None = 0x0,
        NotEqual = 0x1,
        Equal = 0x2,
        SourceEmpty = 0x4,
        TargetEmpty = 0x8
    }
}