using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaencTests
{
    class TestHelper
    {
        public static string TestFile(string relativePath)
        {
            return System.IO.Path.Combine(
                new DirectoryInfo(tta.PathUtil.GetDirectory()).Parent.FullName,
                "test-data",
                relativePath);
        }
    }
}
