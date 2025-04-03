using Blue.Cosacs.NonStocks.ExternalHttpService;
using Blue.Cosacs.NonStocks.Models;
using Blue.Cosacs.NonStocks.Models.WinCosacs;
using Blue.Cosacs.NonStocks.Solr;
using Blue.Events;
using Blue.Glaucous.Client.Api;
using Blue.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Blue.Cosacs.NonStocks.Api.Controllers
{
    [RoutePrefix("api/NonStock")]
    public class NonStockController : ApiController
    {
        private readonly INonStocksRepository nonStocksRepository = null;
        private readonly IClock clock = null;
        private readonly ICourtsNetWS courtsNetWS = null;
        private readonly IHttpClientJson httpClientJson = null;
        private readonly IEventStore audit;

        public NonStockController(INonStocksRepository nonStocksRepository,
            IClock clock, ICourtsNetWS courtsNetWS, IHttpClientJson httpClientJson, IEventStore audit)
        {
            this.nonStocksRepository = nonStocksRepository;
            this.clock = clock;
            this.courtsNetWS = courtsNetWS;
            this.httpClientJson = httpClientJson;
            this.audit = audit;
        }

        [Route("ForceIndex")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage ForceIndex()
        {
            try
            {
                SolrIndex.Index();

                audit.Log(@event: GetUniversalTimeLogObject(),
                    category: AuditCategories.NonStocks, type: AuditEventTypes.IndexAllNonStocks);

                return Request.CreateResponse(new { Result = "Done" });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e);
            }
        }

        [Route("Save")]
        [HttpPost]
        [Permission(PermissionsEnum.NonStocksEdit)]
        public HttpResponseMessage Save(Models.NonStockModel item)
        {
            var code = HttpStatusCode.InternalServerError;
            var duplicateSKUErrorMsg = "Product SKU must be unique. Another product already exists with the same SKU.";
            try
            {
                var hierarchy = CreateNonStockHierarchy(item.Hierarchy);

                if (item.Id > 0)
                {
                    nonStocksRepository.SaveNonStockDetails(item.ToEntity(), hierarchy);
                    code = HttpStatusCode.Accepted;
                }
                else
                {
                    item.Id = nonStocksRepository.SaveNonStockDetails(item.ToEntity(), hierarchy);
                    code = HttpStatusCode.Created;
                }

                SolrIndex.Index(item.Id);

                audit.Log(@event: item, category: AuditCategories.NonStocks, type: AuditEventTypes.CreateNonStock);

                return Request.CreateResponse(code, new { Result = "Done", Id = item.Id });
            }
            catch (System.Data.DataException de)
            {
                if (de.InnerException != null && de.InnerException.InnerException != null) 
                {
                    var sqlInnerException = de.InnerException.InnerException;
                    var errorToSend = string.Empty;

                    if (de.InnerException.InnerException.ToString().Contains(
                        "Violation of UNIQUE KEY constraint 'UQ_NonStocks_NonStock_SKU"))
                    {
                        errorToSend = duplicateSKUErrorMsg;
                    }

                    return Request.CreateResponse(HttpStatusCode.BadRequest, errorToSend);
                }
                else
                {
                    throw;
                }
            }
            catch (OperationCanceledException ex)
            {
                var stringToCheck = "Duplicate VendorUPC.";
                var errorToSend = string.Empty;
                if (ex.ToString().Contains(stringToCheck))
                {
                    errorToSend = "Save operation failed due to duplicate Vendor UPC. Please choose another value.";
                }
                else if (ex.ToString().Contains("Duplicate SKU."))
                {
                    errorToSend = duplicateSKUErrorMsg;

                }
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, errorToSend);
            }
        }

        private string cleanJsonChars(string json)
        {
            return json
                .Replace("\r", "").Replace("\n", "")
                .Replace("'", "").Replace("`", "")
                .Replace("\\", "/");
        }

        [Route("Load")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage Load(int id)
        {
            if (id <= 0)
            {
                return Request.CreateResponse(new { Result = "Error - Not a valid Id." });
            }

            var nonStock = nonStocksRepository.Load(id);

            audit.Log(@event: nonStock, category: AuditCategories.NonStocks, type: AuditEventTypes.ViewNonStock);

            return Request.CreateResponse(new { Result = "Done", @NonStock = nonStock });
        }

        [HttpGet]
        //[Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage LoadBySku(string sku)
        {
            if (sku == string.Empty)
            {
                return Request.CreateResponse(new { Result = "Error - Not a valid SKU." });
            }

            var nonStock = nonStocksRepository.Load(sku);

            audit.Log(@event: nonStock, category: AuditCategories.NonStocks, type: AuditEventTypes.ViewNonStockBySku);

            return Request.CreateResponse(new { Result = "Done", @NonStock = nonStock });


        }

        [Route("LoadAll")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksView)]
        public HttpResponseMessage LoadAll()
        {
            var nonStocks = nonStocksRepository.LoadAll();

            audit.Log(@event: GetUniversalTimeLogObject(),
                category: AuditCategories.NonStocks, type: AuditEventTypes.GuiGetAllNonStocks);

            return Request.CreateResponse(new { Result = "Done", @NonStocks = nonStocks });
        }

        [Route("Export")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksExport)]
        public HttpResponseMessage Export()
        {
            var user = this.GetUser().Login;
            
            courtsNetWS.UpdateCodeMaintenance(
                nonStocksRepository.GetActiveNonStocks(new List<string>()
                {
                    NonStockTypes.Installation, NonStockTypes.ReadyAssist
                }));

            courtsNetWS.Fact2000SetupEODFile(
                            new
                            {
                                ProdFileContent = nonStocksRepository.ExportProductsFile(user),
                                PromoFileContent = nonStocksRepository.ExportPromotionsFile(user),
                                ProdAssocFileContent = nonStocksRepository.ExportProductLinksFile(user),
                            });
            
            audit.Log(@event: GetUniversalTimeLogObject(),
                category: AuditCategories.NonStocks, type: AuditEventTypes.ExportNonStocks);

            return Request.CreateResponse(new { Result = "Done" });
        }

        [Route("DownloadProductsFile")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksExport)]
        public HttpResponseMessage DownloadProductsFile()
        {
            var user = this.GetUser().Login;
            var file = nonStocksRepository.ExportProductsFile(user);

            audit.Log(@event: GetUniversalTimeLogObject(),
                category: AuditCategories.NonStocks, type: AuditEventTypes.DownloadProductsFile);

            var response = GetHttpResponseFileSteam(NonStocksFactExport.ProdFileName, file);
            response.Headers.Location = GetNonStocksUriForGlaucous();

            return response;
        }

        [Route("DownloadPromotionsFile")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksExport)]
        public HttpResponseMessage DownloadPromotionsFile()
        {
            var user = this.GetUser().Login;
            var file = nonStocksRepository.ExportPromotionsFile(user);

            audit.Log(@event: GetUniversalTimeLogObject(),
                category: AuditCategories.NonStocks, type: AuditEventTypes.DownloadPromotionsFile);

            var response = GetHttpResponseFileSteam(NonStocksFactExport.PromoFileName, file);
            response.Headers.Location = GetNonStocksUriForGlaucous();

            return response;
        }

        [Route("DownloadProductAssociationsFile")]
        [HttpGet]
        [Permission(PermissionsEnum.NonStocksExport)]
        public HttpResponseMessage DownloadProductAssociationsFile()
        {
            var user = this.GetUser().Login;
            var file = nonStocksRepository.ExportProductLinksFile(user);

            audit.Log(@event: GetUniversalTimeLogObject(),
                category: AuditCategories.NonStocks, type: AuditEventTypes.DownloadProductAssociationsFile);

            var response = GetHttpResponseFileSteam(NonStocksFactExport.ProdAssocFileName, file);
            response.Headers.Location = GetNonStocksUriForGlaucous();

            return response;
        }

        private HttpResponseMessage GetHttpResponseFileSteam(string fileName, string fileString)
        {
            var output = new byte[] { };
            using (var stream = new MemoryStream())
            {
                var sw = new StreamWriter(stream);

                sw.Write(fileString);
                sw.Flush();

                stream.Position = 0;
                output = stream.ToArray();
                stream.Flush();
            }

            var result = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(output) };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = fileName
            };

            return result;
        }

        private Uri GetNonStocksUriForGlaucous()
        {
            var requestContext = Request.GetRequestContext();
            if (requestContext != null && requestContext.Url != null)
            {
                var requestUri = requestContext.Url.Request.RequestUri;
                var glaucousRootUri = new Uri(requestUri.GetLeftPart(UriPartial.Authority));

                return glaucousRootUri;
            }
            return new Uri(string.Empty);
        }

        private List<NonStockHierarchy> CreateNonStockHierarchy(List<NonStockModel.NonStockHierarchyModel> items)
        {
            var retList = new List<NonStockHierarchy>();

            for (int i = 0; i < items.Count; i++)
            {
                retList.Add(new NonStockHierarchy()
                {
                    Id = 0,
                    NonStockId = 0,
                    Level = (byte)(i + 1),
                    LevelKey = items[i].SelectedKey,
                    LevelName = items[i].SelectedValue,
                });
            }

            return retList;
        }

        /// <summary>
        /// WTF is this shit. This is so bad. 
        /// </summary>
        /// <returns>
        /// A stupid thing that should not exist is the fisrt place
        /// </returns>
        private object GetUniversalTimeLogObject()
        {
            return new { date = clock.Now.Date.ToString("u") };
        }
    }
}
