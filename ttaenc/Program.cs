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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ttaenc
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            log4net.Config.BasicConfigurator.Configure();

            var pen = TipToiPen.GetAll().FirstOrDefault();
            if (pen == null)
            {
                pen = TipToiPen.Simulated;
                log.InfoFormat("Pen is not attached. Output will be written to {0}", pen.RootDirectory);
            }

            var package = Package.CreateFromInputPaths(args);
    
            var cacheDirectory = Path.Combine(About.Get().LocalApplicationDataDirectory, "cache");
            var converter = new MediaFileConverter(cacheDirectory);

            var structure = new PackageDirectoryStructure(pen.RootDirectory, package);
            var packageBuilder = new PackageBuilder(structure, converter, Settings.Read().CreateOidSvgWriter());
            packageBuilder.Build(CancellationToken.None).Wait();
        }
    }
}
