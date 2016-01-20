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

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ttaenc;

namespace ttaencTests
{
    [TestFixture()]
    public class AlbumReaderTests
    {
        [Test()]
        public void FromTagsTest()
        {
            var audioFiles = new[] { TestHelper.TestFile(@"audio\187950__soundmatch24__rnb-beat.mp3") };
            var ar = new AlbumReader();
            var c = ar.FromTags(audioFiles);
            var track = c.Album.Single().Tracks.Single();
            Assert.AreEqual("Beat", track.Title);
            Assert.AreEqual("Beats", track.Album);
            Assert.AreEqual(42, track.TrackNumber);
            Assert.AreEqual(TimeSpan.Parse("00:00:08.5910000"), track.Duration);

            var pictureExportDir = TestHelper.TestFile("picture");
            PathUtil.EnsureDirectoryExists(pictureExportDir);
            var p = AlbumReader.ExportPicture(track.GetPictures()[0], pictureExportDir);
            Assert.IsTrue(File.Exists(p));
        }
    }
}
