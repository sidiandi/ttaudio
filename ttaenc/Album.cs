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
    public class Album
    {
        public string Artist
        {
            get
            {
                return String.Join(", ", Tracks.SelectMany(_ => _.Artists).Distinct());
            }
        }

        public string Title;

        public IPicture GetPicture()
        {
            return Tracks.SelectMany(t => t.GetPictures())
                .Concat(GetAlbumPictures())
                .FirstOrDefault();
        }

        IEnumerable<IPicture> GetAlbumPictures()
        {
            return Tracks.Select(t => System.IO.Path.GetDirectoryName(t.Path)).Distinct()
                .SelectMany(d => new System.IO.DirectoryInfo(d).GetFiles())
                .Where(f => AlbumReader.IsImageFile(f))
                .OrderByDescending(f => AlbumReader.IsFrontCover(f))
                .Select(f => new Picture(f));
        }

        public Track[] Tracks;
    }
}
