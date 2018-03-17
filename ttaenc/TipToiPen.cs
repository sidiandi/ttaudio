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
    public class TipToiPen
    {
        TipToiPen(string root)
        {
            RootDirectory = root;
        }

        public string RootDirectory
        {
            get; private set;
        }

        public static TipToiPen Simulated
        {
            get
            {
                return new TipToiPen(Path.Combine(About.Get().DocumentsDirectory, "pen"));
            }
        }

        public static TipToiPen Get()
        {
            var pen = GetAll().FirstOrDefault();
            if (pen == null)
            {
                pen = Simulated;
            }
            return pen;
        }

        public static IEnumerable<TipToiPen> GetAll()
        {
            // copy to stick, if possible
            var stickFile = @".tiptoi.log";
            return DriveInfo.GetDrives()
                .Select(d => d.RootDirectory.FullName)
                .Where(_ => File.Exists(Path.Combine(_, stickFile)))
                .Select(_ => new TipToiPen(_));
        }
    }
}
