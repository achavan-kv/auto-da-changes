'use strict';

//searchOrderService.$inject =['$scope'];

function searchOrderService() {
 return {
     receiptSearch: {
         branchNo: null,
         dateFrom:null,
         dateTo: null,
         invoiceNoMin: null,
         invoiceNoMax: null,
         start: 0,
         rows: 8
     }
 }
}



module.exports = searchOrderService;