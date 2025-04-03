using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Unicomer.Cosacs.Business.Interfaces;
using Unicomer.Cosacs.Model;
using Unicomer.Cosacs.Repository;
using Blue.Cosacs.Warehouse.Search;
using Blue.Events;
using Blue.Cosacs.Merchandising.Repositories;
using Blue.Cosacs.Merchandising;
using Blue.Config;
using Blue.Cosacs.Merchandising.Solr;

namespace Unicomer.Cosacs.Business
{
    public class InventoryBusiness : IInventory
    {
        public InventoryBusiness()
        {
            log4net.GlobalContext.Properties["LogFileName"] = ConfigurationManager.AppSettings["LogFolderPath"] != null ? ConfigurationManager.AppSettings["LogFolderPath"] : @"C://MVE_Unicomer//Log//VE_COSACS_";
            log4net.Config.XmlConfigurator.Configure();
        }

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public dynamic PriceUpdate(PriceUpdate objPriceUpdate)
        {
            _log.Info("Info : Price Update Json request :" + JsonConvert.SerializeObject(objPriceUpdate));
            string obj = XmlObjectSerializer.Serialize<PriceUpdate>(objPriceUpdate);
            JResponse objJResponse = new JResponse();
            InventoryRepository objInventory = new InventoryRepository();
            List<string> Result = objInventory.UpdateInventory(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.OK;
                    objJResponse.Message = "Parent SKU updated successfully";//Need to create resource file.
                    _log.Info("Info" + " - " + "Parent SKU Updated" + " - " + objJResponse.StatusCode + objJResponse.Message);
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                    _log.Info("Info" + " - " + "Update Parent SKU " + " - " + objJResponse.StatusCode + objJResponse.Message);
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
                _log.Info("Info" + " - " + "Update Parent SKU " + " - " + objJResponse.StatusCode + objJResponse.Message);
            }
            return objJResponse;
        }

        public dynamic CreateStockTransfer(StockTransferModel objStockTransfer)
        {
            _log.Info("Info : Create StockTransfer Json request :" + JsonConvert.SerializeObject(objStockTransfer));
            string obj = XmlObjectSerializer.Serialize<StockTransferModel>(objStockTransfer);
            _log.Info("Info : Create StockTransfer Xml request :" + PrintXmlsingleLine(obj));
            JResponse objJResponse = new JResponse();
            InventoryRepository objInventory = new InventoryRepository();
            List<string> Result = objInventory.CreateStockTransfer(obj);
            if (Result != null && Result.Count > 1)
            {
                if (Result[1].Equals("200"))
                {
                    objJResponse.Result = JsonConvert.SerializeObject(new { StockTransferId = Result[0] });
                    objJResponse.Status = true;
                    objJResponse.StatusCode = (int)HttpStatusCode.Created;
                    objJResponse.Message = "Stock Transferred successfully";//Need to create resource file.
                }
                else
                {
                    objJResponse.Result = string.Empty;
                    objJResponse.Status = false;
                    objJResponse.StatusCode = Convert.ToInt32(Result[1]);
                    objJResponse.Message = Result[0];//Need to create resource file.
                }
            }
            else
            {
                objJResponse.Result = string.Empty;
                objJResponse.Status = false;
                objJResponse.StatusCode = (int)HttpStatusCode.NotFound;
                objJResponse.Message = "No record found.";
            }
            return objJResponse;
        }
        static string PrintXmlsingleLine(string xml)
        {
            var stringBuilder = new StringBuilder();
            var element = XElement.Parse(xml);
            var settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = false;
            settings.Indent = false;
            settings.NewLineOnAttributes = false;
            using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                element.Save(xmlWriter);
            }
            return stringBuilder.ToString();
        }

        public dynamic StockTransfer(string StkTrfNo)
        {
            InventoryRepository objStock = new InventoryRepository();
            _log.Info("Info" + " - " + "get StockTransfer Details" + " - " + StkTrfNo);
            return objStock.StockTransfer(StkTrfNo);

        }

        public dynamic ReIndexing(string doc, int[] ids)
        {
            InventoryRepository objStock = new InventoryRepository();
            JResponse objJResponse = new JResponse();

            switch (doc)
            {
                case "CR":
                    var result = SolrIndex.Booking(ids);
                    _log.Info("Info" + " - " + "get CR ReIndexing Details");
                    break;

                case "GR":
                    //objJResponse = new JResponse();
                    //result = SolrIndex.Booking(ids);
                    //_log.Info("Info" + " - " + "get GR ReIndexing Details");
                    break;

                case "PO":
                    try
                    {
                        Blue.Cosacs.Merchandising.Settings mSettings = new Blue.Cosacs.Merchandising.Settings();
                        IEventStore eventStore = null;
                        Blue.Config.Settings config = new Blue.Config.Settings();
                        LocationRepository locationRepository = new LocationRepository(eventStore);
                        RepossessedConditionsRepository conditionsRepository = new RepossessedConditionsRepository(eventStore);
                        TagValuesRepository tagValuesRepository = new TagValuesRepository(eventStore, conditionsRepository);
                        ProductStatusProgresser productStatusProgresser = new ProductStatusProgresser(mSettings, tagValuesRepository);
                        RetailPriceRepository retailPriceRepository = new RetailPriceRepository(eventStore, productStatusProgresser);
                        ProductSalesRepository productSalesRepository = new ProductSalesRepository(locationRepository);
                        PromoPrice promoPrice = new PromoPrice(mSettings, config);
                        PromotionRepository promotionRepository = new PromotionRepository(eventStore, retailPriceRepository, locationRepository, null, promoPrice, mSettings, config);
                        CostRepository cost = new CostRepository(eventStore, retailPriceRepository, mSettings, config, productStatusProgresser);
                        StockSummarySolrIndexer summary = new StockSummarySolrIndexer(productSalesRepository, promotionRepository, mSettings, config, cost, locationRepository);
                        StockLevelsSolrIndexer levels = new StockLevelsSolrIndexer();
                        StockSolrIndexer stockSolrIndexer = new StockSolrIndexer(summary, levels);
                        Blue.Cosacs.Merchandising.Solr.PurchaseOrderSolrIndexer purchaseOrderSolrIndexer = new Blue.Cosacs.Merchandising.Solr.PurchaseOrderSolrIndexer(stockSolrIndexer, mSettings);

                        purchaseOrderSolrIndexer.Index(ids);
                        _log.Info("Info" + " - " + "get PO ReIndexing Details");
                    }
                    catch (Exception ex)
                    {
                        _log.Error("Error" + " - " + "PO ReIndexing Details" + ex.Message);
                    }
                    break;
            }


            return true;
        }
    }
}
