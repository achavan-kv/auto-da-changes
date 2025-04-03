using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using STL.DAL;
using STL.Common;
using System.Xml.Serialization;


/// <summary>
/// This is Oracle Customer Interface
/// </summary>
///
namespace STL.BLL.OracleIntegration
{
    public class Customer
    {
        public string CustomerName = " ";
        public string CustType1 = " ";
        public string Title = " ";
        public string Custid = " ";
        public string Acctno = " ";
        public string FirstName = " ";
        public string Name = " ";
        public string CustType2 = " ";
        public string CustClass = " ";
        public string CustCat = " ";
        public string HomeTelno = " ";
        [XmlElement(IsNullable = false)] 
        public string Email = " ";
        public string Passport = " ";
        public int empeeno;
        public string BillAddr1 = " ";
        public string BillAddr2 = " ";
        public string BillAddr3 = " ";
        [XmlElement(IsNullable = false)] 
        public string BillCity = " ";
        public string BillPostCode = " ";
        public string BillCountry = " ";
        public int BillAdrRef;
        public string ShipAddr1 = " ";
        public string ShipAddr2 = " ";
        public string ShipAddr3 = " ";
        public string ShipCity = " ";
        public string ShipPostCode = " ";
        public string ShipCountry = " ";
        public int ShipAdrRef;
        [XmlElement(IsNullable = false)] 
        public string MobileTelno = " ";
        public string WorkTelNo = " ";
        public int RunNo;


        public static Customer[] Populate()
        {
            ArrayList customers = new ArrayList();

            DataSet Customerdata = new DataSet();
            // Get Customers
            DOracleIntegration Oi = new DOracleIntegration();
            Customerdata = Oi.GetCustomerdata();

            Customer CurrCust = new Customer();

            foreach (DataRow data in Customerdata.Tables[0].Rows)
            {
                CurrCust = new Customer();

                CurrCust.CustomerName = data["CustomerName"].ToString();                
                CurrCust.Custid = data["Custid"].ToString();
                CurrCust.Acctno = data["Acctno"].ToString();
                CurrCust.CustType1 = data["CustType1"].ToString();
                CurrCust.Title = data["Title"].ToString();
                CurrCust.FirstName = data["FirstName"].ToString();
                CurrCust.Name = data["Name"].ToString();
                CurrCust.CustType2 = data["CustType2"].ToString();
                CurrCust.CustClass = data["CustClass"].ToString();
                CurrCust.CustCat = data["CustCat"].ToString();
                CurrCust.HomeTelno = data["HomeTelno"].ToString();
                CurrCust.Email = data["Email"].ToString();
                CurrCust.Passport = data["Passport"].ToString();
                CurrCust.empeeno = Convert.ToInt32(data["empeeno"].ToString());
                CurrCust.BillAddr1 = data["BillAddr1"].ToString();
                CurrCust.BillAddr2 = data["BillAddr2"].ToString();
                CurrCust.BillAddr3 = data["BillAddr3"].ToString();
                CurrCust.BillCity = data["BillCity"].ToString();
                CurrCust.BillPostCode = data["BillPostCode"].ToString();
                CurrCust.BillCountry = data["BillCountry"].ToString();
                CurrCust.BillAdrRef = Convert.ToInt32(data["BillAdrRef"].ToString());
                CurrCust.ShipAddr1 = data["ShipAddr1"].ToString();
                CurrCust.ShipAddr2 = data["ShipAddr2"].ToString();
                CurrCust.ShipAddr3 = data["ShipAddr3"].ToString();
                CurrCust.ShipCity = data["ShipCity"].ToString();
                CurrCust.ShipPostCode = data["ShipPostCode"].ToString();
                CurrCust.ShipCountry = data["ShipCountry"].ToString();
                CurrCust.ShipAdrRef = Convert.ToInt32(data["ShipAdrRef"].ToString());
                CurrCust.MobileTelno = data["MobileTelno"].ToString();
                CurrCust.WorkTelNo = data["WorkTelNo"].ToString();
                CurrCust.RunNo = Convert.ToInt32(data["RunNo"].ToString());

                // Add Customer
                customers.Add(CurrCust);

            }

            return (Customer[])customers.ToArray(typeof(Customer));
        }

    }
}

