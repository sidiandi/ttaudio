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
            package = new Package(new ProductIdProvider());
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
                package = XmlSerializerUtil.Read<Package>(ttaFile)
            };
            return doc;
        }

        public const string fileDialogFilter = "ttaudio Files (*.tta)|*.tta";

        public void Save()
        {
            var s = new XmlSerializer(package.GetType());
            package.FileName = Path.GetFileNameWithoutExtension(ttaFile);
            XmlSerializerUtil.Write(ttaFile, package);
        }
    }
}
