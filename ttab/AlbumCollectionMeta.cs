using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttab
{
    public class TrackInfo
    {
        public int Oid;
        public string Id;
        
        /// <summary>
        /// Length of the media file
        /// </summary>
        public long Length;
    }

    public class AlbumCollectionMeta
    {
        [XmlIgnore]
        public Dictionary<string, TrackInfo> TrackInfo = new Dictionary<string, TrackInfo>();

        public AlbumCollection AlbumCollection;

        public int StartOid = 10250;
        public string Name;
        public int ProductId;

        public string YamlFile;
        public string HtmlFile;
        public string GmeFile;

        public int StopOid;
    }

}
