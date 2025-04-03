using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.EOD;
using System.IO;
using System.Linq;
using STL.Common.Constants;

namespace STL.DAL
{
    /// <summary>
    /// Data access object for RI
    /// </summary>
    public class DRI : DALObject
    {
        private string filename;
        private string path;

        public DRI()
		{
		}

        public string CreateCos2RICommittedStock(int runNo, bool rerun, bool repo, out string filename, out string path)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@fileName", SqlDbType.VarChar, 40)
                {
                    Value = " ", Direction = ParameterDirection.Output
                };
                parmArray[1] = new SqlParameter("@path", SqlDbType.VarChar, 500)
                {
                    Value = " ", Direction = ParameterDirection.Output
                };
                parmArray[2] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[3] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[4] = new SqlParameter("@Repo", SqlDbType.Bit)
                {
                    Value = repo
                };

                //this.RunSP(conn, trans, "RICommittedStockSP", parmArray);
                this.RunSP("RICommittedStockSP", parmArray);

                if (parmArray[0].Value != DBNull.Value)
                    filename = Convert.ToString(parmArray[0].Value);
                if (parmArray[1].Value != DBNull.Value)
                    path = Convert.ToString(parmArray[0].Value);
                    
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                string progress = repo == false ? "Finished CommittedStock " : "Finished CommittedStock (Repossessed)";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string CreateCos2RIDeliveryTransfers(int runNo, bool rerun, bool repo, out string filename, out string path)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@fileName", SqlDbType.VarChar, 40)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[1] = new SqlParameter("@path", SqlDbType.VarChar, 500)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[2] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[3] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[4] = new SqlParameter("@Repo", SqlDbType.Bit)
                {
                    Value = repo
                };
                //this.RunSP(conn, trans, "RIDeliveryTransfersSP", parmArray);
                this.RunSP("RIDeliveryTransfersSP", parmArray);

                if (parmArray[0].Value != DBNull.Value)
                    filename = Convert.ToString(parmArray[0].Value);
                if (parmArray[1].Value != DBNull.Value)
                    path = Convert.ToString(parmArray[1].Value);                

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                //const string progress = "Finished DeliveryTransfers ";
                string progress = repo == false ? "Finished DeliveryTransfers " : "Finished DeliveryTransfers (Repossessed)";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string CreateCos2RIDeliveriesReturns(int runNo, bool rerun, bool repo, out string filename, out string path)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                
                parmArray = new SqlParameter[5];
                parmArray[0] = new SqlParameter("@fileName", SqlDbType.VarChar, 40)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[1] = new SqlParameter("@path", SqlDbType.VarChar, 500)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[2] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[3] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[4] = new SqlParameter("@Repo", SqlDbType.Bit)
                {
                    Value = repo
                };

                //this.RunSP(conn, trans, "RIDeliveriesReturnsSP", parmArray);
                this.RunSP("RIDeliveriesReturnsSP", parmArray);

                if (parmArray[0].Value != DBNull.Value)
                    filename = Convert.ToString(parmArray[0].Value);
                if (parmArray[1].Value != DBNull.Value)
                    path = Convert.ToString(parmArray[1].Value);
                  
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                string progress = repo == false ? "Finished DeliveriesReturns " : "Finished DeliveriesReturns (Repossessed)";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string CreateCos2RIRepossessions(int runNo, bool rerun, out string filename, out string path)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@fileName", SqlDbType.VarChar, 40)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[1] = new SqlParameter("@path", SqlDbType.VarChar, 500)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[2] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[3] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };

                //this.RunSP(conn, trans, "RIRepossessionsSP", parmArray);
                this.RunSP("RIRepossessionsSP", parmArray);

