﻿using System;

namespace Acklann.Daterpillar.Attributes
{
    [AttributeUsage((AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct), AllowMultiple = false, Inherited = true)]
    public sealed class SqlIgnoreAttribute : Attribute { }
}