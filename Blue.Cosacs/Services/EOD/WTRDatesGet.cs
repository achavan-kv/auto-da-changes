using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.EOD;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.EOD
{
	partial class Server 
    {
        public WTRDatesGetResponse Call(WTRDatesGetRequest request)
        {
            return new WTRDatesGetResponse 
            { 
                wtrDates = new EODRepository().WTRDatesGet() 
            };
        }
    }
}
