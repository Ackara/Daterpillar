namespace Acklann.Daterpillar
{
    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value as an existing instance.
    /// </summary>
    /// <remarks>The ICloneable interface enables you to provide a customized implementation that creates a copy of an existing object. The ICloneable interface contains one member, the Clone method, which is intended to provide cloning support beyond that supplied by Object.MemberwiseClone.</remarks>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new T object that is a copy of this instance.</returns>
        T Clone();
    }
}