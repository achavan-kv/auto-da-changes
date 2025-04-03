using System;
using System.Xml;
using STL.DAL;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Summary description for ConsumerEnquiryResponse.
	/// </summary>
	public class ConsumerEnquiryResponse : CreditBureauMessage, ICreditBureauResponse
	{
		public ConsumerEnquiryResponse()
		{
			ValidatingSchema = "http://localhost/Courts.NET.WS/schemas/enquiryresp.xsd";
		}

		/* Litigations */ 
		private TimeSpan _sincelastlit;
		/// <summary>
		/// The amount of time since the consumers last litigation
		/// </summary>
		public TimeSpan TimeSinceLastLitigation
		{
			get{return _sincelastlit;}
			set{_sincelastlit = value;}
		}

		private short _numlitigations = 0;
		/// <summary>
		/// Number of litigations for the consumer
		/// </summary>
		public short NumberOfLitigations
		{
			get{return _numlitigations;}
			set{_numlitigations = value;}
		}

		private short _numlitigations12 = 0;
		/// <summary>
		/// Number of litigations for the consumer in the last 12 months
		/// </summary>
		public short NumberOfLitigationsLastYear
		{
			get{return _numlitigations12;}
			set{_numlitigations12 = value;}
		}

		private short _numlitigations24 = 0;
		/// <summary>
		/// Number of litigations for the consumer in the last 24 months
		/// </summary>
		public short NumberOfLitigationsLast2Year
		{
			get{return _numlitigations24;}
			set{_numlitigations24 = value;}
		}

		private decimal _avvallitigation = 0;
		/// <summary>
		/// Average value of consumers litigations
		/// </summary>
		public decimal AverageLitigationValue
		{
			get{return _avvallitigation;}
			set{_avvallitigation = value;}
		}

		private decimal _totallitvalue = 0;
		/// <summary>
		/// Total value of consumers litigations
		/// </summary>
		public decimal TotalLitigationValue
		{
			get{return _totallitvalue;}
			set{_totallitvalue = value;}
		}

		/* bankruptcies */
		private TimeSpan _sincelastbk;
		/// <summary>
		/// The amount of time since the consumers last litigation
		/// </summary>
		public TimeSpan TimeSinceLastBankruptcy
		{
			get{return _sincelastbk;}
			set{_sincelastbk = value;}
		}

		private short _numbankruptcies = 0;
		/// <summary>
		/// Number of bankruptcies for the consumer
		/// </summary>
		public short NumberOfBankruptcies
		{
			get{return _numbankruptcies;}
			set{_numbankruptcies = value;}
		}

		private short _numbankruptcies12 = 0;
		/// <summary>
		/// Number of bankruptcies for the consumer in the last 12 months
		/// </summary>
		public short NumberOfBankruptciesLastYear
		{
			get{return _numbankruptcies12;}
			set{_numbankruptcies12 = value;}
		}

		private short _numbankruptcies24 = 0;
		/// <summary>
		/// Number of bankruptcies for the consumer in the last 24 months
		/// </summary>
		public short NumberOfBankruptciesLast2Year
		{
			get{return _numbankruptcies24;}
			set{_numbankruptcies24 = value;}
		}

		private decimal _avbankruptciesval = 0;
		/// <summary>
		/// Average value of consumers bankruptcies
		/// </summary>
		public decimal AverageBankruptciesValue
		{
			get{return _avbankruptciesval;}
			set{_avbankruptciesval = value;}
		}

		private decimal _totalbankvalue = 0;
		/// <summary>
		/// Total value of consumers bankruptcies
		/// </summary>
		public decimal TotalBankruptciesValue
		{
			get{return _totalbankvalue;}
			set{_totalbankvalue = value;}
		}
		
		/* previous enquiries */
		private short _numprevenquiries = 0;
		/// <summary>
		/// Number of previous enquiries
		/// </summary>
		public short NumberOfPreviousEnquiries
		{
			get{return _numprevenquiries;}
			set{_numprevenquiries = value;}
		}

		private decimal _avprevenqval = 0;
		/// <summary>
		/// Average value of consumers previous enquiries
		/// </summary>
		public decimal AveragePreviousEnquiryValue
		{
			get{return _avprevenqval;}
			set{_avprevenqval = value;}
		}

		private decimal _totalprevenqval = 0;
		/// <summary>
		/// Total value of consumers litigations
		/// </summary>
		public decimal TotalPreviousEnquiryValue
		{
			get{return _totalprevenqval;}
			set{_totalprevenqval = value;}
		}

		private decimal _avprevenqval12 = 0;
		/// <summary>
		/// Average value of consumers previous enquiries in the last 12 months
		/// </summary>
		public decimal AveragePreviousEnquiryValueLastYear
		{
			get{return _avprevenqval12;}
			set{_avprevenqval12 = value;}
		}

		private short _numprevenquiries12 = 0;
		/// <summary>
		/// Number of previous enquiries in the ast 12 months
		/// </summary>
		public short NumberOfPreviousEnquiriesLastYear
		{
			get{return _numprevenquiries12;}
			set{_numprevenquiries12 = value;}
		}

		/// <summary>
		/// Validates the incoming XmlDocument against the appropriate 
		/// credit bureau schema. If it is valid, the required information is
		/// picked out using an xPath query and exposed as properties of the
		/// ICreditBureauResponse instance
		/// </summary>
		/// <returns>ICreditBureauResponse</returns>
		public ICreditBureauResponse DeSerialise(SqlConnection conn, SqlTransaction trans, string customerID, XmlDocument response)
		{
			/* Remove the validation of the response for now because the 
			 * schema seems to be wrong */
			Validate(response);
			
			if(ValidationErrors.Length > 0)
			{
				/* failed schema validation */
				string error = "Credit Bureau Response failed validation: " + Environment.NewLine + Environment.NewLine;
				throw new CreditBureauException(error+ValidationErrors);
			}
			else
			{
				/* check the status node to make sure it's OK */
				XmlNode responseNode = response.DocumentElement;
				XmlNode statusNode = responseNode.SelectSingleNode(CreditBureauElements.STATUS);
				if(statusNode != null)
				{
					switch(statusNode.InnerText)
					{
						case CreditBureauStatus.OK:		ProcessResponse(conn, trans, customerID, responseNode);
							break;
						case CreditBureauStatus.Error:	ProcessErrors(responseNode);
							break;		
						case CreditBureauStatus.ParsingError:	ProcessParseError(responseNode);
							break;
						default:
							break;
					}
				}					
			}

			return this;
		}

		/// <summary>
		/// Unpicks the required details from the response document
		/// </summary>
		/// <param name="response"></param>
		public void ProcessResponse(SqlConnection conn, SqlTransaction trans, string customerID, XmlNode response)
		{
			DateTime lastYear = DateTime.Today.AddYears(-1);
			DateTime last2Years = DateTime.Today.AddYears(-2);
			DateTime latestLit = DateTime.Today.AddYears(-100);
			DateTime latestBank = DateTime.Today.AddYears(-100);
			string xPath = "";
			bool isLitigation = false;

			#region Litigations 
			xPath = "//PUBLIC_NOTICE[IPN_TYPE = 'LITG']";
			XmlNodeList lits = response.SelectNodes(xPath);
			this.NumberOfLitigations = Convert.ToInt16(lits.Count);
			
			/* loop through the litigations to work out how many are in the
			 * last year when we get to line 10 try and pick out the value */
			foreach(XmlNode lit in lits)
			{
				// exclude litigations where no details have been found
				string litStr = "";
				XmlNodeList litNodes = lit.SelectNodes("IPN_TEXT[IPT_LINE_NUMBER]");
				foreach(XmlNode ln in litNodes)
				{
					litStr = ln.SelectSingleNode("IPT_LINE").InnerText;
					if(litStr == "There are no litigation details found on the subject")
					{
						this.NumberOfLitigations--;
						isLitigation = false;
						break;
					}
					else
						isLitigation = true;
				}
				
				/* some litigations have no details and therefore we cannot include them */
				string dateStr = lit.SelectSingleNode("IPN_LOAD_DATE").InnerText;
				if(dateStr.Length > 0 && isLitigation)
				{
					DateTime litDate = ConvertDateString(dateStr);
					//DateTime litDate = Convert.ToDateTime((lit.SelectSingleNode("IPN_LOAD_DATE")).InnerText);
					if(litDate > latestLit)
						latestLit = litDate;

					if(litDate > lastYear)
						this.NumberOfLitigationsLastYear++;
					if(litDate > last2Years)
						this.NumberOfLitigationsLast2Year++;

					XmlNode amountNode = lit.SelectSingleNode("IPN_TEXT[IPT_LINE_NUMBER = '10']");
					if(amountNode!=null)
					{
						string amtStr = amountNode.SelectSingleNode("IPT_LINE").InnerText;
						int index = amtStr.IndexOf(":");
						this.TotalLitigationValue += Convert.ToDecimal(amtStr.Substring(index+1, amtStr.Length - (index+1)));
					}
				}
			}
			if(this.NumberOfLitigations > 0)
				this.TimeSinceLastLitigation = DateTime.Now - latestLit;
			else
				this.TimeSinceLastLitigation = new TimeSpan(0,0,0);
			if(NumberOfLitigations > 0)
				this.AverageLitigationValue = TotalLitigationValue / NumberOfLitigations;
			#endregion

			#region Bankruptcies
			xPath = "//PUBLIC_NOTICE[IPN_TYPE = 'BKPT']";
			XmlNodeList bkpts = response.SelectNodes(xPath);
			this.NumberOfBankruptcies = Convert.ToInt16(bkpts.Count);
			
			/* loop through the bankruptcies to work out how many are in the
			 * last year */
			foreach(XmlNode b in bkpts)
			{
				string dateStr = b.SelectSingleNode("IPN_LOAD_DATE").InnerText;
				if(dateStr.Length>0)
				{
					DateTime bkptDate = ConvertDateString(dateStr);
					//DateTime bkptDate = Convert.ToDateTime((b.SelectSingleNode("IPN_LOAD_DATE")).InnerText);
					if(bkptDate > latestBank)
						latestBank = bkptDate;

					if(bkptDate > lastYear)
						this.NumberOfBankruptciesLastYear++;

					if(bkptDate > last2Years)
						this.NumberOfBankruptciesLast2Year++;

					XmlNode amountNode = b.SelectSingleNode("IPN_TEXT[IPT_LINE_NUMBER = '10']");
					if(amountNode!=null)
					{
						string amtStr = amountNode.SelectSingleNode("IPT_LINE").InnerText;
						int index = amtStr.IndexOf(":");
						this.TotalBankruptciesValue += Convert.ToDecimal(amtStr.Substring(index+1, amtStr.Length - (index+1)));
					}
				}
			}
			if(this.NumberOfBankruptcies > 0)
				this.TimeSinceLastBankruptcy = DateTime.Now - latestBank;
			else
				this.TimeSinceLastBankruptcy = new TimeSpan(0,0,0);
			if(NumberOfBankruptcies > 0)
				this.AverageBankruptciesValue = TotalBankruptciesValue / NumberOfBankruptcies;
			#endregion

			#region Previous Enquiries
			xPath = "//PREVIOUS_INQUIRY";
			XmlNodeList prevenqs = response.SelectNodes(xPath);
			this.NumberOfPreviousEnquiries = Convert.ToInt16(prevenqs.Count);
			
			/* loop through the previous enquiries to work out how many are in the
			 * last year */
			decimal peVal12 = 0;
			foreach(XmlNode prevenq in prevenqs)
			{
				DateTime peDate = ConvertDateString((prevenq.SelectSingleNode("IPI_LOAD_DATE")).InnerText);
				//DateTime peDate = Convert.ToDateTime((prevenq.SelectSingleNode("IPI_LOAD_DATE")).InnerText);
				if(peDate > lastYear)
				{
					this.NumberOfPreviousEnquiriesLastYear++;
					peVal12 += Convert.ToDecimal(prevenq.SelectSingleNode("IPI_AMOUNT").InnerText);
				}
				this.TotalPreviousEnquiryValue += Convert.ToDecimal(prevenq.SelectSingleNode("IPI_AMOUNT").InnerText);
			}
			if(NumberOfPreviousEnquiries > 0)
				this.AveragePreviousEnquiryValue = TotalPreviousEnquiryValue / NumberOfPreviousEnquiries;
			if(NumberOfPreviousEnquiriesLastYear > 0)
				this.AveragePreviousEnquiryValueLastYear = peVal12 / NumberOfPreviousEnquiriesLastYear;
			#endregion

			#region Payment Defaults
			DCode c = new DCode();
			c.GetCategoryCodes("PDC", "L", "PaymentDefaults");
			DCreditBureauDefaults cbd = new DCreditBureauDefaults();
			foreach(DataRow r in c.Codes.Rows)
			{
				XmlNodeList payDefs = null;
				short defaults = 0, defaultsExMotor = 0;
				decimal defaultsBal = 0, defaultsBalExMotor = 0;
				string code = (string)r[CN.CodeDescription];

				xPath = "//PAYMENT_DEFAULT[IPD_STATUS = '" + code + "']";
				payDefs = response.SelectNodes(xPath);
				defaults = Convert.ToInt16(payDefs.Count);
				foreach(XmlNode p in payDefs)
					defaultsBal += Convert.ToDecimal(p.SelectSingleNode("IPD_BALANCE").InnerText);

				xPath = "//PAYMENT_DEFAULT[IPD_STATUS = '" + code + "' and IPD_PRODUCT_TYPE != 'Hire Purchase – Car']";
				payDefs = response.SelectNodes(xPath);
				defaultsExMotor = Convert.ToInt16(payDefs.Count);
				foreach(XmlNode p in payDefs)
					defaultsBalExMotor += Convert.ToDecimal(p.SelectSingleNode("IPD_BALANCE").InnerText);
				
				if( defaults > 0 || 
					defaultsExMotor > 0 )
				{
					cbd.Save(conn, trans, customerID, (string)r[CN.Code],
						defaultsBal, defaults, defaultsBalExMotor, defaultsExMotor);
				}
			}
			#endregion

			/* save the credit bureau response */
			DCreditBureau creditBureau = new DCreditBureau();
			creditBureau.Save(conn, trans, customerID, DateTime.Today, 
								response.OuterXml, Convert.ToInt16(TimeSinceLastLitigation.Days), 
								Convert.ToInt16(TimeSinceLastBankruptcy.Days),
								NumberOfLitigations, NumberOfLitigationsLastYear,
								NumberOfLitigationsLast2Year, 
								AverageLitigationValue, TotalLitigationValue,
								NumberOfBankruptcies, NumberOfBankruptciesLastYear,
								NumberOfBankruptciesLast2Year,
								AverageBankruptciesValue, TotalBankruptciesValue,
								NumberOfPreviousEnquiries, TotalPreviousEnquiryValue,
								AveragePreviousEnquiryValue, NumberOfPreviousEnquiriesLastYear,
                                AveragePreviousEnquiryValueLastYear, STL.Common.Constants.CreditBureau.CreditBureau.Baycorp); //CR 843 
		}

		private DateTime ConvertDateString(string dateString)
		{
			/* dates are fixed format so we can't trust Convert.ToDateTime */
			/* will always be dd/mm/yy */
			int dd = 0;
			int mm = 0;
			int yy = 0;

			string[] dateparts = dateString.Split('/');
			if(dateparts.Length == 3)
			{
				dd = Convert.ToInt32(dateparts[0]);
				mm = Convert.ToInt32(dateparts[1]);
				yy = Convert.ToInt32(dateparts[2]);

				/* yy is sometimes 2 characters */
				if(dateparts[2].Length<4)
					if(yy<75)
						yy = Convert.ToInt32( "20"+dateparts[2] );
					else
						yy = Convert.ToInt32( "19"+dateparts[2] );
			}
			return new DateTime(yy, mm, dd);
		}

		/// <summary>
		/// Loops through the errors nodeList and records all 
		/// returned errors. Probably should then package them into 
		/// a CreditBureauException and throw it.
		/// </summary>
		/// <param name="response"></param>
		private void ProcessErrors(XmlNode response)
		{
			string errorList = "Business Errors have been returned from the Credit Bureau: " + Environment.NewLine + Environment.NewLine;
			XmlNode errorsNode = response.SelectSingleNode(CreditBureauElements.ERRORS);
			if(errorsNode!=null)
			{
				foreach(XmlNode error in errorsNode.ChildNodes)
				{	
					errorList += error.InnerText + Environment.NewLine;
				}
			}
			throw new CreditBureauException(errorList);
		}

		/// <summary>
		/// Pick out the description of the parse error and
		/// add it to a CreditBureauException and throw it.
		/// </summary>
		/// <param name="response"></param>
		private void ProcessParseError(XmlNode response)
		{
			string error = "A Parsing Error has been returned from the Credit Bureau: " + Environment.NewLine + Environment.NewLine;
			XmlNode errorNode = response.SelectSingleNode(CreditBureauElements.PARSER_ERROR);
			if(errorNode!=null)
			{
				error += errorNode.InnerText;
			}
			throw new CreditBureauException(error);
		}
	}
}
