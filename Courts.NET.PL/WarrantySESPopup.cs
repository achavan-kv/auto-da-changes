using System;
using System.Data;
using System.Windows.Forms;
using STL.Common;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.Warranty;
using System.Collections.Generic;
using STL.Common.Services.Model;
using Blue.Cosacs.Shared;

namespace STL.PL
{
    public partial class WarrantySESPopup : CommonForm
    {

        public WarrantySESPopup(TranslationDummy d)
        {
            InitializeComponent();

        }

        public WarrantySESPopup(List<WarrantyResultFlat> results, DataTable itemNoWarranty)
        {
            InitializeComponent();

            dgvWarrantyItems.DataSource = filteredResult(results);          // #16019

            dgvWarrantyItems.Columns["Id"].Visible = false;
            dgvWarrantyItems.Columns["CostPrice"].Visible = false;
            dgvWarrantyItems.Columns["TaxRate"].Visible = false;
            dgvWarrantyItems.Columns["WarrantyType"].Visible = false;       // #18042
            dgvWarrantyItems.Columns[CN.Code].Visible = false;
            dgvWarrantyItems.Columns["Location"].Visible = false;
            dgvWarrantyItems.Columns[CN.Code].HeaderText = GetResource("T_ITEMNO");
            dgvWarrantyItems.Columns[CN.Code].Width = 70;
            dgvWarrantyItems.Columns["Length"].Width = 50;
            dgvWarrantyItems.Columns["Description"].HeaderText = GetResource("T_DESCRIPTION");
            dgvWarrantyItems.Columns["Description"].Width = 120;

            
            items = itemNoWarranty;
            items.TableName = "WarrantyList";
            ds.Tables.Add(items);
        }

        private DataTable items = new DataTable();
        private DataSet ds = new DataSet();

        //public WarrantySESPopup(DataSet dsCustomerWarrantySolicitation)
        //{
        //    InitializeComponent();

        //    dgvWarrantyItems.DataSource = dsCustomerWarrantySolicitation;
        //    dgvWarrantyItems.DataMember = TN.WarrantyList;

        //    dgvWarrantyItems.Columns[CN.NoOfPrompts].Visible = false;
        //    dgvWarrantyItems.Columns[CN.AccountNumber].HeaderText = GetResource("T_ACCTNO");
        //    dgvWarrantyItems.Columns[CN.AccountNumber].Width = 90;
        //    dgvWarrantyItems.Columns[CN.ItemNo].HeaderText = GetResource("T_ITEMNO");
        //    dgvWarrantyItems.Columns[CN.ItemNo].Width = 70;
        //    dgvWarrantyItems.Columns[CN.ItemDescr1].HeaderText = GetResource("T_DESCRIPTION");
        //    dgvWarrantyItems.Columns[CN.ItemDescr1].Width = 260;
        //}

        private void SaveConfirmation()
        {
            CustomerManager.SaveWarrantySESConfirmation(ds, Credential.UserId, out Error);
            //if (Error.Length > 0) ShowError(Error);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void WarrantySESPopup_Load(object sender, EventArgs e)
        {

        }

        private void WarrantySESPopup_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CustId"></param>
        /// <param name="owner"></param>
        /// <param name="showAllItems">Shows all items regardless of how many times they have been prompted for</param>
        public static void ShowSESPopUp(string CustId, CommonForm owner, bool showAllItems)
        {
            int noSESPrompts;

            if (showAllItems)
                noSESPrompts = 1000; //Large number to ensure all items are shown in the prompt
            else
                noSESPrompts = Convert.ToInt32(owner.Country[CountryParameterNames.WarrantySESPrompts]);

            if (noSESPrompts > 0)
            {
                string err = "";
                int daysAllowedAfterDelivery = Convert.ToInt32(owner.Country[CountryParameterNames.WarrantyDays]);

                //DataSet ds = owner.CustomerManager.GetWarrantySecondEffortSolicitationItems(CustId, noSESPrompts, daysAllowedAfterDelivery, out err);
                //if (err.Length > 0) owner.ShowError(err);

                //if (ds.Tables.IndexOf(TN.WarrantyList) >= 0)
                //    if (ds.Tables[TN.WarrantyList].Rows.Count > 0)
                //    {
                //        WarrantySESPopup pu = new WarrantySESPopup(ds);
                //        pu.ShowDialog((IWin32Window)owner);
                //        pu.SaveConfirmation();
                //    }

                //Arguably the shittest code i have ever written.
                Client.Call(new GetAvailableWarrantiesRequest { CustomerId = CustId }, response =>
                {
                    var warrantyAdditional = FormatResponse(response.WarrantyResult);

                    if (warrantyAdditional.Count > 0)
                    {
                        WarrantySESPopup pu = new WarrantySESPopup(warrantyAdditional, response.items);
                        pu.ShowDialog((IWin32Window)owner);
                        pu.SaveConfirmation();
                    }
                });
            }
        }

        private static new List<WarrantyResultFlat> FormatResponse(WarrantyResult[] warrantyResult)
        {
            var warranty = new List<WarrantyResultFlat>();
            if (warrantyResult != null && warrantyResult.Length > 0)
            {
                foreach (var result in warrantyResult)
                    warranty.AddRange(result.ToFlat());
            }
            return warranty;
        }

        public List<WarrantyResultFlat> filteredResult(List<WarrantyResultFlat> results)
        {
            List<WarrantyResultFlat> listWarranties = new List<WarrantyResultFlat>();
            List<string> itemsAdded = new List<string>();       //#15618
            
            foreach (var item in results)
            {
                if (WarrantyType.IsFree(item.WarrantyType) == false && !(itemsAdded.Contains(item.Code))) //#17883 //#15618
                {
                     listWarranties.Add(item);
                     itemsAdded.Add(item.Code);
                }
            }

            return listWarranties;
        }

    }
}