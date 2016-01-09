using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttab
{
    /// <summary>
    /// Translates between the 9-digit representation of an OID code to its integer representation
    /// </summary>
    public interface IOidCode
    {
        /// <summary>
        /// Converts an OID code from its 9-digit form to an integer ID
        /// </summary>
        /// <param name="digits"></param>
        /// <returns></returns>
        int FromDigits(int[] digits);

        /// <summary>
        /// Converts an id into its 9-digit OID representation
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        int[] ToDigits(int oid);
    }
}
