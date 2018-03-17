using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class Settings
    {
        public Settings()
        {
            NextProductId = 800;
            var w = new OidSvgWriter(new TiptoiOidCode());
            OidDotSize = w.DotSize;
            OidGridSpacing = w.GridSpacing;
        }

        public int NextProductId { set; get; }

        public float OidDotSize
        {
            get; set;
        }

        public float OidGridSpacing
        {
            get; set;
        }

        public OidSvgWriter CreateOidSvgWriter()
        {
            return new OidSvgWriter(new TiptoiOidCode())
            {
                DotSize = OidDotSize,
                GridSpacing = OidGridSpacing
            };
        }

        static string SettingsFile
        {
            get
            {
                var about = About.Get();
                return Path.Combine(about.LocalApplicationDataDirectory, PathUtil.GetValidFileName(about.Product) + ".settings.xml");
            }
        }

        public static Settings Read()
        {
            return XmlSerializerUtil.SafeRead<Settings>(SettingsFile);
        }

        public void Write()
        {
            XmlSerializerUtil.Write(SettingsFile, this);
        }
    }

}
