using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blue;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Web.Http;
using System.Net.Http;

namespace Blue.Cosacs.File
{
    public class SqlStorageProvider : IStorageProvider
    {
        public Guid Save(FileDescription file)
        {
            using (var scope = Context.Write())
            {
                var fileDesc = scope.Context.FileDescription.Where(p => p.FileId == file.FileId).FirstOrDefault();

                if (fileDesc != null)
                {
                    fileDesc = new FileDescription()
                    {
                        CreatedBy = file.CreatedBy,
                        CreatedOn = file.CreatedOn,
                        FileContent = file.FileContent,
                        FileContentType = file.FileContentType,
                        FileExtension = file.FileExtension,
                        FileId = file.FileId,
                        FileName = file.FileName,
                        FileSize = file.FileSize
                    };
                }
                else
                {
                    scope.Context.FileDescription.Add(file);
                }

                scope.Context.SaveChanges();
                scope.Complete();

                return file.FileId;
            }
        }

        public void Delete(Guid guid)
        {
            using (var scope = Context.Write())
            {
                scope.Context.FileDescription.Remove(scope.Context.FileDescription.Where(p => p.FileId == guid).FirstOrDefault());
                scope.Context.SaveChanges();
                scope.Complete();
            }
        }

        public FileDescription Get(Guid guid)
        {
            using (var scope = Context.Read())
            {
               return scope.Context.FileDescription.Where(f => f.FileId == guid).FirstOrDefault();
            }
        }
    }
}
