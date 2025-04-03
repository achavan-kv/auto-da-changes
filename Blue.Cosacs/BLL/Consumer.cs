using System;

namespace STL.BLL.CreditBureau
{
	/// <summary>
	/// This class will represent a consumer for a CreditBureau request or
	/// response. 
	/// </summary>
	public class Consumer
	{
		protected AddressCollection _addresses;
		public AddressCollection Addresses
		{
			get{return _addresses;}
			set{_addresses = value;}
		}

		protected DateTime _dob;
		/// <summary>
		/// Consumer date of birth - must be in format as described by schema
		/// Mandatory
		/// </summary>
		public DateTime IICDOB
		{
			get{return _dob;}
			set{_dob = value;}
		}

		protected string _gendercode = "";
		/// <summary>
		/// Consumer gender code - must be valid value on table 'Gender' INDGENCOD
		/// Mandatory
		/// </summary>
		public string IICGENCOD
		{
			get{return _gendercode;}
			set{_gendercode = value;}
		}

		protected string _idnum = "";
		/// <summary>
		/// Consumer ID number
		/// Mandatory
		/// </summary>
		public string IICIDCOD
		{
			get{return _idnum;}
			set{_idnum = value;}
		}

		protected string _idtype = "";
		/// <summary>
		/// Consumer ID type - must be valid value on table 'Consumer ID Type' INDIDTYP
		/// Mandatory
		/// </summary>
		public string IICIDTYP
		{
			get{return _idtype;}
			set{_idtype = value;}
		}

		protected string _familyname = "";
		/// <summary>
		/// Consumer family name
		/// Mandatory
		/// </summary>
		public string IICNAM1
		{
			get{return _familyname;}
			set{_familyname = value;}
		}

		protected string _givenname = "";
		/// <summary>
		/// Consumer given name
		/// Optional
		/// </summary>
		public string IICNAM2
		{
			get{return _givenname;}
			set{_givenname = value;}
		}

		protected string _dialectname = "";
		/// <summary>
		/// Consumer dialect name
		/// Optional
		/// </summary>
		public string IICNAM3
		{
			get{return _dialectname;}
			set{_dialectname = value;}
		}

		protected string _maidenname = "";
		/// <summary>
		/// Consumer maiden name
		/// Optional
		/// </summary>
		public string IICNAM4
		{
			get{return _maidenname;}
			set{_maidenname = value;}
		}

		protected string _aliasfamilyname = "";
		/// <summary>
		/// Consumer alias family name
		/// Optional
		/// </summary>
		public string IICANAM1
		{
			get{return _aliasfamilyname;}
			set{_aliasfamilyname = value;}
		}

		protected string _aliasgivenname = "";
		/// <summary>
		/// Consumer alias given name
		/// Optional
		/// </summary>
		public string IICANAM2
		{
			get{return _aliasgivenname;}
			set{_aliasgivenname = value;}
		}

		protected string _aliasdialectname = "";
		/// <summary>
		/// Consumer alias dialect name
		/// Optional
		/// </summary>
		public string IICANAM3
		{
			get{return _aliasdialectname;}
			set{_aliasdialectname = value;}
		}

		protected string _aliasmaidenname = "";
		/// <summary>
		/// Consumer alias maiden name
		/// Optional
		/// </summary>
		public string IICANAM4
		{
			get{return _aliasmaidenname;}
			set{_aliasmaidenname = value;}
		}

		protected string _spousename = "";
		/// <summary>
		/// Consumer spouse name
		/// Optional
		/// </summary>
		public string IICSPSNAM
		{
			get{return _spousename;}
			set{_spousename = value;}
		}

		protected string _nationalitycode = "";
		/// <summary>
		/// Consumer nationality code
		/// Optional
		/// </summary>
		public string IICNATCOD
		{
			get{return _nationalitycode;}
			set{_nationalitycode = value;}
		}

		protected string _maritalstat = "";
		/// <summary>
		/// Consumer marital status
		/// Optional
		/// </summary>
		public string IICMARSTS
		{
			get{return _maritalstat;}
			set{_maritalstat = value;}
		}

		protected string _ethnicgroup = "";
		/// <summary>
		/// Consumer ethnic group
		/// Optional
		/// </summary>
		public string IICETHGRP
		{
			get{return _ethnicgroup;}
			set{_ethnicgroup = value;}
		}

		protected string _occupation = "";
		/// <summary>
		/// Consumer occupation
		/// Optional
		/// </summary>
		public string IIEOCCDES
		{
			get{return _occupation;}
			set{_occupation = value;}
		}

		protected string _employer = "";
		/// <summary>
		/// Consumer employer
		/// Optional
		/// </summary>
		public string IIEEMP
		{
			get{return _employer;}
			set{_employer = value;}
		}

		protected string _phonenumtype = "";
		/// <summary>
		/// Consumer phone number type
		/// Optional
		/// </summary>
		public string IINNUMTYP
		{
			get{return _phonenumtype;}
			set{_phonenumtype = value;}
		}

		protected string _iddcode = "";
		/// <summary>
		/// Consumer IDD Code
		/// Optional
		/// </summary>
		public string IINISD
		{
			get{return _iddcode;}
			set{_iddcode = value;}
		}

		protected string _areacode = "";
		/// <summary>
		/// Consumer area code
		/// Optional
		/// </summary>
		public string IINPFX
		{
			get{return _areacode;}
			set{_areacode = value;}
		}

		protected string _phonenum = "";
		/// <summary>
		/// Consumer phone number
		/// Optional
		/// </summary>
		public string IINNUM
		{
			get{return _phonenum;}
			set{_phonenum = value;}
		}

		public Consumer()
		{
			_addresses = new AddressCollection();
		}
	}
}
