using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using STL.BLL;
using STL.DAL;
using STL.Common;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.Elements;
using System.Threading;
using System.Globalization;
using STL.Common.Constants.ColumnNames;
using System.Diagnostics;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Tags;
using STL.Common.Constants.ItemTypes;
using System.Configuration;
using STL.Common.Constants.StoreInfo;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using Blue.Cosacs;
using System.Data.SqlClient;
using STL.BLL.WebApi;

namespace STL.WS
{
	/// <summary>
	/// Summary description for WAgreement.
	/// </summary>
	public partial class WCashLoan : CommonWebPage
	{
		private string CustomerID = "";
		private string AccountNo = "";
		private string AccountType = "";
		private string Culture = "";
		private string CountryCode = "";
		private decimal Deposit = 0;
		private decimal Total = 0;
		private decimal GoodsValue = 0;
		private decimal GoodsTax = 0;
		private decimal DTTax = 0;
		private decimal InstalNo = 0;
		private decimal ServicePCent = 0;
		private decimal ToPay = 0;
		private string TermsDescription = "";
		private bool PaymentHolidays = false;
		private short PaymentHolidaysMax = 0;
		private short PaymentHolidaysMin = 0;
		private string AgreementText = "";
		private string JointName = "";
		private string JointID = "";
        private string relationship = "";
        private bool Multiple = false;
		//private BCountry bCountry = null;
		private BCustomer bCustomer = null;
		private DataSet dCustomer = null;
		private BAccount bAccount = null;
		private DTermsType dTermsType = null;
		//private BAgreement bAgreement = null;
		private DataSet dAgreement = null;
		private BItem bItem = null;
		private XmlNode lineItems = null;
		private double Pages = 0;
		private double itemsPerPage = 10;
		private DataTable dJoint = null;
		private int monthsintfree = 0;
		private int monthsdeferred = 0;
		private decimal insPcent = 0;
		private bool insIncluded = false;
		private decimal chargeableAdminPrice = 0;
		private decimal chargeablePrice = 0;
		private decimal insuranceCharge = 0;
		string addType = "";
        private int printBranch = 0;                //#19425 - CR18938
        private decimal AdminPCent = 0;
        private decimal AdminValue = 0;  //kedar 
        private decimal Maxterm = 0;//kedar
        private decimal MonthlyAdminCharge = 0;
        private decimal Adminchg = 0;

        NumberFormatInfo LocalFormat = null;
		PaymentWebApi paymentWebApi = new PaymentWebApi();

