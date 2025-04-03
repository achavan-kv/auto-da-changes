using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Credit;
using Blue.Cosacs.Repositories;
using System.Data.SqlClient;
using System.Data;

namespace Blue.Cosacs.Services.Credit
{
	partial class Server 
    {
        public CompleteReferralStageResponse Call(CompleteReferralStageRequest request)
        { 
               var CRF = new CompleteReferralStageResponse();
            var Crep= new CreditRepository();
            var conn = new SqlConnection(STL.DAL.Connections.Default); conn.Open();
         
            try
            {
                using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                {

                    Crep.CompleteReferralStage(conn, trans, request);
                    trans.Commit();
                    CRF.Errormessage = "Ok";
                }
            }
            catch
            {

                CRF.Errormessage = "Fail CompleteReferralStage Call ";

            }

            return CRF;
            
        }
    }
}
