using System;
using System.Data;
using System.Globalization;
using System.Xml;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.ServiceRequest;
using System.Linq;

namespace STL.WS
{
    public partial class WFoodLoss : CommonWebPage
    {
        private string Servicerequestno;
        private string customerId;
        private string AccountNo;
        private string Deliveryaddress = "XX";
        private string User;
        private string countryCode;
        private string culture;

        //private BCountry bCountry = null;
        private BCustomer bCustomer = null;
        private BServiceRequest bServiceRequest = null;
        private DataSet dCustomer = null;
        private DataSet dSR = null;
        private DataSet dFoodLoss = null;
        NumberFormatInfo LocalFormat = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Function = "PrintFoodLoss";
                Servicerequestno = Request[CN.ServiceRequestNo];
                customerId = Request[CN.CustomerID];
                AccountNo = Request[CN.AccountNumber];
                countryCode = Request[CN.CountryCode];
                culture = Request[CN.Culture];
                User = Request["User"];

                #region Retrieve all the required data
                //Set the culture for currency formatting
                //Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);
                base.SetCulture();

                //Set printer display options for numbers and currency
                LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
                LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];

                bCustomer = new BCustomer();
                dCustomer = bCustomer.GetBasicCustomerDetails(null, null, customerId, AccountNo, "H");

                BServiceRequest bSR = new BServiceRequest();
                dFoodLoss = bSR.GetFoodLoss(Convert.ToInt32(Servicerequestno.Substring(3)));

                string serviceType = "";
                bool isPaidAndTaken = false;

                dSR = bSR.GetServiceRequest(ServiceType.All, Convert.ToInt64(Servicerequestno), customerId, AccountNo, 1, Convert.ToInt16(AccountNo.Substring(0, 3)), Convert.ToInt32(User), out serviceType, out isPaidAndTaken);
                #endregion

                //DBranch branch = new DBranch();
                //string acctNo = AccountNo.Substring(0, 3);
                //string storeType = branch.GetStoreType(Convert.ToInt16(acctNo));

                var total = 0M;

                FoodLossXML fxml = new FoodLossXML(countryCode);
                fxml.Load("<FOODLOSS></FOODLOSS>");
                
                fxml.GetNode("FOODLOSS").AppendChild(CreateHeader(fxml));
                fxml.GetNode("FOODLOSS").AppendChild(CreateItems(fxml, out total));
                
                if (fxml.GetNode("FOODLOSS/ITEMS") != null)
                    fxml.GetNode("FOODLOSS/ITEMS").AppendChild(CreateFooter(fxml, total));

                Response.Write(fxml.Transform());

