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
            Tracks = new Track[] { };
            NextOid = 10250;
            StopOid = GetNextOid();
        }

        public string Name { set; get; }
        public int ProductId { get; set; }

        public int NextOid {get; set; }

        public int GetNextOid()
        {
            return NextOid++;
        }

        public Track[] Tracks
        {
            get; set;
        }

        public int StopOid { get; set; }

        public string ConfirmationSound { set; get; }
        public IEnumerable<Album> Albums
        {
            get
            {
                return Tracks.GroupBy(_ => _.Album)
                    .Select(tracks => new Album
                    {
                        Title = tracks.Key,
                        Tracks = tracks.ToArray()
                    });
            }
        }

        public static Package CreateFromInputPaths(IEnumerable<string> inputPaths)
        {
            var albumReader = new AlbumReader();

            var audioFiles = albumReader.GetAudioFiles(inputPaths);

            var package = new Package
            {
                Tracks = albumReader.GetTracks(audioFiles)
            };

            var artists = package.Tracks.SelectMany(track => track.Artists).Distinct();
            package.Name = String.Join(", ", artists);

            return package;
        }

        /// <summary>
        /// Add all new tracks found in inputfiles
        /// </summary>
        /// <param name="inputFiles"></param>
        public void AddTracks(IEnumerable<string> inputFiles)
        {
            var albumReader = new AlbumReader();
            var existing = Tracks.ToLookup(_ => _.Path);
            var toAdd = albumReader.GetTracks(albumReader.GetAudioFiles(inputFiles))
                .Where(t => !existing.Contains(t.Path));

            foreach (var track in toAdd)
            {
                track.Oid = this.GetNextOid();
            }

            Tracks = Tracks.Concat(toAdd)
                .OrderBy(_ => _.Album)
                .ThenBy(_ => _.TrackNumber)
                .ToArray();
        }

        public void RemoveTracks(IEnumerable<Track> enumerable)
        {
            Tracks = Tracks.Except(enumerable).ToArray();
        }
    }
}
