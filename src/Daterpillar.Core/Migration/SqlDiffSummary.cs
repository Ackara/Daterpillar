namespace Acklann.Daterpillar.Migration
{
    [System.Flags]
    public enum SqlDiffSummary
    {
        Equal = 0x0,
        NotEqual = 0x1,
        SourceEmpty = 0x2,
        TargetEmpty = 0x4
    }
}