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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class About
    {
        public static Uri GithubUri
        {
            get
            {
                var a = Assembly.GetCallingAssembly();
                return new Uri(String.Format("https://github.com/{0}/{1}", Company, Product));
            }
        }

        public static string Product
        {
            get
            {
                var a = Assembly.GetCallingAssembly();
                return a.GetCustomAttribute<AssemblyProductAttribute>().Product;
            }
        }

        public static string Company
        {
            get
            {
                var a = Assembly.GetCallingAssembly();
                return a.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
        }

        public static string LocalApplicationDataDirectory
        {
            get
            {
                return Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    PathUtil.GetValidFileName(Company),
                    PathUtil.GetValidFileName(Product));
            }
        }

        public static string DocumentsDirectory
        {
            get
            {
                return Path.Combine(
                    System.Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                    PathUtil.GetValidFileName(Product));
            }
        }
    }
}
    
