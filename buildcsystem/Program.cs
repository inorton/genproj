using System;
using System.IO;
using Mono.Options;
using BuildCSystem.Projects;
using System.Collections.Generic;

namespace buildcsystem
{
    class MainClass
    {


        public static int Main(string[] args)
        {
            string projfile = null;
            bool createnew = false;
            string asmname = null;
            string asmnamespace = null;

            List<string> sources = new List<string>();
            List<string> contents = new List<string>();
            List<string> nones = new List<string>();
            List<string> resources = new List<string>();
            List<string> projrefs = new List<string>();
            List<string> asmrefs = new List<string>();

            string toolsversion = null;

            bool dohelp = true;

            var opts = new OptionSet();
            opts.Add("csproj=", "csproj file to create/update", 
                     (string x) => projfile = x);

            opts.Add("create", "create a new blank file",
                     (x) => createnew = true);

            opts.Add("name=", "set the output assembly name",
                     (string x) => asmname = x);

            opts.Add("namespace=", "set the default assembly namespace",
                     (string x) => asmnamespace = x);

            opts.Add("toolsversion=", "set the ToolsVersion value",
                     (string x) => toolsversion = x);

            opts.Add("cs=", "add a c# source file to compile",
                     (string x) => sources.Add(Path.GetFullPath(x)));

            opts.Add("content=", "add a 'content' file (copy to output dir=true)",
                     (string x) => contents.Add(Path.GetFullPath(x)));

            opts.Add("none=", "add a 'none' file (not copied to output)",
                     (string x) => nones.Add(Path.GetFullPath(x)));

            opts.Add("resource=", "add a file an embedded resource",
                     (string x) => resources.Add(x));

            opts.Add("projectref=", "reference another project file",
                     (string x) => projrefs.Add(Path.GetFullPath(x)));

            opts.Add("asmref=", "reference an assembly (eg 'System.Web' or '/opt/nunit/nunit.framework.dll'",
                     (string x) => asmrefs.Add(x));


            opts.Parse(args);

            if (!string.IsNullOrEmpty(projfile))
                dohelp = false;

            if (dohelp)
            {
                Console.Out.WriteLine("Usage: buildcsystem [OPTIONS]");
                opts.WriteOptionDescriptions(Console.Out);
                return -1;
            }

            var pe = new ProjectEditor();
            pe.ProjectFolder = Path.GetDirectoryName( Path.GetFullPath(projfile));

            if (!File.Exists(projfile))
                createnew = true;

            if (!createnew)
            {
                using (var sf = new StreamReader(projfile))
                    pe.Load(sf);
            }

            if (toolsversion != null)
            {
                pe.ToolsVersion = toolsversion;
            }

            if (pe.AssemblyName == "Untitled")
            {
                if (string.IsNullOrEmpty(asmname))
                {
                    asmname = Path.GetFileNameWithoutExtension(projfile);
                }
                pe.AssemblyName = asmname;
            }

            if (pe.RootNameSpace == "Untitled")
            {
                if (string.IsNullOrEmpty(asmnamespace))
                {
                    asmnamespace = Path.GetFileNameWithoutExtension(projfile);
                }
                pe.RootNameSpace = asmnamespace;
            }

            foreach (var a in asmrefs)
            {
                if (File.Exists(a))
                {
                    var name = Path.GetFileNameWithoutExtension(a);
                    pe.AddReference(name, Path.GetFullPath(a));
                } else
                {
                    pe.AddReference(a, null);
                }
            }

            foreach (var p in projrefs)
            {
                pe.AddProjectReference(p);
            }

            foreach (var s in sources)
            {            
                pe.AddCompileSource(Path.GetFullPath(s));
            }

            foreach (var n in nones)
                pe.AddNone(Path.GetFullPath(n));

            foreach (var c in contents)
                pe.AddContent(Path.GetFullPath(c));

            foreach (var e in resources)
            {
                if (File.Exists(e))
                {
                    pe.AddEmbeddedResource(Path.GetFullPath(e), null);
                } else
                {
                    if (e.Contains(";"))
                    {
                        var tmp = e.Split(new char[] { ';' }, 2);
                        if (!File.Exists(tmp [0]))
                            throw new FileNotFoundException(tmp [0]);
                        pe.AddEmbeddedResource(Path.GetFullPath(tmp [0]), tmp [1]);
                    } else
                    {
                        throw new FileNotFoundException(e);
                    }
                }
            }

            using (var sf = new StreamWriter(projfile))
                pe.Save(sf);


            return 0;

        }
    }
}
