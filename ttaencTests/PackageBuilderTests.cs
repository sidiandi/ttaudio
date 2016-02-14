using NUnit.Framework;
using ttaenc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace ttaenc.Tests
{
    [TestFixture()]
    public class PackageBuilderTests : TestBase
    {
        [Test()]
        public void PackageBuilderTest()
        {
            var penDirectory = TestFile("penDirectory");
            PathUtil.EnsureNotExists(penDirectory);
            PathUtil.EnsureDirectoryExists(penDirectory);
            var converter = new MediaFileConverter(TestFile("media-cache"));
            var albumReader = new AlbumReader();

            var package = Package.CreateFromInputPaths(new[] { TestFile("audio") });
            package.Albums.First().Tracks = Enumerable.Range(0, 20).Select(_ => package.Albums.First().Tracks.First()).ToArray();
            package.ProductId = 800;
                
            var structure = new PackageDirectoryStructure(penDirectory, package);
            var pb = new PackageBuilder(structure, converter, new OidSvgWriter(new TiptoiOidCode()));

            pb.Build(CancellationToken.None).Wait();

            Console.WriteLine(structure.GmeFile);
        }
    }
}