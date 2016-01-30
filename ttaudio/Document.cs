using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaudio
{
    public class Document
    {
        public Document()
        {
        }

        string gmeFile;
        ttaenc.AlbumCollection package;

        public static Document Load(string gmeFile)
        {
            var doc = new Document
            {
                gmeFile = gmeFile,
            };
            return doc;
        }
    }
}
