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

    public SubProcess(string fileName, params string[] args)
    {
        this.fileName = fileName;
        this.args = args;
    }

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
			Console.WriteLine(this);
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

        Console.WriteLine("start: {0}", this);
        
        using (var output = new StringWriter())
        using (var error = new StringWriter())
        using (var input = new StringReader(Input))
        {
            await System.Threading.Tasks.Task.WhenAll(
                CopyToAsync(p.StandardOutput, output),
                CopyToAsync(p.StandardError, error),
                CopyToAsync(input, p.StandardInput));
            p.WaitForExit();
            Console.WriteLine("exit with exit code {1}: {0}", this, this.ExitCode);
            Output = output.ToString();
            Error = error.ToString();
            if (!String.IsNullOrEmpty(Error))
            {
                Console.WriteLine("Error: {0}", Error);
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
    public static string Pack(string csProjFile, string outputDirectory, string version)
    {
        string package = null;
        var p = new List<string>();
        p.AddRange(new[]{ "pack", csProjFile, "-OutputDirectory", outputDirectory });
        if (!String.IsNullOrEmpty(version))
        {
            p.AddRange(new[]{"-Version", version});
        }

        var nuget = new SubProcess("nuget", p.ToArray());
        nuget.Run().Wait();
        package = nuget.Output.RegexGet(@"Successfully created package '([^']+)'.");
        Console.WriteLine(package);
        return package;
    }
}

public class NugetPack: ITask  
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

    public string OutputDirectory { get; set; } 
    public string Version { set; get; }
    public ITaskItem[] Targets { set; get; }
    public ITaskItem[] Outputs { set; get; }

    public bool Execute()  
    {  
        var targetsToPack = this.Targets
            .Where(_ => _.ItemSpec.EndsWith(".dll"))
            .Where(_ => !_.ItemSpec.EndsWith(".Test.dll"));

        Console.WriteLine(String.Join("\r\n", targetsToPack));

        var output = new List<ITaskItem>();
        foreach (var target in targetsToPack)
        {
            // Util.Dump(Console.Out, target);
            output.Add(new TaskItem(Nuget.Pack(target.GetMetadata("MSBuildSourceProjectFile"), this.OutputDirectory, Version)));
        }
        Outputs = output.ToArray();

        return true;  
    }  
}  

public class NugetPush: ITask  
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

    public ITaskItem[] Packages { set; get; }
    public string Source {get; set; }
    public string ApiKey {get; set; }

    public bool Execute()  
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
            new SubProcess("nuget",args.ToArray()).RunChecked().Wait();
        }
        return true;  
    }  
}

public class NugetRestore: ITask  
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

    public string SolutionFile { get; set; } 

    public bool Execute()  
    {  
		new SubProcess("nuget", new[]{"restore", SolutionFile}).RunChecked().Wait();
        return true;  
    }  
}  

public class NugetUpdate: ITask  
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

    public string SolutionFile { get; set; } 

    public bool Execute()  
    {  
		new SubProcess("nuget", new[]{"update", SolutionFile}).RunChecked().Wait();
        return true;  
    }  
}  