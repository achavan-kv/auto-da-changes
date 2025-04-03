using System;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// Static class containing constant definitions for
	/// the service elements available from the credit bureau
	/// </summary>
	public class CreditBureauService
	{
		public static string ConsumerEnquiryRequest = "ENTCONWS";
		public static string HistoricalReportRequest = "ENTGETWS";
		public static string LoadDefaultRequest = "ENTIPDWS";
		public static string LoadMonitorRequest = "ENTIMNWS";

		public CreditBureauService()
		{
			
		}
	}
}
