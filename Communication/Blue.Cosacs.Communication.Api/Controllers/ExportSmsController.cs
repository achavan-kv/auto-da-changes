using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Http;
using Blue.Cosacs.Communication.Repositories;
using Blue.Events;
using Blue.Glaucous.Client.Api;

namespace Blue.Cosacs.Communication.Api.Controllers
{
   // [RoutePrefix("api/ExportSms")]
    [Permission(CommunicationPermissionEnum.ExportSendSms)]
    public class ExportSmsController : ApiController
    {
        private readonly ICommunicationRepository repository;
        private readonly IClock clock;
        private readonly IEventStore audit;

        public ExportSmsController(IEventStore audit, ICommunicationRepository repository, IClock clock)
        {
            this.audit = audit;
            this.repository = repository;
            this.clock = clock;
        }

        [Route("api/ExportSms/LoadData")]
        [HttpGet]
        public HttpResponseMessage LoadData()
        {
            return Request.CreateResponse(new
            {
                NotExportedYet = repository.GetTotalSmsNotSent(),
                PreviousExports = repository.GetPreviousExports(10)
            });
        }

        [Route("api/ExportSms/{exportedOn:DateTime?}")]
        public HttpResponseMessage Get(DateTime? exportedOn = null)
        {
            var data = repository.GetExport(exportedOn);

            var file = new StringBuilder();
            var replacer = new Regex("\"", RegexOptions.Compiled);

            if (data != null)
            {
                foreach (var item in data)
                {
                    file.Append(string.Format("\"{0}\"", replacer.Replace(item.Item1, "\"\"")))
                        .Append(",")
                        .Append(string.Format("\"{0}\"", replacer.Replace(item.Item2, "\"\"")))
                        .AppendLine();

                }
            }

            var result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(Encoding.GetEncoding("Windows-1252").GetBytes(file.ToString())),

            };

            result.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Text.Plain);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = string.Format("{0} - SmsToSend.csv", clock.Now.ToString("yyyyMMdd"))
            };

            if (!exportedOn.HasValue)
            {
                repository.UpdateCustomerInteraction(data
                    .Select(p=> p.Item3)
                    .Distinct()
                    .Select(p => new CustomerInteraction
                    {
                        CustomerId = p,
                        LastSmsSentOn = clock.Now
                    })
                    .ToList());
            }

            audit.LogAsync(new
            {
                TotalSmsExported = data.Count
            },
            "SmsExported");

            return result;
        }
    }
}
