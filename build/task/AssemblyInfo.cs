using System;  
using Microsoft.Build.Framework;  
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Build.Utilities;

public class WriteCommonAssemblyInfo: Task
{  
    static string Quote(string x)
    {
        return "\"" + x + "\"";
    }
	
    public string CompanyName { get; set; }
    public string ProductName { get; set; }
    public string Copyright {get; set; }
    public string Output { get; set; }

    override public bool Execute()  
    {  
        File.WriteAllText(Output, @"// Generated. Changes will be lost.
[assembly: System.Reflection.AssemblyCopyright(" + Quote(String.Format("Copyright (c) {0} {1}", CompanyName, DateTime.Now.Year)) + @")]
[assembly: System.Reflection.AssemblyCompany(" + Quote(CompanyName) + @")]
[assembly: System.Reflection.AssemblyProduct(" + Quote(ProductName) + @")]
");
        return true;  
    }  
}
