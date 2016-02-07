using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttaenc
{
    public class Settings
    {
        public Settings()
        {
            NextProductId = 800;
        }
        public int NextProductId { set; get; }

        static string SettingsFile
        {
            get { return Path.Combine(About.LocalApplicationDataDirectory, PathUtil.GetValidFileName(About.Product + ".settings.xml")); }
        }

        public static Settings Read()
        {
            return XmlSerializerUtil.Read<Settings>(SettingsFile);
        }

        public void Write()
        {
            XmlSerializerUtil.Write(SettingsFile, SettingsFile);
        }
    }

    public class ProductIdProvider : IProductIdProvider
    {
        public int GetNextAvailableProductId()
        {
            var s = Settings.Read();
            var id = s.NextProductId++;
            s.Write();
            return id;
        }
    }
}
