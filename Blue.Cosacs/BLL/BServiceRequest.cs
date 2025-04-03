 using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using STL.DAL;
using STL.Common;
using STL.Common.Static;
using STL.Common.ServiceRequest;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Delivery;
using System.Xml;
using STL.Common.Constants.Categories;
using System.Collections;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.CodeCategories;
using STL.Common.Constants.FTransaction;
using Blue.Cosacs.Shared.Extensions;

namespace STL.BLL
{
    public partial class BServiceRequest : CommonObject
    {
        private decimal totalTax = 0;

        private void SplitSRNo(
            long serviceRequestNo,
            out short serviceBranchNo,
            out int serviceUniqueId)
        {
            // Split the SR number into the branch number and the unique id
            serviceBranchNo = 0;
            serviceUniqueId = 0;

            string serviceRequestNoStr = serviceRequestNo.ToString();
            if (serviceRequestNoStr.Length > 3)
            {
                serviceBranchNo = Convert.ToInt16(serviceRequestNoStr.Substring(0,3));
                serviceUniqueId = Convert.ToInt32(serviceRequestNoStr.Substring(3));
            }
        }


        public DataSet CreateServiceRequest(SqlConnection conn, SqlTransaction trans, 
            short serviceBranchNo,
            string serviceType,
            string custId,
            string accountNo,
            int invoiceNo,
            string user,
            short stockLocn,
            string productCode,
            string description,
            DateTime purchaseDate,
            decimal unitPrice,
            string serialNo,
            int? printLocn,
            int itemId             // RI
            )
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            if (serviceType == ServiceType.Courts || serviceType == ServiceType.CourtsBySR)
            {
                // Mark any previous SR on the same item as a history record
                serviceRequest.SaveHistory(conn, trans, accountNo, invoiceNo, stockLocn, itemId, serialNo);         // RI
            }

            // Create a new SR
            DataTable newServiceRequest = serviceRequest.CreateServiceRequest(conn, trans,
                serviceBranchNo,
                serviceType,
                custId,
                accountNo,
                invoiceNo,
                user,
                stockLocn,
                productCode,
                description,
                purchaseDate,
                unitPrice,
                serialNo,
                printLocn,
                itemId         // RI
                );

            this.CheckForWarranty(newServiceRequest,true);

            //UAT 40 A charge-to acount is to be created as soon as the SR is created if the item is not under warranty
           int serviceRequestNo = (int)newServiceRequest.Rows[0][CN.ServiceRequestNo];
           string fyw = newServiceRequest.Rows[0][CN.FYWarranty].ToString();
           string exw = newServiceRequest.Rows[0][CN.ExtWarranty].ToString();
           string custID = newServiceRequest.Rows[0][CN.CustID].ToString();


           //IP - 07/08/08 - UAT5.1 - UAT(503) - A 'Cash' account was incorrectly being created for an
           //'Internal Stock Repair'. Therefore two entries appeared in the 'SR_Chargeacct' table
           //one to the special internal account and one to the cash account.
           //if(fyw == "N" && exw == "N")
           if (fyw == "N" && exw == "N" && serviceType != "S")
           {
              CreateChargeToAccount(conn, trans, serviceRequestNo, serviceBranchNo, custID, String.Empty);
           }

            newServiceRequest.TableName = TN.ServiceRequest;
            DataSet serviceRequestSet = new DataSet();
            serviceRequestSet.Tables.Add(newServiceRequest);
            return serviceRequestSet;
        }


