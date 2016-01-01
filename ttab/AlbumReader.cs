using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ttab
{
    class AlbumReader
    {
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

        static bool IsAudioFile(FileInfo file)
        {
            return
                file.Extension.Equals(".mp3", StringComparison.CurrentCultureIgnoreCase) ||
                file.Extension.Equals(".ogg", StringComparison.CurrentCultureIgnoreCase);
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
                    .Select(_ => _.FullName).ToArray(),
                Picture = files.Where(_ => IsImageFile(_))
                    .OrderByDescending(_ => Regex.IsMatch(_.Name, "front", RegexOptions.IgnoreCase))
                    .ThenBy(_ => _.Length)
                    .FirstOrDefault().OrNull(_ => _.FullName)
            };
        }
    }
}
