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
using System.Threading.Tasks;
using TagLib;

namespace ttaenc
{
    public class AlbumReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool IsAudioFile(FileInfo file)
        {
            switch (file.Extension.ToLowerInvariant())
            {
                case ".mp3":
                case ".ogg":
                case ".wav":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsImageFile(FileInfo file)
        {
            return
                file.Extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                file.Extension.Equals(".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                file.Extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsFrontCover(FileInfo file)
        {
            return Regex.IsMatch(file.Name, "(front|cover)", RegexOptions.IgnoreCase);
        }

        public Album ReadAlbum(DirectoryInfo directory)
        {
            var files = directory.GetFiles("*.*", SearchOption.AllDirectories);

            return new Album
            {
                Title = directory.Name,
                Tracks = files
                    .Where(x => IsAudioFile(x))
                    .Select(_ => new Track { Path = _.FullName }).ToArray()
            };
        }

        TagLib.IPicture CreateFilePicture(string picturePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recursively finds all valid audio files in a list of files or directories
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAudioFiles(IEnumerable<string> paths)
        {
            return paths.SelectMany(file =>
            {
                if (System.IO.Directory.Exists(file))
                {
                    return GetAudioFiles(new System.IO.DirectoryInfo(file).GetFileSystemInfos().Select(_ => _.FullName));
                }

                var info = new FileInfo(file);
                if (info.Exists && IsAudioFile(info))
                {
                    return new[] { file };
                }

                return Enumerable.Empty<string>();
            });
        }

        public Track GetTrack(FileInfo audioFile)
        {
            log.InfoFormat("Read meta information from {0}", audioFile);
            var track = new Track { Path = audioFile.FullName };
            try
            {
                using (var f = TagLib.File.Create(audioFile.FullName))
                {
                    track.Duration = f.Properties.Duration;
                    if (!f.Tag.IsEmpty)
                    {
                        track.Title = f.Tag.Title;
                        track.TrackNumber = f.Tag.Track;
                        track.Album = f.Tag.Album;
                        track.Artists = f.Tag.AlbumArtists.Concat(f.Tag.Performers).ToArray();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn(String.Format("Exception ignored while reading {0}", audioFile), ex);
            }

            if (track.Artists == null)
            {
                track.Artists = new string[] { };
            }
            if (String.IsNullOrEmpty(track.Title))
            {
                track.Title = System.IO.Path.GetFileNameWithoutExtension(track.Path);
            }

            if (String.IsNullOrEmpty(track.Album))
            {
                track.Album = System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(track.Path));
            }

            if (track.TrackNumber == 0)
            {
                track.TrackNumber = GetDirectoryIndex(track.Path);
            }

            return track;
        }

        public Album[] GetAlbums(IEnumerable<string> audioFiles)
        {
            var tracks = audioFiles
                .Select(_ => new FileInfo(_))
                .Select(GetTrack)
                .ToList();

            var albums = tracks
                .GroupBy(_ => _.Album)
                .Select(_ => new Album
                {
                    Title = _.Key,
                    Tracks = _.OrderBy(t => t.TrackNumber).ToArray()
                })
                .OrderBy(_ => _.Title)
                .ToArray();

            return albums;
        }

        internal Track[] GetTracks(IEnumerable<string> audioFiles)
        {
            return audioFiles
                .Select(_ => new FileInfo(_))
                .Select(GetTrack)
                .ToArray();
        }

        static uint GetDirectoryIndex(string path)
        {
            var name = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            return (uint)new DirectoryInfo(dir).GetFileSystemInfos()
                .OrderBy(_ => _.Name)
                .Select((info, index) => new { Info = info, Index = index })
                .Single(_ => object.Equals(_.Info.Name, name)).Index;
        }

        public static string ExportPicture(IPicture picture, string exportDirectory)
        {
            if (picture == null)
            {
                picture = new Picture(new FileInfo(Path.Combine(PathUtil.GetDirectory(), "media", "default-album-art.png")));
            }

            var extension = "." + Regex.Split(picture.MimeType, "/").Last();
            var digest = Digest.Get(picture.Data.Data);
            var picturePath = Path.Combine(exportDirectory, digest + extension);
            using (var w = System.IO.File.OpenWrite(picturePath))
            {
                w.Write(picture.Data.Data, 0, picture.Data.Data.Length);
            }
            return picturePath;
        }
    }
}