        public void SaveServiceRequest(SqlConnection conn, SqlTransaction trans, DataSet serviceRequestSet)
        {
            // Update the SR (or list of SRs)
            DServiceRequest serviceRequest = new DServiceRequest();
            foreach (DataRow row in serviceRequestSet.Tables[TN.ServiceRequest].Rows)
            {
                int curServiceRequestNo = (int)row[CN.ServiceRequestNo];
                short curServiceBranchNo = (short)row[CN.ServiceBranchNo];

                // Only update open SR's (or those just closed) - Some SR's may have  become closed at a later date as a result of payment having been made
               //UAT 385 Also update BER SR's
                if (curServiceRequestNo > 0 && 
                    ((DateTime)row[CN.DateClosedOrig] == Date.blankDate || 
                    AbreviateServiceStatus((string)row[CN.HiddenStatus]) == ServiceStatus.Resolution || // UAT 576 - originally it was ServiceStatus.Closed
                    AbreviateServiceStatus((string)row[CN.HiddenStatus]) == ServiceStatus.BERReplacement ||
                    AbreviateServiceStatus((string)row[CN.HiddenStatus]) == ServiceStatus.Estimate ||       //CR1030 - #2621 jec 06/12/10 save if Estimate required
                    AbreviateServiceStatus((string)row[CN.HiddenStatus]) == ServiceStatus.Closed )) //UAT 691
                {
                    // Check whether the deposit has been paid on a Charge To account
                    string depositAcctNo = "";
                    decimal depositAmount = 0;
                    decimal depositPaid = 0;

                    // Only required if 'Charge To' is a Customer
                    if ((string)row[CN.ChargeTo] == ServiceChargeTo.Customer)
                    {
                        this.GetChargeToDeposit(conn, trans,
                            curServiceRequestNo, curServiceBranchNo,
                            out depositAcctNo, out depositAmount, out depositPaid);
                    }

                    if (depositPaid >= depositAmount)
                        serviceRequest.depositPaid = "Y";
                    else
                        serviceRequest.depositPaid = "N";

                    // Check whether the SR status should be closed
                    string srStatus = AbreviateServiceStatus((string)row[CN.HiddenStatus]);
                    //If Charge to is Internal or AIG then close it irrespective of Balance
                    if ((DateTime)row[CN.DateClosed] > Date.blankDate.AddDays(1.0)
                        && ((string)row[CN.ChargeTo] == ServiceChargeTo.Internal
                        || (string)row[CN.ChargeTo] == ServiceChargeTo.EW))
                    {
                        if (srStatus == ServiceStatus.Resolution &&
                            (bool)Country[CountryParameterNames.SRClsdTabBeforeResTab] == false
                            && (DateTime)row[CN.ReturnDate] < Date.blankDate.AddDays(1) )
                        {
                            srStatus = ServiceStatus.Resolution;
                        }
                        else //CR 1024 (NM 30/04/2009) - Status will not change to closed until the returnDate is entered
                        {
                            srStatus = ServiceStatus.Closed;
                        }
                    }
                    

                   // UAT 247 Status will be closed if the outstanding balance is less than or equal to zero if charge to is customer or deliverer OR supplier and customer charge on there.
                    if ((DateTime)row[CN.DateClosed] > Date.blankDate.AddDays(1.0)
                        && ((string)row[CN.ChargeTo] == ServiceChargeTo.Customer || (string)row[CN.ChargeTo] == ServiceChargeTo.Deliverer
                        || (string)row[CN.ChargeTo] == ServiceChargeTo.Supplier )                      
                        && (decimal)row[CN.OutstBal] <= 0)
                    {
                        if (srStatus == ServiceStatus.Resolution &&
                            (bool)Country[CountryParameterNames.SRClsdTabBeforeResTab] == false
                            && (DateTime)row[CN.ReturnDate] < Date.blankDate.AddDays(1.0))
                        {
                            srStatus = ServiceStatus.Resolution;
                        }
                        else //CR 1024 (NM 30/04/2009) - Status will not change to closed until the returnDate is entered
                        {
                            srStatus = ServiceStatus.Closed;
                        }
                    }

                    //UAT 692 - Status will be closed if the charge to is internal and resolution details entered
                    if ((DateTime)row[CN.DateClosed] > Date.blankDate.AddDays(1.0) && (string)row[CN.ChargeTo] == ServiceChargeTo.Internal)
                    {
                        srStatus = ServiceStatus.Closed;
                    }

                    // Set Properties
                    // SR_ServiceRequest
                    serviceRequest.dateLogged = (DateTime)row[CN.DateLogged];
                    serviceRequest.dateReopened = (DateTime)row[CN.DateReOpened];
                    serviceRequest.purchaseDate = (DateTime)row[CN.PurchaseDate];
                    serviceRequest.receivedDate = (DateTime)row[CN.ReceivedDate];
                    serviceRequest.depositAmount = (decimal)row[CN.DepositAmount];
                    serviceRequest.repairEstimate = (decimal)row[CN.RepairEstimate];
                    serviceRequest.stockLocn = (short)row[CN.StockLocn];
                    serviceRequest.deliveryDamage = (string)row[CN.DeliveryDamage];
                    serviceRequest.description = (string)row[CN.Description];
                    serviceRequest.extWarranty = (string)row[CN.ExtWarranty];
                    serviceRequest.goodsOnLoan = (string)row[CN.GoodsOnLoan];
                    serviceRequest.modelNo = (string)row[CN.ModelNo];
                    serviceRequest.productCode = (string)row[CN.ProductCode];
                    serviceRequest.retailer = (string)row[CN.Retailer];      //CR1030 jec
                    serviceRequest.custID = (string)row[CN.CustID];      //CR1030 jec
                    serviceRequest.unitPrice = (decimal)row[CN.UnitPrice];
                    serviceRequest.serialNo = (string)row[CN.SerialNo];
                    serviceRequest.serviceEvaln = (string)row[CN.ServiceEvaluation];
                    serviceRequest.serviceLocn = (string)row[CN.ServiceLocn];
                    serviceRequest.status = srStatus;
                    serviceRequest.transitNotes = (string)row[CN.TransitNotes];
                    serviceRequest.comments = (string)row[CN.Comments];
                    //CR1030 - needs to be included with Reports Release
                    //serviceRequest.softscriptdate = (DateTime)row[CN.SoftScriptDate];
                    serviceRequest.dateCollected = (DateTime)row[CN.DateCollected];
                    // SR_Allocation
                    serviceRequest.zone = (string)row[CN.Zone];
                    serviceRequest.technicianId = (int)row[CN.TechnicianId];
                    serviceRequest.partsDate = (DateTime)row[CN.PartsDate];
                    serviceRequest.repairDate = (DateTime)row[CN.RepairDate];
                    serviceRequest.isAM = (string)row[CN.IsAM];
                    serviceRequest.instructions = (string)row[CN.Instructions];
                    serviceRequest.reAssignCode = (string)row[CN.ReAssignCode];      //CR1030 jec
                    serviceRequest.reAssignedBy = Convert.ToString(row[CN.ReAssignedBy]);      //CR1030 jec
                    // SR_Resolution
                    serviceRequest.dateClosed = (DateTime)row[CN.DateClosed];
                    serviceRequest.resolution = (string)row[CN.Resolution];
                    serviceRequest.resolutionChangedBy = (int)row[CN.ResolutionChangedBy];
                    serviceRequest.chargeTo = (string)row[CN.ChargeTo];
                    serviceRequest.chargeToChangedBy = (int)row[CN.ChargeToChangedBy];
                    serviceRequest.chargeToMake = (string)row[CN.ChargeToMake];
                    serviceRequest.chargeToModel = (string)row[CN.ChargeToModel];
                    serviceRequest.hourlyRate = (decimal)row[CN.HourlyRate];
                    serviceRequest.hours = (decimal)row[CN.Hours];
                    serviceRequest.labourCost = (decimal)row[CN.LabourCost];
                    serviceRequest.additionalCost = (decimal)row[CN.AdditionalCost];
                    serviceRequest.TransportCost = (decimal)row[CN.TransportCost];// CR 1024 (NM 29/04/2009)
                    serviceRequest.totalCost = (decimal)row[CN.TotalCost];
                    serviceRequest.goodsOnLoanCollected = (string)row[CN.GoodsOnLoanCollected];
                    serviceRequest.replacement = (string)row[CN.Replacement];
                    serviceRequest.foodLoss = (string)row[CN.FoodLoss];
                    serviceRequest.softScript = (string)row[CN.SoftScript];
                    serviceRequest.deliverer = (string)row[CN.Deliverer];
                    serviceRequest.fault = row[CN.Fault].ToString();
                   //CR 949/958
                    serviceRequest.actionRequired = row[CN.ActionRequired].ToString();
                    serviceRequest.printLocn = (row[CN.PrintLocn]) == DBNull.Value ? Convert.ToInt32(Config.BranchCode) : Convert.ToInt32(row[CN.PrintLocn]);
                    serviceRequest.returnDate = (row[CN.ReturnDate]) == DBNull.Value ? Convert.ToDateTime(Date.blankDate) : Convert.ToDateTime(row[CN.ReturnDate]);
                    serviceRequest.failureReason = (row[CN.FailureReason].ToString());
                    serviceRequest.delivered = (row[CN.Delivered].ToString());
                   serviceRequest.collected = (row[CN.CustomerCollected].ToString());
                   serviceRequest.repaired = (row[CN.RepairedHome].ToString());

                   serviceRequest.LbrCostEstimate = (decimal)row[CN.LbrCostEstimate]; // CR 1024 (NM 29/04/2009)
                   serviceRequest.AdtnlLbrCostEstimate = (decimal)row[CN.AdtnlLbrCostEstimate]; // CR 1024 (NM 29/04/2009)
                   serviceRequest.TransportCostEstimate = (decimal)row[CN.TransportCostEstimate]; // CR 1024 (NM 29/04/2009)
                   serviceRequest.TechnicianReport = row[CN.TechnicianReport].ToString(); // CR 1024 (NM 29/04/2009)
                   //prevent error saving service request in Mauritius due to zone difference on client. 
                   #region correctdates
                   if (serviceRequest.dateAllocated < Date.blankDate.AddDays(1.0))
                       serviceRequest.dateAllocated = Date.blankDate;

                   if (serviceRequest.dateClosed < Date.blankDate.AddDays(1.0))
                       serviceRequest.dateClosed = Date.blankDate;

                   if (serviceRequest.dateCollected < Date.blankDate.AddDays(1.0))
                       serviceRequest.dateCollected = Date.blankDate;

                   if (serviceRequest.dateReopened < Date.blankDate.AddDays(1.0))
                       serviceRequest.dateReopened = Date.blankDate;

                   if (serviceRequest.dateLogged < Date.blankDate.AddDays(1.0))
                       serviceRequest.dateLogged = Date.blankDate;

                   if (serviceRequest.purchaseDate < Date.blankDate.AddDays(1.0))
                       serviceRequest.purchaseDate = Date.blankDate;

                   if (serviceRequest.partsDate < Date.blankDate.AddDays(1.0))
                       serviceRequest.partsDate = Date.blankDate;

                   if (serviceRequest.receivedDate < Date.blankDate.AddDays(1.0))
                       serviceRequest.receivedDate = Date.blankDate;

                   if (serviceRequest.repairDate < Date.blankDate.AddDays(1.0))
                       serviceRequest.repairDate = Date.blankDate;

                   if (serviceRequest.returnDate < Date.blankDate.AddDays(1.0))
                       serviceRequest.returnDate = Date.blankDate;

                   #endregion correctdates
                       
                   //UAT 453
                   if (AbreviateServiceStatus((string)row[CN.HiddenStatus]) == ServiceStatus.BERReplacement)
                   {
                        var costPricePCent = Convert.ToDecimal(Country[CountryParameterNames.ServiceBERCostPricePCent]);
                        serviceRequest.totalCost += Convert.ToDecimal(row[CN.CostPrice]) * (100 + costPricePCent) / 100;
                        
                       if (row[CN.ChargeTo].ToString() == ServiceChargeTo.EW) //Bug #3448
                            serviceRequest.totalCost = serviceRequest.totalCost - Convert.ToString(row[CN.PreviousCosts]).TryParseDecimal(0M).Value;
                   }

                    // Save this SR
                    serviceRequest.SaveServiceRequest(conn, trans,
                        curServiceBranchNo,
                        curServiceRequestNo);

                    // Delete the old part list for this SR
                    serviceRequest.DeletePartList(conn, trans, curServiceRequestNo);

                    // Add the new part list for this SR
                    foreach (DataRow partRow in serviceRequestSet.Tables[TN.PartList].Rows)
                    {
                        if (partRow.RowState != DataRowState.Deleted)
                        {
                            if ((int)partRow[CN.ServiceRequestNo] == curServiceRequestNo)
                            {
                                serviceRequest.SavePartResolved(conn, trans,
                                    curServiceRequestNo,
                                    partRow[CN.PartNo].ToString(),
                                    Convert.ToInt32(partRow[CN.PartID]),                                        //IP - 04/07/11 - CR1254 - RI - #3994
                                    Convert.ToDecimal(partRow[CN.Quantity].ToString()),
                                    Convert.ToDecimal(StripCurrency(partRow[CN.UnitPrice].ToString())),
                                    partRow[CN.Description].ToString(),
                                    partRow[CN.PartType].ToString(),
                                    Convert.ToInt32(partRow[CN.StockLocn]));                                    //IP - 18/06/09 - UAT(687) - Added StockLocn
                            }
                        }
                    }

                    // UAT 40 If 'Charge To' is Deliverer then need to remove charge-to customer account 
                    if ((string)row[CN.ChargeTo] == ServiceChargeTo.Deliverer)
                    {
                        this.GetChargeToDeposit(conn, trans,
                            curServiceRequestNo, curServiceBranchNo,
                            out depositAcctNo, out depositAmount, out depositPaid);
                    }

                    // The Charge To Analysis is only saved when the SR has just been closed
                    if ((DateTime)row[CN.DateClosed] > Date.blankDate.AddDays(1.0) && (DateTime)row[CN.DateClosedOrig] <= Date.blankDate.AddDays(1.0))
                    {
                        // Post the Charge To to the appropriate Charge To accounts
                        this.SaveChargeTo(conn, trans,
                            serviceRequestSet.Tables[TN.ChargeToAnalysis],
                            curServiceRequestNo,
                            (string)row[CN.ServiceRequestNoStr],
                            curServiceBranchNo,
                            (string)row[CN.ChargeToMake],
                            (string)row[CN.Deliverer],
                            (string)row[CN.CustID]);

                        // Post the Courts parts to the Stock Inventory account
                        if (serviceRequestSet.Tables[TN.PartList].Rows.Count > 0)
                       {
                           //this.UpdateStock(conn, trans,
                           //    curServiceRequestNo, (string)row[CN.ServiceRequestNoStr],
                           //    curServiceBranchNo, serviceRequestSet.Tables[TN.PartList]);

                           //IP - 18/06/09 - UAT(687) - Removed curServiceBranchNo
                           this.UpdateStock(conn, trans,
                               curServiceRequestNo, (string)row[CN.ServiceRequestNoStr],
                               serviceRequestSet.Tables[TN.PartList], curServiceBranchNo);
                       }

                        // Save a snapshot of the Customer Details as at this SR being closed
                        this.SaveSRCustomer(conn, trans,
                            curServiceRequestNo);
                    }
                }
                else if (curServiceRequestNo > 0 && ((DateTime)row[CN.DateClosedOrig]).ToShortDateString() != Date.blankDate.ToShortDateString())
                {
                    // The SR was previously closed, so the user can only update the comments
                    //Changed this to update the date reopened for comments
                    serviceRequest.SaveServiceRequestComments(conn, trans,
                        curServiceBranchNo,
                        curServiceRequestNo,
                        (string)row[CN.Comments],
                        (DateTime)row[CN.DateReOpened]);
                }
            }
        }

