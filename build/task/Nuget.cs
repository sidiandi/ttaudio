using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Build.Utilities;

public class SubProcess
{
    readonly string fileName;
    readonly string[] args;
    
    static async System.Threading.Tasks.Task CopyToAsync(TextReader reader, TextWriter writer)
    {
        for (;;)
        {
            var line = await reader.ReadLineAsync();
            if (line == null)
            {
                break;
            }
            await writer.WriteLineAsync(line);
        }
    }
    
    public static string JoinCommandLine(params string[] args)
    {
        return String.Join(" ", args.Select(QuoteIfRequired));
    }

    public static string Quote(string x)
    {
        return "\"" + x + "\"";
    }

    public static string QuoteIfRequired(string x)
    {
        if (Regex.IsMatch(x, @"\s"))
        {
            return Quote(x);
        }
        else
        {
            return x;
        }
    }

    public SubProcess(TaskLoggingHelper log, string fileName, params string[] args)
    {
        this.log = log;
		this.fileName = fileName;
        this.args = args;
    }

	TaskLoggingHelper log;
    public string WorkingDirectory;
    public string Input = String.Empty;
    public string Output;
    public string Error;

    Process p;
    public int ExitCode { get{ return p.ExitCode; } }

    public async System.Threading.Tasks.Task RunChecked()
    {
		await Run();
		if (ExitCode != 0)
		{
			log.LogMessage("{0}", this);
			throw new Exception("Exit code not null");
		}
	}

    public override string ToString()
    {
        return String.Format("{0} {1}, pid: {2}", p.StartInfo.FileName, p.StartInfo.Arguments, p.Id);
    }

    public async System.Threading.Tasks.Task Run()
    {
        p = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = JoinCommandLine(args),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true
            }
        };

        p.Start();

		log.LogMessage(MessageImportance.Normal, "start: {0}", this);
        
        using (var output = new StringWriter())
        using (var error = new StringWriter())
        using (var input = new StringReader(Input))
        {
            await System.Threading.Tasks.Task.WhenAll(
                CopyToAsync(p.StandardOutput, output),
                CopyToAsync(p.StandardError, error),
                CopyToAsync(input, p.StandardInput));
            p.WaitForExit();
			log.LogMessage(MessageImportance.Normal, "exit with exit code {1}: {0}", this, this.ExitCode);
            Output = output.ToString();
            Error = error.ToString();
            if (!String.IsNullOrEmpty(Error))
            {
				log.LogMessage(MessageImportance.Normal, "Error: {0}", Error);
            }
        }
    }
}

public static class Util
{
    public static void Dump(TextWriter w, ITaskItem taskItem)
    {
        w.WriteLine(taskItem.ItemSpec);
        foreach (string n in taskItem.MetadataNames)
        {
            w.WriteLine("{0}: {1}", n, taskItem.GetMetadata(n));
        }
    }

    public static string Quote(string x)
    {
        return "\"" + x + "\"";
    }

    public static string RegexGet(this string input, string pattern)
    {
        var m = Regex.Match(input, pattern);
        if (m.Success)
        {
            return m.Groups[1].Value;
        }
        else
        {
            return null;
        }
    }
}

public static class Nuget
{
    public static string Pack(TaskLoggingHelper log, string csProjFile, string outputDirectory, string version)
    {
        string package = null;
        var p = new List<string>();
        p.AddRange(new[]{ "pack", csProjFile, "-OutputDirectory", outputDirectory });
        if (!String.IsNullOrEmpty(version))
        {
            p.AddRange(new[]{"-Version", version});
        }

        var nuget = new SubProcess(log, "nuget", p.ToArray());
        nuget.Run().Wait();
        package = nuget.Output.RegexGet(@"Successfully created package '([^']+)'.");
        return package;
    }
}

public class NugetPack: Task  
{  
    public string OutputDirectory { get; set; } 
    public string Version { set; get; }
    public ITaskItem[] Targets { set; get; }
    public ITaskItem[] Outputs { set; get; }

