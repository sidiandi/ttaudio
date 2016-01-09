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
