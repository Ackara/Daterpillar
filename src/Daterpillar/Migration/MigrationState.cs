namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Describes the current state between two SQL schemas.
    /// </summary>
    [System.Flags]
    public enum MigrationState
    {
        /// <summary>
        /// The schemas are identical.
        /// </summary>
        NoChanges = 1,

        /// <summary>
        /// The schemas are not identical.
        /// </summary>
        PendingChanges = 2,

        /// <summary>
        /// The source is empty.
        /// </summary>
        SourceIsEmpty = 4,

        /// <summary>
        /// The target is empty.
        /// </summary>
        TargetIsEmpty = 8
    }
}