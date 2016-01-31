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
    public class Package
    {
        public Package()
        {
            Albums = new Album[] { };
        }

        public string Name { set; get; }
        public int ProductId { get; set; }
        public Album[] Albums { get; set; }

        public IEnumerable<Track> Tracks { get { return Albums.SelectMany(_ => _.Tracks); } }

        public int StopOid { get; set; }

        public string ConfirmationSound { set; get; }

        public static Package CreateFromInputPaths(IEnumerable<string> inputPaths)
        {
            var albumReader = new AlbumReader();

            var audioFiles = albumReader.GetAudioFiles(inputPaths);

            var package = new Package
            {
                Albums = albumReader.GetAlbums(audioFiles),
            };

            var artists = package.Albums.SelectMany(_ => _.Tracks.SelectMany(track => track.Artists)).Distinct();
            package.Name = String.Join(", ", artists);

            return package;
        }
    }
}
