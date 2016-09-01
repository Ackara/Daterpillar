using Gigobyte.Daterpillar.TextTransformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigobyte.Daterpillar.Migration
{
    public interface ISynchronizer
    {
        byte[] GenerateScript(Schema schema);
    }
}
