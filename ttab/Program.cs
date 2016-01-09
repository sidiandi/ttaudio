using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ttab
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            var albumMaker = new AlbumMaker(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ttab"));

            var inputDir = new DirectoryInfo(args[0]);

            var albumCollection = new AlbumReader().ReadCollection(inputDir);

            var cts = new CancellationTokenSource();
            albumMaker.Create(cts.Token, albumCollection);
        }
    }
}
