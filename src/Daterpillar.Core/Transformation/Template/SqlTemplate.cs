using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class SqlTemplate : ITemplate
    {
        public SqlTemplate() : this(SqlTemplateSettings.Default, new SqlTypeNameResolver())
        {
        }

        public SqlTemplate(SqlTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            _settings = settings;
            _typeNameResolver = typeResolver;
        }

        public string Transform(Schema schema)
        {
            throw new NotImplementedException();
        }

        #region Private Members

        private SqlTemplateSettings _settings;
        private ITypeNameResolver _typeNameResolver;

        #endregion
    }
}
