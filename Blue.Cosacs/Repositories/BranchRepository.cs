using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Model;

namespace Blue.Cosacs.Repositories
{
    public class BranchRepository
    {
        public List<string> GetBranchList()
        {
            using (var ctx = Context.Create())
            {
                return ctx.Branch
                       .Select(b => b.branchno.ToString()).ToList();

            }
        }

        public List<Blue.Cosacs.Model.BranchInfo> GetBranchInfo()
        {
            using (var ctx = Context.Create())
            {
                return (from b in ctx.Branch
                        select new Blue.Cosacs.Model.BranchInfo()
                        {
                            BranchNumber = b.branchno,
                            WarehouseNumber = int.Parse(b.warehouseno),
                            BranchName = b.branchname,
                            StoreType = b.StoreType,
                            CountryCode = b.countrycode,
                            BranchAddress1 = b.branchaddr1,
                            BranchAddress2 = b.branchaddr2,
                            BranchAddress3 = b.branchaddr3,
                        })
                        .ToList();

            }
        }

        public DataTable GetUpliftCommissionRates(SqlConnection conn, SqlTransaction trans)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var users = (from u in ctx.UserRoleView
                                 //where li.acctno == acctNo
                                 select new {
                                     u.Id,
                                     u.FullName,
                                     u.UpliftCommissionRate,
                                     currentRate = u.UpliftCommissionRate,
                                     u.RoleId,
                                     u.Name
                                 }).ToList();
                
                return users.ToDataTable();
            }
        }

        public void SaveUpliftCommissionRates(SqlConnection conn, SqlTransaction trans, DataTable upliftRates)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                foreach (DataRow dr in upliftRates.Rows)
                {
                    var cp = ctx.CourtsPersonTable.Where(s => s.UserId == Convert.ToInt32(dr["Id"])).FirstOrDefault();
                    
                    cp.UpliftCommissionRate = Convert.ToDouble(dr["UpliftCommissionRate"]);

                    ctx.SubmitChanges();
                }

            }
        }

        public DataTable GetUserRoles(SqlConnection conn, SqlTransaction trans)
        {
            using (var ctx = Context.Create(conn, trans))
            {
                var roles = (from r in ctx.RoleView
                             //where li.acctno == acctNo
                             select new
                             {
                                 r.Name,
                                 r.Id
                             }).ToList();

                return roles.ToDataTable();
            }
        }

        public BranchNonCourtsStoreType GetNonCourtsStoreType(short branchNo)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                {
                    var nonCourtsStoreType = (from b in ctx.Branch
                                where b.branchno == branchNo
                                select new BranchNonCourtsStoreType
                                {
                                    LuckyDollarStore = b.LuckyDollarStore,
                                    AshleyStore = b.AshleyStore,
                                    RadioShackStore = b.RadioShackStore,
                                    //OmniStore = b.OmniStore,
                                    DisplayType = b.DisplayType
                                }).FirstOrDefault();

                    return nonCourtsStoreType;
                });
        }
     
    }
}