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
    public class PackageBuilderTests : ttaencTests.TestBase
    {
        [Test()]
        public void PackageBuilderTest()
        {
            var penDirectory = TestFile("penDirectory");
            PathUtil.EnsureNotExists(penDirectory);
            PathUtil.EnsureDirectoryExists(penDirectory);
            var converter = new MediaFileConverter(TestFile("media-cache"));
            var albumReader = new AlbumReader();

            var package = new Package
            {
                Albums = albumReader.GetAlbums(AlbumReader.GetAudioFiles(new[] { TestFile("audio") })),
                Name = "test",
                ProductId = 800
            };
                
            var structure = new PackageDirectoryStructure(penDirectory, package);
            var pb = new PackageBuilder(structure, converter);

            pb.Build(CancellationToken.None).Wait();

            Console.WriteLine(structure.GmeFile);
        }
    }
}