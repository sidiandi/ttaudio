using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public interface IProductIdProvider
    {
        int GetNextAvailableProductId();
    }
}
