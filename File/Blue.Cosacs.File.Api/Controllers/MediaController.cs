using Blue.Glaucous.Client.Api;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web.Http;
using System.Drawing;
using System.Drawing.Imaging;

namespace Blue.Cosacs.File.Api.Controllers
{
    [RoutePrefix("api/media")]
    public class MediaController : ApiController
    {
        private readonly IClock clock;
        private readonly IStorageProvider repository;

        public MediaController(IStorageProvider repository, IClock clock)
        {
            this.clock = clock;
            this.repository = repository;
        }

        [Route("{id}")]
        [HttpGet]
        [Permission(PermissionsEnum.DownloadFile)]
        public HttpResponseMessage Get(Guid id)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var file = repository.Get(id);
            result.Content = new ByteArrayContent(file.FileContent);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(file.FileContentType);
            return result;
        }

        [Permission(PermissionsEnum.DownloadFile)]
        [Route("{id}/thumb/{width}/{height}")]
        [HttpGet]
        public HttpResponseMessage GetThumbnail(Guid id, int width, int height)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);
            var file = repository.Get(id);
            result.Content = new ByteArrayContent(file.FileContent);
            var image = file.FileContentType == MediaTypeNames.Application.Pdf ? ByteToImage(Convert.FromBase64String(ThumbNails.Pdf)) : ByteToImage(file.FileContent);

            byte[] thumb;
            using (var stream = new MemoryStream())
            {
                image.GetThumbnailImage(width, height, () => false, IntPtr.Zero).Save(stream, ImageFormat.Png);
                thumb = stream.ToArray();
            }
            result.Content = new ByteArrayContent(thumb);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypes.Png);

            result.Headers.CacheControl = new CacheControlHeaderValue();
            result.Headers.CacheControl.Public = true;
            result.Headers.CacheControl.MaxAge = new TimeSpan(365, 0, 0, 0, 0);
            return result;
        }

        private Image ByteToImage(byte[] content)
        {
            using (var ms = new MemoryStream(content))
            {
                return Image.FromStream(ms);
            }
        }

        [Permission(PermissionsEnum.UploadFile)]
        [Route("")]
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var filesReadToProvider = await Request.Content.ReadAsMultipartAsync();

            var stream = filesReadToProvider.Contents[0];

            if (stream.Headers.ContentType.MediaType == MediaTypeNames.Image.Gif ||
                stream.Headers.ContentType.MediaType == MediaTypeNames.Image.Jpeg ||
                stream.Headers.ContentType.MediaType == MediaTypeNames.Application.Pdf ||
                stream.Headers.ContentType.MediaType == MediaTypes.Png)
            {
                var content = stream.ReadAsByteArrayAsync();
                content.Wait();

                var fileName = UnquoteToken(stream.Headers.ContentDisposition.FileName);

                var fileDesc = new FileDescription()
                {
                    CreatedBy = this.GetUser().Id,
                    FileName = fileName,
                    FileSize = stream.Headers.ContentLength.Value,
                    FileContentType = stream.Headers.ContentType.ToString(),
                    FileId = Guid.NewGuid(),
                    CreatedOn = clock.Now,
                    FileExtension = Path.GetExtension(fileName),
                    FileContent = content.Result
                };

                var guid = repository.Save(fileDesc);

                return Request.CreateResponse(guid);
            }
            else
            {
                throw new Exception("Unsupported Media Type. Please use one of the following : .jpeg, .jpg, .gif, .png, .pdf");
            }
        }

        [Permission(PermissionsEnum.DeleteFile)]
        [Route("{id}")]
        [HttpDelete]
        public HttpResponseMessage Delete(Guid id)
        {
            repository.Delete(id);
            return Request.CreateResponse();
        }

        private static string UnquoteToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return token;
            }

            if (token.StartsWith("\"", StringComparison.Ordinal) && token.EndsWith("\"", StringComparison.Ordinal) && token.Length > 1)
            {
                return token.Substring(1, token.Length - 2);
            }

            return token;
        }
    }
}
