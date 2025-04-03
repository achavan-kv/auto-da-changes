using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.File
{
    [Serializable]
    public class FileMetadata
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileExtension { get; set; }
        public string FileContentType { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public string Tags { get; set; }

        //public string FileSizeFriendly
        //{
        //    get
        //    {
        //        var size = "0 KB";

        //        var byteCount = FileSize;

        //        if (byteCount >= 1073741824)
        //            size = String.Format("{0:##.##}", byteCount / 1073741824) + " GB";
        //        else if (byteCount >= 1048576)
        //            size = String.Format("{0:##.##}", byteCount / 1048576) + " MB";
        //        else if (byteCount >= 1024)
        //            size = String.Format("{0:##.##}", byteCount / 1024) + " KB";
        //        else if (byteCount > 0 && byteCount < 1024)
        //            size = "1 KB";      

        //        return size;
        //    }
        //    set { }
        //}
    }
}
