using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Acklann.Daterpillar.Build
{
    public class Class1 : ITask
    {
        public ITaskHost HostObject { get; set; }
        
        public IBuildEngine BuildEngine { get; set; }

        [Output]
        public string Result { get; set; }

        
    }
}
