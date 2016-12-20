using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ttaenc
{
    /// <summary>
    /// Writes gme, tta, html, and album art files for a package
    /// </summary>
    /// Usage: create an instance, modify properties, then call Build()
    public class PackageBuilder : HtmlGenerator
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        readonly IPackageDirectoryStructure packageDirectoryStructure;
        readonly MediaFileConverter converter;

        string yamlFile;

        public PackageBuilder(
            IPackageDirectoryStructure structure,
            MediaFileConverter converter,
            OidSvgWriter oidWriter
            )
        {
            this.packageDirectoryStructure = structure;
            this.converter = converter;
            this.MaxGmeSize = 800 * 1024 * 1024;
            OidSvgWriter = oidWriter;
        }

        public long MaxGmeSize
        {
            get; set;
        }

        OidSvgWriter OidSvgWriter
        {
            get; set;
        }

        public async Task Build(CancellationToken cancellationToken)
        {
            var p = packageDirectoryStructure.Package;

            PrepareInputFiles(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            // check if size > MaxGmeSize and split into parts if required
            var parts = Split(p);
            if (parts.Count > 1)
            {
                foreach (var i in parts)
                {
                    var structure = new PackageDirectoryStructure(Path.GetDirectoryName(this.packageDirectoryStructure.GmeFile), i);
                    var pb = new PackageBuilder(structure, this.converter, OidSvgWriter);
                    await pb.Build(cancellationToken);
                }
                return;
            }

            // write yaml
            WriteYaml();

            // write html
            WriteHtml(cancellationToken);

            // open html
            Process.Start(packageDirectoryStructure.HtmlFile);

            cancellationToken.ThrowIfCancellationRequested();

            // create gme
            await Assemble(cancellationToken);

            log.InfoFormat("{0} was sucessfully built.", packageDirectoryStructure.GmeFile);
        }

        string TempDir
        {
            get
            {
                return converter.OutputDirectory;
            }
        }

        private void WriteYaml()
        {
            var p = packageDirectoryStructure.Package;
            // write tttool yaml file
            yamlFile = Path.Combine(converter.OutputDirectory, packageDirectoryStructure.Package.FileName + ".yaml");
            log.InfoFormat("Write {0}", yamlFile);

            using (var w = new StreamWriter(yamlFile))
            {
                w.WriteLine(
@"# " + p.FileName + @"
product-id: " + p.ProductId.ToString() + @"
comment: CHOMPTECH DATA FORMAT CopyRight 2009 Ver2.6.0925
welcome: " + YamlPath(p.ConfirmationSound) + @"

scripts:");

                w.WriteLine("  {0}:", p.StopOid);
                w.WriteLine("  - P({0})", YamlPath(p.ConfirmationSound));
                for (int i=0; i<p.Tracks.Length;++i)
                {
                    var current = p.Tracks[i];
                    w.WriteLine("  {0}:", current.Oid);
                    w.Write("  - P({0})", YamlPath(current.PenAudioFile));

                    var next = (i < p.Tracks.Length - 1) ? p.Tracks[i + 1] : null;

                    switch (p.PlaybackMode)
                    {
                        case PlaybackModes.StopAfterEverything:
                            if (next != null)
                            {
                                w.JumpTo(next);
                            }
                            break;
                        case PlaybackModes.StopAfterEveryAlbum:
                            if (next != null)
                            {
                                if (string.Equals(current.Album, next.Album))
                                {
                                    w.JumpTo(next);
                                }
                            }
                            break;
                        case PlaybackModes.StopAfterEveryTrack:
                            break;
                        case PlaybackModes.LoopEverything:
                            if (next != null)
                            {
                                w.JumpTo(next);
                            }
                            else
                            {
                                w.JumpTo(p.Tracks.First());
                            }
                            break;
                        case PlaybackModes.LoopAlbum:
                            if (next != null)
                            {
                                if (string.Equals(current.Album, next.Album))
                                {
                                    w.JumpTo(next);
                                }
                                else
                                {
                                    w.JumpTo(p.Tracks.First(_ => string.Equals(_.Album, current.Album)));
                                }
                            }
                            break;
                        case PlaybackModes.LoopTrack:
                            w.JumpTo(current);
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    w.WriteLine();
                }
            }
        }

        static string YamlPath(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        void AddStaticMediaFile(CancellationToken cancellationToken, string fileName)
        {
            PathUtil.CopyToDir(cancellationToken,
                Path.Combine(PathUtil.GetDirectory(), "media", fileName),
                Path.Combine(packageDirectoryStructure.HtmlMediaDirectory));
        }

        private void WriteHtml(CancellationToken cancellationToken)
        {
            var p = packageDirectoryStructure.Package;
            var htmlMediaFiles = new List<string>();

            AddStaticMediaFile(cancellationToken, "style.css");
            AddStaticMediaFile(cancellationToken, "power.svg");
            AddStaticMediaFile(cancellationToken, "stop.svg");

            var styles = new DirectoryInfo(Path.Combine(PathUtil.GetDirectory(), "media")).GetFiles("*.css")
                .Where(_ => !object.Equals(_.Name, "style.css")).ToList();

            foreach (var i in styles)
            {
                AddStaticMediaFile(cancellationToken, i.Name);
            }

            AddStaticMediaFile(cancellationToken, "note_to_pen.png");

            log.InfoFormat("Write {0}", packageDirectoryStructure.HtmlFile);

            var ow = OidSvgWriter;
            PathUtil.EnsureParentDirectoryExists(packageDirectoryStructure.HtmlFile);
            using (var w = new StreamWriter(packageDirectoryStructure.HtmlFile))
            {
                w.WriteLine(@"
<!doctype html>
<html moznomarginboxes mozdisallowselectionprint>
  <head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
	<link rel=""stylesheet"" href=""media/style.css"" />
	<link rel=""stylesheet"" href=""media/default.css"" />
    <title>" + T(this.packageDirectoryStructure.Package.Title) + @"</title>
	<script>
	function changeCSS(cssFile) {
		var cssLinkIndex = 1;
		var oldlink = document.getElementsByTagName(""link"").item(cssLinkIndex);
		oldlink.setAttribute(""href"", cssFile);
	}
	</script>
  </head>
  <body>
    <div class=""printInstructions"">
        <img src=""media/note_to_pen.png"" />
        <p><a href=""javascript:window.print();"">Click here to print the page with optical codes to play your audio files.</a> Use a printer with at least 600 dpi.</p>
        <p>This page was generated by <a href=""" + About.GithubUri.ToString() + @""">" + T(About.Product) + @"</a> on " + T(DateTime.Now.ToString()) +
        @". Product ID = " + T(p.ProductId.ToString()) + @", GME file = " + T(packageDirectoryStructure.GmeFile) + @"</p>

Style: ");

                foreach (var i in styles)
                {
                    w.WriteLine(@"<a href=""#"" onclick=""changeCSS('media/{0}')"" >{1}</a>", i.Name, Path.GetFileNameWithoutExtension(i.Name));
                }

                w.WriteLine(@"</div>");
                w.WriteLine("<h1>");
                ow.OidButton(w, p.ProductId, "<img class=\"oid-area\" src=\"media/power.svg\" />");
                ow.OidButton(w, p.StopOid, "<img class=\"oid-area\" src=\"media/stop.svg\" />");
                w.WriteLine(T(p.Title));
                w.WriteLine("</h1>");

                foreach (var album in p.Albums)
                {
                    w.WriteLine(@"<div class=""album"">");
                    var pic = Extract(album.GetPicture());
                    htmlMediaFiles.Add(Path.GetFileName(pic));
                    w.WriteLine(@"<img src={0} class=""album-art"" />", ("media/" + Path.GetFileName(pic)).Quote());
                    w.WriteLine("<h2 class=\"album-title\" >{0} - {1}</h2>", T(album.Artist), T(album.Title));
                    w.WriteLine(@"<ul class={0}>", "track-list");
                    foreach (var track in album.Tracks)
                    {
                        w.WriteLine("<li>");
                        ow.OidArea(w, track.Oid);
                        w.WriteLine("</a>");
                        w.WriteLine("<span class={0}>{1}</span> ", "track-number".Quote(), T(track.TrackNumber.ToString()));
                        w.WriteLine("<span class={0}>{1}</span> ", "track-title".Quote(), T(track.Title));
                        w.WriteLine("<span class={0}>({1})</span> ", "track-duration".Quote(), T(track.Duration.ToString(@"mm\:ss")));
                        w.WriteLine("</li>");
                    }
                    w.WriteLine("</ul>");
                    w.WriteLine(@"<br class=""album-clear"" />");
                    w.Write(@"</div>");
                }

                w.WriteLine(@"
  </body>
</html>
");
            }
        }

        IList<Package> Split(Package p)
        {
            var partTracks = p.Tracks
                .Partition(track => new FileInfo(track.PenAudioFile).Length, MaxGmeSize)
                .ToList();

            return partTracks.Select((tracks, partIndex) =>
            {
                var title = String.Format("{0} - Part {1} of {2}", p.Title, partIndex, partTracks.Count);
                return new Package(new ProductIdProvider())
                {
                    Tracks = tracks.ToArray(),
                    Title = title,
                    FileName = PathUtil.GetValidFileName(title)
                };
            }).ToList();
        }

        public async Task OpenHtmlPage(CancellationToken cancellationToken)
        {
            var p = packageDirectoryStructure.Package;

            PrepareInputFiles(cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            // check if size > MaxGmeSize and split into parts if required
            var parts = Split(p);
            if (parts.Count > 1)
            {
                foreach (var i in parts)
                {
                    var structure = new PackageDirectoryStructure(Path.GetDirectoryName(this.packageDirectoryStructure.GmeFile), i);
                    var pb = new PackageBuilder(structure, this.converter, OidSvgWriter);
                    await pb.OpenHtmlPage(cancellationToken);
                }
                return;
            }

            // write yaml
            WriteYaml();

            // write html
            WriteHtml(cancellationToken);

            // open html
            Process.Start(packageDirectoryStructure.HtmlFile);
        }

        void PrepareInputFiles(CancellationToken cancellationToken)
        {
            packageDirectoryStructure.Package.ConfirmationSound = converter.ProvidePenAudioFile(cancellationToken, Path.Combine(PathUtil.GetDirectory(), "media", "ding.ogg")).Result;
            // prepare input files
            Parallel.ForEach(
                packageDirectoryStructure.Package.Tracks,
                new ParallelOptions
                {
                    CancellationToken = cancellationToken,
                    MaxDegreeOfParallelism = System.Environment.ProcessorCount
                },
                i =>
                {
                    i.PenAudioFile = converter.ProvidePenAudioFile(cancellationToken, i.Path).Result;
                });
        }

        private async Task Assemble(CancellationToken cancellationToken)
        {
            var tempGmeFile = Path.Combine(TempDir, Path.GetFileName(packageDirectoryStructure.GmeFile));
            try
            {
                await SubProcess.Cmd(cancellationToken, String.Format("tttool assemble {0} {1}", yamlFile.Quote(), tempGmeFile.Quote()));
                await PathUtil.Copy(cancellationToken, tempGmeFile, packageDirectoryStructure.GmeFile);
            }
            finally
            {
                PathUtil.EnsureFileNotExists(tempGmeFile);
            }
        }

        /// <summary>
        /// Extract an 
        /// </summary>
        /// <param name="picture"></param>
        /// <returns></returns>
        string Extract(TagLib.IPicture picture)
        {
            PathUtil.EnsureDirectoryExists(packageDirectoryStructure.HtmlMediaDirectory);
            return AlbumReader.ExportPicture(picture, packageDirectoryStructure.HtmlMediaDirectory);
        }
    }

    static class TextWriterExtensions
    {
        public static void JumpTo(this TextWriter yamlWriter, Track track)
        {
            yamlWriter.Write(" J({0})", track.Oid);
        }
    }
}

