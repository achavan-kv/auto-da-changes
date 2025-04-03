using System;
using STL.Common;
using System.Diagnostics;
using System.Configuration;
using System.Xml;
using System.IO;
using System.Xml.Schema;
using STL.BLL.CreditBureau;
using System.Xml.Xsl;
using System.Data;
using System.Data.SqlClient;
using STL.DAL;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;


namespace STL.BLL
{
	/// <summary>
	/// This class will manage interaction with the Credit Bureau via 
	/// an MQSeries client interface. 
	/// 
	/// This contains no exception handling. Where error conditions exists
	/// MQExceptions will be raised automatically and passed on to the 
	/// calling layer.
	/// </summary>
	public class BCreditBureau : CommonObject
	{
		private ICreditBureauRequest bureauRequest = null;
		private ICreditBureauResponse bureauResponse = null;
		private CreditBureauMessenger bureauMessenger = null;

		/// <summary>
		/// Creates a CreditBureauRequest for the requested service, validates it
		/// then sends it to the bureau. This is synchronous request and will 
		/// therefore wait for a response from the bureau.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public void ConsumerEnquiry(	SqlConnection conn, 
										SqlTransaction trans, 
										string accountNo, 
										string customerID )
		{
			int numberOfConsumers = 1;		/* hardcoded for now */
			BAccount acct = new BAccount();
			acct.Populate(conn, trans, accountNo);

			BCustomer cust = new BCustomer();

			bureauRequest = new ConsumerEnquiryRequest();
			bureauResponse = new ConsumerEnquiryResponse();
			((ConsumerEnquiryRequest)bureauRequest).SERVICE = CreditBureauService.ConsumerEnquiryRequest;
			((ConsumerEnquiryRequest)bureauRequest).ACTION = CreditBureauAction.ConsumerEnquiryRequest;
			((ConsumerEnquiryRequest)bureauRequest).ACCOUNT_TYPE = "HPCON";
			((ConsumerEnquiryRequest)bureauRequest).GUARANTOR = "NO";			
			((ConsumerEnquiryRequest)bureauRequest).IPIENQTYP = "RV";
			((ConsumerEnquiryRequest)bureauRequest).NUM_ENQUIRIES = 1;
			((ConsumerEnquiryRequest)bureauRequest).CLIENT_REFERENCE = "12345";
			((ConsumerEnquiryRequest)bureauRequest).CONSUMER_AMOUNT = acct.AgreementTotal;

			for(int i=0; i<numberOfConsumers; i++)
			{
				DataSet ds = cust.GetBasicCustomerDetails(conn, trans, customerID, accountNo, "H");
				foreach(DataTable dt in ds.Tables)
				{
					if(dt.TableName == "BasicDetails")
					{				
						((ConsumerEnquiryRequest)bureauRequest).Consumers.Add(new Consumer());

						foreach(DataRow r in dt.Rows)
						{
							string gender = "UNKN";
							if((string)r[CN.Sex]=="M")
								gender = "MALE";
							if((string)r[CN.Sex]=="F")
								gender = "FEML";
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICDOB = (DateTime)r[CN.DOB];
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICGENCOD = gender;
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICIDCOD = customerID;
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICIDTYP = "NRIC";
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICNAM1 = (string)r[CN.LastName];
							((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IICNATCOD = "SG";	
						}
					}

					if(dt.TableName == TN.CustomerAddresses)
					{
						foreach ( DataRow r in dt.Rows )
						{
							if( (((string)r[CN.AddressType]).Trim() == "H" ||
								((string)r[CN.AddressType]).Trim() == "W") &&
								(string)r[CN.Category] == "CA1" )	/* address */
							{
								/* we need to split the content of address line 1 from the 
								 * database which holds both the street number and the street name */

								string address1 = (string)r[CN.Address1];
								string number = "";
								/* some addresses contain the word BLK which we need to remove */
								address1 = address1.Replace("BLK", "");
								address1 = address1.Trim();

								/* find the number by looking for everything before the first space */
								int index = address1.IndexOf(" ");
								if(index!=-1)
									number = address1.Substring(0, index);
								else
									number = address1;

								/* assume the street name is everything else */
								if(index!=-1)
									address1 = address1.Substring(index+1, address1.Length-(index+1));

								Address addr = new Address();
								addr.IIACOUCOD = "SG";
								addr.IIAADRTYP = ((string)r[CN.AddressType]).Trim()=="H"?"RESID":"WORK";
								addr.IIAPOSCOD = (string)r[CN.PostCode];
								addr.IIASTR = address1;
								addr.IIAADRLN2 = "S";
								addr.IIASTRNUMF = number;
								((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).Addresses.Add(addr);
							}

							if( (((string)r[CN.AddressType]).Trim() == "H" ||
								((string)r[CN.AddressType]).Trim() == "W") &&
								(string)r[CN.Category] == "CT1" )	/* phone no */
							{
								((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IINNUM = (string)r[CN.DialCode] + (string)r[CN.Phone];
								((ConsumerEnquiryRequest)bureauRequest).Consumers.Item(i).IINNUMTYP = ((string)r[CN.AddressType]).Trim()=="H"?"RESID":"OFFIC";;
							}
						}
					}
				}
			}
			
			if((bool)Country[CountryParameterNames.LoggingEnabled])
				logMessage(bureauRequest.Serialise().OuterXml, "DEBUG", EventLogEntryType.Information);

			bureauMessenger = new CreditBureauMessenger();
			XmlDocument response = bureauMessenger.SendRequest(bureauRequest.Serialise());

			logMessage(response.OuterXml, "DEBUG", EventLogEntryType.Warning);
			
			bureauResponse.DeSerialise(conn, trans, customerID, response);
		}

        public string Transform(XmlDocument response, string creditBureau)
		{
            XslCompiledTransform xslTrans = new XslCompiledTransform();
			StringWriter sw = null;
            
            //CR 843 new code 
            string stlyeSheet = "";
            switch (creditBureau)
            {
                case STL.Common.Constants.CreditBureau.CreditBureau.Baycorp:
                    stlyeSheet = "http://localhost/Courts.NET.WS/stylesheets/CreditScan.xslt";
                    break;
                case STL.Common.Constants.CreditBureau.CreditBureau.DPGroup:
                    stlyeSheet = "http://localhost/Courts.NET.WS/stylesheets/CreditScan2.xslt";
                    break;
                default:
                    break;
            }

            xslTrans.Load(stlyeSheet);
            // End CR 843
			sw = new StringWriter();
			xslTrans.Transform(response, null, sw);
			return sw.ToString();
		}

		public BCreditBureau()
		{
			
		}


		/// <summary>
		/// GetLastRequest
		/// </summary>
		/// <param name="custid">string</param>
		/// <param name="datelast">DateTime</param>
		/// <returns>DateTime</returns>
		/// 
		public DateTime GetLastRequest (SqlConnection conn, SqlTransaction trans, string custid,string source)
		{			 
			DCreditBureau da = new DCreditBureau();
			return da.GetLastRequest(conn, trans, custid,source);			
		}
	}
}