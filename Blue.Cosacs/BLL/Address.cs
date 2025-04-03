using System;

namespace STL.BLL
{
	/// <summary>
	/// This class will represent an address for a CreditBureau request or
	/// response. 
	/// </summary>
	public class Address
	{
		protected string _streetname = "";
		/// <summary>
		/// Consumer street name
		/// mandatory
		/// </summary>
		public string IIASTR
		{
			get{return _streetname;}
			set{_streetname = value;}
		}

		protected string _addresstype = "";
		/// <summary>
		/// Consumer address type
		/// mandatory
		/// </summary>
		public string IIAADRTYP
		{
			get{return _addresstype;}
			set{_addresstype = value;}
		}

		protected string _addresscountry = "";
		/// <summary>
		/// Consumer address country
		/// mandatory
		/// </summary>
		public string IIACOUCOD
		{
			get{return _addresscountry;}
			set{_addresscountry = value;}
		}

		protected string _postcode = "";
		/// <summary>
		/// Consumer address post code
		/// optional 
		/// </summary>
		public string IIAPOSCOD
		{
			get{return _postcode;}
			set{_postcode = value;}
		}

		protected string _stateCity = "";
		/// <summary>
		/// Consumer address state city
		/// mandatory through business rules 
		/// </summary>
		public string IIAADRLN2
		{
			get{return _stateCity;}
			set{_stateCity = value;}
		}

		protected string _houseNum = "";
		/// <summary>
		/// Consumer address house number
		/// mandatory through business rules 
		/// </summary>
		public string IIASTRNUMF
		{
			get{return _houseNum;}
			set{_houseNum = value;}
		}

		public Address()
		{
		}
	}
}
