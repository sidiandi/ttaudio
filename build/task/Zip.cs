using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO.Compression;
using Microsoft.Build.Utilities;

public class ZipProductFiles: Task  
{  
    static string Quote(string x)
    {
        return "\"" + x + "\"";
    }

    public string Version { get; set; }
    public string OutputDirectory { get; set; }
    public ITaskItem[] Targets { get; set; }
    public ITaskItem[] Output { get; set; }

    public ZipProductFiles()
    {
        OutputDirectory = ".";
    }

    static void Dump(ITaskItem taskItem, TextWriter w)
    {
        w.WriteLine("# {0}", taskItem.ItemSpec);
        foreach (string n in taskItem.MetadataNames)
        {
            w.WriteLine("{0}: {1}", n, taskItem.GetMetadata(n));
        }
    }

    static bool IsExtension(FileInfo f, string extension)
    {
        return string.Equals(f.Extension, extension);
    }

    static string ReplaceExtension(string path, string extension)
    {
        return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + extension);
    }

    static IEnumerable<FileInfo> GetProductFiles(string targetDir)
    {
        return new DirectoryInfo(targetDir).GetFiles("*", SearchOption.AllDirectories)
            .Where(_ => !IsExtension(_, ".pdb"))
            .Where(_ => (!IsExtension(_, ".xml") || !File.Exists(ReplaceExtension(_.FullName, ".xml"))));
    }

    static ITaskItem Zip(ITaskItem target, string OutputDirectory, string version)
    {
        var zipFile = Path.Combine(OutputDirectory, String.Join("-", Path.GetFileNameWithoutExtension(target.ItemSpec), version) + ".zip");
        if (File.Exists(zipFile))
        {
            File.Delete(zipFile);
        }
        using (var archive = ZipFile.Open(zipFile, ZipArchiveMode.Create))
        {
            var targetDir = Path.GetDirectoryName(target.ItemSpec);
            Console.WriteLine("Zipping {0} to {1}", targetDir, zipFile);
            foreach (var f in GetProductFiles(targetDir))
            {
                var name = f.FullName.Substring(targetDir.Length+1);
                archive.CreateEntryFromFile(f.FullName, name);
            }
        }
        // Dump(target, Console.Out);
        return new TaskItem(zipFile);
    }

    override public bool Execute()  
    {  
        Output = Targets.Select(_ => Zip(_, OutputDirectory, Version)).ToArray();
        return true;  
    }  
}  