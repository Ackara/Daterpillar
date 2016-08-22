using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gigobyte.Daterpillar.Arguments
{
    public class CompareVerb
    {
        [Option('s', "source", Required = true, HelpText = "The source connection string.")]
        public string Source { get; set; }

        [Option('t', "target", Required = true, HelpText = "The target connection string.")]
        public string Target { get; set; }

        [Option('o', "outfile", Required = true, HelpText = "The path in which to store the results.")]
        public string OutFile { get; set; }

        [Option('p', "platform", Required = true, HelpText = "The database platform in which to target.")]
        public ConnectionType Provider { get; set; }
    }
}
