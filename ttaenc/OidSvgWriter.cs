// Copyright (c) https://github.com/sidiandi 2016
// 
// This file is part of tta.
// 
// tta is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// tta is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Foobar.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ttaenc
{
    public class OidSvgWriter : HtmlGenerator
    {
        public OidSvgWriter(IOidCode code)
        {
            this.code = code;
            DotSize = 0.005f;
            DotOffset = 0.005f;
            GridSpacing = 0.0125f;
        }

        IOidCode code;

        /// <summary>
        /// Dot size in cm
        /// </summary>
        public float DotSize { get; set; }

        /// <summary>
        /// Dot bit offset in cm
        /// </summary>
        public float DotOffset { get; set; }

        /// <summary>
        /// Double of grid spacing in cm
        /// </summary>
        float GridSpacing2 { get { return GridSpacing * 2.0f; } }
        
        /// <summary>
        /// Grid spacing in cm
        /// </summary>
        public float GridSpacing { get; set; }

        public void OidButton(TextWriter w, int oid, string innerHtml)
        {
            w.WriteLine(@"<span><div class=""oid-button"" >");
            OidArea(w, oid); w.Write(" "); w.Write(innerHtml);
            w.WriteLine("</div></span>");
        }

        public void OidArea(TextWriter w, int oid)
        {
            w.WriteLine(@"<svg class=""oid-area"" >");
            w.WriteLine("<defs>");
            OidPattern(w, oid);
            w.WriteLine(@"</defs>");
            w.WriteLine(@"<rect fill=""url(#Code{0})"" x=""0%"" y=""0%"" width=""100%"" height=""100%""/>", oid);
            // w.WriteLine(@"<circle fill=""url(#Code{0})"" cx=""50%"" cy=""50%"" r=""50%"" />", oid);
            w.WriteLine("</svg>");
        }

        public void Dot(TextWriter w, float cx, float cy)
        {
            w.WriteLine(@"<circle cx={0} cy={1} r={2} style=""stroke: none; fill: #000000;""/>", Cm(cx), Cm(cy), Cm(DotSize));
        }

        static string Cm(float x)
        {
            return String.Format(CultureInfo.InvariantCulture, "\"{0}cm\"", x);
        }

        class Offset
        {
            public int x;
            public int y;
        }

        static Offset[] offsets = new Offset[]
            {
                new Offset { x = 1, y = 1 },
                new Offset { x = -1, y = 1 },
                new Offset { x = -1, y = -1 },
                new Offset { x = 1, y = -1 },
            };

        static Offset GetOffset(int digit)
        {
            return offsets[digit];
        }

        public void OidPattern(TextWriter w, int oid)
        {
            w.WriteLine(@"<pattern id=""Code{0}"" patternUnits=""userSpaceOnUse"" x=""0"" y=""0"" width={1} height={1} >", oid, Cm(GridSpacing*8));

            // guide dots
            Dot(w, GridSpacing, GridSpacing);
            Dot(w, GridSpacing * 3, GridSpacing);
            Dot(w, GridSpacing * 5, GridSpacing);
            Dot(w, GridSpacing*7, GridSpacing);
            Dot(w, GridSpacing, GridSpacing*3);
            Dot(w, GridSpacing + DotOffset, GridSpacing*5);
            Dot(w, GridSpacing, GridSpacing*7);

            // data dots
            var digits = code.ToDigits(oid);

            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    var digit = digits[x + y * 3];
                    var offset = GetOffset(digit);
                    Dot(w, GridSpacing*3 + x * GridSpacing2 + offset.x * DotOffset, GridSpacing*3 + y * GridSpacing2 + offset.y * DotOffset);
                }
            }

            w.WriteLine(@"</pattern>");
        }

        public static void CreatePrinterTestPage(string testPagePath)
        {
            var code = new TiptoiOidCode();

            var mediaDir = Path.Combine(Path.GetDirectoryName(testPagePath), "media");
            PathUtil.EnsureDirectoryExists(mediaDir);
            foreach (var mediaFile in new[]
            {
                "note_to_pen.png",
                "style.css"
            })
            {
                PathUtil.CopyIfNewer(
                    Path.Combine(PathUtil.GetDirectory(), "media", mediaFile),
                    Path.Combine(mediaDir, mediaFile));
            }

            using (var w = new StreamWriter(testPagePath))
            {
                w.WriteLine(@"
<!doctype html>
<html moznomarginboxes mozdisallowselectionprint>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>ttaudio Printer Test Page</title>
	<link rel=""stylesheet"" href=""media/style.css"" />
    <style>
    .oid-button {
        width: 4cm;
    }
    </style>
  </head>
  <body>
    <div class=""printInstructions"">
        <img src=""media/note_to_pen.png"" />
        <p><a href=""javascript:window.print();"">Click here to print the test page with optical codes.</a> Use a printer with at least 600 dpi.</p>
        <p>This page was generated by <a href=""" + About.GithubUri.ToString() + @""">" + T(About.Product) + @"</a> on " + T(DateTime.Now.ToString()) +
        @".</p></div>
    <h1>ttaudio Printer Test Page</h1>
");


                var defaultOidWriter = new OidSvgWriter(code);
                var oid = 10250;

                var powBase = 1.1;

                w.Write("<table>");
                w.Write("<tr>");
                w.Write("<td/>");
                for (int dotSize = -4; dotSize <= 4; ++dotSize)
                {
                    var fDotSize = (float)Math.Pow(powBase, dotSize);
                    w.Write("<td>"); w.Write(T(String.Format("Dot Size {0:F4} cm", defaultOidWriter.DotSize * fDotSize))); w.Write("</td>");
                }
                w.Write("</tr>");

                for (int gridSpacing = -4; gridSpacing <= 4; ++gridSpacing)
                {
                    var fGridSpacing = (float)Math.Pow(powBase, gridSpacing);
                    w.Write("<tr>");
                    w.Write("<td>"); w.Write(T(String.Format("Grid Spacing {0:F4} cm", defaultOidWriter.GridSpacing * fGridSpacing))); w.Write("</td>");
                    for (int dotSize = -4; dotSize <= 4; ++dotSize)
                    {
                        var fDotSize = (float)Math.Pow(powBase, dotSize);

                        var oidWriter = new OidSvgWriter(code)
                        {
                            DotOffset = defaultOidWriter.DotOffset,
                            DotSize = defaultOidWriter.DotSize * fDotSize,
                            GridSpacing = defaultOidWriter.GridSpacing * fGridSpacing
                        };

                        w.Write("<td>");
                        oidWriter.OidArea(w, oid++); // , String.Format("spacing: {0:F0}%, dot: {1:F0}%", fGridSpacing*100, fDotSize*100));
                        w.Write("</td>");
                    }
                    w.Write("</tr>");
                }
                w.Write("</table>");

                w.WriteLine(@"
  </body>
</html>
");
            }
        }
    }
}
