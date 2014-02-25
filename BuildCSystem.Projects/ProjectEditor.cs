using System;
using MonoDevelop.Projects.Formats.MSBuild;
using System.IO;

namespace BuildCSystem.Projects
{
    public class ProjectEditor
    {

        public ProjectEditor()
        {
            Project = new MSBuildProject();
            var pg = Project.AddNewPropertyGroup(true);
            Project.ToolsVersion = "4.0";
            Project.DefaultTargets = "Build";

            pg["SchemaVersion"]= "2.0";
            pg["ProjectGuid"]= ProjectTypeGuids.CSharp;

            OutputType= CSharpProjectOutputTypes.Library.ToString();
            ProductVersion= "10.0.0";
            AssemblyName = "Untitled";
            RootNameSpace = "Untitled";

            pg ["Configuration"] = "Debug";
            pg ["Platform"] = "AnyCPU";

            AddReleaseConfig(false, "AnyCPU", "bin\\Debug");
            AddDebugConfig(true, "AnyCPU", "bin\\Debug");

            ProjectFolder = Environment.CurrentDirectory;
        }

        public void AddDebugConfig(bool asdefault, string platform, string outdir )
        {
            AddPropertyGroup(platform, "Debug", true, false, 4, outdir);
            if (asdefault)
                SetDefaultConfiguration(platform, "Debug");
        }

        public void AddReleaseConfig(bool asdefault, string platform, string outdir )
        {
            AddPropertyGroup(platform, "Release", false, true, 4, outdir);
            if (asdefault)
                SetDefaultConfiguration(platform, "Release");
        }

        public MSBuildProject Project { get; set; }

        public string ProjectFolder { get; set; }

        MSBuildPropertyGroup GlobalProperties {
            get {
                return Project.GetGlobalPropertyGroup();
            }
        }

        public string RootNameSpace { 
            get {
                return GlobalProperties["RootNamespace"];
            }
            set {
                GlobalProperties.SetPropertyValue("RootNamespace",value);
            }
        }

        public string OutputType {
            get {
                return GlobalProperties ["OutputType"];
            }
            set {
                GlobalProperties ["OutputType"] = value;
            }
        }

        public string AssemblyName {
            get {
                return GlobalProperties ["AssemblyName"];
            }
            set {
                GlobalProperties ["AssemblyName"] = value;
            }
        }

        public string ProductVersion {
            get {
                return GlobalProperties ["ProductVersion"];
            }
            set {
                GlobalProperties ["ProductVersion"] = value;
            }
        }

        public string ToolsVersion {
            get {
                return Project.ToolsVersion;
            }
            set {
                Project.ToolsVersion = value;
            }
        }

        public void AddPropertyGroup( string platform, string config, bool debug, bool optimize, int warnings, string outdir )
        {
            var pg = Project.AddNewPropertyGroup(true);
            pg ["DebugSymbols"] = debug.ToString();
            pg ["Optimize"] = optimize.ToString();
            pg ["OutputPath"] = outdir;
            pg ["WarningLevel"] = warnings.ToString();
            pg ["ErrorReport"] = "prompt";
            pg ["ConsolePause"] = "false";
            pg ["DebugType"] = "full";
            pg.Condition = " '$(Configuration)|$(Platform)' == " + string.Format("'{0}|{1}'", config, platform);


        }

        public void SetDefaultConfiguration(string platform, string config)
        {
            var cfg = GlobalProperties.GetProperty("Configuration");
            cfg.Condition = " '$(Configuration)' == '' ";
            cfg.Value = config;
            var plat = GlobalProperties.GetProperty("Platform");
            plat.Condition = " '$(Platform)' == '' ";
            plat.Value = platform;
        }


        public void AddReferences(params string[] assembly)
        {
            var rg = Project.FindBestGroupForItem("Reference");
            foreach (var a in assembly)
            {
                rg.AddNewItem("Reference", a);
            }
        }

        public void AddReference(string assembly, string hintpath)
        {
            var rg = Project.FindBestGroupForItem("Reference");
            var r = rg.AddNewItem("Reference", assembly);
            if ( hintpath != null ){ 
                r.SetMetadata("HintPath", PathTools.NativeToRelativeWindows( hintpath, ProjectFolder));
            }
        }

        public void AddProjectReference(string projfile)
        {
            var p = new MSBuildProject();
            p.Load(projfile);
            var gp = p.GetGlobalPropertyGroup();
            var pid = gp.GetPropertyValue("ProjectGuid");
            var pname = Path.GetFileNameWithoutExtension(projfile);

            var ppath = PathTools.NativeToRelativeWindows(projfile, ProjectFolder );

            var pg = Project.FindBestGroupForItem("ProjectReference");
            var pi = pg.AddNewItem("ProjectReference", ppath);
            pi.SetMetadata("Project", pid);
            pi.SetMetadata("Name", pname);
        }

        public void AddCompileSource(params string[] files)
        {
            var cg = Project.FindBestGroupForItem("Compile");
            foreach (var s in files)
            {
                cg.AddNewItem("Compile", PathTools.NativeToRelativeWindows(s, ProjectFolder));
            }
        }

        public void AddEmbeddedResource(string file, string logicalname)
        {
            file = PathTools.NativeToRelativeWindows(file, ProjectFolder);
            var eg = Project.FindBestGroupForItem("EmbeddedResource");
            var r = eg.AddNewItem("EmbeddedResource", file);
            if (logicalname != null)
            {
                r.SetMetadata("LogicalName", logicalname);
            }
        }

        public void AddContent(string file)
        {
            file = PathTools.NativeToRelativeWindows(file, ProjectFolder);
            var cg = Project.FindBestGroupForItem("Content");
            cg.AddNewItem("Content", file).SetMetadata("CopyToOutputDirectory", "Always");
        }

        public void AddNone(string file)
        {
            file = PathTools.NativeToRelativeWindows(file, ProjectFolder);
            var ng = Project.FindBestGroupForItem("None");
            ng.AddNewItem("None", file);
        }

        public void Load(TextReader r){
            var p = new MSBuildProject();
            p.Load(r);

            Project = p;
        }

        public void Save(TextWriter w)
        {
            Project.Save(w);
        }
    }
}