                bCustomer = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                logException(ex, Function);
                Response.Write(ex.Message);
            }
        }

        private XmlNode CreateItems(FoodLossXML fxml, out decimal total)
        {
            XmlNode items = fxml.Document.CreateElement("ITEMS");

            if (dFoodLoss.Tables.Contains(TN.ServiceFoodLoss) == false)
            {
                total = 0;
                return items;
            }

            var rows = dFoodLoss.Tables[TN.ServiceFoodLoss].AsEnumerable();
            total = rows.Sum(r => r.Field<Decimal>(CN.ItemValue));
    
            XmlNode item;
            int index = 1;
            foreach (var r in rows)
            {
                item = fxml.Document.CreateElement("ITEM");
                item.AppendChild(fxml.CreateNode("BULLETEDNUMBER", String.Format("{0}.", index++)));
                item.AppendChild(fxml.CreateNode("DESC", r.Field<String>(CN.ItemDescription)));
                item.AppendChild(fxml.CreateNode("VALUE", r.Field<Decimal>(CN.ItemValue).ToString(DecimalPlaces, LocalFormat)));
                items.AppendChild(item);
            }

            items.AppendChild(fxml.CreateNode("ITEMTOTAL", total.ToString(DecimalPlaces, LocalFormat)));

            return items;
        }

        private XmlNode CreateFooter(FoodLossXML fxml, decimal total)
        {
            XmlNode footer = fxml.Document.CreateElement("TOTAL");
            footer.AppendChild(fxml.CreateNode("DESC", "TOTAL"));
            footer.AppendChild(fxml.CreateNode("VALUE", Convert.ToDecimal(total).ToString(DecimalPlaces, LocalFormat)));

            return footer;
        }

        private XmlNode CreateHeader(FoodLossXML fxml)
        {
            XmlNode header = fxml.Document.CreateElement("HEADER");
            string datelogged = null;
            string technicianId = null;
            string technicianReport = null;
            string modelNo = null;
            string contractNo = null;
            string EXTWarranty = null;
            string branchno = null;

            header.AppendChild(fxml.CreateNode("SERVICEREQUESTNO", this.Servicerequestno));
            header.AppendChild(fxml.CreateNode("NOW", DateTime.Now));
            foreach (DataTable t in dSR.Tables)
            {
                if (t.TableName == TN.ServiceRequest)
                {
                    foreach (DataRow r in t.Rows)
                    {
                        if (r[CN.ServiceRequestNoStr].ToString() == this.Servicerequestno)
                        {
                            datelogged = r[CN.DateLoggedStr].ToString();
                            technicianId = r[CN.TechnicianId].ToString();
                            technicianReport = r[CN.TechnicianReport].ToString();
                            modelNo = r[CN.ModelNo].ToString();
                            contractNo = r[CN.ContractNo].ToString();
                            EXTWarranty = r[CN.ExtWarranty].ToString();
                            branchno = r[CN.ServiceBranchNo].ToString();
                            break;
                        }
                    }
                }
            }
            if (datelogged != null)
            {
                header.AppendChild(fxml.CreateNode("DATELOGGED", datelogged));
            }
            if (technicianReport != null)
            {
                header.AppendChild(fxml.CreateNode("TECHNICIANNOTES", technicianReport));
            }
            if (modelNo != null)
            {
                header.AppendChild(fxml.CreateNode("MODELNO", modelNo));
            }
            if (contractNo != null)
            {
                header.AppendChild(fxml.CreateNode("CONTRACTNO", contractNo));
            }
            if (technicianId != null && technicianId != "0")
            {
                bServiceRequest = new BServiceRequest();
                DataTable dt = new DataTable();
                dt = bServiceRequest.GetTechnician(Convert.ToInt32(technicianId));

                var technician = "";  //Bug #3037
                if (dt.Rows.Count > 0)
                    technician = String.Format("{0} {1} ({2})",
                                                dt.Rows[0][CN.FirstName],
                                                dt.Rows[0][CN.LastName],
                                                dt.Rows[0][CN.TechnicianId]
                                                );

                header.AppendChild(fxml.CreateNode("TECHNICIANNAME", technician));
            }
            if (EXTWarranty != null)
            {
                header.AppendChild(fxml.CreateNode("EXTWARRANTY", EXTWarranty));
            }
            if (branchno != null)
            {
                BBranch branch = new BBranch();
                DataSet ds = new DataSet();
                ds = branch.Get(Convert.ToInt32(branchno));
                foreach (DataTable t in ds.Tables)
                {
                    if (t.TableName == TN.BranchDetails)
                    {
                        branchno = t.Rows[0][CN.BranchNo].ToString() + " - " + t.Rows[0][CN.BranchName].ToString();
                        break;
                    }
                }
                header.AppendChild(fxml.CreateNode("BRANCHNAME", branchno));
            }

            foreach (DataTable dt in dCustomer.Tables)
            {
                if (dt.TableName == "BasicDetails")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = (string)row["Title"] + " " + (string)row["FirstName"] + " " + (string)row["LastName"];
                        header.AppendChild(fxml.CreateNode("NAME", name));
                        header.AppendChild(fxml.CreateNode("CUSTID", customerId));
                        header.AppendChild(fxml.CreateNode("ACCTNO", AccountNo));
                    }
                }

                if (dt.TableName == "CustomerAddresses")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (((string)row[CN.AddressType]).Trim() == "H" &&
                            ((string)row[CN.Category] == "CA1" || (string)row[CN.Category] == "CT1")) //IP - 13/08/09 - UAT(469) - added or and 'CT1'
                        {
                            header.AppendChild(fxml.CreateNode("ADDR1", (string)row["Address1"]));
                            header.AppendChild(fxml.CreateNode("ADDR2", (string)row["Address2"]));
                            header.AppendChild(fxml.CreateNode("ADDR3", (string)row["Address3"]));
                            header.AppendChild(fxml.CreateNode("POSTCODE", (string)row["PostCode"]));
                        }

                        // malaysia requirement appending delivery address
                        //UAT 233 Delivery Address Type should be 'D' not 'D1'          //IP - 11/05/10 - UAT(136) UAT5.2.1.0 log - Merged from 4.3
                        else if (((string)row[CN.AddressType]).Trim() == Deliveryaddress) //Use the delivery address if an items chosen from this address...
                        {
                            header.AppendChild(fxml.CreateNode("DELADDR1", (string)row["Address1"]));
                            header.AppendChild(fxml.CreateNode("DELADDR2", (string)row["Address2"]));
                            header.AppendChild(fxml.CreateNode("DELADDR3", (string)row["Address3"]));
                            header.AppendChild(fxml.CreateNode("DELPOSTCODE", (string)row["PostCode"]));
                        }
                    }

                    foreach (DataRow row in dt.Rows)
                    {
                        string addType = ((string)row["AddressType"]).Trim();
                        switch (addType)
                        {
                            case "H":
                                header.AppendChild(fxml.CreateNode("HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
                                break;
                            case "W": header.AppendChild(fxml.CreateNode("WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
                                break;
                            case "M": header.AppendChild(fxml.CreateNode("MOBILE", (string)row["Phone"]));
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
            return header;
        }
    }
}