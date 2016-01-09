using NUnit.Framework;
using ttab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttab.Tests
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