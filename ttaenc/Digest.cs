using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public class Digest
    {
        public static string Get(string text)
        {
            return Get(ASCIIEncoding.ASCII.GetBytes(text));
        }

        public static string Get(byte[] data)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(data);
            return String.Join(String.Empty, hash.Select(_ => _.ToString("X2")));
        }
    }
}
