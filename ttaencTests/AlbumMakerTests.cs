using NUnit.Framework;
using tta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace ttaencTests
{
    [TestFixture()]
    public class AlbumMakerTests
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
                var outFile = TestHelper.TestFile(Path.Combine("audio-out", fileName)) + ".ogg";
                PathUtil.EnsureParentDirectoryExists(outFile);
                PathUtil.EnsureFileNotExists(outFile);
                AlbumMaker.AudioFileToTipToiAudioFile(
                    CancellationToken.None,
                    TestHelper.TestFile(Path.Combine("audio", fileName)),
                    outFile).Wait();
                Assert.IsTrue(File.Exists(outFile));
            }
        }
    }
}