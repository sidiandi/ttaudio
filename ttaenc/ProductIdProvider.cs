using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttaenc
{
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
