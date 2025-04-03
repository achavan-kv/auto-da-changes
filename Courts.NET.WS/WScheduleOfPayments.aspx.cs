using System;
using System.Data;
using System.Xml;
using STL.BLL;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.Tags;
using STL.DAL;

namespace STL.WS
{
	public partial class WScheduleOfPayments : CommonWebPage
	{
		XmlNode scheduleItems = null;
		XmlNode scheduleItem = null;
		XmlNode instalments = null;
		XmlNode instalment = null;
		decimal totalCharges = 0;
		decimal totalCapital = 0;
		DateTime dateFirst;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{

				string customerID = Request[CN.CustomerID];
				string accountNo = Request[CN.AccountNumber];
				string culture = Request[CN.Culture];
				string countryCode = Request["countryCode"];
				decimal servicePcent = 0;
				string apr = "";
				string accountType = Request[CN.AccountType];
				int monthsInterestFree = 0;
				int monthsDeferred = 0;
				int oldMonthsInterestFree = 0;
				int oldMonthsDeferred = 0;
				decimal totalInstal = 0;
                string dealerName = Convert.ToString(Country[CountryParameterNames.CourtsDealerName]);
                
				//Set the culture for currency formatting
				//Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
                base.SetCulture();
				
				ScheduleOfPaymentsXML pxml = new ScheduleOfPaymentsXML(countryCode);
				pxml.Load();

				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/DATE", DateTime.Today.ToShortDateString());
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/ACCTNO", accountNo);
                pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/DEALERNAME", dealerName);
                


				#region retrieve required data
				BCustomer customer = new BCustomer();
				DataSet custDetails = customer.GetBasicCustomerDetails(null, null, customerID, accountNo, "H");

				DInstalPlan ip = new DInstalPlan();
				ip.Populate(null, null, accountNo, 1);

				DAccount acct = new DAccount(null, null, accountNo);

				BItem item = new BItem();
				XmlNode lineItems = item.GetLineItems(null, null, accountNo, accountType, countryCode, 1);
				
				DAgreement agree = new DAgreement(null, null, accountNo, 1);

				DTermsType terms = new DTermsType();
                terms.GetTermsTypeDetail(null, null, countryCode, acct.TermsType, acct.AccountNumber, "", acct.DateAccountOpen);
				servicePcent = Convert.ToDecimal(terms.TermsTypeDetails.Rows[0]["servpcent"]);
				apr = (string)terms.TermsTypeDetails.Rows[0][CN.APR];
			
				// Old accounts will have old account types with their own months int free and deferred
				acct.SelectType(null, null, accountType, countryCode, out oldMonthsInterestFree, out oldMonthsDeferred); 
				if (oldMonthsInterestFree != 0 || oldMonthsDeferred != 0)
				{
					// Use the values from the old account type
					monthsInterestFree = oldMonthsInterestFree;
					monthsDeferred = oldMonthsDeferred;
				}
				else 
				{
					if (terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree] != DBNull.Value) 
					{
						monthsInterestFree = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.MonthsIntFree]);
					}
					if (terms.TermsTypeDetails.Rows[0][CN.DeferredMonths] != DBNull.Value) 
					{
						monthsDeferred = Convert.ToInt32(terms.TermsTypeDetails.Rows[0][CN.DeferredMonths]);
					}
				}

				#endregion

				dateFirst = agree.AgreementDate.AddMonths(1).AddDays(7);

				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/NOINSTALMENTS", ip.NumberOfInstalments.ToString());
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/APR", apr);
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/CHARGEABLEPRICE", (agree.AgreementTotal - agree.ServiceCharge - agree.Deposit).ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/BALANCE", (acct.OutstandingBalance).ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/TOTAL", (agree.AgreementTotal).ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/DEPOSIT", (agree.Deposit).ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/AGRDATE", agree.AgreementDate.ToShortDateString());

				scheduleItems = pxml.GetNode("SCHEDULEOFPAYMENTS/HEADER/LINEITEMS");
				scheduleItem = scheduleItems.RemoveChild(scheduleItems.SelectSingleNode("LINEITEM"));

                AddLineItems(lineItems.ChildNodes);	
			
				ip.DateFirst = dateFirst;
				ip.DateLast = dateFirst.AddMonths(ip.NumberOfInstalments - 1);

				DateTime firstPaymentDate = dateFirst.AddMonths(monthsDeferred);
				DateTime lastPaymentDate = firstPaymentDate.AddMonths(ip.NumberOfInstalments - 1);

				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/FIRSTDATE", firstPaymentDate.ToShortDateString());
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/LASTDATE", lastPaymentDate.ToShortDateString());

				foreach(DataTable dt in custDetails.Tables)
				{
					switch(dt.TableName)
					{
						case "BasicDetails":	
							LoadCustDetails(dt, pxml);
							break;
						default:
							break;
					}
				}

				instalments = pxml.GetNode("SCHEDULEOFPAYMENTS/INSTALMENTS");
				instalment = instalments.RemoveChild(instalments.SelectSingleNode("INSTALMENT"));

				CreateSchedule(ip, agree.AgreementTotal - agree.ServiceCharge - agree.Deposit, servicePcent, monthsInterestFree, monthsDeferred, out totalInstal);

				pxml.SetNode("SCHEDULEOFPAYMENTS/FOOTER/TOTAL", totalInstal.ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/FOOTER/CHARGETOTAL", totalCharges.ToString(DecimalPlaces));
				pxml.SetNode("SCHEDULEOFPAYMENTS/FOOTER/CAPITALTOTAL", totalCapital.ToString(DecimalPlaces));

//#if(XMLTRACE)
//				logMessage(pxml.Xml, User.Identity.Name, EventLogEntryType.Information);
//#endif

				Response.Write(pxml.Transform());
				//Response.Write(pxml.Xml);

			}
			catch(Exception ex)
			{
				logException(ex, Function);
				Response.Write(ex.Message);
			}
		}

		private decimal MprCalc(decimal mpr, 
								decimal instalAmount, 
								decimal finalInstal, 
								decimal instalNo, 
								decimal monthsDeferred, 
								decimal monthsInterestFree, 
								decimal balance)
		{
			decimal diff = 0.001M;	
			mpr /= 100;

			for(int j=0; j<500; j++)
			{
				decimal bal = balance;

				for(int i=1; i<=instalNo; i++)
				{
					if(i<=monthsDeferred)			/* payment deferred balance increases */
					{
						if(i>monthsInterestFree)	/* balance will remain unchanged if deferred AND interest free */
							bal += (bal * mpr);
					}
					else
					{
						/* final payment */
						if(i==instalNo)
							if(i>monthsInterestFree)
								bal += (bal * mpr) - finalInstal;
							else
								bal -= finalInstal;
					
						/* normal payment */
						if(i!=instalNo)
							if(i>monthsInterestFree)
								bal += (bal * mpr) - instalAmount;
							else
								bal -= instalAmount;
					}
				}

				if( bal < 0 )	/* mpr too high */
					if( diff < 0 )
						diff = -diff / 2;

				if( bal > 0 )	/* mpr too low */
					if( diff > 0 )
						diff = -diff / 2;

				mpr += diff;
			}

			return mpr * 100;
		}

		private void CreateSchedule(DInstalPlan ip, 
									decimal openingBalance, 
									decimal servicePcent,
									int monthsInterestFree,
									int monthsDeferred,
									out decimal totalInstalments)
		{		
			decimal capital = 0;
			decimal zeroValue = 0;
			decimal deferredInstalments = 0;
			decimal months = ip.NumberOfInstalments + monthsDeferred;

			totalInstalments = 0;
			decimal mpr = MprCalc( servicePcent / 12, ip.InstalmentAmount, ip.FinalInstalment, months, monthsDeferred, monthsInterestFree, openingBalance);


			for(int x=1; x <= months; x++)
			{
				XmlNode n = instalment.CloneNode(true);
				n.SelectSingleNode("NO").InnerText = Convert.ToString(x-monthsDeferred);
				n.SelectSingleNode("DUEDATE").InnerText = ip.DateFirst.AddMonths(x-1).ToShortDateString();
				n.SelectSingleNode("MONTHLYINSTALMENT").InnerText = x == months ? ip.FinalInstalment.ToString(DecimalPlaces) : ip.InstalmentAmount.ToString(DecimalPlaces);
				n.SelectSingleNode("OPENINGBALANCE").InnerText = openingBalance.ToString(DecimalPlaces);
				
				decimal charge = openingBalance * mpr / 100;

				if(x <= monthsDeferred)
				{
					//payment deferred, balance increases by charge amount
					if(x > monthsInterestFree)
					{
						openingBalance += charge;		
						capital = charge;
					}
					//payment deferred AND intereset free, so balance will remain the same
					else
					{
						charge = 0;	
						capital = charge;
					}

					//monthly instalment is 0 for each dererred month
					deferredInstalments += ip.InstalmentAmount;
					n.SelectSingleNode("NO").InnerText = "D";
					n.SelectSingleNode("MONTHLYINSTALMENT").InnerText = zeroValue.ToString(DecimalPlaces);
				}
				else
				{
					if(x > monthsInterestFree)
					{
						capital = x == months ? ip.FinalInstalment - charge : ip.InstalmentAmount - charge;
						openingBalance -= capital;
					}
					//interest free, so no charge will be deducted from instalment
					else
					{
						capital = x == months ? ip.FinalInstalment : ip.InstalmentAmount;
						openingBalance -= capital;
						charge = 0;
					}
				}

				totalCharges += charge;
				
				if(x <= monthsDeferred)
					totalCapital += -capital;
				else
					totalCapital += capital;

				n.SelectSingleNode("MONTHLYCHARGE").InnerText = charge.ToString(DecimalPlaces);
				n.SelectSingleNode("MONTHLYCAPITAL").InnerText = capital.ToString(DecimalPlaces);
				n.SelectSingleNode("CLOSINGBALANCE").InnerText = openingBalance.ToString(DecimalPlaces);
				instalments.AppendChild(n);

				if(x == months)
					totalInstalments += ip.FinalInstalment;
				else
					totalInstalments += ip.InstalmentAmount;
			}

			//reduce total instalments by the value of deferred instalments
			totalInstalments -= deferredInstalments;
		}

		private void AddLineItems(XmlNodeList related)
		{
			foreach(XmlNode item in related)
			{
				if(item.Attributes[Tags.Code].Value!="DT" &&
					item.Attributes[Tags.Code].Value!="STAX" &&
					item.Attributes[Tags.Quantity].Value!="0" )
				{
					XmlNode i = scheduleItem.CloneNode(true);
					i.SelectSingleNode("ITEMNO").InnerText = item.Attributes[Tags.Code].Value;

                    //IP - 22/09/11 - RI - #8223 - CR8201
                    if (Convert.ToString(Country[CountryParameterNames.RIInterfaceOptions]) != "FACT")
                    {
                        i.SelectSingleNode("DESCR").InnerText = item.Attributes[Tags.Description1].Value + " " + item.Attributes[Tags.Brand].Value + " " + item.Attributes[Tags.Style].Value;
                    }
                    else
                    {
                        i.SelectSingleNode("DESCR").InnerText = item.Attributes[Tags.Description1].Value;
                    }

					scheduleItems.AppendChild(i);

					DateTime plannedDel = Convert.ToDateTime(item.Attributes[Tags.PlannedDeliveryDate].Value);
					if(	plannedDel.Date != DateTime.MinValue.AddYears(1899).Date )
						dateFirst = plannedDel.AddMonths(1);

					if(item.SelectSingleNode(Tags.RelatedItems).ChildNodes.Count>0)
						AddLineItems(item.SelectSingleNode(Tags.RelatedItems).ChildNodes);
				}
			}
		}

		private void LoadCustDetails(DataTable dt, ScheduleOfPaymentsXML pxml)
		{
			foreach(DataRow row in dt.Rows)
			{
				pxml.SetNode("SCHEDULEOFPAYMENTS/HEADER/CUSTOMERNAME", (string)row[CN.Title]+" "+ (string)row[CN.FirstName]+" "+ (string)row[CN.LastName]);				
			}
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
