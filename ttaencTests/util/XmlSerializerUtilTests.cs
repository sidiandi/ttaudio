using NUnit.Framework;
using ttaenc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc.Tests
{
    [TestFixture()]
    public class XmlSerializerUtilTests : TestBase
    {
        public class Data
        {
            public string Name;
        }

        [Test()]
        public void WriteTest()
        {
            var data = new Data { Name = "0123456789012345678901234567890123456789" };
            var dataFile = TestFile("data.xml");
            PathUtil.EnsureFileNotExists(dataFile);

            XmlSerializerUtil.Write(dataFile, data);
            Assert.AreEqual(data.Name, XmlSerializerUtil.Read<Data>(dataFile).Name);

            data = new Data { Name = "0123456789" };
            XmlSerializerUtil.Write(dataFile, data);
            Assert.AreEqual(data.Name, XmlSerializerUtil.Read<Data>(dataFile).Name);

        }
    }
}