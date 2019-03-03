using Acklann.Daterpillar.Configuration;
using System;

namespace Acklann.Daterpillar.Migration
{
    public static class EnumConverter
    {
        public static bool TryConvertToLanguage(string value, out Syntax lanaguage)
        {
            foreach (string e in Enum.GetNames(typeof(Syntax)))
                if (string.Equals(value, e, StringComparison.OrdinalIgnoreCase))
                {
                    lanaguage = (Syntax)Enum.Parse(typeof(Syntax), e);
                    return true;
                }

            lanaguage = Syntax.Generic;
            return false;
        }
    }
}