    override public bool Execute()  
    {  
        var targetsToPack = this.Targets
            .Where(_ => _.ItemSpec.EndsWith(".dll"))
            .Where(_ => !_.ItemSpec.EndsWith(".Test.dll"));

        var output = new List<ITaskItem>();
        foreach (var target in targetsToPack)
        {
            output.Add(new TaskItem(Nuget.Pack(Log, target.GetMetadata("MSBuildSourceProjectFile"), this.OutputDirectory, Version)));
        }
        Outputs = output.ToArray();

        return true;  
    }  
}  

public class NugetPush: Task  
{  
    public ITaskItem[] Packages { set; get; }
    public string Source {get; set; }
    public string ApiKey {get; set; }

    override public bool Execute()  
    {  
        if (ApiKey == null)
        {
            throw new ArgumentNullException("ApiKey");
        }

        foreach (var package in Packages)
        {
            var args = new List<string>{"push", package.ItemSpec, ApiKey };
            if (Source != null)
            {
                args.AddRange(new[]{"-Source", Source});
            }
            new SubProcess(Log, "nuget",args.ToArray()).RunChecked().Wait();
        }
        return true;  
    }  
}

public class NugetRestore: Task  
{  
    public string SolutionFile { get; set; } 

    override public bool Execute()  
    {  
		new SubProcess(Log, "nuget", new[]{"restore", SolutionFile}).RunChecked().Wait();
        return true;  
    }  
}  

public class NugetUpdate: Task  
{  
    public string SolutionFile { get; set; } 

    override public bool Execute()  
    {  
		new SubProcess(Log, "nuget", new[]{"update", SolutionFile}).RunChecked().Wait();
        return true;  
    }  
}

public class NugetRead: Task  
{  
    public string SolutionDir { get; set; } 
    public string PropsFile { set; get; }

    public class Package
    {
        [XmlAttribute]
        public string id;
        [XmlAttribute]
        public string version;

        public override string ToString()
        {
            return String.Join(" ", id, version);
        }
    }

    [XmlRoot("packages")]
    public class PackagesConfig
    {
        [XmlElement("package", typeof(Package))]
        public Package[] Packages;
    }

    public static Package[] ReadPackagesConfig(FileInfo packagesConfigFile)
    {
        using (var r = File.OpenRead(packagesConfigFile.FullName))
        {
            return ((PackagesConfig)new XmlSerializer(typeof(PackagesConfig)).Deserialize(r)).Packages;
        }
    }

    static IEnumerable<Package> ReadNugetPackages(string SolutionDir)
    {
        // locate all packages.config files and read them
        var sd = new DirectoryInfo(SolutionDir);
        var packagesConfigFiles = new[]{sd}.Concat(sd.GetDirectories())
            .SelectMany(_ => _.GetFiles("packages.config"));
        var packages = packagesConfigFiles.SelectMany(_ => ReadPackagesConfig(_));
        return packages;
    }

    static string GetPropName(string text)
    {
        return String.Join("_", Regex.Split(text, @"[^_A-Za-z0-9]"));
    }

    static void WriteProp(TextWriter w, Package package, string propName, string value)
    {
        var fullPropName = String.Join("_", "Nuget", GetPropName(package.id), propName);
        w.WriteLine(String.Format("<{0}>{1}</{0}>", fullPropName, value));
    }

    static void WriteNugetPropsFile(string propsFile, IEnumerable<Package> packages, string repositoryDir)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(propsFile));
        using (var w = new StreamWriter(propsFile))
        {
            w.WriteLine(@"<?xml version=""1.0"" encoding=""utf-8""?>
<!-- Generated. Changes will be lost. -->
<Project xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <PropertyGroup>
");
            foreach (var package in packages)
            {
                var id = package.id;
                WriteProp(w, package, "Version", package.version);
                var dir = Path.Combine(repositoryDir, String.Join(".", package.id, package.version));
                WriteProp(w, package, "Directory", dir);
                WriteProp(w, package, "ToolsDirectory", Path.Combine(dir, "Tools"));
            }
            w.WriteLine(@"</PropertyGroup>
</Project>
");
        }
    }

    override public bool Execute()  
    {  
		// locate package repository
        var repositoryDir = Path.GetFullPath(Path.Combine(SolutionDir, "..", "packages"));

        var packages = ReadNugetPackages(SolutionDir);

        WriteNugetPropsFile(PropsFile, packages, repositoryDir);
        return true;  
    }  
}
