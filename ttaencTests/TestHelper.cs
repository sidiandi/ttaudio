using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ttaenc;

namespace ttaenc.Tests
{
    public class TestBase
    {
        static TestBase()
        {
            log4net.Config.BasicConfigurator.Configure();
        }

        protected string TestFile(string relativePath)
        {
            return System.IO.Path.Combine(
                new DirectoryInfo(PathUtil.GetDirectory()).Parent.FullName,
                "test-data",
                relativePath);
        }
    }
}
