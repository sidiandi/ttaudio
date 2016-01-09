using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ttab
{
    public class PathUtil
    {
        public static void AddToPath(string newDirectory)
        {
            var PATH = "PATH";
            System.Environment.SetEnvironmentVariable(PATH, String.Join(";", newDirectory, Environment.GetEnvironmentVariable(PATH)));
        }
        public static void EnsureDirectoryExists(string d)
        {
            if (!Directory.Exists(d))
            {
                Directory.CreateDirectory(d);
            }
        }

        public static void EnsureParentDirectoryExists(string f)
        {
            EnsureDirectoryExists(Path.GetDirectoryName(Path.GetFullPath(f)));
        }

        public static string GetDirectory(Assembly assembly = null)
        {
            if (assembly == null)
            {
                assembly = Assembly.GetExecutingAssembly();
            }

            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        public static void CopyRelativeIfNewer(string sourceDirectory, string destinationDirectory, string relativePath)
        {
            CopyIfNewer(
                Path.Combine(sourceDirectory, relativePath),
                Path.Combine(destinationDirectory, relativePath));
        }

        public static void CopyIfNewer(string sourceFile, string destinationFile)
        {
            if (Directory.Exists(sourceFile))
            {
                var children = new DirectoryInfo(sourceFile).GetFileSystemInfos();
                foreach (var c in children)
                {
                    CopyIfNewer(c.FullName, Path.Combine(destinationFile, c.Name));
                }
                return;
            }

            if (!File.Exists(destinationFile) || File.GetLastWriteTimeUtc(destinationFile) < File.GetLastWriteTimeUtc(sourceFile))
            {
                EnsureParentDirectoryExists(destinationFile);
                File.Copy(sourceFile, destinationFile, true);
            }
        }

        internal static void WriteXml<T>(string xmlFile, T x)
        {
            var s = new XmlSerializer(typeof(T));
            using (var w = File.OpenWrite(xmlFile))
            {
                s.Serialize(w, x);
            }
        }

        public static T ReadXml<T>(string xmlFile) where T : new()
        {
            var s = new XmlSerializer(typeof(T));
            try
            {
                using (var r = File.OpenRead(xmlFile))
                {
                    return (T)s.Deserialize(r);
                }
            }
            catch
            {
                return default(T);
            }
        }

        internal static string GetValidFileName(string title)
        {
            return Regex.Replace(title, @"[^\w]", "_");
        }
    }
}
