using System;
using System.Collections.Generic;
using System.Text;
using STL.Common;
using STL.DAL;
using System.Data;

namespace STL.BLL
{

    public class BLoyalty
    {
        DLoyalty dloyal;

        public BLoyalty()
        {
            dloyal = new DLoyalty();
        }

        public Loyalty LoyaltyGetByCustid(string custid)
        {
            return dloyal.LoyaltyGetByCustid(custid);
        }

        public Loyalty LoyaltyGetByMemberno(string memberno)
        {
            return dloyal.LoyaltyGetByMemberno(memberno);
        }

        //public Loyalty LoyaltyGetData(string custid, string acctno)
        //{
        //    return dloyal.LoyaltyGetData(custid, acctno);
        //}

        public DataSet GetLoyaltyDropData()
        {
            return dloyal.GetLoyaltyDropData();
        }

        public string LoyaltySave(Loyalty loyalty)
        {
            return dloyal.LoyaltySave(loyalty);
        }

        public void LoyaltyAddFee(string acctno, string membertype, string custid,int user)
        { 
            dloyal.LoyaltyAddFee(acctno, membertype, custid, user);
        }

        public bool LoyaltyCheckCustomer(string custid)
        {
            return dloyal.LoyaltyCheckCustomer(custid);
        }

        public string LoyaltyGetCashAccount(string custid)
        {
            return dloyal.LoyaltyGetCashAccount(custid);
        }

        public List<LoyaltyVoucher> LoyaltyGetVouchers(string custid)
        {
            return dloyal.LoyaltyGetVouchers(custid);
        }

        public void LoyaltyAddRejection(string custid, string acctno)
        {
            dloyal.LoyaltyAddRejection(custid,acctno);
        }

        public void LoyaltyGetCharges(string custid, ref string acctno, ref decimal amount, ref bool active )
        {
            dloyal.LoyaltyGetCharges(custid, ref acctno, ref amount, ref active);
        }

        public void LoyaltySaveVouchers(int voucher, bool add,string acctno)
        {
            dloyal.LoyaltySaveVouchers(voucher, add, acctno);
        }

      

        public bool LoyaltyIsLinkAccount(string acctno, string custid)
        {
            return dloyal.LoyaltyIsLinkAccount(acctno, custid);
        }

         public Loyalty LoyaltyGetDatabyacctno(string acctno)
        {
            return dloyal.LoyaltyGetByAcctno(acctno);
        }

         public void LoyaltyPay(string acctno)
         {
             dloyal.LoyaltyPay(acctno);
         }

         public List<LoyaltyValidAddresses> LoyaltyGetValidAddress(string custid)
         {
             return dloyal.LoyaltyGetValidAddress(custid);
         }

         public void LoyaltyCancelAccount(string acctno, int user)
         {
             dloyal.LoyaltyCancelAccount(acctno, user);
         }

         public bool LoyaltyCheckAccountPeriod(string acctno)
         {
             return dloyal.LoyaltyCheckAccountPeriod(acctno);
         }

         public void LoyaltyCheckRemoveFreeDel(string acctno, int user)
         {
             dloyal.LoyaltyCheckRemoveFreeDel(acctno, user);
         }

         public void LoyaltyGRTCancelFreeDelAdjust(string acctno, int user)
         {
             dloyal.LoyaltyGRTCancelFreeDelAdjust(acctno, user);
         }

         public DataTable LoyaltyGetHistory(string custid)
         {
             return dloyal.LoyaltyGetHistory(custid);
         }


         public bool LoyaltyCheckLinkedAccount(string acct)
         {
             return dloyal.LoyaltyCheckLinkedAccount(acct);
         }

         public decimal? LoyaltyGetVoucherValue(string acct)
         {
             return dloyal.LoyaltyGetVoucherValue(acct);
         }

         public void LoyaltyRemoveVoucher(string acct)
         {
             dloyal.LoyaltyRemoveVoucher(acct);
         }
   
    }
}