       private string AbreviateServiceStatus(string status)
       {
          string abbreviatedStatus;
          switch (status)
          {
             case ServiceStatusText.Allocation:
                abbreviatedStatus = ServiceStatus.Allocation;
                break;
             case ServiceStatusText.BERReplacement:
                abbreviatedStatus = ServiceStatus.BERReplacement;
                break;
             case ServiceStatusText.Closed:
                abbreviatedStatus = ServiceStatus.Closed;
                break;
             case ServiceStatusText.CommentUpdate:
                abbreviatedStatus = ServiceStatus.CommentUpdate;
                break;
             case ServiceStatusText.Deposit:
                abbreviatedStatus = ServiceStatus.Deposit;
                break;
             case ServiceStatusText.Estimate:
                abbreviatedStatus = ServiceStatus.Estimate;
                break;
             case ServiceStatusText.New:
                abbreviatedStatus = ServiceStatus.New;
                break;
             case ServiceStatusText.Resolution:
                abbreviatedStatus = ServiceStatus.Resolution;
                break;
                //CR 949/958 New status 'To Be Allocated'
             case ServiceStatusText.ToBeAllocated:
                abbreviatedStatus = ServiceStatus.ToBeAllocated;
                break;
             case ServiceStatusText.TechnicianAllocated: //CR 1024 (NM 23/04/2009)
                abbreviatedStatus = ServiceStatus.TechnicianAllocated;
                break;
             case ServiceStatusText.AllocatedToSupplier: //CR 1024 (NM 23/04/2009)
                abbreviatedStatus = ServiceStatus.AllocatedToSupplier;
                break;
             default:
                abbreviatedStatus = String.Empty;
                break;
          }
          return abbreviatedStatus;
       }


        private void SaveChargeTo(SqlConnection conn, SqlTransaction trans,
            DataTable chargeToTable, int curServiceRequestNo, string curServiceRequestNoStr,
            short curServiceBranchNo, string supplier, string deliverer, string custId)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            string internalAcctNo = "";
            string warrantyAcctNo = "";
            string supplierAcctNo = "";
            string delivererAcctNo = "";
            string customerAcctNo = "";
            bool addSTAX = false;

            // Save the Charge To and post all the charges
            foreach (DataRow chargeToRow in chargeToTable.Rows)
            {
                if ((int)chargeToRow[CN.ServiceRequestNo] == curServiceRequestNo)
                {
                    // Save the Charge To Analysis row
                    decimal internalCharge = Convert.ToDecimal(chargeToRow[CN.Internal]);
                    decimal warrantyCharge = Convert.ToDecimal(chargeToRow[CN.ExtWarranty]);
                    decimal supplierCharge = Convert.ToDecimal(chargeToRow[CN.Supplier]);
                    decimal delivererCharge = Convert.ToDecimal(chargeToRow[CN.Deliverer]);
                    decimal customerCharge = Convert.ToDecimal(chargeToRow[CN.Customer]);

                    serviceRequest.SaveChargeTo(conn, trans,
                        curServiceRequestNo,
                        (short)chargeToRow[CN.SortOrder],
                        Convert.ToDecimal(chargeToRow[CN.ActualCost]),
                        internalCharge,
                        warrantyCharge,
                        supplierCharge,
                        delivererCharge,
                        customerCharge);

                    string serviceItemNumber = String.Empty;
                    var itemid = 0;

                    bool itemEntry = false;
                    switch (Convert.ToInt16(chargeToRow[CN.SortOrder]))
                    {
                        case ServiceAnalysis.PartsCourts:
                            serviceItemNumber = (string)Country[CountryParameterNames.ServiceItemPartsCourts];
                            itemid = StockItemCache.Get(StockItemKeys.ServiceItemPartsCourts);
                            itemEntry = true;
                            break;
                        case ServiceAnalysis.PartsOther:
                            serviceItemNumber = (string)Country[CountryParameterNames.ServiceItemPartsOther];
                            itemid = StockItemCache.Get(StockItemKeys.ServiceItemPartsOther);
                            itemEntry = true;
                            break;
                        case ServiceAnalysis.LabourTotal:
                            serviceItemNumber = (string)Country[CountryParameterNames.ServiceItemLabour];
                            itemid = StockItemCache.Get(StockItemKeys.ServiceItemLabour);
                            itemEntry = true;
                            break;
                        default:
                            break;
                    }

                    if (serviceItemNumber != "")
                    {
                       //
                       // Post the transactions for the charges on this row
                       //

                       if (internalCharge >= 0.01M)
                       {
                          if (internalAcctNo == "")
                          {
                             // Get the Internal special account
                             internalAcctNo = this.GetChargeToAcct(conn, trans,
                                 curServiceRequestNo, curServiceBranchNo, ServiceAcct.Internal);
                          }

                          // Check the Internal special account exists
                          if (internalAcctNo.Length != 12)
                             throw new STLException(GetResource("M_NOCHARGETOINTERNAL", new object[] { curServiceRequestNoStr }));
                          else
                             // Post the transaction to the Customer
                             this.SaveServiceLineItem(conn, trans, AT.Special,
                                 internalAcctNo, Convert.ToInt32(curServiceRequestNoStr), curServiceBranchNo, serviceItemNumber, 1, internalCharge, itemid);    // RI
                       }

                       if (warrantyCharge >= 0.01M)
                       {
                          if (warrantyAcctNo == "")
                          {
                             // Get the Warranty special account
                             warrantyAcctNo = this.GetChargeToAcct(conn, trans,
                                 curServiceRequestNo, curServiceBranchNo, ServiceAcct.Warranty);
                          }

                          // Check the Warranty (AIG) special account exists
                          if (warrantyAcctNo.Length != 12)
                             throw new STLException(GetResource("M_NOCHARGETOWARRANTY", new object[] { curServiceRequestNoStr }));
                          else
                             // Post the transaction to the Customer
                             this.SaveServiceLineItem(conn, trans, AT.Special,
                                 warrantyAcctNo, Convert.ToInt32(curServiceRequestNoStr), curServiceBranchNo, serviceItemNumber, 1, warrantyCharge, itemid);    // RI
                       }

                       if (supplierCharge >= 0.01M)
                       {
                          if (supplierAcctNo == "")
                          {
                             // Get the Supplier special account
                             supplierAcctNo = this.GetChargeToAcct(conn, trans,
                                 curServiceRequestNo, curServiceBranchNo, ServiceAcct.Supplier);
                          }

                          if (supplierAcctNo.Length != 12)
                             throw new STLException(GetResource("M_NOCHARGETOSUPPLIER", new object[] { curServiceRequestNoStr, supplier }));
                          else
                             // Post the transaction to the Customer
                             this.SaveServiceLineItem(conn, trans, AT.Special,
                                 supplierAcctNo, Convert.ToInt32(curServiceRequestNoStr), curServiceBranchNo, serviceItemNumber, 1, supplierCharge, itemid);    // RI
                       }

                       if (delivererCharge >= 0.01M)
                       {
                          if (delivererAcctNo == "")
                          {
                             // Get the Deliverer special account
                             delivererAcctNo = this.GetChargeToAcct(conn, trans,
                                 curServiceRequestNo, curServiceBranchNo, ServiceAcct.Deliverer);
                          }

                          // Check the Warranty (AIG) special account exists
                          if (delivererAcctNo.Length != 12)
                             throw new STLException(GetResource("M_NOCHARGETODELIVERER", new object[] { curServiceRequestNoStr, deliverer }));
                          else
                             // Post the transaction to the Customer
                             this.SaveServiceLineItem(conn, trans, AT.Cash,
                                 delivererAcctNo, 1, curServiceBranchNo, serviceItemNumber, 1, delivererCharge, itemid);    // RI
                       }

                       if (customerCharge >= 0.01M)
                       {
                          if (customerAcctNo == "")
                          {
                             // Get the Customer special account
                             customerAcctNo = this.GetChargeToAcct(conn, trans,
                                 curServiceRequestNo, curServiceBranchNo, ServiceAcct.Customer);
                          }

                          // Check the Warranty (AIG) special account exists
                          if (customerAcctNo.Length != 12)
                             throw new STLException(GetResource("M_NOCUSTOMERACCOUNT", new object[] { curServiceRequestNoStr, custId }));
                          else
                          {
                             // Post the transaction to the Customer
                             this.SaveServiceLineItem(conn, trans, AT.Cash,
                                 customerAcctNo, 1, curServiceBranchNo, serviceItemNumber, 1, customerCharge, itemid);      // RI

                             addSTAX = true;
                          }
                       }
                    }
                    else
                    {
                       //UAT 256 Warn the user that no generic item numbers have been set up and thus no charge-to accounts will be created
                       if (itemEntry == true)
                       {
                          throw new STLException("No generic item numbers have been set up for labour and parts in Country Maintenance");
                       }
                    }
                }
            }

