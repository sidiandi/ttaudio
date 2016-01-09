using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace tta
{
    class About
    {
        public static Uri GitUri
        {
            get
            {
                var a = Assembly.GetExecutingAssembly();
                return new Uri(String.Format("https://github.com/{0}/{1}",
                    a.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                    a.GetCustomAttribute<AssemblyProductAttribute>().Product
                    ));
            }
        }
    }
}
    