		private XmlNode CreateHeader(CashLoanXML axml)
		{
			XmlNode header = axml.Document.CreateElement("HEADER");
			
			header.AppendChild(axml.CreateNode("JOINTNAME", JointName));
			header.AppendChild(axml.CreateNode("JOINTID", JointID));
            header.AppendChild(axml.CreateNode("RELATIONSHIP", relationship.Trim()));

            if (dJoint != null)
			{
				foreach(DataRow r in dJoint.Rows)
				{
					header.AppendChild(axml.CreateNode("JOINTADDR1", (string)r[CN.Address1]));
					header.AppendChild(axml.CreateNode("JOINTADDR2", (string)r[CN.Address2]));
					header.AppendChild(axml.CreateNode("JOINTADDR3", (string)r[CN.Address3]));
					header.AppendChild(axml.CreateNode("JOINTPOSTCODE", (string)r[CN.PostCode]));
				}
			}

			foreach(DataTable dt in dCustomer.Tables)
			{
                if (dt.TableName == "BasicDetails")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string name = (string)row["Title"] + " " + (string)row["FirstName"] + " " + (string)row["LastName"];
                        header.AppendChild(axml.CreateNode("NAME", name));
                        header.AppendChild(axml.CreateNode("CUSTID", CustomerID));
                        header.AppendChild(axml.CreateNode("ACCTNO", AccountNo));
                    }
                }
                if (dt.TableName == "Employment")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        header.AppendChild(axml.CreateNode("EMPLOYER", (string)row["Employer"]));
                        header.AppendChild(axml.CreateNode("EMPYRADD1", (string)row["EmpyrAddr1"]));
                        header.AppendChild(axml.CreateNode("EMPYRADD2", (string)row["EmpyrAddr2"]));
                        header.AppendChild(axml.CreateNode("EMPYRADD3", (string)row["EmpyrAddr3"]));
                        header.AppendChild(axml.CreateNode("EMPYRPOCODE", (string)row["EmpyrPOCode"]));
                    }
                }

                if (dt.TableName == "CustomerAddresses")
				{
					foreach(DataRow row in dt.Rows)
					{
						if(((string)row[CN.AddressType]).Trim()=="H" &&
                            ((string)row[CN.Category] == "CA1" || (string)row[CN.Category] == "CT1"))       //#19425 - CR18938
						{
							header.AppendChild(axml.CreateNode("ADDR1", (string)row["Address1"]));
							header.AppendChild(axml.CreateNode("ADDR2", (string)row["Address2"]));
							header.AppendChild(axml.CreateNode("ADDR3", (string)row["Address3"]));
							header.AppendChild(axml.CreateNode("POSTCODE", (string)row["PostCode"]));
						}
					}

					foreach(DataRow row in dt.Rows)
					{
						addType = ((string)row["AddressType"]).Trim();
						switch(addType)
						{
							case "H":	
								header.AppendChild(axml.CreateNode("HOMETEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
								break;
							case "W":	header.AppendChild(axml.CreateNode("WORKTEL", ((string)row["DialCode"]).Trim() + " " + (string)row["Phone"]));
								break;
							case "M":	header.AppendChild(axml.CreateNode("MOBILE", (string)row["Phone"]));
								break;
							default:
								break;
						}
					}
				}

			}

            //#19425 - CR18938 - Print the Loan amount on Cash Loan for Trinidad & Tobago
            CashLoan loan = new CreditRepository().GetCashLoan(AccountNo);

            if (loan != null)
            {
                header.AppendChild(axml.CreateNode("LOANAMOUNT", loan.LoanAmount.Value.ToString(DecimalPlaces, LocalFormat)));
            }

            DBranch dbranch = new DBranch();
            var branchname = dbranch.Get(printBranch).Tables[0].Rows[0]["branchname"];
            
            header.AppendChild(axml.CreateNode("PRINTBRANCH", branchname.ToString()));

			/*foreach (DataTable dt in dAgreement.Tables)
			{
				if(dt.TableName==TN.AccountDetails)
				{
					foreach(DataRow row in dt.Rows)
					{
						if((bool)Country[CountryParameterNames.AgrTimePrint])
							header.AppendChild(axml.CreateNode("DATE", ((DateTime)row["Account Opened"]).ToShortDateString()));
					}
				}
			}*/

			if((bool)Country[CountryParameterNames.AgrTimePrint])
				header.AppendChild(axml.CreateNode("DATE", DateTime.Now.ToString()));

			return header;
		}

		private XmlNode CreateFooter(CashLoanXML axml)
		{
            bool agreementProcessed = false;
            decimal srvcharge = 0;
            decimal instaltot = 0;
			XmlNode footer = axml.Document.CreateElement("FOOTER");

			foreach (DataTable dt in dAgreement.Tables)
			{
				if(dt.TableName==TN.Agreements)
				{
					foreach(DataRow row in dt.Rows)
					{
                        //LiveWire Call 68210
                        if (!agreementProcessed)
                        {
                            Deposit = (decimal)row["Deposit"];
                            footer.AppendChild(axml.CreateNode("DEPOSIT", Deposit.ToString(DecimalPlaces, LocalFormat)));
                            //Total amount on the Promissory note for normal cashloan should be inclusive of service charge and other taxes.
                            if ((bool)Country[CountryParameterNames.CL_Amortized])
                            {
                                var ttl = new CreditRepository().GetCashLoan(AccountNo);  
                                Total = Convert.ToDecimal(ttl.LoanAmount);
                            }
                            else
                            {
                                Total = (decimal)row["Agreement Total"];
                            }                                                          
                            footer.AppendChild(axml.CreateNode("TOTAL", Total.ToString(DecimalPlaces, LocalFormat)));
                            if ((decimal)Country[CountryParameterNames.PercentToPay] > 0)
                            {
                                ToPay = Total * ((decimal)Country[CountryParameterNames.PercentToPay] / 100);
                                footer.AppendChild(axml.CreateNode("TOPAY", ToPay.ToString(DecimalPlaces, LocalFormat)));
                            }
                            srvcharge = (decimal)row["Service Charge"];
                            footer.AppendChild(axml.CreateNode("DT", srvcharge.ToString(DecimalPlaces, LocalFormat)));
                            InstalNo = Convert.ToInt16(row["instalno"]) - 1;
                            InstalNo -= PaymentHolidaysMax;

                            //instaltot = Total + srvcharge;
                            instaltot = (decimal)row["Agreement Total"];
                            footer.AppendChild(axml.CreateNode("AMORTIZEDTOTAL", instaltot.ToString(DecimalPlaces, LocalFormat)));

                            // Instalments are either fixed or variable
                            string instalments = this.CreateInstalments(row);

                            footer.AppendChild(axml.CreateNode("FIRSTINST", ((decimal)row["Instalment Amount"]).ToString(DecimalPlaces, LocalFormat)));
                            footer.AppendChild(axml.CreateNode("FINALINST", ((decimal)row["Final Instalment"]).ToString(DecimalPlaces, LocalFormat)));
                            footer.AppendChild(axml.CreateNode("INSTALNO", InstalNo.ToString()));
                            footer.AppendChild(axml.CreateNode("INSTALMENTS", instalments));

                            DataTable InstPlan;

                            if ((bool)Country[CountryParameterNames.CL_Amortized])
                            {
                                //Prepare instalment plan to display in promissory note
                                InstPlan = this.CreateAmortizedCLScheduleTable();
                                int counter = 1;
                                foreach (DataRow CLSchedulerow in InstPlan.Rows)
                                {
                                    XmlNode InstalTable = footer.AppendChild(axml.CreateNode("INSTALTABLE", ""));
                                    InstalTable.AppendChild(axml.CreateNode("PAYMENTNUM", counter));
                                    InstalTable.AppendChild(axml.CreateNode("INSTALDATE", ((DateTime)CLSchedulerow["InstalDate"]).ToShortDateString()));
                                    InstalTable.AppendChild(axml.CreateNode("OPENINGBAL", ((decimal)CLSchedulerow["OpeningBalance"]).ToString(DecimalPlaces, LocalFormat)));
                                    //InstalTable.AppendChild(axml.CreateNode("INSTALAMT", ((decimal)CLSchedulerow["InstalAmt"]).ToString(DecimalPlaces, LocalFormat)));
                                    //if(((decimal)CLSchedulerow["InstalAmt"])> MonthlyAdminCharge)
                                    //{ 

									//InstalTable.AppendChild(axml.CreateNode("INSTALAMT", (((decimal)CLSchedulerow["InstalAmt"]) - MonthlyAdminCharge).ToString(DecimalPlaces, LocalFormat)));
									InstalTable.AppendChild(axml.CreateNode("INSTALAMT", (((decimal)CLSchedulerow["InstalAmt"])).ToString(DecimalPlaces, LocalFormat)));
									//} 
									//else
									//{ 
									//InstalTable.AppendChild(axml.CreateNode("INSTALAMT", (MonthlyAdminCharge - ((decimal)CLSchedulerow["InstalAmt"])).ToString(DecimalPlaces, LocalFormat)));
									//}
									InstalTable.AppendChild(axml.CreateNode("MONTHLYADMINAMT", MonthlyAdminCharge.ToString(DecimalPlaces, LocalFormat)));
									//InstalTable.AppendChild(axml.CreateNode("TOTALAMT", (MonthlyAdminCharge + (decimal)CLSchedulerow["InstalAmt"]).ToString(DecimalPlaces, LocalFormat)));
									//InstalTable.AppendChild(axml.CreateNode("TOTALAMT", ( (decimal)CLSchedulerow["InstalAmt"]).ToString(DecimalPlaces, LocalFormat)));
									InstalTable.AppendChild(axml.CreateNode("TOTALAMT", ((decimal)CLSchedulerow["InstalAmt"] + MonthlyAdminCharge).ToString(DecimalPlaces, LocalFormat)));
									InstalTable.AppendChild(axml.CreateNode("PRINCIPAL", ((decimal)CLSchedulerow["Principal"]).ToString(DecimalPlaces, LocalFormat)));
									InstalTable.AppendChild(axml.CreateNode("INTEREST", ((decimal)CLSchedulerow["Interest"]).ToString(DecimalPlaces, LocalFormat)));
									if ((decimal)CLSchedulerow["ClosingBalance"] > 0)
									{
										InstalTable.AppendChild(axml.CreateNode("CLOSINGBAL", ((decimal)CLSchedulerow["ClosingBalance"]).ToString(DecimalPlaces, LocalFormat)));
									}
									else
									{
										InstalTable.AppendChild(axml.CreateNode("CLOSINGBAL", 00));
									}
									InstalTable.AppendChild(axml.CreateNode("CLOSINGBAL", ((decimal)CLSchedulerow["ClosingBalance"]).ToString(DecimalPlaces, LocalFormat)));
									InstalTable.AppendChild(axml.CreateNode("CUMINT", ((decimal)CLSchedulerow["CumInterest"]).ToString(DecimalPlaces, LocalFormat)));
									counter += 1;
								}
							}
							else
							{
								
									DataTable dtMambus = paymentWebApi.GetMambuAccountsForPayment(null, AccountNo);
									if (dtMambus.Rows.Count > 0)
									{
										//string result = paymentWebApi.GetLoanSchedule(AccountNo);
										//if (result == "Success")
										//{
										InstPlan = this.PrintPromissonary();

										foreach (DataRow Planrow in InstPlan.Rows)
										{
											XmlNode InstalTable = footer.AppendChild(axml.CreateNode("INSTALTABLE", ""));
											// InstalTable.AppendChild(axml.CreateNode("INSTALMENTAMOUNT", ((decimal)(Convert.ToDecimal(Planrow["Amount"]).ToString()).ToString(DecimalPlaces, LocalFormat));
											InstalTable.AppendChild(axml.CreateNode("INSTALMENTAMOUNT", Convert.ToDecimal(Planrow["InstallmentAmount"]).ToString(DecimalPlaces, LocalFormat)));
											InstalTable.AppendChild(axml.CreateNode("INSTALMENTDATE", Planrow["Installmentdate"].ToString()));
										}




										//}
									}

								else
								{
									InstPlan = this.CreateInstalPlanTable(row);
									foreach (DataRow Planrow in InstPlan.Rows)
									{
										XmlNode InstalTable = footer.AppendChild(axml.CreateNode("INSTALTABLE", ""));
										// InstalTable.AppendChild(axml.CreateNode("INSTALMENTAMOUNT", ((decimal)(Convert.ToDecimal(Planrow["Amount"]).ToString()).ToString(DecimalPlaces, LocalFormat));
										InstalTable.AppendChild(axml.CreateNode("INSTALMENTAMOUNT", Convert.ToDecimal(Planrow["Amount"]).ToString(DecimalPlaces, LocalFormat)));
										InstalTable.AppendChild(axml.CreateNode("INSTALMENTDATE", Planrow["Due Date"].ToString()));
									}

								}
								
		                    }

                            agreementProcessed = true;
                        }
					}
				}
			}

			short ninetyDays = Convert.ToInt16(Country[CountryParameterNames.Print90]);
			if(TermsDescription.ToUpper().IndexOf("90 DAYS") != -1 && Convert.ToBoolean(ninetyDays))
				footer.AppendChild(axml.CreateNode("NINETYDAYS", (GoodsValue / 4).ToString(DecimalPlaces,LocalFormat)));

			if (AgreementText.Trim().Length > 0)
			{
				try
				{
					if (PaymentHolidays)
					{
						// Payment holiday text
						footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", String.Format(AgreementText, new Object[]{PaymentHolidaysMax, PaymentHolidaysMin})));
					}
					else
					{
						// Any other text
						footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", AgreementText));
					}
				}
				catch(System.FormatException)
				{
					/* users are able to maintain the text so we can't rely on it formating
					 * properly. If there is a formatting error just use the text without formatting */
					footer.AppendChild(axml.CreateNode("PAYMENTHOLIDAYS", AgreementText));
				}
			}
            
			footer.AppendChild(axml.CreateNode("GOODSVAL", GoodsValue.ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("CREDIT", (GoodsValue-Deposit).ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("BALANCE", (Total-Deposit).ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("INSURANCE", insuranceCharge.ToString(DecimalPlaces,LocalFormat)));

            if ((bool)Country[CountryParameterNames.CL_Amortized])
            {
                footer.AppendChild(axml.CreateNode("ANNUALINTERESTRATE", ServicePCent.ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                footer.AppendChild(axml.CreateNode("MONTHLYINTERESTRATE", (ServicePCent / 12).ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                footer.AppendChild(axml.CreateNode("ANNUALADMINRATE", AdminPCent.ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                footer.AppendChild(axml.CreateNode("MONTHLYADMINRATE", (AdminPCent / 12).ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
            }
            else
            {
                switch ((string)Country[CountryParameterNames.ServicePrint])
                {
                    case "A":
                        footer.AppendChild(axml.CreateNode("INTERESTRATE", ServicePCent.ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                        break;
                    case "M":
                        footer.AppendChild(axml.CreateNode("INTERESTRATE", CalculateMonthlyInterest(ServicePCent).ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                        break;
                    case "L":
                        footer.AppendChild(axml.CreateNode("INTERESTRATE", (ServicePCent / 12).ToString((string)Country[CountryParameterNames.ServicePrintDP]) + "%"));
                        break;
                    default:
                        break;
                }
            }

			footer.AppendChild(axml.CreateNode("SERVICEPRINT", (string)Country[CountryParameterNames.ServicePrint]));
			footer.AppendChild(axml.CreateNode("PCENTTOPAY", ((decimal)Country[CountryParameterNames.PercentToPay]).ToString()));

			footer.AppendChild(axml.CreateNode("PRETAXGOODSVAL", (GoodsValue - GoodsTax).ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("GOODSVALTAX", GoodsTax.ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("PRETAXDT", (srvcharge - DTTax).ToString(DecimalPlaces,LocalFormat)));
			footer.AppendChild(axml.CreateNode("DTTAX", DTTax.ToString(DecimalPlaces,LocalFormat)));

            //#19425 - CR18938 (Trinidad & Tobago)
            int earlySettlementPenaltyPeriod = int.Parse(Country[CountryParameterNames.CL_EarlySettPenaltyPeriod].ToString());
            footer.AppendChild(axml.CreateNode("EARLYSETTLEMENTPENALTYPERIOD",earlySettlementPenaltyPeriod));

            // 31/01/08 rdb
            decimal arm = CalculateAnnualPercentageRate(Total,srvcharge,Convert.ToDouble( InstalNo )+ 1);

            footer.AppendChild(axml.CreateNode("APR", decimal.Round(arm * 100, 2).ToString()));
			
			return footer;
		}

		private string CreateInstalments (DataRow agreementRow)
		{
			// Check for variable payments
			string instalments = "";

			// Load any variable instalments
			BInstalPlan bInstalPlan = new BInstalPlan();
			DataSet variableInstalPlanSet = bInstalPlan.GetVariableInstalmentsByAcctNo(AccountNo);
            
			foreach (DataTable variableInstalList in variableInstalPlanSet.Tables)
			{
				if (variableInstalList.TableName == TN.VariableInstal)
				{
					foreach (DataRow variableInstal in variableInstalList.Rows)
					{
						if (instalments == "")
							instalments = GetResource("M_AGREEMENTSUMMARYVARIABLE", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,LocalFormat),
																									 ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,LocalFormat)});
						else
							instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFOLLOW", new Object[] {((decimal)variableInstal[CN.InstalmentNumber]).ToString(DecimalPlaces,LocalFormat),
																												  ((decimal)variableInstal[CN.InstalAmount]).ToString(DecimalPlaces,LocalFormat)});
					}
					if (instalments != "")
						instalments += " " + GetResource("M_AGREEMENTSUMMARYVARIABLEFINAL", new Object[] {((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces,LocalFormat)});
				}
			}

			if (instalments == "")
			{
				// Default to the standard instalment text
				instalments = GetResource("M_AGREEMENTSUMMARY", new Object[] {InstalNo.ToString(),
																				 ((decimal)agreementRow["Instalment Amount"]).ToString(DecimalPlaces,LocalFormat),
																				 ((decimal)agreementRow["Final Instalment"]).ToString(DecimalPlaces,LocalFormat)});
			}

            bInstalPlan = null;

			return instalments;
		}

		private void AddLineItemByPage(CashLoanXML axml, XmlNode page, XmlNode item)
		{
			decimal quantity = 0;
			decimal itemValue = 0;
			decimal kitValue = 0;
			XmlUtilities xml = new XmlUtilities();

			if(	item.Attributes[Tags.Type].Value != IT.Kit &&
				item.Attributes[Tags.Code].Value != "DT" &&
				Convert.ToDouble(item.Attributes[Tags.Quantity].Value) != 0)
			{
				quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);
				itemValue = Convert.ToDecimal(item.Attributes[Tags.Value].Value);

				XmlNode node = page.SelectSingleNode("LINEITEMS");
				XmlNode itemNode = axml.Document.CreateElement("LINEITEM");
				XmlNode itemNoNode = axml.Document.CreateElement("ITEMNO");
				XmlNode qtyNode = axml.Document.CreateElement("QUANTITY");
				XmlNode priceNode = axml.Document.CreateElement("VALUE");
				XmlNode descNode = axml.Document.CreateElement("DESC");
				XmlNode desc2Node = axml.Document.CreateElement("DESC2");
				XmlNode trimNode = axml.Document.CreateElement("TRIM");
				XmlNode addToNode = axml.Document.CreateElement("ADDTO");
				XmlNode acctnoNode = axml.Document.CreateElement("ACCTNO");
				XmlNode preTaxPriceNode = axml.Document.CreateElement("PRETAXVALUE");
				XmlNode typeNode = axml.Document.CreateElement("TYPE");

				qtyNode.InnerText = quantity.ToString("F2");
				priceNode.InnerText = itemValue.ToString(DecimalPlaces,LocalFormat);
				descNode.InnerText = item.Attributes[Tags.Description1].Value;
				desc2Node.InnerText = item.Attributes[Tags.Description2].Value;
				trimNode.InnerText = item.Attributes[Tags.ColourTrim].Value;
				itemNoNode.InnerText = item.Attributes[Tags.Code].Value;
				addToNode.InnerText = item.Attributes[Tags.AddTo].Value;
				acctnoNode.InnerText = item.Attributes["ACCTNO"].Value;
				typeNode.InnerText = "";
				
				if((string)Country[CountryParameterNames.TaxType] == "E")
					preTaxPriceNode.InnerText = priceNode.InnerText;
				else
					preTaxPriceNode.InnerText = (itemValue - Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value)).ToString(DecimalPlaces,LocalFormat);

				itemNode.AppendChild(qtyNode);
				itemNode.AppendChild(priceNode);
				itemNode.AppendChild(descNode);
				itemNode.AppendChild(desc2Node);
				itemNode.AppendChild(trimNode);
				itemNode.AppendChild(itemNoNode);
				itemNode.AppendChild(addToNode);
				itemNode.AppendChild(acctnoNode);
				itemNode.AppendChild(preTaxPriceNode);
				itemNode.AppendChild(typeNode);
				node.AppendChild(itemNode);
			
				if(item.Attributes[Tags.AddTo].Value==Boolean.FalseString)
				{
					GoodsValue += itemValue;			
					GoodsTax += Convert.ToDecimal(item.Attributes[Tags.TaxAmount].Value);
				}
			}
			else
			{
				if(item.Attributes[Tags.Type].Value == IT.Kit &&
					Convert.ToDouble(item.Attributes[Tags.Quantity].Value) != 0)
				{
					XmlNode related = item.SelectSingleNode(Elements.RelatedItem);
					foreach(XmlNode comp in related.ChildNodes)
					{
						if(comp.Attributes[Tags.Type].Value != IT.KitWarranty)
						{
							kitValue += Convert.ToDecimal(comp.Attributes[Tags.Value].Value);
							
							if(comp.Attributes[Tags.AddTo].Value==Boolean.FalseString)
								GoodsValue += Convert.ToDecimal(comp.Attributes[Tags.Value].Value);
						}
					}

					quantity = Convert.ToDecimal(item.Attributes[Tags.Quantity].Value);

					XmlNode node = page.SelectSingleNode("LINEITEMS");
					XmlNode itemNode = axml.Document.CreateElement("LINEITEM");
					XmlNode qtyNode = axml.Document.CreateElement("QUANTITY");
					XmlNode priceNode = axml.Document.CreateElement("VALUE");
					XmlNode descNode = axml.Document.CreateElement("DESC");
					XmlNode itemNoNode = axml.Document.CreateElement("ITEMNO");
					XmlNode addToNode = axml.Document.CreateElement("ADDTO");
					XmlNode acctnoNode = axml.Document.CreateElement("ACCTNO");
					XmlNode typeNode = axml.Document.CreateElement("TYPE");

					typeNode.InnerText = item.Attributes[Tags.Type].Value;
					qtyNode.InnerText = quantity.ToString("F2");
					descNode.InnerText = item.Attributes[Tags.Description1].Value;
					itemNoNode.InnerText = item.Attributes[Tags.Code].Value;
					priceNode.InnerText = kitValue.ToString("C2",LocalFormat);
					addToNode.InnerText = item.Attributes[Tags.AddTo].Value;
					acctnoNode.InnerText = item.Attributes["ACCTNO"].Value;

					itemNode.AppendChild(qtyNode);
					itemNode.AppendChild(priceNode);
					itemNode.AppendChild(descNode);
					itemNode.AppendChild(itemNoNode);
					itemNode.AppendChild(addToNode);
					itemNode.AppendChild(acctnoNode);
					itemNode.AppendChild(typeNode);

					node.AppendChild(itemNode);

					foreach(XmlNode comp in related.ChildNodes)
					{
						if(comp.Attributes[Tags.Type].Value != IT.KitWarranty)
						{
							decimal qty = Convert.ToDecimal(comp.Attributes[Tags.Quantity].Value);
							
							XmlNode compNode = page.SelectSingleNode("LINEITEMS");
							XmlNode compItemNode = axml.Document.CreateElement("LINEITEM");
							XmlNode compTypeNode = axml.Document.CreateElement("TYPE");
							XmlNode compQtyNode = axml.Document.CreateElement("QUANTITY");
							XmlNode compItemNoNode = axml.Document.CreateElement("ITEMNO");
							XmlNode compDescNode = axml.Document.CreateElement("DESC");
							XmlNode compDescNode2 = axml.Document.CreateElement("DESC2");
							
							compTypeNode.InnerText = comp.Attributes[Tags.Type].Value;
							compQtyNode.InnerText = qty.ToString("F2");
							compItemNoNode.InnerText = comp.Attributes[Tags.Code].Value;
							compDescNode.InnerText = comp.Attributes[Tags.Description1].Value;
							compDescNode2.InnerText = comp.Attributes[Tags.Description2].Value;
							
							compItemNode.AppendChild(compQtyNode);
							compItemNode.AppendChild(compItemNoNode);
							compItemNode.AppendChild(compDescNode);
							compItemNode.AppendChild(compDescNode2);

							compNode.AppendChild(compItemNode);
						}
					}
				}
			}
		}

		private void FlagItems(XmlNode item, string flag, string accountNo)
		{
			item.Attributes.Append(item.OwnerDocument.CreateAttribute(Tags.AddTo));
			item.Attributes[Tags.AddTo].Value = flag;
			item.Attributes.Append(item.OwnerDocument.CreateAttribute("ACCTNO"));
			item.Attributes["ACCTNO"].Value = accountNo;
			foreach(XmlNode child in item.SelectSingleNode(Elements.RelatedItem).ChildNodes)
				FlagItems(child, flag, accountNo);
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				Function = "PrintCashLoan";
				CustomerID = Request["customerID"];
				AccountNo = Request["accountNo"];
				AccountType = Request["accountType"];
				Culture = Request["culture"];
				CountryCode = Request["countryCode"];
				Multiple = Convert.ToBoolean(Request["multiple"]);
                printBranch = int.Parse(Request["branch"]);             //#19425 - CR18938
				
				try
				{
					itemsPerPage = Convert.ToDouble(ConfigurationSettings.AppSettings["agreementLineItems"]);
				}
				catch(Exception)
				{
					/* don't want to fail if the config key is not set, just use default */
				}

				#region Retrieve all the required data
				//Set the culture for currency formatting
				Thread.CurrentThread.CurrentCulture = new CultureInfo(Culture);

				//Set printer display options for numbers and currency
				LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
				LocalFormat.CurrencySymbol = (string)Country[CountryParameterNames.CurrencySymbolForPrint];

				bCustomer = new BCustomer();
				dCustomer = bCustomer.GetBasicCustomerDetails(null, null,CustomerID, AccountNo, "H");

				/* retrieve the service percentage */
				bAccount = new BAccount();
				bAccount.Populate(AccountNo);

				/* if there is a joint applicant for credit, include their name */
				if(AT.IsCreditType(AccountType))
				//if (AccountType != AT.Cash && AccountType != AT.Special) //Acct Type Translation DSR 29/9/03
				{
                    JointName = bAccount.GetApplicantTwoName(CustomerID, AccountNo, out JointID, out relationship);
					if(JointName.Length > 0)
						dJoint = bCustomer.GetDetails(JointID);
				}

				dTermsType = new DTermsType();
                dTermsType.GetTermsTypeDetail(null, null, CountryCode, bAccount.TermsType, bAccount.AccountNumber, "", bAccount.DateAccountOpen);

				ServicePCent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["servpcent"]);
                AdminPCent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0][CN.AdminPcent]);
               AdminValue = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["AdminValue"]);//kedar
                Maxterm = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0]["Maxterm"]);
                CashLoan loan1 = new CreditRepository().GetCashLoan(AccountNo);

                if ((bool)Country[CountryParameterNames.CL_Amortized])
                {
                     if ((bool)(Country[CountryParameterNames.CL_TaxRateApplied]))
                    {
                        decimal ln = loan1.LoanAmount == null ? 0 : Convert.ToDecimal(loan1.LoanAmount);
                        Adminchg = (AdminPCent / 100) * (Maxterm / 12) * ln + AdminValue;
                        // MonthlyAdminCharge = (((AdminPCent / 100) * bAccount.AgreementTotal) + AdminValue) / 12;
                        MonthlyAdminCharge = (Adminchg + (Adminchg * Convert.ToDecimal(Country[CountryParameterNames.TaxRate]) / 100))/Maxterm;
                        //* Convert.ToDecimal(Country[CountryParameterNames.TaxRate])/100)/12);
                    }
                    else
                    {
                        //Flag for taxrate on admin charge when off must print monthly admin fee on Promissory note
                        decimal ln = loan1.LoanAmount == null ? 0 : Convert.ToDecimal(loan1.LoanAmount);
                        MonthlyAdminCharge = ((AdminPCent / 100) * (Maxterm / 12) * ln + AdminValue) / Maxterm;
                       // MonthlyAdminCharge = ((AdminPCent / 100)*(Maxterm/12) * bAccount.ClAmt + AdminValue) / Maxterm;
                    }
                }
                TermsDescription = (string)dTermsType.TermsTypeDetails.Rows[0][CN.Description];				
				PaymentHolidays = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHoliday]);
				PaymentHolidaysMin = (short)dTermsType.TermsTypeDetails.Rows[0][CN.PaymentHolidayMin];
				AgreementText = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementText];				
				insPcent = Convert.ToDecimal(dTermsType.TermsTypeDetails.Rows[0][CN.InsPcent]);
				insIncluded = Convert.ToBoolean(dTermsType.TermsTypeDetails.Rows[0][CN.InsIncluded]);
                //string xsltFilename = (string)dTermsType.TermsTypeDetails.Rows[0][CN.AgreementPrintDesc];

                string xsltFilename = string.Empty;
                if ((bool)Country[CountryParameterNames.CL_Amortized])
                    xsltFilename = "AmortizedCashLoan.xslt";
                else
                    xsltFilename = "CashLoan.xslt";


				/* load the agreement table stuff */
				BAgreement bAgreement = new BAgreement();
				DataTable dtMambu = paymentWebApi.GetMambuAccountsForPayment(null, AccountNo);
				if (dtMambu.Rows.Count > 0)
				{
					string result = paymentWebApi.GetLoanSchedule(AccountNo);
					if (result == "Success")
					{
						dAgreement = bAgreement.GetAgreement(null, null, AccountNo, 1, false);
					}
				}
				else
				{
					dAgreement = bAgreement.GetAgreement(null, null, AccountNo, 1, false); //IP - 11/02/11 - Sprint 5.10 - #2978 - added null, null for conn, trans
				}
				PaymentHolidaysMax = (short)dAgreement.Tables[TN.Agreements].Rows[0][CN.PaymentHoliday];

				/* load the line items for the account */
				bItem = new BItem();
				DAccount dAccount = new DAccount();

				if(dAccount.IsWarrantyRenewal(AccountNo))
					lineItems = bItem.GetWarrantyRenewalItems(AccountNo, AccountType, CountryCode, 1);
				else
					lineItems = bItem.GetLineItems(null, null, AccountNo, AccountType, CountryCode, 1);

				#endregion

                //CR903 - Lucky Dollar Stores will need a seperate
                //stylesheet displaying the Lucky Dollar logo.
                DBranch branch = new DBranch();
                string acctNo = AccountNo.Substring(0, 3);
                string storeType = branch.GetStoreType(Convert.ToInt16(acctNo));

                //if (storeType == StoreType.NonCourts)
                //{
                //    xsltFilename = "Agreement_LD.xslt";
                //}

                //CR 440 - Additional parameter 'xsltFilename' to AgreementXML constructor call
                CashLoanXML axml = new CashLoanXML(CountryCode, xsltFilename);
				axml.Load("<AGREEMENT><CUSTOMER/><LAST/></AGREEMENT>");

				/* first thing we need to know is how many pages we need, which 
				 * depends on how many line items */
				double numItems = 0;				
				XmlNode[] addToItems = null;

				/* flag all lineitems as not belonging to an added on account */
				foreach(XmlNode i in lineItems.ChildNodes)
					FlagItems(i, Boolean.FalseString, AccountNo);

				/* if this account has been added to */
				if(lineItems.SelectSingleNode("//Item[@Quantity != '0' and @Code = 'ADDDR']")!=null)
				{
					DataTable addedToAccts = bAccount.GetAccountsAddedTo(null, null, AccountNo);
					addToItems = new XmlNode[addedToAccts.Rows.Count];
					for(int x=0; x<addedToAccts.Rows.Count; x++)
						addToItems[x] = bItem.GetLineItems(null, null, (string)addedToAccts.Rows[x][CN.AccountNumber], AccountType, CountryCode, 1);

					/* need to try and combine the lineitem nodes */
					XmlNode addcr = lineItems.SelectSingleNode("//Item[@Quantity != '0' and @Code = 'ADDDR']");
					addcr.Attributes[Tags.Description1].Value = GetResource("M_BALANCEFROMPREV");
					for(int i=0; i<addToItems.Length; i++)
					{
						for(int a=0; a<addToItems[i].ChildNodes.Count; a++)
						{
							XmlNode addNode = addToItems[i].ChildNodes[a];
							if(addNode.Attributes[Tags.Code].Value != "ADDCR")
							{
								addNode = lineItems.OwnerDocument.ImportNode(addNode, true);
								FlagItems(addNode, Boolean.TrueString, (string)addedToAccts.Rows[i][CN.AccountNumber]);	/* flag as add tos */
								lineItems.InsertAfter(addNode, addcr);
							}
						}
					}
				}

				/* extract the tax on the DT */
                if(!(bool)Country[CountryParameterNames.CL_Amortized])
				    DTTax = Convert.ToDecimal(lineItems.SelectSingleNode("//Item[@Code='DT']").Attributes[Tags.TaxAmount].Value);

				XmlNodeList allLineItems = lineItems.SelectNodes("//Item[@Code!='DT' and @Quantity!='0' and @Type != 'KitDiscount' and @Type != 'Component']");
				if(allLineItems!=null)
					numItems = allLineItems.Count;

				Pages = Math.Ceiling(numItems / itemsPerPage);
				Pages = Pages==0?1:Pages;

				for (int i=0; i<Pages; i++)
				{
					XmlNode page = axml.Document.CreateElement("PAGE");
					if(i==Pages-1)
						page.AppendChild(axml.CreateNode("LAST", Boolean.TrueString));
					else
						page.AppendChild(axml.CreateNode("LAST", Boolean.FalseString));
					page.AppendChild(CreateHeader(axml));
					page.AppendChild(axml.Document.CreateElement("LINEITEMS"));
					axml.GetNode("AGREEMENT").AppendChild(page);
				}				
		
				XmlNodeList pages = axml.Document.SelectNodes("//PAGE");
				if(allLineItems!=null)
				{
					for(double i=1; i<=allLineItems.Count; i++)
					{
						double pageIndex = Math.Ceiling(Convert.ToDouble(i / itemsPerPage));
						XmlNode item = allLineItems[Convert.ToInt32(i)-1];
						AddLineItemByPage(axml, pages[Convert.ToInt32(pageIndex)-1], item);
					}
					
					GetInsuranceCharge(bAgreement.Deposit, allLineItems); 
				}
			
				axml.GetNode("AGREEMENT").AppendChild(CreateFooter(axml));
				
                int noCopies = 0;
				if(Multiple)
					noCopies = Convert.ToInt32(Country[CountryParameterNames.NoAgrCopies]);
				else
					noCopies = 1;

				if(JointName.Length > 0)
					noCopies += 1;

				int custCopies = Convert.ToInt32(Country[CountryParameterNames.AgreementCustomerCopies]);
				
				axml.CreateCopies(noCopies, custCopies);

                bCustomer = null;
                bAccount = null;
                bAgreement = null;
                bItem = null;
                dAccount = null;
                dTermsType = null;
                branch = null;
                GC.Collect();

                Response.Write(axml.Transform());
				//Response.Write(axml.TransformXml());
				this.DeleteAccountInstallment(AccountNo);
			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

        // 31/01/08 rdb
        private decimal CalculateAnnualPercentageRate(decimal agrmtTotal, decimal serviceCharge, double monthsToPay)
        {
            double yearToPayRoundedUp = Math.Floor((monthsToPay / 12) + 0.5);
            double monthsInFinalYear = monthsToPay - (12 * (yearToPayRoundedUp - 1));

            decimal installAmmount = agrmtTotal / Convert.ToDecimal(monthsToPay);
            installAmmount = decimal.Floor(installAmmount + 0.5M);
            decimal finalInstallAmount = agrmtTotal - (installAmmount * (Convert.ToDecimal(monthsToPay) - 1));

            decimal totalBalance = agrmtTotal - serviceCharge;
            // guesstimated value
            decimal annualPercentRate = 0.05M;
            decimal Diff = 0.001M;


            decimal outstandingBal;
            do
            {
                outstandingBal = totalBalance;

                for (int i = 0; i < (yearToPayRoundedUp - 1); i++)
                {
                    outstandingBal += (-(installAmmount * 12) + (outstandingBal * annualPercentRate));
                }

                outstandingBal += (-((installAmmount * Convert.ToDecimal(monthsInFinalYear - 1)) + finalInstallAmount)
                                + (outstandingBal * annualPercentRate * Convert.ToDecimal(monthsInFinalYear / 12)));


                if ((outstandingBal > 0 && Diff > 0) || (outstandingBal < 0 && Diff < 0))
                {
                    Diff = -Diff / 2;
                }

                annualPercentRate += Diff;

            } while (decimal.Round(outstandingBal, 2) != 0);

            return annualPercentRate;
        }

		private void GetInsuranceCharge(decimal deposit, XmlNodeList allItems)
		{
			DAccount acct = new DAccount();
			acct.SelectType(null, null, AccountType, CountryCode, out monthsintfree, out monthsdeferred);

			BItem item = new BItem();
			
			DInstalPlan ip = new DInstalPlan();
			ip.Populate(null, null, AccountNo, 1);
			decimal months = ip.NumberOfInstalments += monthsdeferred - monthsintfree;

			chargeablePrice = bAccount.GetChargeableCashPrice(null, null, AccountNo, ref chargeableAdminPrice);

			if(insIncluded)
				insuranceCharge = (chargeablePrice-deposit) * insPcent * (months/12);
			else
				foreach(XmlNode node in allItems)
					if(node.Attributes[Tags.Code].Value == (string)Country[CountryParameterNames.InsuranceChargeItem])
						insuranceCharge = Convert.ToDecimal(node.Attributes[Tags.Quantity].Value) * Convert.ToDecimal(node.Attributes[Tags.UnitPrice].Value);

            acct = null;
            item = null;
            ip = null;
		}

        private DataTable CreateInstalPlanTable(DataRow row)
        {
            DataTable InstalTable = new DataTable();
            int i;
            DataRow newRow;
            var lastDueDate = new DateTime();
            InstalTable.Columns.Add("Due Date");
            InstalTable.Columns.Add("Amount");

            DateTime DueDate = Convert.ToDateTime(row["Date First Instalment"] ?? DateTime.Today.AddMonths(1));

            //IP - 25/10/11 - CR1232 - If printing through Cash Loan screen and account not delivered then use todays date else use Instalplan.datefirst
            if (DueDate == new DateTime(1900, 01, 01))
            {
                //#13779 - align due date
                DInstalPlan inst = new DInstalPlan();

                inst.GetDueDay(this.CustomerID);
                var dueDay = inst.DueDay;
                
                //If printing before disbursement, assume disbursement will be today.
                var datefirst = new AccountRepository().ReturnDateFirst(AccountNo, DateTime.Today);

                if (datefirst.HasValue)
                {
                    DueDate = datefirst.Value;
                }
                else
                {
                    DueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, dueDay).AddMonths(1);
                }
           

                //DueDate = DateTime.Today.AddMonths(1);
            }
            // This should be the way if the due day is used in any calculation (for future reference)
            //newRow["Due Date"] = row["Date First Instalment"];
            //newRow["Amount"] = row["Instalment Amount"];

                
            for (i = 0; i < (Convert.ToInt16(row["instalno"]) - 1);i++)
            {   
                newRow = InstalTable.NewRow();

                newRow["Due Date"] = DueDate.AddMonths(i).ToShortDateString();
                newRow["Amount"] = row["Instalment Amount"];
                InstalTable.Rows.Add(newRow);


                lastDueDate = Convert.ToDateTime(newRow["Due Date"]); //IP - 25/10/11 - CR1232 - Save last date to add a month to get final instalment 
            }

 
            newRow = InstalTable.NewRow();

            //IP - 25/10/11 - CR1232 - If printing through Cash Loan screen and account not delivered then use todays date else use Instalplan.datelast
            if (((DateTime)row["Date Last Instalment"]) == new DateTime(1900, 01, 01))      
            {
                newRow["Due Date"] = lastDueDate.AddMonths(1).ToShortDateString(); 
            }
            else
            {
                newRow["Due Date"] = ((DateTime)row["Date Last Instalment"]).ToShortDateString();
            }

            newRow["Amount"] = row["Final Instalment"];
            InstalTable.Rows.Add(newRow);
            return (InstalTable);
        }

        //Get amortized cash loan schedule to print promissory note
        private DataTable CreateAmortizedCLScheduleTable()
        {
            DInstalPlan inst = new DInstalPlan();
            inst.GetAmortizedCLSchedule(AccountNo, InstalNo);
            return inst.AmortizedCLSchedule;
        }
		//Arun
		private DataTable PrintPromissonary()
		{
			DInstalPlan inst = new DInstalPlan();
			inst.PrintPromisonaryNote(AccountNo);
			return inst.Printpromissonary;
		}
		//Arun
		private void DeleteAccountInstallment(string AccountNo)
		{
			DInstalPlan inst = new DInstalPlan();
			inst.DeleteAccountInstallment(AccountNo);

		}
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
