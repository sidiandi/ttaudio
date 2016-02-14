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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public static class Extensions
    {
        public static string Quote(this object x)
        {
            return "\"" + x.ToString() + "\"";
        }

        public static Y OrNull<X, Y>(this X x, Func<X, Y> f)
        {
            if ((object)x == null)
            {
                return default(Y);
            }

            return f(x);
        }

        public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> e, Func<T, long> f, long maximalPartSize)
        {
            List<T> part = new List<T>();
            long partSize = 0;
            foreach (var i in e)
            {
                var size = f(i);
                if ((partSize + size) > maximalPartSize)
                {
                    yield return part;
                    partSize = 0;
                    part = new List<T>();
                }

                partSize += size;
                part.Add(i);
            }
            if (part.Any())
            {
                yield return part;
            }
        }

        public static IEnumerable<LinkedList<T>> Window<T>(this IEnumerable<T> data, int windowLength, int skip = 1)
        {
            var window = new LinkedList<T>();
            int count = 0;
            int skipCount = skip;

            using (var e = data.GetEnumerator())
            {
                for (; e.MoveNext();)
                {
                    window.AddLast(e.Current);
                    ++count;
                    if (count > windowLength)
                    {
                        window.RemoveFirst();
                    }
                    if (count >= windowLength)
                    {
                        if (skipCount >= skip)
                        {
                            yield return window;
                            skipCount = 0;
                        }
                        ++skipCount;
                    }
                }
            }
        }
    }
}
