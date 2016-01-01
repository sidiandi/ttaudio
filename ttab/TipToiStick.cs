using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttab
{
    class TipToiStick
    {
        TipToiStick(string root)
        {
            RootDirectory = root;
        }

        public string RootDirectory
        {
            get; private set;
        }

        public static IEnumerable<TipToiStick> GetAll()
        {
            // copy to stick, if possible
            var stickFile = @".tiptoi.log";
            return DriveInfo.GetDrives()
                .Select(d => d.RootDirectory.FullName)
                .Where(_ => File.Exists(Path.Combine(_, stickFile)))
                .Select(_ => new TipToiStick(_));
        }
    }
}
