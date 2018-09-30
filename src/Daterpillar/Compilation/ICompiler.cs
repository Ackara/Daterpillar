using Acklann.Daterpillar.Configuration;
using System.Reflection;

namespace Acklann.Daterpillar.Compilation
{
    public interface ICompiler
    {
        Schema ToSchema(Assembly assembly);
    }
}