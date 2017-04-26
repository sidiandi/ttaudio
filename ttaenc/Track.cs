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
using TagLib;

namespace ttaenc
{
    public class Track
    {
        public string[] Artists;
        public string Album;
        public string Title;
        public string Path;
        public uint TrackNumber;
        public TimeSpan Duration;

        public string PenAudioFile { get; set; }
        public int Oid { get; set; }

        public TagLib.IPicture[] GetPictures()
        {
            try
            {
                using (var f = TagLib.File.Create(this.Path))
                {
                    if (!f.Tag.IsEmpty)
                    {
                        return f.Tag.Pictures;
                    }
                }
            }
            catch (Exception)
            {
            }

            return new IPicture[] { };
        }
    }
}
