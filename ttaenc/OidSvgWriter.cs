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
        }

        IOidCode code;
        int radius = 2;
        int bitOffset = 2;
        int grid = 10;

        public void OidButton(TextWriter w, int oid, string text)
        {
            w.WriteLine(@"<span><div class=""OidButton"" >");
            OidArea(w, oid); w.Write(" "); w.Write(HttpUtility.HtmlEncode(text));
            w.WriteLine("</div></span>");
        }

        public void OidPowerIcon(TextWriter w, int oid)
        {
            w.WriteLine(@"
<svg width=""0.5cm"" height=""0.5cm"" viewBox=""0 0 200 200"" >
<defs>");
            OidPattern(w, oid);
            w.WriteLine(@"
</defs>
< path fill = ""url(#Code{0})"" d=""M255.594-0.531c-141.406,0-256,114.625-256,256s114.594,256,256,256c141.375,0,256-114.625,256-256  S396.969-0.531,255.594-0.531z M271.594,223.469h-32v-128h32V223.469z M383.594,255.469c0,70.625-57.406,128-128,128  s-128-57.375-128-128c0-53.5,33.25-98.813,80-117.875v35.375c-28.594,16.625-48,47.125-48,82.5c0,52.938,43.063,96,96,96  s96-43.063,96-96c0-35.313-19.438-65.875-48-82.5v-35.375C350.344,156.656,383.594,201.969,383.594,255.469z""/>
</svg>", oid);
        }

        public void OidArea(TextWriter w, int oid)
        {
            w.Write(@"
<svg width=""0.5cm"" height=""0.5cm"" viewBox=""0 0 200 200"" >
< defs >");
            OidPattern(w, oid);
            w.Write(@"
</defs>
<rect fill=""url(#Code{0})"" x=""10"" y=""10"" width=""190"" height=""190""/>
</svg>", oid);
        }

        public void Dot(TextWriter w, int cx, int cy)
        {
            w.WriteLine(@"<circle cx={0} cy={1} r={2} style=""stroke: none; fill: #000000;""/>", cx.Quote(), cy.Quote(), radius.Quote());
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
            w.WriteLine(@"
  <pattern id=""Code{0}"" patternUnits=""userSpaceOnUse"" x=""0"" y=""0"" width=""40"" height=""40"" viewBox=""0 0 40 40"" >", oid);

            // guide dots
            Dot(w, 5, 5);
            Dot(w, 15, 5);
            Dot(w, 25, 5);
            Dot(w, 35, 5);
            Dot(w, 5, 15);
            Dot(w, 5 + bitOffset, 25);
            Dot(w, 5, 35);

            // data dots
            var digits = code.ToDigits(oid);

            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {
                    var digit = digits[x + y * 3];
                    var offset = GetOffset(digit);
                    Dot(w, 15 + x * grid + offset.x * bitOffset, 15 + y * grid + offset.y * bitOffset);
                }
            }

            w.WriteLine(@"</pattern>");
        }
    }
}
