using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttaenc
{
    class XmlSerializerUtil
    {
        public static T Read<T>(string xmlFile) where T : new()
        {
            var s = new XmlSerializer(typeof(T));
            using (var r = File.OpenRead(xmlFile))
            {
                return (T)s.Deserialize(r);
            }
        }

        public static void Write<T>(string xmlFile, T data)
        {
            var s = new XmlSerializer(typeof(T));
            PathUtil.EnsureParentDirectoryExists(xmlFile);
            using (var w = File.OpenWrite(xmlFile))
            {
                s.Serialize(w, data);
            }
        }
    }
}
