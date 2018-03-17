using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class WriteCommonAssemblyInfo: ITask  
{  
    static string Quote(string x)
    {
        return "\"" + x + "\"";
    }
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

    public string CompanyName { get; set; }
    public string ProductName { get; set; }
    public string Copyright {get; set; }
    public string Output { get; set; }

    public bool Execute()  
    {  
        File.WriteAllText(Output, @"// Generated. Changes will be lost.
[assembly: System.Reflection.AssemblyCopyright(" + Quote(String.Format("Copyright (c) {0} {1}", CompanyName, DateTime.Now.Year)) + @")]
[assembly: System.Reflection.AssemblyCompany(" + Quote(CompanyName) + @")]
[assembly: System.Reflection.AssemblyProduct(" + Quote(ProductName) + @")]
");
        return true;  
    }  
}  