﻿using System;

namespace Gigobyte.Daterpillar.Annotation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        public ColumnAttribute(string name)
        {
            Name = name;
        }

        public readonly string Name;

        public bool IsKey { get; set; }

        public bool AutoIncrement { get; set; }
    }
}