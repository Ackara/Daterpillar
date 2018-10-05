﻿using System;

namespace Acklann.Daterpillar
{
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = false, Inherited = true)]
    public class SUIDAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SUIDAttribute"/> class.
        /// </summary>
        /// <param name="id">The unique identifier.</param>
        public SUIDAttribute(int id)
        {
            Id = id;
        }

        /// <summary>
        /// The unique identifier
        /// </summary>
        public readonly int Id;
    }
}