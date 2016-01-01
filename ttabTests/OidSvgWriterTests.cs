using NUnit.Framework;
using ttab;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.XPath;
using System.Xml;
using System.Drawing;

namespace ttab.Tests
{
    [TestFixture()]
    public class OidSvgWriterTests
    {
        [Test()]
        public void OidPatternTest()
        {
            var ow = new OidSvgWriter(new TiptoiOidCode());

            var expected = @"
  <pattern id=""Code19"" patternUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""40"" height=""40"" viewBox=""0 0 40 40"" >
<circle cx=""5"" cy=""5"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""15"" cy=""5"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""25"" cy=""5"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""35"" cy=""5"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""5"" cy=""15"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""7"" cy=""25"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""5"" cy=""35"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""13"" cy=""17"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""27"" cy=""17"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""37"" cy=""17"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""17"" cy=""27"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""27"" cy=""27"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""33"" cy=""27"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""13"" cy=""37"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""23"" cy=""37"" r=""2"" style=""stroke: none; fill: #000000;""/>
<circle cx=""33"" cy=""37"" r=""2"" style=""stroke: none; fill: #000000;""/>
  </pattern>

";
            using (var w = new StringWriter())
            {
                ow.OidPattern(w, 19);
                Console.WriteLine(w.ToString());
            }
        }

        class Code
        {
            public int Oid;
            public int[] Digits;
        }

        static Code ReadCode(XPathNavigator pattern, IXmlNamespaceResolver resolver)
        {
            var dots = pattern.Select("svg:circle", resolver)
                .Cast<XPathNavigator>().Skip(7)
                .Select(dot => new Point(Int32.Parse(dot.GetAttribute("cx", String.Empty)) % 10, Int32.Parse(dot.GetAttribute("cy", String.Empty)) % 10));

            /*
            foreach (var i in dots)
            {
                Console.WriteLine(i);
            }
            */

            return new Code
            {
                Oid = Int32.Parse(pattern.GetAttribute("id", String.Empty).Substring(4)),
                Digits = dots.Select(ToDigit).ToArray()
            };
        }

        static int ToDigit(Point p)
        {
            if (p.X == 3)
            {
                if (p.Y == 3)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (p.Y == 3)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
        }

        [Test()]
        public void ReadPatterns()
        {
            IOidCode oidCode = new TiptoiOidCode();

            var codes =
                new DirectoryInfo(@"C:\work\tip-toi-reveng\docs").GetFiles("*.svg")
                .SelectMany(svg =>
                {
                    Console.WriteLine(svg);
                    var d = new XmlDocument();
                    d.Load(svg.FullName);
                    var manager = new XmlNamespaceManager(d.NameTable);
                    manager.AddNamespace("svg", "http://www.w3.org/2000/svg");
                    var e = d.CreateNavigator()
                        .Select("/svg:svg/svg:defs/svg:pattern", manager)
                        .Cast<XPathNavigator>()
                        .Select(_ => ReadCode(_, manager));
                    return e;
                });

            var errors = 0;
            foreach (var c in codes
                )
            {
                var calculated = oidCode.FromDigits(c.Digits);
                if (calculated != c.Oid)
                {
                    Console.WriteLine("{0} {1} {2} {3}", calculated - c.Oid, c.Oid, calculated, String.Join(String.Empty, c.Digits));
                    ++errors;
                }
                if (!(c.Digits.SequenceEqual(oidCode.ToDigits(c.Oid))))
                {
                    Console.WriteLine("Actual : {0}", String.Join(String.Empty, c.Digits));
                    Console.WriteLine("Desire : {0}", String.Join(String.Empty, oidCode.ToDigits(c.Oid)));
                    ++errors;
                }
            }

            Console.WriteLine("{0} errors", errors);
            Assert.AreEqual(80, errors);
        }
    }
}