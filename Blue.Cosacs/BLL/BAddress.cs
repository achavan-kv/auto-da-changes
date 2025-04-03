using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Xml;
using System.Data.SqlClient;

namespace STL.BLL
{
	/// <summary>
	/// Summary description for BAddress.
	/// </summary>
	public class BAddress : CommonObject
	{
		private string _address1 = "";
		public string Address1
		{
			get{return _address1;}
			set{_address1 = value;}
		}
		private string _address2 = "";
		public string Address2
		{
			get{return _address2;}
			set{_address2 = value;}
		}
		
		public string Notes
		{
		   get {return _notes;}
		   set {_notes = value;}
		}    
		private string _notes; 
		private string _address3 = "";
		public string Address3
		{
			get{return _address3;}
			set{_address3 = value;}
		}
		private DataSet ds = null;
		public DataSet Address
		{
			get{return ds;}
		}
		public void PostCodeLookUp(string postCode)
		{
			DPostCode po = new DPostCode();
			po.LookUp(postCode);

			if(po.AddressType=="P" ||
				po.AddressType=="B" ||
				po.AddressType=="W")
			{
				this.Address1 = po.BuildingKey;
				this.Address3 = po.Country;
			}
			else
			{
				this.Address1 = po.BuildingNumber + " " + po.StreetName;
				this.Address2 = po.BuildingName;
				this.Address3 = po.Country;
			}

			//package the details into a dataset to be returned to the client
			ds = new DataSet();
			DataTable dt = new DataTable("Address");
			dt.Columns.AddRange(new DataColumn[]{new DataColumn("Address1"),
													new DataColumn("Address2"),
													new DataColumn("Address3")});
			DataRow row = dt.NewRow();
			row["Address1"] = this.Address1;
			row["Address2"] = this.Address2;
			row["Address3"] = this.Address3;
			dt.Rows.Add(row);
			ds.Tables.Add(dt);
		}
		public BAddress()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
