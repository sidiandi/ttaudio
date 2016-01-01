using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ttab
{
    class Program
    {

        static string Id(string fileName)
        {
            return Regex.Replace(fileName, @"[^\w]", "_");
        }

        class Track
        {
            public int Oid;
            public string SourceFile;
            public string OggFile;
            public string Id;
        }

        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener(false));

            var albumMaker = new AlbumMaker(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ttab"));

            var inputDir = new DirectoryInfo(args[0]);

            var albumCollection = new AlbumReader().ReadCollection(inputDir);

            albumMaker.Create(albumCollection);
        }

    }
}
