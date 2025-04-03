using System;
using Unicomer.Cosacs.Repository;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicomer.Cosacs.Repository
{
    public class ApiTokenKeyRepository
    {
        public string ReadWriteToken(DateTime expiration, string token = "")
        {
            string resultToken = string.Empty;
            var ds = new DataSet();
            var RT = new ReadWriteToken();
            RT.ReadWriteTokenDeatils(ds, expiration, token);
            //TokenFilterModel TFM = new TokenFilterModel();
            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows != null && ds.Tables[0].Rows.Count > 0)
                resultToken = Convert.ToString(ds.Tables[0].Rows[0]["token"]);

            return resultToken;
        }
    }
}
