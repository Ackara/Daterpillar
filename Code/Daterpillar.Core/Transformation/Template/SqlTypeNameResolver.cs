using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ackara.Daterpillar.Transformation.Template
{
    public class SqlTypeNameResolver : TypeNameResolverBase
    {
        public SqlTypeNameResolver() : base()
        {
            
        }

        public override string GetName(DataType dataType)
        {
            string name = "";
            string typeName = dataType.Name;

            switch (typeName)
            {
                case VARCHAR:
                    int size = dataType.Scale == 0? dataType.Precision : dataType.Scale;
                    name = $"{typeName}({size})";
                    break;

                case DECIMAL:
                    name = $"{typeName}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    name = TypeNameDictionary[typeName];
                    break;
            }

            return name;
        }
    }
}
