using System;
using System.Net;
using System.Security;
using System.Security.Cryptography.X509Certificates;


namespace STL.BLL
{
	/// <summary>
	/// This is dodgy - we are effectively saying that any certification problems
	/// should be ignored. In our case it is probably just about OK but should
	/// not be used as a model of how things should be done.
	/// </summary>
	public class TrustedCertificatePolicy : System.Net.ICertificatePolicy 
	{
		public TrustedCertificatePolicy() {}

		public bool CheckValidationResult(	System.Net.ServicePoint sp,
											System.Security.Cryptography.X509Certificates.X509Certificate certificate,
											System.Net.WebRequest request, int problem )
		{
			return true;
		}
	}

}
