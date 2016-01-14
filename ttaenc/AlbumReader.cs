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

namespace tta
{
    public class AlbumReader
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public AlbumCollection ReadCollection(DirectoryInfo inputDir)
        {
            var albumDirs = inputDir.GetDirectories();

            if (!albumDirs.Any())
            {
                albumDirs = new[] { inputDir };
            }

            return new AlbumCollection
            {
                Title = inputDir.Name,
                Album = albumDirs
                    .OrderBy(_ => _.Name)
                    .Select(ReadAlbum).ToArray()
            };
        }

        public static bool IsAudioFile(FileInfo file)
        {
            switch (file.Extension.ToLowerInvariant())
            {
                case ".mp3":
                case ".ogg":
                    return true;
                default:
                    return false;
            }
        }

        static bool IsImageFile(FileInfo file)
        {
            return
                file.Extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                file.Extension.Equals(".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                file.Extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase);
        }

        public Album ReadAlbum(DirectoryInfo directory)
        {
            var files = directory.GetFiles("*.*", SearchOption.AllDirectories);

            return new Album
            {
                Title = directory.Name,
                Tracks = files
                    .Where(x => IsAudioFile(x))
                    .Select(_ => new Track { Path = _.FullName }).ToArray(),
                Picture = files.Where(_ => IsImageFile(_))
                    .OrderByDescending(_ => Regex.IsMatch(_.Name, "front", RegexOptions.IgnoreCase))
                    .ThenBy(_ => _.Length)
                    .FirstOrDefault().OrNull(_ => _.FullName)
            };
        }

        /// <summary>
        /// Recursively finds all valid audio files in a list of files or directories
        /// </summary>
        /// <param name="paths"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetAudioFiles(IEnumerable<string> paths)
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

        Track GetTrack(string path)
        {
            log.InfoFormat("Read meta information from {0}", path);
            var track = new Track { Path = path };
            try
            {
                using (var f = TagLib.File.Create(path))
                {
                    track.Duration = f.Properties.Duration;
                    if (!f.Tag.IsEmpty)
                    {
                        track.Title = f.Tag.Title;
                        track.TrackNumber = f.Tag.Track;
                        track.Album = f.Tag.Album;
                        track.Artists = f.Tag.Artists;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn(String.Format("Exception ignored while reading {0}", path), ex);
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

        static uint GetDirectoryIndex(string path)
        {
            var name = Path.GetFileName(path);
            var dir = Path.GetDirectoryName(path);
            return (uint)new DirectoryInfo(dir).GetFileSystemInfos()
                .OrderBy(_ => _.Name)
                .Select((info, index) => new { Info = info, Index = index })
                .Single(_ => object.Equals(_.Info.Name, name)).Index;
        }

        public AlbumCollection FromTags(IEnumerable<string> audioFiles)
        {
            var tracks = audioFiles.Select(_ => GetTrack(_)).ToList();

            var albums = tracks
                .GroupBy(_ => _.Album)
                .Select(_ => new Album
                {
                    Title = _.Key,
                    Tracks = _.OrderBy(t => t.TrackNumber).ToArray()
                })
                .OrderBy(_ => _.Title)
                .ToArray();

            return new AlbumCollection
            {
                Album = albums,
                Title = albums.First().Title
            };
        }
    }
}
