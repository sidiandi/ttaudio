using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ttaenc
{
    public class MediaFileConverter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static MediaFileConverter()
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

        const string oggExtension = ".ogg";
        const string mp3Extension = ".mp3";
        const string wavExtension = ".wav";

        bool alwaysConvert = false;

        public async static Task AudioFileToTipToiAudioFile(CancellationToken cancellationToken, string sourceFile, string oggDestinationFile)
        {
            using (new LogScope(log, "Convert {0} to {1}", sourceFile, oggDestinationFile))
            {
                using (var t = new FileTransaction(oggDestinationFile))
                {
                    var ext = Path.GetExtension(sourceFile).ToLowerInvariant();
                    var wavFile = oggDestinationFile + ".wav";
                    try
                    {
                        switch (ext)
                        {
                            case mp3Extension:
                                {
                                    await SubProcess.CheckedCall(cancellationToken
                                        , "mpg123.exe",
                                        "-w", wavFile.Quote(),
                                        sourceFile.Quote());

                                    await SubProcess.CheckedCall(cancellationToken,
                                        "oggenc2.exe",
                                        wavFile.Quote(),
                                        ("--output=" + t.TempPath).Quote(),
                                        "--quiet",
                                        "--resample", "22500",
                                        "--downmix");
                                }
                                break;
                            case oggExtension:
                                {
                                    await SubProcess.CheckedCall(cancellationToken
                                        , "oggdec.exe",
                                        "--wavout", wavFile.Quote(),
                                        "-q",
                                        sourceFile.Quote());

                                    await SubProcess.CheckedCall(cancellationToken,
                                        "oggenc2.exe",
                                        "-o", t.TempPath.Quote(),
                                        "--quiet",
                                        "--resample", "22500",
                                        "--downmix",
                                        wavFile.Quote()
                                        );
                                }
                                break;
                            case wavExtension:
                                {
                                    await SubProcess.CheckedCall(cancellationToken,
                                        "oggenc2.exe",
                                        sourceFile.Quote(),
                                        "-o", t.TempPath.Quote(),
                                        "--quiet",
                                        "--resample", "22500",
                                        "--downmix");
                                }
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("sourceFile", sourceFile, "File type is not supported.");
                        }
                    }
                    finally
                    {
                        PathUtil.EnsureFileNotExists(wavFile);
                    }

                    t.Commit();
                }
            }
        }

        public MediaFileConverter(string cacheDirectory)
        {
            this.cacheDirectory = cacheDirectory;
        }

        readonly string cacheDirectory;

        public async Task<string> ProvidePenAudioFile(CancellationToken cancellationToken, string mp3SourceFile)
        {
            var oggFile = Path.Combine(cacheDirectory, Digest.Get(mp3SourceFile.ToLowerInvariant()) + oggExtension);
            if (!File.Exists(oggFile) || alwaysConvert)
            {
                await AudioFileToTipToiAudioFile(cancellationToken, mp3SourceFile, oggFile);
            }
            return oggFile;
        }
    }
}
