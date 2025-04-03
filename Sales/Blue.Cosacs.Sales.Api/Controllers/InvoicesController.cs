using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Blue.Cosacs.Sales.Models;
using Blue.Cosacs.Sales.Repositories;
using Blue.Glaucous.Client.Api;
using RazorEngine;
using RazorEngine.Templating;
using Blue.Cosacs.Sales.Api.Extensions;
using Blue.Cosacs.Sales.Common;

namespace Blue.Cosacs.Sales.Api.Controllers
{
    public class InvoicesController : ApiController
    {
        private readonly IOrderRepository repository;

        public InvoicesController(IOrderRepository repo)
        {
            repository = repo;
        }

        //CR 2018-13
        public HttpResponseMessage Get(string invoiceNo = "", int copyCount = 1, string receiptType = null, bool isThermalPrint = false)
        {
            var user = this.GetUser();
            var orderExtendedDto = repository.GetOrderForRePrint(invoiceNo, user.FullName, receiptType);
            var models = new List<OrderExtendedDto>();
            var orderExtendedDtoCopy = orderExtendedDto.DeepClone();

            orderExtendedDtoCopy.PrintCopy = "COPY";
            
            for (var i = 0; i < copyCount; i++)
            {
                models.Add(orderExtendedDto);
            }

            models.Add(orderExtendedDtoCopy);

            var body = "";

            if (isThermalPrint)
            {
                var thPrinter = new ThermalPrinter();

                body = thPrinter.GetThermalReciept(models);
            }
            else
            {
                body = this.RenderViewToString("Invoice", models);
            }

            return Request.CreateResponse(HttpStatusCode.OK, body);
        }

        public HttpResponseMessage Get(int branchNo, System.DateTime dateFrom, System.DateTime dateTo, int invoiceNoMin, int invoiceNoMax, bool isThermalPrint = false)
        {
            var user = this.GetUser();
            var models = repository.GetOrdersForRePrintAll(branchNo, dateFrom, dateTo, invoiceNoMin, invoiceNoMax, user.FullName);

            var body = "";
            if (isThermalPrint)
            {
                var thPrinter = new ThermalPrinter();

                body = thPrinter.GetThermalReciept(models);
            }
            else
            {
                body = this.RenderViewToString("Invoice", models);
            }

            return Request.CreateResponse(HttpStatusCode.OK, body);
        }



    }
}
