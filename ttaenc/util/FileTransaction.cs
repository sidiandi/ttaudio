using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ttaenc
{
    public sealed class FileTransaction : IDisposable
    {
        public FileTransaction(string finalPath)
        {
            this.finalPath = finalPath;
            for (;;)
            {
                tempPath = Path.Combine(Path.GetDirectoryName(finalPath),
                    System.IO.Path.GetRandomFileName() + "." + Path.GetFileName(finalPath));
                if (!(File.Exists(tempPath) || Directory.Exists(tempPath)))
                {
                    break;
                }
            }
            PathUtil.EnsureParentDirectoryExists(tempPath);
        }

        public void Commit()
        {
            committed = true;
        }

        public string TempPath
        {
            get { return tempPath; }
        }

        string finalPath;
        string tempPath;
        bool committed = false;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (committed)
                    {
                        PathUtil.EnsureFileNotExists(finalPath);
                        File.Move(tempPath, finalPath);
                    }
                    else
                    {
                        PathUtil.EnsureFileNotExists(tempPath);
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileTransaction() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
