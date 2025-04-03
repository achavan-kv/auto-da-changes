using System;
using System.Collections.Generic;
using System.Text;
using Blue.Cosacs.Shared.Services.Warehouse;
using Blue.Cosacs.Repositories;

namespace Blue.Cosacs.Services.Warehouse
{
	partial class Server 
    {
        public SaveLineItemFailureNotesResponse Call(SaveLineItemFailureNotesRequest request)
        {
            return Context.ExecuteTx((ctx, connection, transaction) =>
                {
                    new AccountRepository().SaveLineItemFailureNotes(connection, transaction, request.AcctNo, request.Notes, request.EmpeeNo);
                    return new SaveLineItemFailureNotesResponse();
                });

        }
    }
}
