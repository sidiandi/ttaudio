using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttab
{
    static class Extensions
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
    }
}
