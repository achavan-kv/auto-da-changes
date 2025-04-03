using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Blue.Cosacs.Payments.Models;
using Blue.Cosacs.Payments.Models.WinCosacs;
using Blue.Networking;

namespace Blue.Cosacs.Payments.Repositories
{
    public class PaymentMethodMapRepository : IPaymentMethodMapRepository
    {
        private IHttpClientJson httpClientJson;
        private readonly IPaymentSetupRepository paymentSetupRepository;

        public PaymentMethodMapRepository(IHttpClientJson httpCj, IPaymentSetupRepository paymentSetupRepo)
        {
            httpClientJson = httpCj;
            paymentSetupRepository = paymentSetupRepo;
        }

        public List<PaymentMethodMapDto> Get(short branchNo)
        {
            var retLst = new List<PaymentMethodMapDto>();
            var uriString = string.Format("/Courts.NET.WS/Sales/GetPaymentMethodMapping?branchNo={0}", branchNo);
            var lst = httpClientJson.Do<byte[], List<PayMethodMap>>(RequestJson<byte[]>
                .Create(uriString, WebRequestMethods.Http.Get)).Body;
            var paymentMethods = paymentSetupRepository.GetActivePaymentMethods();

            if (lst != null && paymentMethods.Any())
            {
                retLst = (from d in paymentMethods
                          join m in lst on d.Id equals m.PosId into dm
                          from sub in dm.DefaultIfEmpty()
                          select new PaymentMethodMapDto
                          {
                              Id = d.Id,
                              Description = d.Description,
                              WinCosacsId = (sub == null ? 0 : (int)sub.WinCosacsId)
                          }).ToList();
            }

            return retLst;
        }

        public string Save(short branchNo, short posId, short winCosacsId)
        {
            var ret = "";
            var uriString = string.Format("/Courts.NET.WS/Sales/SavePaymentMethodMapping?branchNo={0}&posId={1}&winCosacsId={2}",
                branchNo, posId, winCosacsId);

            ret = httpClientJson.Do<byte[], dynamic>(RequestJson<byte[]>
                .Create(uriString, WebRequestMethods.Http.Get)).Body;
            return ret;
        }
    }
}
