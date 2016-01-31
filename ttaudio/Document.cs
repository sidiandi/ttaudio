using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ttaenc;

namespace ttaudio
{
    public class Document
    {
        public Document()
        {
            package = new Package();
        }

        public string ttaFile
        {
            get; set;
        }

        public ttaenc.Package package;

        public static Document Load(string ttaFile)
        {
            var doc = new Document
            {
                ttaFile = ttaFile,
            };

            var s = new XmlSerializer(typeof(Package));
            using (var r = File.OpenRead(ttaFile))
            {
                doc.package = (Package) s.Deserialize(r);
            }

            return doc;
        }

        public const string fileDialogFilter = "ttaudio Files (*.tta)|*.tta";

        public void Save()
        {
            var s = new XmlSerializer(package.GetType());
            using (var w = File.OpenWrite(ttaFile))
            {
                s.Serialize(w, package);
            }
        }
    }
}
