using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Data.SqlClient;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Constants.Tags;
using STL.Common.Constants.Enums;




namespace STL.BLL
{
	/// <summary>
	/// Summary description for BMandatoryFields.
	/// </summary>
	public class BMandatoryFields : CommonObject
	{
		public DataSet GetMandatoryFields(string country, string screen)
		{
			DataSet ds = new DataSet();
			DMandatoryFields mf = new DMandatoryFields();
			mf.GetMandatoryFields(country, screen);
			//As returned from the SP the boolean fields will be shorts therefore
			//we have to go through them and convert them to bool

			DataTable dt = mf.Fields.Clone();	//copy the structure
			dt.Columns["enabled"].DataType = Type.GetType("System.Boolean");
			dt.Columns["visible"].DataType = Type.GetType("System.Boolean");
			dt.Columns["mandatory"].DataType = Type.GetType("System.Boolean");

			foreach(DataRow row in mf.Fields.Rows)
			{
				DataRow newRow = dt.NewRow();
				newRow["country"] = row["country"];
				newRow["screen"] = row["screen"];
				newRow["control"] = row["control"];
				newRow["description"] = row["description"];
				newRow["enabled"] = Convert.ToBoolean(row["enabled"]);
				newRow["visible"] = Convert.ToBoolean(row["visible"]);
				newRow["mandatory"] = Convert.ToBoolean(row["mandatory"]);				
				dt.Rows.Add(newRow);
			}

			dt.AcceptChanges();
			ds.Tables.Add(dt);
			return ds;
		}

		public void Save(SqlConnection conn, SqlTransaction trans, DataSet changes)
		{
			DMandatoryFields mf = new DMandatoryFields();

			foreach(DataRow row in changes.Tables[0].Rows)
			{
				mf.Country = (string)row["country"];
				mf.Screen = (string)row["screen"];
				mf.Control = (string)row["control"];
				mf.Description = (string)row["description"];
				mf.Enabled = (bool)row["enabled"];
				mf.Visible = (bool)row["visible"];
				mf.Mandatory = (bool)row["mandatory"];
				mf.Save(conn, trans);
			}
		}


		public DataSet GetScreens(XmlNode screens)
		{
			DataSet ds = new DataSet();

			foreach(XmlNode node in screens.ChildNodes)
			{
				if(node.Attributes[Tags.Name].Value == TN.Screens)
				{
					DMandatoryFields mf = new DMandatoryFields();
					if(mf.GetScreens() == (int)Return.Success)
					{
						ds.Tables.Add(mf.Screens);
					}
				}
			}
			return ds;
		}

		public BMandatoryFields()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
