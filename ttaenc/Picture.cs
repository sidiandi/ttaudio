using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace ttaenc
{
    class Picture : IPicture
    {
        public Picture(System.IO.FileInfo file)
        {
            using (var r = System.IO.File.OpenRead(file.FullName))
            {
                Data = new ByteVector(PathUtil.ReadAll(r));
            }
            MimeType = "image/" + file.Extension.Substring(1);
        }
        public ByteVector Data { get; set; }

        public string Description
        {
            get; set;
        }

        public string MimeType { get; set; }

        public PictureType Type
        {
            get
            {
                return PictureType.FrontCover;
            }

            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
