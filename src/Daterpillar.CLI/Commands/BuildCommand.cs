﻿using Acklann.Daterpillar.Configuration;
using Acklann.GlobN;
using Acklann.NShellit.Attributes;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Commands
{
    [Command("build", Cmdlet = "Invoke-DSBuild")]
    [Summary("Compiles an assembly file (.dll) to a schema (.schema.xml)")]
    public class BuildCommand : ICommand
    {
        [UseConstructor]
        public BuildCommand(string targetAssembly)
        {
            TargetAssembly = targetAssembly.ResolvePath(Directory.GetCurrentDirectory()).FirstOrDefault();
        }

        [Required, Parameter('t', "Path", Kind = "path")]
        [Summary("The absolute or relative path to an assembly file.")]
        public string TargetAssembly { get; }

        public int Execute()
        {
            if (File.Exists(TargetAssembly) == false)
                throw new FileNotFoundException(Error.FileNotFound(TargetAssembly, "assembly"), TargetAssembly);
            
            Schema schema = Compilation.SchemaConvert.ToSchema(TargetAssembly);
            schema.Save(Path.ChangeExtension(TargetAssembly, ".schema.xml"));
            return 0;
        }
    }
}