            if (addSTAX && (string)Country[CountryParameterNames.AgreementTaxType] == "E" && 
                (decimal)Country[CountryParameterNames.TaxRate] > 0)
            {
                AddSalesTaxToCustomerAccount(conn, trans, customerAcctNo, curServiceBranchNo);
            }
        }


        public void SaveServiceLineItem(SqlConnection conn, SqlTransaction trans, string acctType,
            string serviceAcctNo, int serviceAgreementNo, short serviceBranchNo,
            string serviceItemNumber, decimal quantity, decimal chargeToValue, int itemid,              // RI
            string installationTransType = null)
        {
            DBranch branch = new DBranch();
            BItem lineItem = new BItem();
            DDelivery delivery = new DDelivery();
            DateTime deliveryDate = DateTime.Now;

            DStockItem stock = new DStockItem();
            //stock.GetItemDetailsByItemNo(conn, trans, serviceItemNumber, serviceBranchNo, acctType, (string)Country[CountryParameterNames.CountryCode], false, false);    // CR1212 jec need to supply itemID
            stock.GetItemDetails(conn, trans, itemid, serviceBranchNo, acctType, (string)Country[CountryParameterNames.CountryCode], false, false);    // CR1212 jec 
         
            if (acctType == AT.Cash)
            {
                // Sales tax is applied to the Cash accounts for Deliverer and Customer
                //lineItem.CalculateTaxAmount(chargeToValue, false, Convert.ToDecimal(stock.TaxRate), 1.0M, 0 /*serviceItemNumber*/, false, 0); //TODO; Need to pass in service itemId
                lineItem.CalculateTaxAmount(chargeToValue, false, Convert.ToDecimal(stock.TaxRate), 1.0M, itemid, false, 0);   // RI
            }
            
            lineItem.OrigBr = 0;
            lineItem.AccountNumber = serviceAcctNo;
            lineItem.AgreementNumber = serviceAgreementNo;
            lineItem.StockLocation = serviceBranchNo;
            lineItem.ItemNumber = serviceItemNumber;
            lineItem.ItemId = itemid;               // RI
            lineItem.Price = chargeToValue;
            lineItem.Quantity = Convert.ToDouble(quantity);
            lineItem.OrderValue = chargeToValue * quantity;         // #13736   
            lineItem.ItemType = "N";
            lineItem.DeliveryNoteBranch = serviceBranchNo;
            lineItem.DeliveryProcess = "I";
            lineItem.QuantityDiff = "N";

            lineItem.DeliveredQuantity = lineItem.Quantity;
            lineItem.ScheduledQuantity = lineItem.Quantity;
            lineItem.ItemSuppText = "";
            lineItem.DateRequiredDelivery = Date.blankDate;
            lineItem.TimeRequiredDelivery = "";
            lineItem.ContractNo = "";
            lineItem.IsKit = 0;
            lineItem.HasString = 0;
            lineItem.Notes = "";
            lineItem.DeliveryAddress = "";
            lineItem.ExpectedReturnDate = Date.blankDate;
            lineItem.DeliveryArea = "";
            lineItem.Damaged = "";
            lineItem.Assembly = "";
            lineItem.ParentItemNumber = "";
            lineItem.Taxrate = stock.TaxRate;
            lineItem.Save(conn, trans);

            delivery.OrigBr = 0;
            delivery.AccountNumber = serviceAcctNo;
            delivery.AgreementNumber = serviceAgreementNo;
            delivery.StockLocation = serviceBranchNo;
            delivery.BranchNumber = serviceBranchNo;
            delivery.BuffBranchNumber = serviceBranchNo;
            delivery.BuffNo = branch.GetBuffNo(conn, trans, serviceBranchNo);       //#16451 
            delivery.ItemNumber = serviceItemNumber;
            delivery.ItemId = itemid;           // RI 
            delivery.Quantity = lineItem.Quantity;
            delivery.TransValue = lineItem.OrderValue;          // #13736   
            delivery.DateTrans = deliveryDate;
            delivery.DateDelivered = deliveryDate;
            delivery.TransRefNo = branch.GetTransRefNo(conn, trans, serviceBranchNo);       //#16451 
            delivery.RunNumber = 0;
            delivery.DeliveryOrCollection = DelType.Normal;
            delivery.ReturnItemNumber = "";
            delivery.ReturnValue = 0;
            delivery.ReturnStockLocation = 0;
            delivery.NotifiedBy = this.User;
            delivery.ftNotes = installationTransType ?? "DNSR";
            delivery.ParentItemNo = "";
            delivery.Write(conn, trans);

            DAgreement agree = new DAgreement(conn, trans, serviceAcctNo, serviceAgreementNo);      // #13736 
            agree.AccountNumber = serviceAcctNo;     // #13736 
            agree.AgreementNumber = serviceAgreementNo;     // #13736 
            agree.DeliveryFlag = "Y";
            agree.AgreementTotal += chargeToValue * quantity;           // #13736
            agree.CashPrice += chargeToValue * quantity;           // #13736;
            //UAT 457 Set HoldProp field to 'N' so that charge-to account is seen as fully delivered upon resolution
            agree.HoldProp = "N";
            agree.DateChange = DateTime.Now; //#14481
            agree.Save(conn, trans);

            DAccount account = new DAccount();
            account.Populate(conn, trans, serviceAcctNo);
            account.AgreementTotal = agree.AgreementTotal;
            account.OutstandingBalance += chargeToValue * quantity;           // #13736;
            account.Save(conn, trans);
        }

        //IP - 18/06/09 - UAT(687) - Removed curServiceBranchNo

        //private void UpdateStock(SqlConnection conn, SqlTransaction trans,
        //    int curServiceRequestNo, string curServiceRequestNoStr,
        //    short curServiceBranchNo, DataTable partList)
        private void UpdateStock(SqlConnection conn, SqlTransaction trans,
           int curServiceRequestNo, string curServiceRequestNoStr,
           DataTable partList, short curServiceBranchNo) //LW 71844 (29/10/2009)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            // Get the Stock Item Inventory account number
            string stockAcctNo = ((string)Country[CountryParameterNames.ServiceStockAccount]).Replace("-","");

            short stockLocn; //LW 71844 (29/10/2009)

            // Check the Service Stock account number is set up
            if (stockAcctNo.Length != 12)
                throw new STLException(GetResource("M_NOSTOCKACCOUNT", new object[] { curServiceRequestNoStr }));
            else
            {
                // Save the Parts List (for Courts Items only) to the Stock Item Inventory account
                foreach (DataRow partRow in partList.Rows)
                {
                    if (partRow.RowState != DataRowState.Deleted)
                    {
                        if ((int)partRow[CN.ServiceRequestNo] == curServiceRequestNo &&
                            partRow[CN.NonCourts].ToString() == "N")
                        {
                            //-- LW 71844 (29/10/2009)
                            stockLocn = Convert.ToInt16((partRow[CN.StockLocn]));
                            if (stockLocn == 0)
                                stockLocn = curServiceBranchNo;

                            // Save the Part row with a quantity but zero value
                            this.SaveServiceLineItem(conn, trans,
                                AT.Special,
                                stockAcctNo,
                                Convert.ToInt32(curServiceRequestNoStr),
                                //curServiceBranchNo,
                                stockLocn, //IP - 18/06/09 - UAT(687) //LW 71844 (29/10/2009)
                                partRow[CN.PartNo].ToString(), 
                                Convert.ToDecimal(partRow[CN.Quantity].ToString()),
                                0, Convert.ToInt32(partRow[CN.PartID]));                // RI
                        }
                    }
                }
            }
        }

        //#12116
        private void UpdateInstallationStock(SqlConnection conn, SqlTransaction trans,
        int installationNo,  short branchNo, DataTable partList)
        {
            var instNoString = branchNo.ToString() + installationNo.ToString();

            // Get the Installation Stock Inventory Account
            var stockAcctNo = ((string)Country[CountryParameterNames.InstallationStockAccount]).Replace("-", "");

            var stockLocn = 0;

            // Check the Installation Stock account number is set up
            if (stockAcctNo.Length != 12)
                throw new STLException(GetResource("M_NOINSTSTOCKACCOUNT", new object[] { installationNo.ToString() }));
            else
            {
                // Save the Parts List (for Courts Items only) to the Stock Item Inventory account
                foreach (DataRow partRow in partList.Rows)
                {

                    if ((int)partRow[CN.InstallationNo] == installationNo && Convert.ToBoolean(partRow[CN.IsNonCourts]) == false)
                        {
                            stockLocn = Convert.ToInt16((partRow["StockLocation"]));
                            if (stockLocn == 0)
                                stockLocn = branchNo;

                            // Save the Part row with a quantity but zero value
                            this.SaveServiceLineItem(conn, trans,
                                AT.Special,
                                stockAcctNo,
                                Convert.ToInt32(instNoString),
                                Convert.ToInt16(stockLocn), 
                                partRow[CN.PartNo].ToString(),
                                Convert.ToDecimal(partRow[CN.Quantity].ToString()),
                                0, Convert.ToInt32(partRow[CN.PartID]));             
                        }
                }
            }
        }

        private void SaveSRCustomer(SqlConnection conn, SqlTransaction trans,
            int curServiceRequestNo)
        {
            // Save the SR customer details
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.SaveSRCustomer(conn, trans, curServiceRequestNo);
        }

        public DataSet GetServiceRequest(
            string serviceTypeSearch,
            long serviceRequestNo,
            string custId,
            string acctNo,
            int invoiceNo,
            short branchNo,
            int user,
            out string serviceType,
            out bool isPaidAndTakenAcct)  //IP - 31/07/09 - UAT(741) - added bool to check if the account being searched on is a Paid and Taken accountmo.
        {
            DataTable serviceRequestCustomerSR = new DataTable();
            DataTable serviceRequestCustomerCurrent = new DataTable();
            DataSet serviceRequestSet = new DataSet();
            DataTable serviceRequestList = new DataTable();
            DataTable dtChargeToAuthorisation = new DataTable();
            serviceType = "";
            isPaidAndTakenAcct = false;
            DServiceRequest serviceRequest = new DServiceRequest();

            // Split the SR number into the branch number and the unique id
            short serviceBranchNo = 0;
            int serviceUniqueId = 0;
            this.SplitSRNo(serviceRequestNo, out serviceBranchNo, out serviceUniqueId);

            // Load the Customer and Service Request details
            switch (serviceTypeSearch)
            {
                case ServiceType.Courts:
                    // Ignore the SR number to search by account / invoice number
                    serviceType = ServiceType.Courts;
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer("", 0, 0, acctNo, invoiceNo, branchNo).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer("", 0, 0, acctNo, invoiceNo, branchNo).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    }
                    serviceRequestSet = serviceRequest.GetCourtsAccount(0, 0, acctNo, invoiceNo, branchNo,user, out isPaidAndTakenAcct); 
                    break;
                case ServiceType.CourtsBySR:
                    // Search on the SR number
                    serviceType = ServiceType.Courts;
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    }
                    serviceRequestSet = serviceRequest.GetCourtsAccount(serviceBranchNo, serviceUniqueId, "", 0, 0,user, out isPaidAndTakenAcct);
                    break;
                case ServiceType.NonCourts:
                    // Ignore the SR number to search by customer id
                    serviceType = ServiceType.NonCourts;
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer(custId, 0, 0, "", 0, 0).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer(custId, 0, 0, "", 0, 0).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    }
                    serviceRequestSet = serviceRequest.GetNonCourtsAccount(0, 0, custId);
                    break;
                case ServiceType.NonCourtsBySR:
                    // Search on the SR number
                    serviceType = ServiceType.NonCourts;
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    }
                    serviceRequestSet = serviceRequest.GetNonCourtsAccount(serviceBranchNo, serviceUniqueId, "");
                    break;
                case ServiceType.Stock:
                    // There could be a lot of stock repair Service Requests
                    // so must search on the SR number and the stock repair customer id
                    serviceType = ServiceType.Stock;
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer(StockRepair.CustomerId, 0, 0, "", 0, 0).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer(StockRepair.CustomerId, 0, 0, "", 0, 0).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    } 
                    // stock 
                    serviceRequestSet = serviceRequest.GetInternalStock(branchNo /*serviceBranchNo*/, serviceUniqueId, StockRepair.CustomerId);
                    break;
                case ServiceType.All:
                // Search on the SR number and execute the stored proc applicable to the service type
                    try
                    {
                        serviceRequestCustomerSR = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[0].Copy();
                        serviceRequestCustomerCurrent = serviceRequest.GetSRCustomer("", serviceBranchNo, serviceUniqueId, "", 0, 0).Tables[1].Copy();
                    }
                    catch
                    {
                        serviceRequestCustomerSR = null;
                        serviceRequestCustomerCurrent = null;
                    }
                    serviceRequestSet = serviceRequest.GetServiceRequestNo(serviceUniqueId, serviceBranchNo, user);

                    try
                    {
                        if (serviceRequestSet.Tables[0].Rows.Count > 0)
                        {
                            serviceType = serviceRequestSet.Tables[0].Rows[0]["ServiceType"].ToString();
                        }
                        else
                        {
                            serviceType = ServiceType.All;
                        }
                    }
                    catch
                    {
                        serviceType = ServiceType.All;
                    }
                    break;
                default:
                    break;
            }

            foreach (DataTable curTable in serviceRequestSet.Tables)
            {
               if (curTable.Columns.Contains(CN.PartNo))
                  curTable.TableName = TN.PartList;
               else if (curTable.Columns.Contains(CN.ActualCost))
                  curTable.TableName = TN.ChargeToAnalysis;
               else if (curTable.TableName == "Table3")
                  curTable.TableName = TN.WarrantyList;
               else
               {
                  curTable.TableName = TN.ServiceRequest;
                  if (curTable.Columns[0].ColumnName != CN.Locked)
                  {
                     this.CheckForWarranty(curTable, false);
                  }
               }
            }

        
            //---------UAT 383 -------------------------------------- NM
            // To cross check against WarrantyList table ---------------
            if (serviceRequestSet.Tables.Contains(TN.ServiceRequest))
            {
                foreach (DataRow dr in serviceRequestSet.Tables[TN.ServiceRequest].Rows)
                {
                    if (serviceRequestSet.Tables.Contains(TN.WarrantyList) &&
                        serviceRequestSet.Tables[TN.WarrantyList].Select(CN.ProductCode + " = '" + dr[CN.ProductCode].ToString() + "'").Length > 0)
                    {
                        dr[CN.FYWarranty] = "N";
                        dr[CN.ExtWarranty] = "N";
                    }
                }
            }
            //----------------------------------------------------------

            if (serviceRequestCustomerSR != null && serviceRequestCustomerCurrent != null)
            {
                serviceRequestCustomerSR.TableName = TN.CustomerSR;
                serviceRequestCustomerCurrent.TableName = TN.CustomerCurrent;
           
                serviceRequestSet.Tables.Add(serviceRequestCustomerSR);
                serviceRequestSet.Tables.Add(serviceRequestCustomerCurrent);
            }

           //If the status of any SR in the ServiceRequest DataTable is not new or closed then bring back the charge to authorisation's as well
            if (serviceRequestSet.Tables[TN.ServiceRequest] != null)
            {
               foreach (DataRow dr in serviceRequestSet.Tables[TN.ServiceRequest].Rows)
               {
               try
                  {
                  if (dr[CN.Status].ToString() != "N" && dr[CN.Status].ToString() != "C")
                     {
                        dtChargeToAuthorisation = serviceRequest.ChargeToAuthorisationLoad();
                        dtChargeToAuthorisation.TableName = TN.ServiceChargeToAuthorisation;
                        serviceRequestSet.Tables.Add(dtChargeToAuthorisation);
                        break;
                     }
                  }
               catch
                  {
                  }
               }
            }

            return serviceRequestSet;
        }
        /// <summary>
        ///  // added for CR ZEN/UNC/CRF/CR2018-009 Service Installation, cancel installation service when shipment is rejected.
        /// </summary>
        /// <param name="acctno"></param>
        /// <param name="itemno"></param>
        /// <returns></returns>
        public DataSet GetServiceRequest(
           string acctno, string itemno
         )
        {
            try
            {
                DServiceRequest serviceRequest = new DServiceRequest();
                DataSet serReq = new DataSet();

                serReq = serviceRequest.GetServiceRequestDetails(acctno, itemno);
                return serReq;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }


        private void CheckForWarranty(DataTable serviceRequestDetails,bool newSR)
        {
            // The DA only loads the Extended Warranty flag and the warranty refcode
            // Set up FYW / Instant Replacement / Extended Warranty
            // NOTE that only FYW is relevant for Non-Courts and Internal SRs
            serviceRequestDetails.Columns.Add(CN.InstantReplace, Type.GetType("System.String"));
            serviceRequestDetails.Columns.Add(CN.FYWarranty, Type.GetType("System.String"));
            serviceRequestDetails.Columns.Add(CN.InstantReplaceOrig, Type.GetType("System.String"));
            serviceRequestDetails.Columns.Add(CN.ExtWarrantyOrig, Type.GetType("System.String"));
            serviceRequestDetails.Columns.Add(CN.DateLoggedOrig, Type.GetType("System.DateTime"));
            serviceRequestDetails.Columns.Add(CN.DateClosedOrig, Type.GetType("System.DateTime"));
            serviceRequestDetails.Columns.Add(CN.BERCheck, Type.GetType("System.String"));

            DateTime dateLogged;
            

            foreach (DataRow row in serviceRequestDetails.Rows)
            {
                // UAT 380 If an item with an SR is cancelled or repossessed then FYW must not apply (if it was going to)
                // UAT 381 If an item is exchanged and the warranty is within the warranty valid period, however, then the FYW (if currently exists) will still apply

                bool warrantable = true;
                //if (!newSR)               //IP - 18/08/11 - #4564 - UAT53 - Needs to check this regardless if its a new or existing SR
                //{
                   //if (Convert.ToInt32(row[CN.ServiceRequestNo].ToString()) != 0 && ((Convert.ToInt32(row[CN.Quantity].ToString()) == 0 && row["Exchanged"].ToString() == "N") || row["Repossessed"].ToString() == "Y" || row["Exchanged"].ToString() == "Y"))
                   if ((Convert.ToInt32(row[CN.Quantity].ToString()) == 0 && row["Exchanged"].ToString() == "N") || row["Repossessed"].ToString() == "Y" || row["Exchanged"].ToString() == "Y")
                   {
                      warrantable = false;
                   }

                   if (row["Exchanged"].ToString() == "E")
                   {
                      warrantable = true;
                   }

                   // UAT 381 A replaced item (without a valid warranty) needs to be identified so that FYW can be set to 'N'
                   if (row["Replaced"].ToString() == "Y")
                   {
                      warrantable = false;
                   }
               // }

                if ((int)row[CN.ServiceRequestNo] > 0)
                    // Use the date of the existing SR if this is not a re-opened SR
                    if (newSR)
                    {
                        dateLogged = DateTime.Today;
                    }
                    else
                    {
                        dateLogged = (DateTime)row[CN.DateLogged];
                    }
                else
                    // There is no SR so used today
                    dateLogged = DateTime.Today;
                
                //RM CR1051 - add years should add man warranty length
                if (row[CN.Status].ToString() == "C")
                {
                    //if (((DateTime)row[CN.PurchaseDate]).AddYears(GetWarrantyLength((string)row[CN.ProductCode], Convert.ToInt32(row[CN.StockLocn]))) > DateTime.Now && warrantable)
                    if (((DateTime)row[CN.PurchaseDate]).AddMonths(GetWarrantyLength((string)row[CN.ProductCode], Convert.ToInt32(row[CN.StockLocn]))) > DateTime.Now && warrantable)       //#8263 jec 22/09/11
                    {
                        // Within First Year Warranty
                        row[CN.FYWarranty] = "Y";
                        row[CN.ExtWarranty] = "N";
                        if (row[CN.RefCode].ToString() == "ZZ")
                        {
                            row[CN.InstantReplace] = "Y";
                        }
                        else
                        {
                            row[CN.InstantReplace] = "N";
                        }
                    }
                    else if (row[CN.ExtWarranty].ToString() == "Y" && row[CN.RefCode].ToString() == "ZZ" && warrantable)
                    {
                        // Covered by Instant Replacement
                        row[CN.FYWarranty] = "N";
                        row[CN.InstantReplace] = "Y";
                        row[CN.ExtWarranty] = "N";
                    }
                    else
                    {
                        // No FYW / Instant Replacement but there might be an Extended Warranty
                        row[CN.FYWarranty] = "N";
                        row[CN.InstantReplace] = "N";
                    }
                }
                else
                {
                    //if (((DateTime)row[CN.PurchaseDate]).AddYears(GetWarrantyLength((string)row[CN.ProductCode], Convert.ToInt32(row[CN.StockLocn]))) > dateLogged && warrantable)
                    if (((DateTime)row[CN.PurchaseDate]).AddMonths(GetWarrantyLength((string)row[CN.ProductCode], Convert.ToInt32(row[CN.StockLocn]))) > dateLogged && warrantable)     //#8263 jec 22/09/11
                    {
                        // Within First Year Warranty
                        row[CN.FYWarranty] = "Y";
                        row[CN.ExtWarranty] = "N";
                        if (row[CN.RefCode].ToString() == "ZZ")
                        {
                            row[CN.InstantReplace] = "Y";
                        }
                        else
                        {
                            row[CN.InstantReplace] = "N";
                        }
                    }
                    else if (row[CN.ExtWarranty].ToString() == "Y" && row[CN.RefCode].ToString() == "ZZ" && warrantable)
                    {
                        // Covered by Instant Replacement
                        row[CN.FYWarranty] = "N";
                        row[CN.InstantReplace] = "Y";
                        row[CN.ExtWarranty] = "N";
                    }
                    else
                    {
                        // No FYW / Instant Replacement but there might be an Extended Warranty
                        row[CN.FYWarranty] = "N";
                        row[CN.InstantReplace] = "N";
                    }
                }
            
                // Original values are copied to allow the user to change the tick boxes
                // from Y to N but not N to Y
                row[CN.InstantReplaceOrig] = (string)row[CN.InstantReplace];
                row[CN.ExtWarrantyOrig] = (string)row[CN.ExtWarranty];

                // The user can change the date to a date before the saved logged date
                row[CN.DateLoggedOrig] = (DateTime)row[CN.DateLogged];

                // Updates are only saved for SRs that are open (or re-opend for comments)
                // However if a closed SR has been re-created then [CN.DateClosedOrig] needs to be set to Date.blankDate
                if (newSR)
                {
                    row[CN.DateClosedOrig] = Date.blankDate;
                }
                else
                {
                   //Only do this if the SR has been closed
                   if (row[CN.Status].ToString() == ServiceStatus.Resolution || row[CN.Status].ToString() == ServiceStatus.Closed)
                   {
                      row[CN.DateClosedOrig] = (DateTime)row[CN.DateClosed];
                   }
                   else
                   {
                      row[CN.DateClosedOrig] = Date.blankDate;
                   }
                }

                // Calculate whether the BER check is required
                if (!newSR)
                {
                    if ((string)row[CN.FYWarranty] == "Y" || (string)row[CN.ExtWarranty] == "Y")
                    {
                        // BER if sum of previous claims > % of original price
                       // UAT 385 Incorrect logic/calculation corrected 
                        if ((decimal)row[CN.PreviousCosts] > (decimal)Country[CountryParameterNames.ServiceBER] * (decimal)row[CN.UnitPrice] / 100)
                        {
                            row[CN.BERCheck] = "Y";
                        }
                    }
                }
            }
        }


        //RM CR1051 - procedure to get man warranty length by product and stocklocn (refcode)
        public int GetWarrantyLength(string prodcode, int stockLocn)
        {
            DServiceRequest serviceRequest = new DServiceRequest();

            return serviceRequest.GetWarrantyLength(prodcode, stockLocn);
        }

       /// <summary>
       /// Retrieves price index matrix values from the database
       /// </summary>
       /// <param name="screen">If true then the values remain in $US; if false then values are converted to local currency</param>
       /// <returns></returns>
        public DataSet LoadPriceIndexMatrix(bool screen)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            DataSet priceIndexMatrix = new DataSet();
            DataTable priceIndexList = serviceRequest.LoadPriceIndexMatrix();
            priceIndexList.TableName = TN.PriceIndexMatrix;

            if (screen == false)
            {
               //UAT 2 $US exchange rate to be applied to PriceIndexMatrix
               BPayment payment = new BPayment();
               DataSet ExchangeRateSet = payment.GetExchangeRates(null, null);

               if (ExchangeRateSet != null)
               {
                  foreach (DataRow dr in ExchangeRateSet.Tables[TN.ExchangeRates].Rows)
                  {
                     if (dr[CN.Currency].ToString().Contains("US"))
                     {
                        string strRate = dr[CN.Rate].ToString();
                        decimal rate = Convert.ToDecimal(strRate);
                        foreach (DataRow drPIL in priceIndexList.Rows)
                        {
                           decimal labourLimit = (decimal)drPIL[CN.LabourLimit] * rate;
                           drPIL[CN.LabourLimit] = labourLimit.ToString();
                           decimal additionalLabour = (decimal)drPIL[CN.AdditionalLimit] * rate;
                           drPIL[CN.AdditionalLimit] = additionalLabour.ToString();
                           decimal partLimit = (decimal)drPIL[CN.PartLimit] * rate;
                           drPIL[CN.PartLimit] = partLimit.ToString();
                        }
                        break;
                     }
                  }
               }
            }

            //Get the list of suppliers here
            DCode code = new DCode();
            code.GetCategoryCodes(CAT.ServiceModel, "L", TN.ServiceMake);

            priceIndexMatrix.Tables.Add(priceIndexList);
            priceIndexMatrix.Tables.Add(code.Codes);

            return priceIndexMatrix;
        }


        public void SavePriceIndexMatrix(SqlConnection conn, SqlTransaction trans,
            DataSet priceIndexMatrix)
        {
            // Delete the Price Index Matrix and then save the new copy
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.ClearPriceIndexMatrix(conn, trans);

            foreach (DataTable priceIndexMatrixTable in priceIndexMatrix.Tables)
            {
                if (priceIndexMatrixTable.TableName == TN.PriceIndexMatrix)
                {
                    foreach (DataRow row in priceIndexMatrixTable.Rows)
                    {
                        if (row.RowState != DataRowState.Deleted)
                        {
                            if (row[CN.Supplier].ToString().Trim() != "")
                            {
                                serviceRequest.SavePriceIndexMatrix(conn, trans,
                                    row[CN.Supplier].ToString(),
                                    row[CN.Product].ToString(),
                                    Convert.ToInt16(row[CN.Year]),
                                    row[CN.PartType].ToString(),
                                    Convert.ToInt16(row[CN.PartPercent]),
                                    Convert.ToDecimal(row[CN.PartLimit]),
                                    Convert.ToInt16(row[CN.LabourPercent]),
                                    Convert.ToDecimal(row[CN.LabourLimit]),
                                    Convert.ToInt16(row[CN.AdditionalPercent]),
                                    Convert.ToDecimal(row[CN.AdditionalLimit]));
                            }
                        }
                    }
                }
            }
        }


        public void GetChargeToDeposit(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo, short serviceBranchNo, 
            out string customerAcctNo, out decimal depositAmount, out decimal depositPaid)
        {
            string custId = "";
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.GetChargeToDeposit(conn, trans, 
                serviceRequestNo, out custId, out customerAcctNo, out depositAmount, out depositPaid);

            CreateChargeToAccount(conn, trans, serviceRequestNo, serviceBranchNo, custId, customerAcctNo);            
        }


       private void CreateChargeToAccount(SqlConnection conn, SqlTransaction trans, int serviceRequestNo, short serviceBranchNo, string custId, string customerAcctNo)
       {
          if (customerAcctNo.Length != 12)
          {
             // Create a new cash account
             bool rescore = false;
             BAccount customerAccount = new BAccount();
             customerAcctNo = customerAccount.CreateCustomerAccount(conn, trans,
                 (string)Country[CountryParameterNames.CountryCode],
                 serviceBranchNo,
                 custId,
                 AT.Cash,
                 User,
                 false,
                 out rescore,
                 "");

             //#14453
             if (customerAcctNo != string.Empty)
             { 
                 customerAccount.Populate(customerAcctNo);
                 customerAccount.TermsType = "00";
                 customerAccount.Save(conn, trans);
             }  
              

          }

          customerAcctNo = customerAcctNo.Trim().Replace("-", "");
          if (customerAcctNo.Length == 12)
          {
             // Link the Charge To account to the SR
             DServiceRequest serviceRequest = new DServiceRequest();
             serviceRequest.LinkChargeToAccount(conn, trans, serviceRequestNo, customerAcctNo, ServiceAcct.Customer);
          }
       }


        public string GetChargeToAcct(SqlConnection conn, SqlTransaction trans,
            int serviceRequestNo, short serviceBranchNo, string chargeType)
        {   
            //
            // Check a Charge To account exists for an SR
            // If it does not exist a new link to a Charge To account will be created
            // For a Deliverer or Customer a new cash account will be created
            // The Charge To acctno is returned
            //
            bool newLink = false;
            string gotAcctNo = "";
            string gotId = "";
            string internalAcctNo = "";
            string warrantyAcctNo = "";
            string supplierAcctNo = "";
            string supplierSpecialAcctNo = "";
            string delivererAcctNo = "";
            string customerAcctNo = "";
            string supplierId = "";
            string delivererId = "";
            string customerId = "";
            decimal depositAmount = -1;
            decimal depositPaid = 0;

            // Load the existing links for this SR
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.GetChargeToAccts(conn, trans,
                serviceRequestNo,
                out internalAcctNo, out warrantyAcctNo,
                out supplierAcctNo, out supplierSpecialAcctNo,
                out delivererAcctNo, out customerAcctNo,
                out supplierId, out delivererId, out customerId,
                out depositAmount, out depositPaid);

            // Check whether this type of Charge To account ia already linked
            switch (chargeType)
            {
                case ServiceAcct.Internal:
                    if (internalAcctNo.Length != 12)
                    {
                        // Need a new link to Internal special account
                        internalAcctNo = (string)Country[CountryParameterNames.ServiceInternal];
                        newLink = true;
                    }
                    gotAcctNo = internalAcctNo;
                    break;
                case ServiceAcct.Warranty:
                    if (warrantyAcctNo.Length != 12)
                    {
                        // Need a new link to AIG special account
                        warrantyAcctNo = (string)Country[CountryParameterNames.ServiceWarranty];
                        newLink = true;
                    }
                    gotAcctNo = warrantyAcctNo;
                    break;
                case ServiceAcct.Supplier:
                    if (supplierAcctNo.Length != 12)
                    {
                        // Need a new link to this supplier's special account
                        supplierAcctNo = supplierSpecialAcctNo;
                        newLink = true;
                    }
                    gotAcctNo = supplierAcctNo;
                    break;
                case ServiceAcct.Deliverer:
                    // If a new Deliverer link is required then need a new cash account (below)
                    if (delivererAcctNo.Length != 12)
                    {
                        newLink = true;
                        gotId = delivererId;
                    }
                    else
                    {
                        gotAcctNo = delivererAcctNo;
                    }
                    break;
                case ServiceAcct.Customer:
                    // If a new Customer link is required then need a new cash account (below)
                    if (customerAcctNo.Length != 12)
                    {
                        newLink = true;
                        gotId = customerId;
                    }
                    else
                    {
                        gotAcctNo = customerAcctNo;
                    }
                    break;
                default:
                    break;
            }

            if (newLink)
            {
                // Create a new link from the SR to a Charge account

                // Deliverer and Customer must create a new cash acct first

               if (chargeType == ServiceAcct.Deliverer || chargeType == ServiceAcct.Customer)
                {
                    // Create a new cash account
                    bool rescore = false;
                    BAccount customerAccount = new BAccount();
                    gotAcctNo = customerAccount.CreateCustomerAccount(conn, trans,
                        (string)Country[CountryParameterNames.CountryCode],
                        serviceBranchNo,
                        gotId,
                        AT.Cash,
                        User,
                        false,
                        out rescore
                        , "");
                }

               // UAT 312 Decision taken by KL that a new charge-to cash acct should be created for each SR that is charged to a deliverer
                // For this to work the deliverer's custID must be entered into Code Maintenance as 'code'.

               // if (chargeType == ServiceAcct.Deliverer)
               //{
               //   gotAcctNo = serviceRequest.GetDeliveryAccount(conn, trans, serviceRequestNo, gotId);
               //}

                gotAcctNo = gotAcctNo.Trim().Replace("-", "");
                if (gotAcctNo.Length == 12)
                {
                    // Link the Charge To account to the SR
                    serviceRequest.LinkChargeToAccount(conn, trans, serviceRequestNo, gotAcctNo, chargeType);
                }
            }
            return gotAcctNo;
        }

        public DataSet GetTechnicianDiary(int technicianId)
        {
            DataSet technicianDiarySet = new DataSet();
            DataTable technicianDiaryList = new DataTable();
            DServiceRequest serviceRequest = new DServiceRequest();
            technicianDiaryList = serviceRequest.GetTechnicianDiary(technicianId);

            technicianDiaryList.TableName = TN.TechnicianDiary;
            technicianDiarySet.Tables.Add(technicianDiaryList);
            return technicianDiarySet;
        }


        public void BookServiceRequest(SqlConnection conn, SqlTransaction trans,
            string zone, int technicianId, DateTime slotDate, short slotNo, 
            short multiSlot, string bookingType, 
            long serviceRequestNo,string IsAM, int allocatedBy, out bool notFound,
            out bool alreadyBooked, out DateTime curSlotDate, out string curBookingType, string reassignCode, string reassignedBy)      //CR1030 jec
        {
            // Split the SR number into the branch number and this unique id
            short serviceBranchNo = 0;
            int serviceUniqueId = 0;
            this.SplitSRNo(serviceRequestNo, out serviceBranchNo, out serviceUniqueId);

            // first check that slotDate is acceptable i.e. a parts expected date has not already been set that is after this date

            DataSet ds = new DataSet();
            ds = this.CheckPartsDate(serviceRequestNo);
            DateTime dtPartsdate = slotDate;
            if (ds.Tables[0].Rows.Count > 0)
            {
                dtPartsdate = (DateTime)ds.Tables[0].Rows[0][CN.PartsDate];
            }
            if (dtPartsdate > slotDate)
            {
                string date = dtPartsdate.ToShortDateString();
                throw new STLException(GetResource("M_PARTSDATEISSUE", new object[] { date }));
            }
            else
            {
                DServiceRequest serviceRequest = new DServiceRequest();
                serviceRequest.BookServiceRequest(conn, trans,
                    zone, technicianId, slotDate, slotNo, multiSlot, bookingType, serviceUniqueId,IsAM,
                    allocatedBy, out notFound, out alreadyBooked, out curSlotDate, out curBookingType, reassignCode, reassignedBy);     //CR1030 jec
            }
        }


        public void FreeServiceRequest(SqlConnection conn, SqlTransaction trans,
            int technicianId, DateTime slotDate, short slotNo)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.FreeServiceRequest(conn, trans,
                technicianId, slotDate, slotNo);
        }


        public DataSet GetTechnicians(DateTime dateAvailable)
        {
            DataSet technicianSet = new DataSet();
            DataTable technicianList = new DataTable();
            DServiceRequest serviceRequest = new DServiceRequest();
            technicianList = serviceRequest.GetTechnicians(dateAvailable);
            technicianList.TableName = TN.Technician;
            technicianSet.Tables.Add(technicianList);
            return technicianSet;
        }


        public DataSet GetTechniciansByZone(DateTime dateAvailable)
        {
            DataSet technicianSet = new DataSet();
            DataTable technicianList = new DataTable();
            DServiceRequest serviceRequest = new DServiceRequest();
            technicianList = serviceRequest.GetTechniciansByZone(dateAvailable);
            technicianList.TableName = TN.TechnicianByZone;
            technicianSet.Tables.Add(technicianList);
            return technicianSet;
        }

        public void BalanceAccounts(SqlConnection conn, SqlTransaction trans)
        {
            DServiceRequest serviceRequest = new DServiceRequest();
            serviceRequest.BalanceAccounts(conn, trans);
        }

        public void AddSalesTaxToCustomerAccount(SqlConnection conn, SqlTransaction trans, string customerAcctNo, 
                                                    short serviceBranchNo)
        {
            BAccount acct = new BAccount();
            BDelivery delivery = new BDelivery();
            BItem lineItem = new BItem();
            DBranch branch = new DBranch();

            decimal chargeableAdmin = 0;
            decimal chargeable = 0;
            decimal tax = 0;
            decimal transValue = 0;

            int transRefNo = branch.GetTransRefNo(conn, trans, serviceBranchNo);

            XmlNode lineItems = lineItem.GetLineItems(conn, trans, customerAcctNo, AT.Cash,
                                (string)Country[CountryParameterNames.CountryCode], 1);
            if (lineItems != null)
            {
                tax = acct.CalculateSalesTax((string)Country[CountryParameterNames.CountryCode], lineItems,
                                                AT.Cash, false, true, ref chargeableAdmin, ref chargeable);

                lineItem.OrigBr = 0;
                lineItem.AccountNumber = customerAcctNo;
                lineItem.AgreementNumber = 1;
                lineItem.StockLocation = serviceBranchNo;
                lineItem.ItemNumber = "STAX";
                lineItem.ItemId = StockItemCache.Get(StockItemKeys.STAX);
                lineItem.Price = 0;
                lineItem.TaxAmount = 0;
                lineItem.Quantity = 1;
                lineItem.OrderValue = tax;
                lineItem.ItemType = "N";
                lineItem.DeliveryNoteBranch = serviceBranchNo;
                lineItem.DeliveryProcess = "I";
                lineItem.QuantityDiff = "N";
                lineItem.DeliveredQuantity = lineItem.Quantity;
                lineItem.ScheduledQuantity = lineItem.Quantity;
                lineItem.ItemSuppText = "";
                lineItem.DateRequiredDelivery = Date.blankDate;
                lineItem.TimeRequiredDelivery = "";
                lineItem.ContractNo = "";
                lineItem.IsKit = 0;
                lineItem.HasString = 0;
                lineItem.Notes = "";
                lineItem.DeliveryAddress = "";
                lineItem.ExpectedReturnDate = Date.blankDate;
                lineItem.DeliveryArea = "";
                lineItem.Damaged = "";
                lineItem.Assembly = "";
                lineItem.Save(conn, trans);

                delivery.User = this.User;
                delivery.DeliverNonStocks(conn, trans, customerAcctNo, AT.Cash,
                                          (string)Country[CountryParameterNames.CountryCode], serviceBranchNo,
                                           transRefNo, ref transValue, 1);

                DAgreement agree = new DAgreement(conn, trans, customerAcctNo, 1);
                //UAT 335 Agreement total and Cash price need to be rounded to 2 decimal places so that account is settled when full payment is made.
                agree.AgreementTotal = Math.Round(agree.AgreementTotal, 2);
                agree.AgreementTotal += tax;
                agree.CashPrice = Math.Round(agree.CashPrice, 2);
                agree.CashPrice += tax;
                agree.DateChange = DateTime.Now; //#14481
                agree.Save(conn, trans);

                DAccount account = new DAccount();
                account.Populate(conn, trans, customerAcctNo);
               //UAT 335 Agreement total and Outstanding balance need to be rounded to 2 decimal places so that account is settled when full payment is made.
                account.AgreementTotal = Math.Round(agree.AgreementTotal,2);
                account.OutstandingBalance = Math.Round(account.OutstandingBalance, 2);
                account.OutstandingBalance += tax;
                account.Save(conn, trans);
            }
        }

        public void UpdateSRCustomer(SqlConnection conn, SqlTransaction trans, int serviceRequestNo, string custID, string title, string firstName, string lastName, decimal arrears, string address1, string address2, string address3, string postCode, string directions, string telHome, string telWork, string telMobile)
        {
           // Update the SR customer details
           DServiceRequest serviceRequest = new DServiceRequest();
           serviceRequest.UpdateSRCustomer(conn, trans, serviceRequestNo, custID, title, firstName, lastName, arrears, address1, address2,
           address3, postCode, directions, telHome, telWork, telMobile);
        }

       /// <summary>
       /// Returns unique invoice number (agreement/buff number) for the selected branch
       /// </summary>
       /// <param name="conn"></param>
       /// <param name="trans"></param>
       /// <param name="customerID"></param>
       /// <returns></returns>
       public int GetInvoiceNumber(SqlConnection conn, SqlTransaction trans, CustomerDetails cusDetails)
        {
           DServiceRequest serviceRequest = new DServiceRequest();
           DCustomer customer = new DCustomer();
           customer.GetCustomerDetails(conn, trans, cusDetails.customerID);
           int invoice = 0;
           if (customer.Name == String.Empty)
           {
              serviceRequest.SaveCustomerDetails(conn, trans, cusDetails);
              DBranch branch = new DBranch();
              invoice = branch.GetBuffNo(conn, trans, cusDetails.branchNo);
           }
           return invoice;
       }

       public bool CheckTechMainPermissions(int userId)
       {
           DServiceRequest serviceRequest = new DServiceRequest();
           return serviceRequest.CheckTechMainPermissions(userId);
       }

        //CR1030 Installation ChargeTo jec 02/02/11
        public void PostInstallationCharges(SqlConnection conn, SqlTransaction trans, List<InstChargeAnalysisResult> analyses, int instNo, short branchNo, DataTable parts = null) //#12116
        {
    
            string instAcctNo = Country[CountryParameterNames.InstalChgAcct].ToString().Replace("-", "").Trim();

            if (instAcctNo.Length != 12)
                throw new STLException(GetResource("M_NOCHARGETOINTSTALL"));

            foreach (var analysis in analyses)
            {
                string specialItemNumber = String.Empty;
                var itemid = 0;             // RI
                bool itemEntry = false;
                switch (analysis.BreakDownCode)
                {
                    case INST.CHARGEBREAKDOWN.PartsCourts:
                        specialItemNumber = (string)Country[CountryParameterNames.ServiceItemPartsCourts];
                        itemid = StockItemCache.Get(StockItemKeys.ServiceItemPartsCourts);
                        itemEntry = true;
                        break;
                    case INST.CHARGEBREAKDOWN.PartsOther:
                        specialItemNumber = (string)Country[CountryParameterNames.ServiceItemPartsOther];
                        itemid = StockItemCache.Get(StockItemKeys.ServiceItemPartsOther);
                        itemEntry = true;
                        break;
                    case INST.CHARGEBREAKDOWN.LabourTotal:
                        specialItemNumber = (string)Country[CountryParameterNames.ServiceItemLabour];
                        itemid = StockItemCache.Get(StockItemKeys.ServiceItemLabour);
                        itemEntry = true;
                        break;
                    default:
                        break;
                }

                if (itemEntry == true)
                {
                    if(specialItemNumber == "")
                        throw new STLException("No generic item numbers have been set up for labour and parts in Country Maintenance");

                    if (analysis.Electrical > 0M)
                        SaveServiceLineItem(conn, trans, AT.Special, instAcctNo, instNo, branchNo, specialItemNumber, 1,
                                                analysis.Electrical, itemid, TransType.InstallationElectrical);             // RI

                    if (analysis.Furniture > 0M)
                        SaveServiceLineItem(conn, trans, AT.Special, instAcctNo, instNo, branchNo, specialItemNumber, 1,
                                                analysis.Furniture, itemid, TransType.InstallationFurniture);               // RI
                }
            }
        }

        public void SaveServiceChargeTax(SqlConnection conn, SqlTransaction trans, decimal totalTax, string acctType,
                                             string serviceAcctNo, int serviceAgreementNo, short serviceBranchNo, int? taxItemId = null)
        {
            if ((string)Country[CountryParameterNames.AgreementTaxType] == "E" &&
              (decimal)Country[CountryParameterNames.TaxRate] > 0 && totalTax > 0)
            {
                this.SaveServiceLineItem(conn, trans, acctType, serviceAcctNo, serviceAgreementNo, serviceBranchNo, "STAX", 1,
                                         Convert.ToDecimal(totalTax), Convert.ToInt32(taxItemId));
            }
        }

        //#11839 - called from service web 
        public void SaveServiceRequestCharges(SqlConnection conn, SqlTransaction trans, string acctType,
          string serviceAcctNo, int serviceAgreementNo, short serviceBranchNo,
          string serviceItemNumber, decimal chargeToValue, int itemid, bool finalCharge, decimal? tax = null, int? taxItemId = null)
        {
            //If Stock tax is Exclusive
            if ((string)Country[CountryParameterNames.AgreementTaxType] == "I" && tax != null)
            {
                chargeToValue = chargeToValue + Convert.ToDecimal(tax);
            }

            ////if (tax != null)
            ////{
            ////    totalTax += Convert.ToDecimal(tax);
            ////}

            this.SaveServiceLineItem(conn, trans, acctType, serviceAcctNo, serviceAgreementNo, serviceBranchNo, serviceItemNumber, 1,
                                         chargeToValue, itemid);

            
            ////if ((string)Country[CountryParameterNames.AgreementTaxType] == "E" &&
            ////    (decimal)Country[CountryParameterNames.TaxRate] > 0 && finalCharge && totalTax > 0)
            ////{
            ////    this.SaveServiceLineItem(conn, trans, acctType, serviceAcctNo, serviceAgreementNo, serviceBranchNo, "STAX", 1,
            ////                             Convert.ToDecimal(totalTax), Convert.ToInt32(taxItemId));
            ////}
        }

        /// <summary>
        ///  // added for CR ZEN/UNC/CRF/CR2018-009 Service Installation, cancel installation service when shipment is rejected.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="DeliveryDateUpdated"></param>
        public void UpdateServiceRequest(int id, string DeliveryDateUpdated)
        {
            try
            {
                DServiceRequest serviceRequest = new DServiceRequest();
                string status = ServiceStatus.Closed;
                serviceRequest.User = STL.Common.Static.Credential.UserId;
                serviceRequest.UpdateServiceRequest(id, status,serviceRequest.User, DeliveryDateUpdated);
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

