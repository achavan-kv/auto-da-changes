using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Unicomer.Cosacs.Model
{
    public class UploadDownloadDocumentModel
    {
    }
    public class UploadDocument
    {
        public byte[] ByteArrayFile { get; set; }
        public string CustId { get; set; }
        public string AccountNumber { get; set; }
        public HttpFileCollection UploadedFiles { get; set; }
        public string TargetFolderPath { get; set; }
        public string TargetFileName { get; set; }
        public string UploadedFileName { get; set; }
    }
    public class CustCreditDocument
    {
        public string CustId { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string AccountNumber { get; set; }
        public bool IsThirdParty { get; set; }
        //public DateTime CreatedOn { get; set; }
        public string Message { get; set; }
    }
}
