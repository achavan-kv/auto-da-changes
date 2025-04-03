using System;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Configuration;
using STL.DAL;

//CR 843 This entire class added 
namespace STL.BLL.CreditBureau
{
    /// <summary>
    /// This communicates with a different credit bureau, using a simplified class structure
    /// </summary>
    public class CreditBureauMessenger2
    {
        private string bureauUser = "";
        private bool useProxy = false;
        private string proxyHost = "";
        private string proxyPort = "";
        private int bureauEnqCode = 0;

        private TimeSpan _TimeSinceLastLitigation = new TimeSpan(0);
        private TimeSpan _TimeSinceLastBankruptcy = new TimeSpan(0);

        private short _NumberOfLitigations = 0;
        private short _NumberOfLitigationsLastYear = 0;
        private short _NumberOfLitigationsLast2Year = 0;
        private decimal _AverageLitigationValue = 0;
        private decimal _TotalLitigationValue = 0;
        private short _NumberOfBankruptcies = 0;
        private short _NumberOfBankruptciesLastYear = 0;
        private short _NumberOfBankruptciesLast2Year = 0;
        private decimal _AverageBankruptciesValue = 0;
        private decimal _TotalBankruptciesValue = 0;
        private short _NumberOfPreviousEnquiries = 0;
        private decimal _TotalPreviousEnquiryValue = 0;
        private decimal _AveragePreviousEnquiryValue = 0;
        private short _NumberOfPreviousEnquiriesLastYear = 0;
        private decimal _AveragePreviousEnquiryValueLastYear = 0;

        public CreditBureauMessenger2()
        {
            bureauUser = ConfigurationManager.AppSettings["bureauUser2"];
            useProxy = Convert.ToBoolean(ConfigurationManager.AppSettings["bureauUseProxy"]);
            bureauEnqCode = Convert.ToInt32(ConfigurationManager.AppSettings["bureauEnqCode"]);
            if (useProxy)
            {
                proxyHost = ConfigurationManager.AppSettings["proxyHost"];
                proxyPort = ConfigurationManager.AppSettings["proxyPort"];
            }
        }

        public void ConsumerEnquiry(SqlConnection conn,
            SqlTransaction trans, string AccountNumber, string customerID)
        {
            XmlNode doc = new XmlDocument();
			//New web reference added 09/08/2007 - requires valid UserID for parameter bureauUser

			//CreditBureau2.CourtsWs creditBureau2 = new CreditBureau2.CourtsWs();
		//	CreditBureau2.CourtsWs creditBureau3 = new CreditBureau2.CourtsWs();
			//timeout test
       //     creditBureau3.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["bureau2Timeout"]);
			
			//doc = creditBureau2.GetCBS(bureauUser, customerID, "", AccountNumber, bureauEnqCode);
			//doc = creditBureau3.GetCBS(bureauUser, customerID, "", AccountNumber, Convert.ToString(bureauEnqCode),"1");
            //AA -removing as this is only used by Singapore and unfortunately they will not be upgrading to this version. 
            //doc.Load()
            if (doc != null)
                this.ProcessBureauXML(conn, trans, doc, customerID);

        }

        private void ProcessBureauXML(SqlConnection conn, SqlTransaction trans, XmlNode doc, string customerID)
        {
            DCreditBureau creditBureau = new DCreditBureau();

            string xpath = "";

            xpath = "//LitSum";
            XmlNode xnode = doc.SelectSingleNode(xpath);

            if (!xnode.Attributes["Bankrupt"].Value.Equals(string.Empty))
            {
                // now being sent as Y or N
				//_NumberOfBankruptcies = Convert.ToInt16(xnode.Attributes["Bankrupt"].Value);
                _NumberOfBankruptcies = xnode.Attributes["Bankrupt"].Value.ToUpper() == "Y" ? Convert.ToInt16(1) : Convert.ToInt16(0);
            }

            // now litigation is handled in Court sub elmentments
//			if (xnode.Attributes["LitPast"] != null && !xnode.Attributes["LitPast"].Value.Equals(string.Empty))
//				_NumberOfLitigations = Convert.ToInt16(xnode.Attributes["LitPast"].Value);
//
//			if (xnode.Attributes["LitPast2"] != null && !xnode.Attributes["LitPast2"].Value.Equals(string.Empty))
//				_NumberOfLitigationsLast2Year = Convert.ToInt16(xnode.Attributes["LitPast2"].Value);

            XmlNodeList courtList = xnode.SelectNodes("Court");
            foreach (XmlNode courtNode in courtList)
            {
                if (courtNode.Attributes["LitCur"] != null && !courtNode.Attributes["LitCur"].Value.Equals(String.Empty))
                    _NumberOfLitigations += Convert.ToInt16(courtNode.Attributes["LitCur"].Value);

                if (courtNode.Attributes["LitPrev"] != null && !courtNode.Attributes["LitPrev"].Value.Equals(String.Empty))
                    _NumberOfLitigationsLast2Year += Convert.ToInt16(courtNode.Attributes["LitPrev"].Value);

            }


            xpath = "//SrhSum/Info";
            XmlNodeList xnodelist = doc.SelectNodes(xpath);

            foreach (XmlNode x in xnodelist)
            {

                if (!x.Attributes["Curr"].Value.Equals(string.Empty))
                {
                    _NumberOfPreviousEnquiries += Convert.ToInt16(x.Attributes["Curr"].Value);
                    _NumberOfPreviousEnquiriesLastYear += Convert.ToInt16(x.Attributes["Curr"].Value); //This looks at the current field only
                }
                if (!x.Attributes["Prev"].Value.Equals(string.Empty))
                    _NumberOfPreviousEnquiries += Convert.ToInt16(x.Attributes["Prev"].Value);
            }

            creditBureau.Save(conn, trans, customerID, DateTime.Today,
                doc.OuterXml, Convert.ToInt16(_TimeSinceLastLitigation.Days),
                Convert.ToInt16(_TimeSinceLastBankruptcy.Days),
                _NumberOfLitigations, _NumberOfLitigationsLastYear,
                _NumberOfLitigationsLast2Year,
                _AverageLitigationValue, _TotalLitigationValue,
                _NumberOfBankruptcies, _NumberOfBankruptciesLastYear,
                _NumberOfBankruptciesLast2Year,
                _AverageBankruptciesValue, _TotalBankruptciesValue,
                _NumberOfPreviousEnquiries, _TotalPreviousEnquiryValue,
                _AveragePreviousEnquiryValue, _NumberOfPreviousEnquiriesLastYear,
                _AveragePreviousEnquiryValueLastYear, STL.Common.Constants.CreditBureau.CreditBureau.DPGroup);

        }
    }
}
