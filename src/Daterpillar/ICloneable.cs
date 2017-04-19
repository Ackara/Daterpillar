namespace Acklann.Daterpillar
{
    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value as an existing instance.
    /// </summary>
    /// <typeparam name="T">The method's return type.</typeparam>
    internal interface ICloneable<T>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}