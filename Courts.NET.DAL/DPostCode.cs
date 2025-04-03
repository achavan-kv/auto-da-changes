using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DPostCode.
	/// </summary>
	public class DPostCode : DALObject
	{
		private string _addtype = "";
		public string AddressType
		{
			get{return _addtype;}
			set{_addtype = value;}
		}
		private string _buildno = "";
		public string BuildingNumber
		{
			get{return _buildno;}
			set{_buildno = value;}
		}
		private string _bkey = "";
		public string BuildingKey
		{
			get{return _bkey;}
			set{_bkey = value;}
		}
		private string _bname = "";
		public string BuildingName
		{
			get{return _bname;}
			set{_bname = value;}
		}
		private string _sname = "";
		public string StreetName
		{
			get{return _sname;}
			set{_sname = value;}
		}
		private string _country = "";
		public new string Country
		{
			get{return _country;}
			set{_country = value;}
		}

		public void LookUp(string postCode)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@postCode", SqlDbType.NVarChar,6);
				parmArray[0].Value = postCode;
				parmArray[1] = new SqlParameter("@addType", SqlDbType.NVarChar,1);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;
				parmArray[2] = new SqlParameter("@buildNo", SqlDbType.NVarChar,7);
				parmArray[2].Value = "";
				parmArray[2].Direction = ParameterDirection.Output;
				parmArray[3] = new SqlParameter("@bkey", SqlDbType.NVarChar,6);
				parmArray[3].Value = "";
				parmArray[3].Direction = ParameterDirection.Output;
				parmArray[4] = new SqlParameter("@bname", SqlDbType.NVarChar,45);
				parmArray[4].Value = "";
				parmArray[4].Direction = ParameterDirection.Output;
				parmArray[5] = new SqlParameter("@sname", SqlDbType.NVarChar,32);
				parmArray[5].Value = "";
				parmArray[5].Direction = ParameterDirection.Output;
				parmArray[6] = new SqlParameter("@country", SqlDbType.NVarChar,20);
				parmArray[6].Value = "";
				parmArray[6].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_PostCodeLookUpSP", parmArray);

				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[1].Value))
						this.AddressType = (string)parmArray[1].Value;
					if(!Convert.IsDBNull(parmArray[2].Value))
						this.BuildingNumber = (string)parmArray[2].Value;
					if(!Convert.IsDBNull(parmArray[3].Value))
						this.BuildingKey = (string)parmArray[3].Value;
					if(!Convert.IsDBNull(parmArray[4].Value))
						this.BuildingName = (string)parmArray[4].Value;
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.StreetName = (string)parmArray[5].Value;
					if(!Convert.IsDBNull(parmArray[6].Value))
						this.Country = (string)parmArray[6].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public DPostCode()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
