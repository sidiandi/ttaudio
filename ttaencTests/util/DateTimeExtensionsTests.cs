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
    public class DateTimeExtensionsTests
    {
        [Test()]
        public void ToInternetDateTimeFormatTest()
        {
            var time = new DateTime(2016, 2, 7, 16, 27, 23, 343);

            var f = time.ToInternetDateTimeFormat();
            Assert.AreEqual(@"2016-02-07T16:27:23.343+01:00", f);

            var readTime = DateTimeExtensions.FromInternetDateTimeFormat(f);
            Assert.AreEqual(time, readTime);
        }
    }
}