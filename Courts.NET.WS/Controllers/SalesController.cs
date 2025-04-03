using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Blue.Cosacs.Model;
using Blue.Cosacs.Repositories;

namespace Cosacs.Web.Controllers
{
    public class SalesController : Controller
    {
        public JsonResult GetLinkDiscounts(int branch, string itemNo, string department)
        {
            var repository = new SalesRepository();

            return Json(repository.GetLinkDiscounts(branch, itemNo, department), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDiscountPrice(int branch, string sku)
        {
            var repository = new SalesRepository();

            return Json(repository.GetDiscountPrice(branch, sku), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAssociatedProducts(int branch, string itemNo)
        {
            var repository = new SalesRepository();

            return Json(repository.GetAssociatedProducts(branch, itemNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetInternalInstallations(int branch, string itemNo, string taxType, decimal taxRate)
        {
            var repository = new SalesRepository();

            return Json(repository.GetInternalInstallations(branch, itemNo, taxType, taxRate), JsonRequestBehavior.AllowGet);
        }

        #region Item's Kit

        public JsonResult GetItemKitProducts(int branch, string itemNo)
        {
            var repository = new SalesRepository();
            return Json(repository.GetItemKitProducts(branch, itemNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKitDiscount(int branch, string category)
        {
            var repository = new SalesRepository();

            return Json(repository.GetKitDiscount(branch, category), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Store Card

        public JsonResult GetStoreCardAvailableBalance(long storeCardNo)
        {
            var repository = new SalesRepository();

            return Json(repository.GetStoreCardAvailableBalance(storeCardNo), JsonRequestBehavior.AllowGet);
        }

        public string GetStoreCardCustomerId(long storeCardNo)
        {
            var repository = new SalesRepository();

            return repository.GetStoreCardCustomerId(storeCardNo);
        }

        #endregion

        public JsonResult GetWarrantyContractNos(short? branch, int noOfContracts)
        {
            var repository = new SalesRepository();

            return Json(repository.GetWarrantyContractNumbers(branch, noOfContracts), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGiftVoucherDetails(char giftVoucherIssuer, string otherCompanyNo, string giftVoucherNo)
        {
            var repository = new SalesRepository();

            return Json(repository.GetGiftVoucherDetails(giftVoucherIssuer, otherCompanyNo, giftVoucherNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCashAndGoAccountNo(int branchNo)
        {
            var repository = new SalesRepository();

            return Json(repository.GetCashAndGoAccountNo(branchNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsCashierBalanceOutstanding(int userId)
        {
            var repository = new SalesRepository();
            return Json(repository.IsCashierBalanceOutstanding(userId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAvailableQuantity(string itemNo, short branchNo)
        {
            var repository = new SalesRepository();
            return Json(repository.GetAvailableQuantity(itemNo, branchNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentMethodMapping(short branchNo)
        {
            var repository = new SalesRepository();
            return Json(repository.GetPaymentMethodMapping(branchNo), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SavePaymentMethodMapping(short branchNo, short posId, short winCosacsId)
        {
            var repository = new SalesRepository();
            return Json(repository.SavePaymentMethodMapping(branchNo, posId, winCosacsId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranchDisplayType(short branchNo)
        {
            var repository = new SalesRepository();
            return Json(repository.GetBranchDisplayType(branchNo), JsonRequestBehavior.AllowGet);
        }

        //BOC Added by Suvidha - CR 2018-13 - 21/12/18 - to get the InvoiceDetails.
        public JsonResult GenerateInvoiceNumber(string branchNo)
        {
            var repository = new SalesRepository();
            //return repository.GenerateAgreementInvNum(branchNo).ToString();
            return Json(repository.GenerateAgreementInvNum(branchNo), JsonRequestBehavior.AllowGet);
        }
    }
}
