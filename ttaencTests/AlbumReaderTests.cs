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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using tta;

namespace ttaencTests
{
    [TestFixture()]
    public class AlbumReaderTests 
    {
        [Test()]
        public void IsAudioFile()
        {
            Assert.IsTrue(AlbumReader.IsAudioFile(new System.IO.FileInfo("test.mp3")));
            Assert.IsTrue(AlbumReader.IsAudioFile(new System.IO.FileInfo("test.ogg")));
            Assert.IsFalse(AlbumReader.IsAudioFile(new System.IO.FileInfo("test.txt")));
        }
    }
}
