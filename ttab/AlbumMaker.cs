using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace ttab
{
    /// <summary>
    /// Creates audio album collections for the TipToi pen
    /// </summary>
    class AlbumMaker
    {
        long maxGmeSize = 800 * 1024 * 1024;

        static AlbumMaker()
        {
            foreach (var p in new[] {
                "tools",
                @"tools\tttool-win32-1.5.1",
                @"tools\mpg123-1.22.0-x86-64"
                })
            {
                PathUtil.AddToPath(Path.Combine(PathUtil.GetDirectory(), p));
            }
        }

        public AlbumMaker(string directory)
        {
            this.RootDirectory = directory;
            PathUtil.EnsureDirectoryExists(MediaDir);
            this.OutputDir = RootDirectory;

            var binDir = PathUtil.GetDirectory();
            PathUtil.CopyIfNewer(Path.Combine(binDir, "media"), MediaDir);
        }

        int GetNextProductId()
        {
            var nextProductIdFile = Path.Combine(TempDir, "next-product-id");

            var productId = 131;
            if (File.Exists(nextProductIdFile))
            {
                productId = Int32.Parse(File.ReadAllText(nextProductIdFile));
            }

            productId++;

            File.WriteAllText(nextProductIdFile, productId.ToString());

            return productId;
        }

        string RootDirectory { get; set; }
        string OutputDir
        {
            set; get;
        }

        string TempDir
        {
            get { return Path.Combine(RootDirectory, "temp"); }
        }

        string MediaDir { get { return Path.Combine(RootDirectory, "media"); } }

        public static void Mp3ToOgg(string mp3SourceFile, string oggDestinationFile)
        {
            Trace.TraceInformation("Convert {0} to {1}", mp3SourceFile, oggDestinationFile);
            var ext = Path.GetExtension(mp3SourceFile);
            if (ext.Equals(mp3Extension, StringComparison.InvariantCultureIgnoreCase))
            {
                SubProcess.Cmd(String.Format("mpg123 -w - {0} | oggenc - -o{1} --resample 22500 -downmix", mp3SourceFile.Quote(), oggDestinationFile.Quote()));
            }
            else if (ext.Equals(oggExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                SubProcess.Cmd(String.Format("oggdec -o{1} {0} & oggenc {1} -o{2} --resample 22500 -downmix & del {1}", 
                    CygPath(mp3SourceFile).Quote(), 
                    (oggDestinationFile + ".wav").Quote(),
                    oggDestinationFile.Quote()));
            }
        }

        static string CygPath(string windowsPath)
        {
            if (windowsPath.StartsWith(@"\\"))
            {
                return windowsPath.Replace('\\', '/');
            }
            else
            {
                return windowsPath;
            }
        }

        const string oggExtension = ".ogg";
        const string mp3Extension = ".mp3";
        bool alwaysConvert = false;

        public string ProvideOggFile(string mp3SourceFile)
        {
            var oggFile = Path.Combine(MediaDir, Digest(mp3SourceFile.ToLowerInvariant()) + oggExtension);
            if (!File.Exists(oggFile) || alwaysConvert)
            {
                Mp3ToOgg(mp3SourceFile, oggFile);
            }
            return oggFile;
        }

        static string Digest(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            return String.Join(String.Empty, hash.Select(_ => _.ToString("X2")));
        }

        public void Create(AlbumCollection collection)
        {
            var meta = new AlbumCollectionMeta
            {
                AlbumCollection = collection,
                Name = collection.Title,
                ProductId = GetNextProductId(),
            };

            var metaXml = Path.Combine(TempDir, meta.Name + ".xml");

            var oldMeta = PathUtil.ReadXml<AlbumCollectionMeta>(metaXml);
            if (oldMeta != null)
            {
                meta.ProductId = oldMeta.ProductId;
            }

            // assign oids
            meta.StopOid = meta.StartOid++;
            foreach (var i in collection.Album.SelectMany(_ => _.Tracks))
            {
                var oggFile = ProvideOggFile(i);
                meta.TrackInfo[i] = new TrackInfo
                {
                    Oid = meta.StartOid++,
                    Id = Path.GetFileNameWithoutExtension(oggFile),
                    Length = new FileInfo(oggFile).Length
                };
            }

            // check if size > 800 MB and split into parts if required
            var parts = collection.Album.Partition(_ => _.Tracks.Sum(track => meta.TrackInfo[track].Length), maxGmeSize).ToList();
            if (parts.Count > 1)
            {
                int partIndex = 1;
                foreach (var part in parts)
                {
                    var partcollection = new AlbumCollection
                    {
                        Album = part.ToArray(),
                        Title = String.Format("{0} - Part {1} of {2}", collection.Title, partIndex, parts.Count)
                    };
                    Create(partcollection);
                    ++partIndex;
                }
                return;
            }

            PathUtil.WriteXml(metaXml, meta);
            
            // write yaml
            WriteYaml(meta);

            // write html
            WriteHtml(meta);

            // open html
            Process.Start(meta.HtmlFile);

            // create gme
            Assemble(meta);

            // copy gme file to stick
            CopyToStick(meta);
        }

        private void Assemble(AlbumCollectionMeta meta)
        {
            // assemble
            meta.GmeFile = Path.Combine(OutputDir, meta.Name + ".gme");
            SubProcess.Cmd(String.Format("tttool assemble {0} {1}", meta.YamlFile.Quote(), meta.GmeFile.Quote()));
        }

        private void WriteHtml(AlbumCollectionMeta meta)
        {
            // create html page
            meta.HtmlFile = Path.Combine(OutputDir, meta.Name + ".html");
            var ow = new OidSvgWriter(new TiptoiOidCode());
            using (var w = new StreamWriter(meta.HtmlFile))
            {
                w.WriteLine(@"
<!doctype html>
<html moznomarginboxes mozdisallowselectionprint>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
	<link rel=""stylesheet"" href=""media/style.css"" />
    <title>" + HttpUtility.HtmlEncode(meta.AlbumCollection.Title) + @"</title>
  </head>
  <body>
");
                w.WriteLine("<h1>");
                ow.OidButton(w, meta.ProductId, "Start"); ow.OidButton(w, meta.StopOid, "Stop");
                w.WriteLine(HttpUtility.HtmlEncode(meta.AlbumCollection.Title));
                w.WriteLine("</h1>");

                foreach (var album in meta.AlbumCollection.Album)
                {
                    w.WriteLine(@"<div class=""album"">");
                    if (album.Picture != null)
                    {
                        var pic = Path.Combine(MediaDir, Digest(album.Picture) + Path.GetExtension(album.Picture));
                        PathUtil.CopyIfNewer(album.Picture, pic);
                        w.WriteLine(@"<img src={0} class=""album-art"" />", ("media/" + Path.GetFileName(pic)).Quote());
                    }
                    else
                    {
                        w.Write(@"<div class=""album-art"" >");
                        w.Write(HttpUtility.HtmlEncode(album.Title));
                        w.Write(@"</div>");
                    }
                    w.WriteLine("<h2>{0}</h2>", album.Title);
                    foreach (var i in album.Tracks)
                    {
                        var trackInfo = meta.TrackInfo[i];
                        w.WriteLine("<a href={0}>", ("media/" + trackInfo.Id + oggExtension).Quote());
                        ow.OidArea(w, trackInfo.Oid);
                        w.WriteLine("</a>");
                    }
                    w.WriteLine(@"</div>");
                }

                w.WriteLine(@"
  </body>
</html>
");
            }
        }

        private void WriteYaml(AlbumCollectionMeta meta)
        {
            // write tttool yaml file
            meta.YamlFile = Path.Combine(TempDir, meta.Name + ".yaml");
            using (var w = new StreamWriter(meta.YamlFile))
            {
                w.WriteLine(@"
product-id: " + meta.ProductId.ToString() + @"
media-path: ../media/%s
comment: CHOMPTECH DATA FORMAT CopyRight 2009 Ver2.6.0925
welcome: ding

scripts:
");

                w.WriteLine("  {0}:", meta.StopOid);
                w.WriteLine("  - P({0})", "ding");
                foreach (var album in meta.AlbumCollection.Album)
                {
                    foreach (var track in album.Tracks)
                    {
                        var trackInfo = meta.TrackInfo[track];
                        w.WriteLine("  {0}:", trackInfo.Oid);
                        w.WriteLine("  - P({0})", trackInfo.Id);
                    }
                }
            }
        }

        private void CopyToStick(AlbumCollectionMeta meta)
        {
            var stick = TipToiStick.GetAll().FirstOrDefault();

            if (stick != null)
            {
                var source = meta.GmeFile;
                var dest = Path.Combine(stick.RootDirectory, Path.GetFileName(meta.GmeFile));
                Trace.TraceInformation("Copy {0} to {1}", source, dest);
                File.Copy(source, dest, true);
            }
        }
    }
}
