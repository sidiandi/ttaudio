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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace tta
{
    /// <summary>
    /// Creates audio album collections for the TipToi pen
    /// </summary>
    public class AlbumMaker
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        long maxGmeSize = 800 * 1024 * 1024;

        static AlbumMaker()
        {
            foreach (var p in new[] {
                "tools",
                @"tools\tttool-win32-1.5.1",
                @"tools\mpg123-1.22.0-x86-64"
                })
            {
                var d = Path.Combine(PathUtil.GetDirectory(), p);
                if (!Directory.Exists(d))
                {
                    throw new System.IO.FileNotFoundException(d);
                }
                PathUtil.AddToPath(d);
            }
        }

        public static string GetDefaultDataDirectory()
        {
            var a = Assembly.GetExecutingAssembly();

            return Path.Combine(
                System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                a.GetCustomAttribute<AssemblyCompanyAttribute>().Company,
                a.GetCustomAttribute<AssemblyProductAttribute>().Product);
        }

        public AlbumMaker(string directory)
        {
            this.RootDirectory = directory;
            PathUtil.EnsureDirectoryExists(MediaDir);
            this.OutputDir = RootDirectory;
            PathUtil.EnsureDirectoryExists(TempDir);

            var binDir = PathUtil.GetDirectory();
            PathUtil.CopyIfNewer(Path.Combine(binDir, "media"), MediaDir);
        }

        int GetNextProductId()
        {
            var nextProductIdFile = Path.Combine(TempDir, "next-product-id");

            var productId = 800;
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

        public async static Task Mp3ToOgg(CancellationToken cancellationToken, string mp3SourceFile, string oggDestinationFile)
        {
            using (new LogScope(log, "Convert {0} to {1}", mp3SourceFile, oggDestinationFile))
            {
                using (var t = new FileTransaction(oggDestinationFile))
                {
                    var ext = Path.GetExtension(mp3SourceFile);
                    if (ext.Equals(mp3Extension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        await SubProcess.Cmd(cancellationToken, String.Format("mpg123 -w - {0} | oggenc2 - -o{1} --resample 22500 -downmix", mp3SourceFile.Quote(), oggDestinationFile.Quote()));
                    }
                    else if (ext.Equals(oggExtension, StringComparison.InvariantCultureIgnoreCase))
                    {
                        await SubProcess.Cmd(cancellationToken, String.Format("oggdec -o{1} {0} & oggenc2 {1} -o{2} --resample 22500 -downmix & del {1}",
                            CygPath(mp3SourceFile).Quote(),
                            (oggDestinationFile + ".wav").Quote(),
                            t.TempPath.Quote()));
                    }
                }
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

        public async Task<string> ProvideOggFile(CancellationToken cancellationToken, string mp3SourceFile)
        {
            var oggFile = Path.Combine(MediaDir, Digest(mp3SourceFile.ToLowerInvariant()) + oggExtension);
            if (!File.Exists(oggFile) || alwaysConvert)
            {
                await Mp3ToOgg(cancellationToken, mp3SourceFile, oggFile);
            }
            return oggFile;
        }

        static string Digest(string text)
        {
            var md5 = new MD5CryptoServiceProvider();
            var hash = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            return String.Join(String.Empty, hash.Select(_ => _.ToString("X2")));
        }

        public async Task Create(CancellationToken cancellationToken, AlbumCollection collection)
        {
            var meta = new AlbumCollectionMeta
            {
                AlbumCollection = collection,
                Name = PathUtil.GetValidFileName(collection.Title),
                ProductId = collection.ProductId
            };
            meta.GmeFile = Path.Combine(OutputDir, meta.Name + ".gme");

            var metaXml = Path.Combine(TempDir, meta.Name + ".xml");

            var oldMeta = PathUtil.ReadXml<AlbumCollectionMeta>(metaXml);

            if (collection.ProductId != 0)
            {
                meta.ProductId = collection.ProductId;
            }
            else
            {
                if (oldMeta == null)
                {
                    meta.ProductId = GetNextProductId();
                }
                else
                {
                    meta.ProductId = oldMeta.ProductId;
                }
            }

            using (new LogScope(log, "Convert {0} (ProductId={1})", meta.Name, meta.ProductId))
            {

                // convert audio files to ogg
                var tracks = collection.Album.SelectMany(_ => _.Tracks).ToList();
                Parallel.ForEach(
                    tracks,
                    new ParallelOptions { CancellationToken = cancellationToken, MaxDegreeOfParallelism = System.Environment.ProcessorCount },
                    i =>
                    {
                        var oggFile = ProvideOggFile(cancellationToken, i.Path).Result;
                    });

                cancellationToken.ThrowIfCancellationRequested();

                // assign oids
                meta.StopOid = meta.StartOid++;
                foreach (var i in tracks)
                {
                    var oggFile = await ProvideOggFile(cancellationToken, i.Path);
                    meta.TrackInfo[i.Path] = new TrackInfo
                    {
                        Oid = meta.StartOid++,
                        Id = Path.GetFileNameWithoutExtension(oggFile),
                        Length = new FileInfo(oggFile).Length
                    };
                }

                cancellationToken.ThrowIfCancellationRequested();

                // check if size > 800 MB and split into parts if required
                var parts = collection.Album.Partition(_ => _.Tracks.Sum(track => meta.TrackInfo[track.Path].Length), maxGmeSize).ToList();
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
                        await Create(cancellationToken, partcollection);
                        ++partIndex;
                    }
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();

                PathUtil.WriteXml(metaXml, meta);

                // write yaml
                WriteYaml(meta);

                // write html
                WriteHtml(cancellationToken, meta);

                // open html
                Process.Start(meta.HtmlFile);

                cancellationToken.ThrowIfCancellationRequested();

                // create gme
                await Assemble(cancellationToken, meta);

                cancellationToken.ThrowIfCancellationRequested();

                // copy gme file to stick
                await CopyToStick(cancellationToken, meta);
            }
        }

        private async Task Assemble(CancellationToken cancellationToken, AlbumCollectionMeta meta)
        {
            await SubProcess.Cmd(cancellationToken, String.Format("tttool assemble {0} {1}", meta.YamlFile.Quote(), meta.GmeFile.Quote()));
        }

        private void WriteHtml(CancellationToken cancellationToken, AlbumCollectionMeta meta)
        {
            // create html page
            meta.HtmlFile = Path.Combine(OutputDir, meta.Name + ".html");
            meta.HtmlMediaFiles.Add("style.css");
            meta.HtmlMediaFiles.Add("note_to_pen.png");
            meta.HtmlMediaFiles.Add("default-album-art.png");

            log.InfoFormat("Write {0}", meta.HtmlFile);

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
    <div class=""printInstructions"">
        <img src=""media/note_to_pen.png"" />
        <p><a href=""javascript:window.print();"">Click here to print the page with optical codes to play your audio files.</a> Use a printer with at least 600 dpi.</p>
        <p>This page was generated by <a href=""https://github.com/sidiandi/tta"">tta</a> on " + HttpUtility.HtmlEncode(DateTime.Now.ToString()) + 
        @". Product ID = " + HttpUtility.HtmlEncode(meta.ProductId.ToString()) + @", GME file = " + HttpUtility.HtmlEncode(Path.GetFileName(meta.GmeFile)) + @"</p>
    </div>
");
                w.WriteLine("<h1>");
                ow.OidButton(w, meta.ProductId, "Start"); ow.OidButton(w, meta.StopOid, "Stop");
                w.WriteLine(HttpUtility.HtmlEncode(meta.AlbumCollection.Title));
                w.WriteLine("</h1>");

                foreach (var album in meta.AlbumCollection.Album)
                {
                    w.WriteLine(@"<div class=""album"">");
                    string pic;
                    if (album.Picture != null)
                    {
                        pic = Path.Combine(MediaDir, Digest(album.Picture) + Path.GetExtension(album.Picture));
                        meta.HtmlMediaFiles.Add(Path.GetFileName(pic));
                        PathUtil.Copy(cancellationToken, album.Picture, pic);
                    }
                    else
                    {
                        pic = Path.Combine(MediaDir, "default-album-art.png");
                    }
                    w.WriteLine(@"<img src={0} class=""album-art"" />", ("media/" + Path.GetFileName(pic)).Quote());
                    w.WriteLine("<h2>{0}</h2>", album.Title);
                    foreach (var i in album.Tracks)
                    {
                        var trackInfo = meta.TrackInfo[i.Path];
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
            log.InfoFormat("Write {0}", meta.YamlFile);

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
                        var trackInfo = meta.TrackInfo[track.Path];
                        w.WriteLine("  {0}:", trackInfo.Oid);
                        w.WriteLine("  - P({0})", trackInfo.Id);
                    }
                }
            }
        }

        private async Task CopyToStick(CancellationToken cancel, AlbumCollectionMeta meta)
        {
            var stick = TipToiPen.GetAll().FirstOrDefault();

            if (stick == null)
            {
                log.InfoFormat("Audio stick not plugged in. You must copy {0} to the stick manually.", meta.GmeFile);
            }
            else
            {
                log.InfoFormat("Audio stick found at {0}. {1} will be copied to the stick now.", stick.RootDirectory, meta.GmeFile);

                await PathUtil.CopyToDir(cancel, meta.GmeFile, stick.RootDirectory);
                var htmlDest = Path.Combine(stick.RootDirectory, About.Product, meta.Name);
                await PathUtil.CopyToDir(cancel, meta.HtmlFile, htmlDest);
                foreach (var mf in meta.HtmlMediaFiles)
                {
                    await PathUtil.CopyToDir(cancel, Path.Combine(MediaDir, mf), Path.Combine(htmlDest, Path.GetFileName(MediaDir)));
                }
            }
        }
    }
}
