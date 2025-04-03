using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blue.Cosacs.Repositories
{
    public class CacheRepository
    {
        public DateTime? CheckChange(string tableName)
        {
            using (var ctx = Context.Create())
            {
                var tableDate = (DateTime?)new CacheGetChange().ExecuteScalar(tableName);
                return tableDate;
            }
        }
    }
}