                if (parmArray[0].Value != DBNull.Value)
                    filename = Convert.ToString(parmArray[0].Value);
                if (parmArray[1].Value != DBNull.Value)
                    path = Convert.ToString(parmArray[0].Value);
                    
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished Repossessions ";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportRI2CosKitProductLoad(int runNo, bool rerun, out string filename, out string path)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@fileName", SqlDbType.VarChar, 40)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[1] = new SqlParameter("@path", SqlDbType.VarChar, 500)
                {
                    Value = " ",
                    Direction = ParameterDirection.Output
                };
                parmArray[2] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[3] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };

                //this.RunSP(conn, trans, "RIKitProductDataLoadSP", parmArray);
                //this.RunSP(conn, trans, "RIKitProductDataLoadSP");
                this.RunSP("RIKitProductDataLoadSP");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished Kit Product Load ";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportRI2CosKitProductImport(SqlConnection conn, SqlTransaction trans, int runNo, bool rerun, string interfaceName, bool repo) //IP - 26/08/11 - #4621 - Added repo flag
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.VarChar, 10)
                {
                    Value = interfaceName
                };
                        
                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[2] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[3] = new SqlParameter("@repo", SqlDbType.Bit)                         //IP - 26/08/11 - #4621
                {
                    Value = repo
                };

                this.RunSP("RIKitProductImportSP", parmArray);                       

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished Kit Product Import ";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportRI2CosPODetails(SqlConnection conn, SqlTransaction trans, int runNo, bool rerun, string interfaceName)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.VarChar, 10)
                {
                    Value = interfaceName
                };

                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[2] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };

                this.RunSP("RIPurchaseOrderLoadSP", parmArray);
                
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished Purchase Order Import ";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportRI2CosOnHandQty(SqlConnection conn, SqlTransaction trans, int runNo, bool rerun, bool repo, string interfaceName)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.VarChar, 10)
                {
                    Value = interfaceName
                };

                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[2] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[3] = new SqlParameter("@repo", SqlDbType.Bit)
                {
                    Value = repo
                };

                this.RunSP("RIStockQuantityLoadSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                string progress = repo == false ? "Finished On Hand Qty" : "Finished On Hand Qty  Repossessed";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportRI2CosProductInfo(SqlConnection conn, SqlTransaction trans, int runNo, bool rerun, bool repo, string interfaceName)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                // this is required to create a new instance of parmArray for each method execution
                Action setParam = () =>
                {
                    parmArray = new SqlParameter[4];

                    parmArray[0] = new SqlParameter("@interface", SqlDbType.VarChar, 10)
                    {
                        Value = interfaceName
                    };

                    parmArray[1] = new SqlParameter("@runno", SqlDbType.Int)
                    {
                        Value = runNo
                    };
                    parmArray[2] = new SqlParameter("@Rerun", SqlDbType.Bit)
                    {
                        Value = rerun
                    };
                    parmArray[3] = new SqlParameter("@repo", SqlDbType.Bit)
                    {
                        Value = repo
                    };
                };                

                // validate
                setParam();
                this.RunSP("RIProductLoadValidationSP", parmArray);
                // update
                setParam();
                this.RunSP("RIProductLoadSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                string progress = repo == false ? "Finished Product Import " : "Finished Product Import Repossessed";                
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public string ImportProductHeirachy(SqlConnection conn, SqlTransaction trans, int runNo, bool rerun, bool repo, string interfaceName)
        {
            filename = "";
            path = "";
            const string eodResult = EODResult.Pass;
            try
            {
                parmArray = new SqlParameter[4];
                parmArray[0] = new SqlParameter("@interface", SqlDbType.VarChar, 10)
                {
                    Value = interfaceName
                };

                parmArray[1] = new SqlParameter("@runno", SqlDbType.Int)
                {
                    Value = runNo
                };
                parmArray[2] = new SqlParameter("@Rerun", SqlDbType.Bit)
                {
                    Value = rerun
                };
                parmArray[3] = new SqlParameter("@repo", SqlDbType.Bit)
                {
                    Value = repo
                };

                this.RunSP("RIProductHeirarchyLoadSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                string progress = repo == false ? "Finished Product Heirachy" : "Finished Product Heirachy  Repossessed";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        public void Initialise()
        {            
            try
            {
                this.RunSP("RITruncateRawLoadTablesSP");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished Initialising ";
                Console.WriteLine(progress);
            }

        }

        //IP - 16/06/11 - CR1212 - RI - #3961
        public string DeliverWarrantyRenewals()
        {
            const string eodResult = EODResult.Pass;
            try
            {

                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@empeeno", SqlDbType.Int)
                {
                    Value = Users.RIExport
                };

                this.RunSP("DN_DeliverWarrantyRenewalSP", parmArray);

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished delivering renewal warranties";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

        //IP - 21/06/11 - CR1212 - RI - #3979
        public string CollectWarrantiesOnCredit()
        {
            const string eodResult = EODResult.Pass;
            try
            {

                this.RunSP("or_creditwarranties_return");

            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw;
            }
            finally
            {
                const string progress = "Finished collecting credit warranties not paid for";
                Console.WriteLine(progress);
            }

            return eodResult;
        }

    }
}
