namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Foreign key collision rule.
    /// </summary>
    public enum ForeignKeyRule
    {
        NONE,
        CASCADE,
        SET_NULL,
        SET_DEFAULT,
        RESTRICT
    }
}