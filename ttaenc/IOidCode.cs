// Copyright (c) https://github.com/sidiandi 2016
// 
// This file is part of tta.
// 
// tta is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// tta is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tta
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
