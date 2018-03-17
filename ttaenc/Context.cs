using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class Context
    {
        public static MediaFileConverter GetDefaultMediaFileConverter()
        {
            var cacheDirectory = Path.Combine(About.Get().LocalApplicationDataDirectory, "cache");
            var converter = new MediaFileConverter(cacheDirectory);
            return converter;
        }
    }
}
