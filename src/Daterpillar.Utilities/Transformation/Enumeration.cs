using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Transformation
{
    public class Enumeration
    {
        public string Name { get; set; }

        public ICollection<KeyValuePair<string, int>> Values { get; set; }
    }
}