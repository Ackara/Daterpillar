namespace Gigobyte.Daterpillar.Compare
{
    [System.Flags]
    public enum ReportConclusions
    {
        NotEqual = 0,
        Equal = 1,
        SourceEmpty = 2,
        TargetEmpty = 4
    }
}