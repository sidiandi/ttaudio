using NUnit.Framework;
using ttaenc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ttaenc.Tests
{
    [TestFixture()]
    public class MediaFileConverterTests : TestBase
    {
        [Test()]
        public void TestConvert(string source)
        {
            var cacheDir = TestFile("media-cache");
            PathUtil.EnsureNotExists(cacheDir);
            PathUtil.EnsureDirectoryExists(cacheDir);
            var converter = new MediaFileConverter(cacheDir);
            var convertedFile = converter.ProvidePenAudioFile(CancellationToken.None, source).Result;
        }

        [Test()]
        public void ConvertAllTestFiles()
        {
            var audioFiles = new AlbumReader().GetAudioFiles(new[] { TestFile(@"audio") }).ToList();
            Assert.AreEqual(4, audioFiles.Count);
            foreach (var i in audioFiles)
            {
                TestConvert(i);
            }
        }

        [Test()]
        public void ConvertMonoOgg()
        {
            TestConvert(TestFile(@"audio\mono\ding.ogg"));
        }

        [Test()]
        public void MediaFileConverterTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void ProvidePenAudioFileTest()
        {
            Assert.Fail();
        }

        [Test()]
        public void GetPenAudioFilePathTest()
        {
            Assert.Fail();
        }
    }
}