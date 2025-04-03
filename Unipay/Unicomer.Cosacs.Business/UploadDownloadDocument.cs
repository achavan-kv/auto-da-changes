using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;

namespace Unicomer.Cosacs.Business
{
    public class UploadDownloadDocument
    {
        public static string UploadDocumentWithByteArray(string targetPath, byte[] file, string custId, string targetFolderPath, string accountNumber, bool isThirdParty)
        {
            string message = string.Empty;
            if (!string.IsNullOrWhiteSpace(targetPath) && file != null)
            {
                #region "Save details of Upload Document"
                string fileName = Path.GetFileName(targetPath);
                UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                objCustCreditDocument.AccountNumber = accountNumber;
                objCustCreditDocument.CustId = custId;
                objCustCreditDocument.FileName = fileName;
                objCustCreditDocument.FolderPath = targetFolderPath;
                objCustCreditDocument.IsThirdParty = isThirdParty;
                message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                #endregion "Save details of Upload Document"
                if (!message.Equals("No user found"))
                {
                    if (!Directory.Exists(targetFolderPath))
                        Directory.CreateDirectory(targetFolderPath);
                    System.IO.File.WriteAllBytes(targetPath, file);
                }
            }
            else
                message = "File not uploaded.";
            return message;
        }

        public static string UploadDocumentWithFiles(string targetFolderPath, string targetFileName, HttpFileCollection fileList, string accountNumber, bool isThirdParty)
        {
            string message = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(targetFolderPath) && fileList != null)
                {
                    foreach (string file in fileList)
                    {
                        var postedFile = fileList[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {
                            int MaxContentLength = 1024 * 1024 * 20; //Size = 20 MB  

                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png", ".pdf", ".doc", ".docx" };
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(extension))
                            {
                                message = string.Format("Please upload file of type jpg, gif, png, pdf, doc, docx.");
                            }
                            else if (postedFile.ContentLength > MaxContentLength)
                            {
                                message = string.Format("Please upload a file upto {0} mb.", MaxContentLength);
                            }
                            else
                            {
                                //var filePath = System.Web.HttpContext.Current.Server.MapPath("~/Areas/Userimage/" + postedFile.FileName + extension);
                                string fileName = targetFileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
                                #region "Save details of Upload Document"
                                UploadDownloadFilesRepository objUploadDownloadFilesRepository = new UploadDownloadFilesRepository();
                                CustCreditDocument objCustCreditDocument = new CustCreditDocument();
                                objCustCreditDocument.AccountNumber = accountNumber;
                                objCustCreditDocument.CustId = targetFileName;
                                objCustCreditDocument.FileName = fileName;
                                objCustCreditDocument.FolderPath = targetFolderPath;
                                objCustCreditDocument.IsThirdParty = isThirdParty;
                                message = objUploadDownloadFilesRepository.CustCreditDocuemntsSave(objCustCreditDocument);
                                #endregion "Save details of Upload Document"
                                if (!message.Equals("No user found"))
                                {
                                    if (!Directory.Exists(targetFolderPath))
                                        Directory.CreateDirectory(targetFolderPath);
                                    var filePath = Path.Combine(targetFolderPath + fileName);
                                    postedFile.SaveAs(filePath);
                                }
                            }
                        }
                        else
                            message = string.Format("No files to upload.");
                        //return Request.CreateErrorResponse(HttpStatusCode.Created, message1); ;
                    }
                }
                else
                {
                    message = string.Format("No files to upload.");
                }
            }
            catch (Exception)
            {
                message = string.Format("Error!! occured wile uploading file.");
            }
            return message;
        }

        public static string GetTargetPath(string targetFolderPath, string targetFileName, string uploadedFileName, bool isCreateFolder = false)
        {
            string targetFilePath = string.Empty;
            if (!string.IsNullOrWhiteSpace(targetFolderPath) && !string.IsNullOrWhiteSpace(targetFileName))
            {
                string extFile = !string.IsNullOrWhiteSpace(uploadedFileName) ? Path.GetExtension(uploadedFileName) : string.Empty;
                if (string.IsNullOrWhiteSpace(extFile))
                    targetFileName += ".pdf";
                else
                    targetFileName += extFile;

                if (isCreateFolder && !Directory.Exists(targetFolderPath))
                    Directory.CreateDirectory(targetFolderPath);

                if (Directory.Exists(targetFolderPath))
                {
                    string[] directoryList = Directory.GetDirectories(targetFolderPath);
                    if (directoryList != null && directoryList.Length == 1)
                        targetFilePath = string.Format("{0}{1}//", targetFolderPath, directoryList[0]);
                }

                targetFilePath = string.Format("{0}{1}", targetFolderPath, targetFileName);
                if (File.Exists(targetFilePath))
                {
                    string[] fileList = Directory.GetFiles(targetFilePath, targetFileName);
                    targetFilePath = string.Format("{0}{1}_{2}.pdf", targetFolderPath, targetFileName, fileList.Length);
                }
            }
            return targetFilePath;
        }

        public void UploadDocumentToNetworkFolder(string serverPath, string fileName, string path, string fileType)
        {
            //string serverPath = String.Empty;
            //if (fileType == "p")
            //{
            //    serverPath = (string)Country[CountryParameterNames.PhotoDirectory];
            //}
            //else
            //{
            //    serverPath = (string)Country[CountryParameterNames.SignatureDirectory];
            //}
            string localPath = path;
            try
            {
                FileInfo localFile = new FileInfo(localPath);

                // Create a new WebClient instance.
                WebClient wc = new WebClient();

                // Upload the photograph and save it into the server directory.
                if (serverPath != String.Empty)
                {
                    FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] picture = new byte[fs.Length];
                    fs.Read(picture, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    NetworkCredential cred = new NetworkCredential();
                    wc.Credentials = CredentialCache.DefaultCredentials;
                    wc.UploadData(serverPath + "/" + fileName, "PUT", picture);
                }
                //else
                //{
                //    throw new STLException(GetResource("M_INVALIDPHOTODIRECTORY"));
                //}
            }
            catch (Exception)
            {
                // URL not found
                //throw ex;
            }

            //return File.Exists(localPath);
        }

        public static bool UploadDocumentToFTPFolder(string fileNameWithExtension, string localDirectoryPath, string serverDirectoryPath, string ftpUsername, string ftpPassword)
        {
            bool isFileUploaded = true;

            if (string.IsNullOrWhiteSpace(localDirectoryPath) || string.IsNullOrWhiteSpace(serverDirectoryPath))
            {
                return false;
            }

            try
            {
                string localFilePath = string.Format("{0}\\{1}", localDirectoryPath, fileNameWithExtension);

                FileInfo localFile = new FileInfo(localFilePath);
                if (!localFile.Exists)
                {
                    return false;
                }
                // Upload the photograph and save it into the server directory.
                using (FileStream fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] file = new byte[fs.Length];
                    fs.Read(file, 0, Convert.ToInt32(fs.Length));
                    fs.Close();
                    // Create a new WebClient instance.
                    using (WebClient wc = new WebClient())
                    {
                        //wc.Credentials = CredentialCache.DefaultCredentials;
                        wc.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                        wc.UploadData(string.Format("{0}/{1}", serverDirectoryPath, fileNameWithExtension), WebRequestMethods.Ftp.UploadFile, file);
                    }
                }
            }
            catch (Exception)
            {
                isFileUploaded = false;
            }
            return isFileUploaded;
        }

        public static bool UploadBase64DocumentToFTPFolder(string fileNameWithExtension, byte[] file, string serverDirectoryPath, string ftpUsername, string ftpPassword)
        {
            try
            {
                using (WebClient wc = new WebClient())
                {
                    //wc.Credentials = CredentialCache.DefaultCredentials;
                    wc.Credentials = new NetworkCredential(ftpUsername, ftpPassword);
                    wc.UploadData(string.Format("{0}/{1}", serverDirectoryPath, fileNameWithExtension), WebRequestMethods.Ftp.UploadFile, file);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
