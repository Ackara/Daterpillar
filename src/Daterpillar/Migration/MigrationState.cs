namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Describes the current state between two SQL schemas.
    /// </summary>
    [System.Flags]
    public enum MigrationState
    {
        NoChanges = 0,
        PendingChanges = 1,
        SourceIsEmpty = 2,
        TargetIsEmpty = 4
    }
}