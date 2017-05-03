﻿using System;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Indicates that a enum field represents a table record/row. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class EnumValueAttribute : Attribute
    {
        public EnumValueAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The value name.
        /// </summary>
        public readonly string Name;
    }
}