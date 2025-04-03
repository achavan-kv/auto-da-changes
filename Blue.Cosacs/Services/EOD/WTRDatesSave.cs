using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.EOD;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.EOD
{
	partial class Server 
    {
        public WTRDatesSaveResponse Call(WTRDatesSaveRequest request)
        {
            EODRepository eod = new EODRepository();
                
            eod.WTRDatesSave(request.WTRDates);
 
            return new WTRDatesSaveResponse();
          
        }

    }
}
