using Blue.Cosacs.Merchandising;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Glaucous.Client.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Blue.Cosacs.Web.Areas.Merchandising.Controllers
{
    public class AshleySheduleJobsController : Controller
    {
        private readonly IPurchaseRepository purchaseRepository;
        private readonly IAshleyScheduleJobRepository scheduleJobRepository;
        public AshleySheduleJobsController(IPurchaseRepository purchaseRepository, IAshleyScheduleJobRepository scheduleJobRepository)
        {
            this.purchaseRepository = purchaseRepository;
            this.scheduleJobRepository = scheduleJobRepository;
        }
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 22/05/2019
        /// CR     : #Ashley
        /// Details: Auto Create PO JOB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult CreateAutoPO()
        {
            scheduleJobRepository.CreateAutoPO();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 22/05/2019
        /// CR     : #Ashley
        /// Details: Auto Create Cash Sale JOB
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult CreateCashSale()
        {
            
                scheduleJobRepository.ImportSaleDataFromFile(); // Imports File data into temp Tables using SSIS

                IRenissanceSalesRepository Rsales = new RenissanceSalesRepository();
                new RenissanceSalesController(Rsales).NotificationOfRenissanceSale();
                
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            
        }


        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult ExportStockCSV()
        {
            scheduleJobRepository.ExportStockCSV(); 
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }


        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult ExportPOXML()
        {
            scheduleJobRepository.ExportPOXML();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }


        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult ReadAsnXML()
        {
            scheduleJobRepository.ReadAsnXML();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }

        #region Test Sample
        /// <summary>
        /// Author : Rahul Dubey
        /// Date   : 22/05/2019
        /// CR     : #Ashley
        /// Details: Demo
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CronJob]
        [Permission(MerchandisingPermissionEnum.RunScheduledJobs)]
        [LongRunningQueries]
        public HttpStatusCodeResult RunnSSISPackage()
        {
            scheduleJobRepository.RunnSSISPackage();
            return new HttpStatusCodeResult((int)HttpStatusCode.OK);
        }
        #endregion
    }
}