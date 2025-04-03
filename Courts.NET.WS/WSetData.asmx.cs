using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;
using System.Web.Services.Protocols;
using STL.BLL;
using STL.Common;
using STL.DAL;

namespace STL.WS
{
    /// <summary>
    /// Summary description for WSetData.
    /// </summary>
    /// 
    [WebService(Namespace="http://strategicthought.com/webservices/")]
    public class WSetData : CommonService
    {
        public WSetData()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

		#region Component Designer generated code
		
        //Required by the Web Services Designer 
        private IContainer components = null;
				
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if(disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);		
        }
		
		#endregion
 
		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSetsForTName(string tName,out string err)
        {
            Function = "WSetData::GetSetsForTName()";
            DataSet ds = null;
            err = "";

            try
            {
                BSets sets = new BSets();
                ds = sets.GetSetsForTName(tName);
            }
            catch(Exception ex)
            {
                err = Function + ": "+ex.Message;
                logException(ex, Function);
            }

            return ds;
        }

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetSetsForTNameBranch(string tName, string branchNo, out string err)
		{
			Function = "WSetData::GetSetsForTNameBranch()";
			DataSet ds = null;
			err = "";

			try
			{
				BSets sets = new BSets();
				ds = sets.GetSetsForTNameBranch(tName, Convert.ToInt16(branchNo));
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}

			return ds;
		}

		[WebMethod]		
		[SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
		public DataSet GetSetsForTNameBranchAll(string tName, out string err)
		{
			Function = "WSetData::GetSetsForTNameBranchAll()";
			DataSet ds = null;
			err = "";

			try
			{
				BSets sets = new BSets();
				ds = sets.GetSetsForTNameBranchAll(tName);
			}
			catch(Exception ex)
			{
				err = Function + ": "+ex.Message;
				logException(ex, Function);
			}

			return ds;
		}

		[WebMethod]		
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveSets(string setName, int empeeNo, string tName, string columnType, string setDescript, decimal value, out string err)     // #13691
        {
            Function = "WSetData::SaveSets()";
            
            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted)) 
                        {

                        BSets sets = new BSets();
                        sets.Save(conn,trans,setName,empeeNo,tName,columnType,setDescript,value);           // #13691

                        trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch(SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                }while(retries<=maxRetries);
            }
            catch(Exception ex)
            {
                err = Function + ":  " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if(conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            return 0;
        }

        [WebMethod]		
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSetDetailsForSetName(string setName, string tName,out string err)
        {
            Function = "WSetData::GetSetDetailsForSetName()";
            DataSet ds = null;
            err = "";

            try
            {
                BSetDetails setDetails = new BSetDetails();
                ds = setDetails.GetSetDetailsForSetName(setName,tName);
            }
            catch(Exception ex)
            {
                err = Function + ": "+ex.Message;
                logException(ex, Function);
            }

            return ds;
        }

        [WebMethod]		
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public DataSet GetSets(string setName, string tName,out string err)
        {
            Function = "WSetData::GetSets()";
            DataSet ds = null;
            err = "";

            try
            {
                BSets sets = new BSets();
                ds = sets.GetSets(setName,tName);
            }
            catch(Exception ex)
            {
                err = Function + ": "+ex.Message;
                logException(ex, Function);
            }

            return ds;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
        [TraceExstension]
#endif
        public CategoryItem GetCategoryItem(string categoryCode)
        {
            BSetDetails bSetDetails = new BSetDetails();
            return bSetDetails.GetCategoryItem(categoryCode);
        }


        [WebMethod]		
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public int SaveSetDetails(string setName, string tName,string[] data,int empeeNo,string columnType,string setDescript, decimal value, string [] branchList, out string err)
        {
            Function = "WSetData::InsertSetDetails()";
            
            SqlConnection conn = null;
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BSets sets = new BSets();

                            // If there were no selections in the set, delete the set..
                            if (data.Length == 0)
                            {
                                sets.Delete(conn, trans, setName, tName);
                            }

                            else
                            {
                                // Save the set
                                sets.Save(conn, trans, setName, empeeNo, tName, columnType, setDescript,value);             // #13691

                                // Save the set contents
                                BSetDetails setDetails = new BSetDetails();
                                setDetails.Delete(conn, trans, setName, tName);
                                foreach (string dataValue in data)
                                {
                                    setDetails.Insert(conn, trans, setName, dataValue, tName);
                                }

                                // Save the branches for this set
                                setDetails.DeleteBranch(conn, trans, setName, tName);
                                foreach (string branchNo in branchList)
                                {
                                    setDetails.InsertBranch(conn, trans, setName, tName, branchNo);
                                }
                            }

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch(SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                }while(retries<=maxRetries);
            }
            catch(Exception ex)
            {
                err = Function + ":  " + ex.Message;
                logException(ex, Function);
            }
            finally
            {
                if(conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            // Save set details to Reporting Database
            Function = "WSetData::SaveSetDetails()";
          
          
            err = "";
            try
            {
                conn = new SqlConnection(Connections.Report);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            BSets sets = new BSets();

                            // If there were no selections in the set, delete the set..
                            if (data.Length == 0)
                            {
                                sets.Delete(conn, trans, setName, tName);
                            }

                            else
                            {
                                // Save the set
                                sets.Save(conn, trans, setName, empeeNo, tName, columnType, setDescript, value);            // #13691

                                // Save the set contents
                                BSetDetails setDetails = new BSetDetails();
                                setDetails.Delete(conn, trans, setName, tName);
                                foreach (string dataValue in data)
                                {
                                    setDetails.Insert(conn, trans, setName, dataValue, tName);
                                }

                                // Save the branches for this set
                                setDetails.DeleteBranch(conn, trans, setName, tName);
                                foreach (string branchNo in branchList)
                                {
                                    setDetails.InsertBranch(conn, trans, setName, tName, branchNo);
                                }
                            }

                            trans.Commit();		//commit the transaction
                        }
                        break;
                    }
                    catch (SqlException ex)
                    {
                        CatchDeadlock(ex, conn);
                    }
                } while (retries <= maxRetries);
            }
            catch (Exception ex)
            {
                err = Function + ":  " + ex.Message;
                logException(ex, Function);
              
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                    conn.Close();
            }

            return 0;
        }

        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public void SaveProvisions(List<ProvisionsItem> Items)
        {
            Function = "WSetData::GetSets()";

            //List<ProvisionsItem> ItemsList = new List<ProvisionsItem>(Items);
            DProvisions DPro = new DProvisions();
            DPro.SaveProvisions(ProvisionsConvert.ToDataTable(Items));

        }


        [WebMethod]
        [SoapHeader("authentication")]
#if(TRACE)
		[TraceExtension]
#endif
        public List<ProvisionsItem> LoadProvisions()
        {
            Function = "WSetData::LoadProvisions()";

            DProvisions DPro = new DProvisions();
            return DPro.LoadProvisions();

        }
    
    }
}
