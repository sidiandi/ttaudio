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
                return package.FileName;
            }
        }

        const string gmeExtension = ".gme";
        const string gmePrefix = "tta_";
        const string ttaExtension = ".tta";
        const string htmlExtension = ".html";

        public Package Package { get { return package; } }

        public string GmeFile
        {
            get { return Path.Combine(rootDirectory, gmePrefix + FileName + gmeExtension); }
        }

        string HtmlDirectory
        {
            get
            {
                return Path.Combine(About.LocalApplicationDataDirectory, "html", FileName);
            }
        }

        public string HtmlFile
        {
            get
            {
                return Path.Combine(HtmlDirectory, FileName + htmlExtension);
            }
        }

        public string HtmlMediaDirectory
        {
            get
            {
                return Path.Combine(HtmlDirectory, "media");
            }
        }
    }
}
