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
        public void FromTagsTest()
        {
            // var dir = @"\\calag.de\music\kids\Der kleine Drache Kokosnuss";
            var dir = @"C:\Users\andreas\AppData\Local\ttab\media";

            var c = new AlbumReader().FromTags(AlbumReader.GetAudioFiles(new[] { dir }));

            Console.WriteLine(String.Join("\n", c.Album.Select(_ => _.Title)));
        }
    }
}