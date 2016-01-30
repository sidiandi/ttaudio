using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using ttaenc;

namespace ttaencTests
{
    [TestFixture()]
    public class AlbumMakerTests : TestBase
    {
        [Test]
        public void ProvideOgg()
        {
            log4net.Config.BasicConfigurator.Configure();

            foreach (var fileName in new[] {
                "187950__soundmatch24__rnb-beat.mp3",
                // "268049__sceza__bass-sine-sweep-10-400hz.wav",
                "268049__sceza__bass-sine-sweep-10-400hz.ogg"
            })
            {
                var outFile = TestFile(Path.Combine("audio-out", fileName)) + ".ogg";
                PathUtil.EnsureParentDirectoryExists(outFile);
                PathUtil.EnsureFileNotExists(outFile);
                MediaFileConverter.AudioFileToTipToiAudioFile(
                    CancellationToken.None,
                    TestFile(Path.Combine("audio", fileName)),
                    outFile).Wait();
                Assert.IsTrue(File.Exists(outFile));
            }
        }

        [Test]
        public void CreateGme()
        {
            log4net.Config.BasicConfigurator.Configure();
            var audioFiles = AlbumReader.GetAudioFiles(new[] { TestFile("audio") });
            var albumMakerDirectory = TestFile("album-maker");
            PathUtil.EnsureFileNotExists(albumMakerDirectory);
            var albumMaker = new AlbumMaker(albumMakerDirectory);
            var collection = new AlbumReader().FromTags(audioFiles);
            albumMaker.Create(CancellationToken.None, collection).Wait();
        }
    }
}