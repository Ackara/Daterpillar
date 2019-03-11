﻿using System;

namespace Acklann.Daterpillar
{
    [AttributeUsage((AttributeTargets.Assembly), AllowMultiple = true, Inherited = false)]
    public sealed class IncludeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IncludeAttribute"/> class.
        /// </summary>
        /// <param name="relativePath">The relative/absolute path of the '.schema.xml' file.</param>
        public IncludeAttribute(string relativePath)
        {
            FilePath = relativePath;
        }

        public readonly string FilePath;
    }
}