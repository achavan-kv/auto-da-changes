using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Blue.Cosacs.Repositories;
using Blue.Cosacs.Shared;
using STL.Common;
using System.Web.Services.Protocols;
using System.Data;
using System.Data.SqlClient;
using STL.DAL;
using STL.BLL;

[WebService(Namespace = "http://schemas.bluebridgeltd.com/cosacs/stock/2011/04/")]
public class WStock : CommonService
{
    public WStock()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<StockInfo> GetStockInfo(string IUPC, bool repoItem, int? itemId, bool includeWarranties, out string err)    // RI jec
    {
        err = "";

        try
        {
            return new StockRepository()
                        .GetStockInfo(IUPC, repoItem, itemId, includeWarranties)
                        .ToList();
        }
        catch (Exception ex)
        {
            Catch(ex, "WStock::GetStockInfo()", ref err);
        }

        return new List<StockInfo>();
    }

    [WebMethod]
    [SoapHeader("authentication")]
    public List<short> GetStockLocation(int itemId, out string err)
    {
        err = "";

        try
        {
            return new StockRepository()
                        .GetStockLocation(itemId);
        }
        catch (Exception ex)
        {
            Catch(ex, "WStock::GetStockLocation()", ref err);
        }

        return new List<short>();
    }


    [WebMethod]
    [SoapHeader("authentication")]
    public void UpdateOnlineProducts(DataTable products, out string err)
    {
        err = "";

        try
            {
                SqlConnection conn = new SqlConnection(Connections.Default);
                do
                {
                    try
                    {
                        conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction(IsolationLevel.ReadUncommitted))
                        {

                            new StockRepository().UpdateOnlineProducts(conn, trans, products);
                            trans.Commit();

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
            Catch(ex, "WStock::UpdateOnlineProducts()", ref err);
        }
        
    }
}
