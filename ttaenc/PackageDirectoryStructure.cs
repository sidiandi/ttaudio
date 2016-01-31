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
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class PackageDirectoryStructure : IPackageDirectoryStructure
    {
        public PackageDirectoryStructure(string rootDirectory, Package package)
        {
            this.rootDirectory = rootDirectory;
            this.package = package;
        }

        string rootDirectory;
        Package package;

        string FileName
        {
            get
            {
                return PathUtil.GetValidFileName(package.Name);
            }
        }

        const string gmeExtension = ".tta.gme";
        const string ttaExtension = ".tta";
        const string htmlExtension = ".html";

        public Package Package { get { return package; } }

        public string GmeFile
        {
            get { return Path.Combine(rootDirectory, FileName + gmeExtension); }
        }

        string TtaDirectory
        {
            get
            {
                return Path.Combine(rootDirectory, ttaExtension, FileName);
            }
        }

        public string TtaFile
        {
            get
            {
                return Path.Combine(TtaDirectory, FileName + ttaExtension);
            }
        }

        public string HtmlFile
        {
            get
            {
                return Path.Combine(TtaDirectory, FileName + htmlExtension);
            }
        }

        public string HtmlMediaDirectory
        {
            get
            {
                return Path.Combine(TtaDirectory, "media");
            }
        }
    }
}
