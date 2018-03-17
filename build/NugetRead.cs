using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class NugetRead: ITask  
{  
    //When implementing the ITask interface, it is necessary to  
    //implement a BuildEngine property of type  
    //Microsoft.Build.Framework.IBuildEngine. This is done for  
    //you if you derive from the Task class.  
    private IBuildEngine buildEngine;  
    public IBuildEngine BuildEngine  
    {  
        get  
        {  
            return buildEngine;  
        }  
        set  
        {  
            buildEngine = value;  
        }  
        }  

    // When implementing the ITask interface, it is necessary to  
    // implement a HostObject property of type Object.  
    // This is done for you if you derive from the Task class.  
    private ITaskHost hostObject;  
    public ITaskHost HostObject  
    {  
        get  
        {  
            return hostObject;  
        }  

        set  
        {  
            hostObject = value;  
        }  
    } 

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
        Console.WriteLine(String.Join(", ", packages));
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

    public bool Execute()  
    {  
        // locate package repository
        var repositoryDir = Path.GetFullPath(Path.Combine(SolutionDir, "..", "packages"));

        var packages = ReadNugetPackages(SolutionDir);

        WriteNugetPropsFile(PropsFile, packages, repositoryDir);
        return true;  
    }  
}  