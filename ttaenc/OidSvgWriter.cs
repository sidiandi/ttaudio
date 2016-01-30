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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ttaenc
{
    public class OidSvgWriter
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
            w.WriteLine(@"<span><div class=""OidButton"" >");
            OidArea(w, oid); w.Write(" "); w.Write(innerHtml);
            w.WriteLine("</div></span>");
        }

        public void OidArea(TextWriter w, int oid)
        {
            w.WriteLine(@"<svg class=""oid-area"" >");
            w.WriteLine("<defs>");
            OidPattern(w, oid);
            w.WriteLine(@"</defs>");
            w.WriteLine(@"<rect fill=""url(#Code{0})"" x=""5%"" y=""5%"" width=""90%"" height=""90%""/>", oid);
            // w.WriteLine(@"<circle fill=""url(#Code{0})"" cx=""50%"" cy=""50%"" r=""50%"" />", oid);
            w.WriteLine("</svg>");
        }

        public void Dot(TextWriter w, float cx, float cy)
        {
            w.WriteLine(@"<circle cx={0} cy={1} r={2} style=""stroke: none; fill: #000000;""/>", Cm(cx), Cm(cy), Cm(DotSize));
        }

        static string Cm(float x)
        {
            return String.Format("\"{0}cm\"", x);
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
            w.WriteLine(@"<pattern id=""Code{0}"" patternUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""{1}cm"" height=""{1}cm"" >", oid, GridSpacing*8);

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
    }
}
