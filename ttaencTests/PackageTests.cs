using NUnit.Framework;
using ttaenc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

namespace ttaenc.Tests
{
    [TestFixture()]
    public class PackageTests : TestBase
    {
        [Test()]
        public void Serialize()
        {
            var package = Package.CreateFromInputPaths(new[] { TestFile("audio") });
            var s = new XmlSerializer(package.GetType());
            using (var m = new MemoryStream())
            {
                s.Serialize(m, package);
                m.Seek(0, SeekOrigin.Begin);

                var readPackage = (Package)s.Deserialize(m);
                Assert.AreEqual(package.Title, readPackage.Title);
                Assert.AreEqual(package.FileName, readPackage.FileName);
                Assert.AreEqual(package.Albums.First().Tracks.First().Title, readPackage.Albums.First().Tracks.First().Title);
            }
        }

        [Test()]
        public void CreateFromInputPathsTest_works_with_empty_enumerable()
        {
            var p = Package.CreateFromInputPaths(Enumerable.Empty<string>());
        }
    }
}