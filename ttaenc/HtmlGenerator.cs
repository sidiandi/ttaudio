using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ttaenc
{
    public class HtmlGenerator
    {
        protected static string T(string rawText)
        {
            return HttpUtility.HtmlEncode(rawText);
        }
    }
}
