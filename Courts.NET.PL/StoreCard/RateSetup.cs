using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.PL.WSStoreCard;
using Blue.Cosacs.Shared.Extensions;
using System.Text.RegularExpressions;

namespace STL.PL
{
    public partial class StoreCardRatesSetup : CommonForm
    {

        private BindingList<Blue.Cosacs.Shared.StoreCardRateDetails> details = null;
        private List<Blue.Cosacs.Shared.StoreCardRate> Ratelist;
        private BindingList<Blue.Cosacs.Shared.StoreCardRate> bList = null;

        int currentlistindex = 0;

        bool loading = false;
        bool Edit = false;
        bool FirstTime = true;
        private int newNegativeId = 0; // this will be used for negatively incrementing a new id after each clicking on the new button. When saved this will generate the id and bring back a normal positive one...
      
        public StoreCardRatesSetup(Form root, Form parent)
        {
            InitializeComponent();
           
            FormRoot = root;
            FormParent = parent;
        
            dgv_ratesetup.AutoGenerateColumns = false;
            GridSetup(new List<KeyValuePair<String, String>>(){
                new KeyValuePair<String, String>("AppScoreFrom", "App Score From"),
                new KeyValuePair<String, String>("AppScoreTo", "App ScoreTo"),
                new KeyValuePair<String, String>("BehaveScoreFrom", "Behave Score From"),
                new KeyValuePair<String, String>("BehaveScoreTo", "Behave Score To"),
                new KeyValuePair<String, String>("PurchaseInterestRate", "Purchase Interest Rate") 
            });
            LoadAll();
            loaded = true;
        }

        private void btn_addnew_Click(object sender, EventArgs e)
        {
            loading = true;
            pnl_main.Visible = true;
            newNegativeId--;
            var newRate = new Blue.Cosacs.Shared.StoreCardRate() { IsDefaultRate = false };
            newRate.RateDetails = new List<Blue.Cosacs.Shared.StoreCardRateDetails>(); 

            newRate.Name = "New Rate " + Math.Abs(newNegativeId).ToString();
            newRate.RateFixed = chk_FixedRate.Checked;
            newRate.Id = newNegativeId;
            if (details !=null)
                details.Clear();

            txt_ratename.Text = newRate.Name;
            bList.Add(newRate);
            UpdateListBox();
            loading = false;
            lbx_ratenames.SelectedIndex = bList.Count - 1;
            currentlistindex = bList.Count - 1;
          //  
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Edit)
            {
                if (MessageBox.Show("The data has been changed, are you sure you want to refresh and lose changes?", "Refresh will lose current changes.", MessageBoxButtons.YesNo) == DialogResult.Yes)
                { 
                    LoadAll();
                    Edit = false;
                }
            }
        }

        private void UpdateListBox()
        {
              
                txt_ratename.DataBindings.Clear();
                txt_ratename.DataBindings.Add("Text", bList, "Name");
                SetDefaultName();
                lbx_ratenames.DataSource = null;
                lbx_ratenames.DisplayMember = "Name";
                lbx_ratenames.ValueMember = "IsDefaultRate";
                lbx_ratenames.DataSource = bList;
        }

        private void SetDefaultName()
        {
            foreach (var rate in bList)
            {
                if (rate.Name.IndexOf(" (Default)") != -1)
                    rate.Name = rate.Name.Substring(0, rate.Name.Length - 10);
                if (rate.IsDefaultRate)
                {
                    rate.Name += " (Default)";
                }
            }
        }

        private void RemoveDefaultName()
        {
            foreach (var rate in bList)
            {
                if (rate.Name.IndexOf(" (Default)") != -1)
                    rate.Name = rate.Name.Substring(0, rate.Name.Length - 10);
            }
        }

        private void GridSetup(List<KeyValuePair<String, String>> displayValueMembers)
        {
            foreach (var entry in displayValueMembers)
            {
                var txtColumn = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = entry.Key,
                    HeaderText = entry.Value,
                    Name = entry.Key
                };
                txtColumn.AutoSizeMode =  DataGridViewAutoSizeColumnMode.AllCells;
                dgv_ratesetup.Columns.Add(txtColumn);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            var index = lbx_ratenames.SelectedIndex;

            if (index != -1)
            {
                //Check if already saved first - if so then the Rateid will be > 0...
                if (bList[index].Id < 0)
                {
                    RemoveRate(index);
                }
                else
                {
                    if (!StoreCardManager.RateInUse(bList[index].Id))
                    {
                        RemoveRate(index);
                    }
                    else
                    {
                        MessageBox.Show("This rate is in use. No deletion possible.", "Unable to delete - in use.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            
        }

        private void RemoveRate(int index)
        {
            if (bList[index].IsDefaultRate == true)
            {
                bList[0].IsDefaultRate = true;
            }
            bList.RemoveAt(index);

            if (bList.Count == 0)
            {
                pnl_main.Visible = false;
            }
        }
     
        private void txt_ratename_TextChanged(object sender, EventArgs e)
        {
            if (!loading && !FirstTime)
            {
                Edit = true;
                FirstTime = false;
            }  
            errorProvider1.SetError(btnSave, "");
            //bList[lbx_ratenames.SelectedIndex].Modified = true;
         
        }

        private void dgv_ratesetup_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!loading)
            {
                Edit = true;
                bList[lbx_ratenames.SelectedIndex].Modified = true;
            }
        }


        private void lbx_ratenames_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                pnl_main.Visible = true;
                if (lbx_ratenames.SelectedItem != null)
                {

                    if (((Blue.Cosacs.Shared.StoreCardRate)lbx_ratenames.SelectedItem).RateDetails == null)
                        ((Blue.Cosacs.Shared.StoreCardRate)lbx_ratenames.SelectedItem).RateDetails = new List<Blue.Cosacs.Shared.StoreCardRateDetails>();

                    dgv_ratesetup.DataSource = new BindingList<Blue.Cosacs.Shared.StoreCardRateDetails>(((Blue.Cosacs.Shared.StoreCardRate)lbx_ratenames.SelectedItem).RateDetails);
                }
                ValidateCells();
                currentlistindex = lbx_ratenames.SelectedIndex;
            }
        }

        private void txt_ratename_Leave(object sender, EventArgs e)
        {
            if (!loading)
                bList[currentlistindex].Modified = true;
        }

        //private void CheckDupNames()
        //{
        //    //RemoveDefaultName();
        //    //if (Ratelist.FindAll(delegate(Blue.Cosacs.Shared.StoreCardRate SCR) { return SCR.Name.ToUpper() == txt_ratename.Text.Trim().ToUpper(); }).Count > 1)
        //    //{
        //    //    txt_ratename.Text += "[dup]";                
        //    //}
        //    //else
        //    //SetDefaultName();
        //}

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveAll();
        }

    

        //private void InterestChange()
        //{
        //    if (!loading)
        //    {
        //        if (currentlistindex != -1)
        //        {
        //             bList[currentlistindex].Modified = true;
        //        }
        //    }
        //}
             private void LoadAll()
             {
                 newNegativeId = 0;
                 txt_ratename.Text = string.Empty;
                 dgv_ratesetup.DataSource = null;
                 pnl_main.Visible = false;
                 Ratelist = ConvertToList(StoreCardManager.GetStoreCardRates());
                 bList = new BindingList<Blue.Cosacs.Shared.StoreCardRate>(Ratelist);
                 chk_FixedRate.DataBindings.Clear();
                 chk_FixedRate.DataBindings.Add(new Binding("Checked", bList, "RateFixed"));
                 currentlistindex = 0;
                 UpdateListBox();
                 ValidateCells();
              
                 Edit = false;
             }

        private List<Blue.Cosacs.Shared.StoreCardRate> ConvertToList(StoreCardRate[] rates)
        { 
            var allrates = new List<Blue.Cosacs.Shared.StoreCardRate>();
            foreach (var rate in rates)
            {
                var newrate = new Blue.Cosacs.Shared.StoreCardRate
                {
                    Id = rate.Id,
                    IsDefaultRate = rate.IsDefaultRate,
                    IsDeleted = rate.IsDeleted,
                    Name = rate.Name,
                    Rate = rate.Rate,
                    RateFixed = rate.RateFixed,
                    Version = rate.Version,
                    Modified = rate.Modified,
                    RateDetails = new List<Blue.Cosacs.Shared.StoreCardRateDetails>()
                };

                foreach (var rd in rate.RateDetails)
                {
                    newrate.RateDetails.Add(new Blue.Cosacs.Shared.StoreCardRateDetails() { 
                        Id = rd.Id,
                        AppScoreFrom = rd.AppScoreFrom,
                        AppScoreTo = rd.AppScoreTo,
                        BehaveScoreFrom = rd.BehaveScoreFrom, 
                        BehaveScoreTo = rd.BehaveScoreTo,
                        CreditLimitPercent = rd.CreditLimitPercent,
                        ParentID = rd.ParentID, 
                        PurchaseInterestRate = rd.PurchaseInterestRate
                    });
                }
                allrates.Add(newrate);
            }

            return allrates;
        }

        private StoreCardRate[] ConvertToArray(List<Blue.Cosacs.Shared.StoreCardRate> rates)
        
        {
            var allrates = new List<StoreCardRate>();
            foreach (var rate in rates)
            {
                var newrate = new StoreCardRate
                {
                    Id = rate.Id,
                    IsDefaultRate = rate.IsDefaultRate,
                    IsDeleted = rate.IsDeleted,
                    Name = rate.Name,
                    Rate = rate.Rate,
                    RateFixed = rate.RateFixed,
                    Version = rate.Version,
                    Modified = rate.Modified
                };
                var ratedetails = new List<StoreCardRateDetails>();
                foreach (var rd in rate.RateDetails)
                {
                    ratedetails.Add(new StoreCardRateDetails()
                    {
                        Id = rd.Id,
                        AppScoreFrom = rd.AppScoreFrom,
                        AppScoreTo = rd.AppScoreTo,
                        BehaveScoreFrom = rd.BehaveScoreFrom,
                        BehaveScoreTo = rd.BehaveScoreTo,
                        CreditLimitPercent = rd.CreditLimitPercent,
                        ParentID = rd.ParentID,
                        PurchaseInterestRate = rd.PurchaseInterestRate
                    });
                }
                newrate.RateDetails = ratedetails.ToArray();
                allrates.Add(newrate);
            }
            return allrates.ToArray();
        }
        private void SaveAll()
        {
            if (ValidateAll(Ratelist))
            {
                RemoveDefaultName();
                StoreCardManager.SaveStoreCardRates(ConvertToArray(Ratelist));
                SetDefaultName();
                if (CheckEmpty())
                     ((MainForm)this.FormRoot).ShowStatus("StoreCard Rates Saved Sucessfully. (Empty rates ignored)");
                else
                     ((MainForm)this.FormRoot).ShowStatus("StoreCard Rates Saved Sucessfully.");
                LoadAll();
                Edit = false;
            }
            else
                MessageBox.Show("There are invalid entries. Please check before saving.", "Warning - Changes not saved!", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool CheckEmpty()
        { 
            foreach (var rate in Ratelist)
            {
                if (rate.RateDetails.Count == 0)
                return true;
            }
            return false;
        }


        private void ValidateCells()
        {
            ResetErrors();
            var ratelistvalidation = new List<Rate>();
            foreach (DataGridViewRow row in dgv_ratesetup.Rows)
            {
                CheckValidDecimal(row);
                if (!row.IsNewRow)
                {
                    var rate = new Rate
                    {
                        PurchaseInterestRate = row.Cells["PurchaseInterestRate"].Value != null ? Convert.ToDecimal(row.Cells["PurchaseInterestRate"].Value) : (decimal?)null,
                        AppScoreFrom = row.Cells["AppScoreFrom"].Value != null ? Convert.ToDecimal(row.Cells["AppScoreFrom"].Value) : (decimal?)null,
                        AppScoreTo = row.Cells["AppScoreTo"].Value != null ? Convert.ToDecimal(row.Cells["AppScoreTo"].Value) : (decimal?)null,
                        BehaveScoreTo = row.Cells["BehaveScoreTo"].Value != null ? Convert.ToDecimal(row.Cells["BehaveScoreTo"].Value) : (decimal?)null,
                        BehaveScoreFrom = row.Cells["BehaveScoreFrom"].Value != null ? Convert.ToDecimal(row.Cells["BehaveScoreFrom"].Value) : (decimal?)null,
                        CurrentRow = row
                    };
                    rate.ValidationFailed += new Rate.ValidationEvent(delegate(string name, DataGridViewRow currentrow)
                    {
                        switch (name)
                        {
                            case "PurchaseInterestRate": currentrow.Cells["PurchaseInterestRate"].ErrorText = "Please enter a valid value."; break;
                            case "AppScoreFrom": currentrow.Cells["AppScoreFrom"].ErrorText = "Please enter a valid value."; break;
                            case "AppScoreTo": currentrow.Cells["AppScoreTo"].ErrorText = "Please enter a valid value."; break;
                            case "BehaveScoreFrom": currentrow.Cells["BehaveScoreFrom"].ErrorText = "Please enter a valid value."; break;
                            case "BehaveScoreTo": currentrow.Cells["BehaveScoreTo"].ErrorText = "Please enter a valid value."; break;
                            case "AppScoreFromRange": currentrow.Cells["AppScoreFrom"].ErrorText = "Value conflicts with other score ranges."; break;
                            case "AppScoreToRange": currentrow.Cells["AppScoreTo"].ErrorText = "Value conflicts with other score ranges."; break;
                            case "BehaveScoreFromRange": currentrow.Cells["BehaveScoreFrom"].ErrorText = "Value conflicts with other score ranges."; break;
                            case "BehaveScoreToRange": currentrow.Cells["BehaveScoreTo"].ErrorText = "Value conflicts with other score ranges."; break;
                            case "PurchaseInterestRateDup": currentrow.Cells["PurchaseInterestRate"].ErrorText = "This value has already been entered for another range."; break;

                            default:
                                break;
                        }
                    });
                    rate.Validate();
                    ratelistvalidation.Add(rate);
                }
            } 
            ValidateRates(ratelistvalidation);
        }

        private void ResetErrors()
        {
            foreach (DataGridViewRow row in dgv_ratesetup.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ErrorText = "";
                }
            }
        }

        public void CheckValidDecimal(DataGridViewRow row)
        {
            decimal d;
            if (row.Cells["AppScoreFrom"].Value != null && !decimal.TryParse(row.Cells["AppScoreFrom"].Value.ToString(), out d))
                row.Cells["AppScoreFrom"].Value = null;
            if (row.Cells["AppScoreTo"].Value != null && !decimal.TryParse(row.Cells["AppScoreTo"].Value.ToString(), out d))
                row.Cells["AppScoreTo"].Value = null;
            if (row.Cells["BehaveScoreFrom"].Value != null && !decimal.TryParse(row.Cells["BehaveScoreFrom"].Value.ToString(), out d))
                row.Cells["BehaveScoreFrom"].Value = null;
            if (row.Cells["BehaveScoreTo"].Value != null && !decimal.TryParse(row.Cells["BehaveScoreTo"].Value.ToString(), out d))
                row.Cells["BehaveScoreTo"].Value = null;
            if (row.Cells["PurchaseInterestRate"].Value != null && !decimal.TryParse(row.Cells["PurchaseInterestRate"].Value.ToString(), out d))
                row.Cells["PurchaseInterestRate"].Value = null;
        }

      

        public void ValidateRates(List<Rate> rates)
        {
            for (var i = 0;i < rates.Count;i++)
            {
                for (var j = 0;j < rates.Count;j++)
                {
                    if (i != j) 
                    { 
                        if (rates[i].AppScoreFrom.HasValue &&  rates[j].AppScoreTo.HasValue && rates[j].AppScoreFrom.HasValue &&
                           (rates[i].AppScoreFrom.Value >= rates[j].AppScoreFrom.Value && rates[i].AppScoreFrom.Value <= rates[j].AppScoreTo.Value))
                        {
                            rates[i].OnValidationFailed("AppScoreFromRange",rates[i].CurrentRow);
                        }

                        if (rates[i].AppScoreTo.HasValue && rates[j].AppScoreTo.HasValue && rates[j].AppScoreFrom.HasValue &&
                          (rates[i].AppScoreTo.Value >= rates[j].AppScoreFrom.Value && rates[i].AppScoreTo.Value <= rates[j].AppScoreTo.Value))
                        {
                            rates[i].OnValidationFailed("AppScoreToRange", rates[i].CurrentRow);
                        }

                        if (rates[i].BehaveScoreFrom.HasValue && rates[j].BehaveScoreTo.HasValue && rates[j].BehaveScoreFrom.HasValue &&
                          (rates[i].BehaveScoreFrom.Value >= rates[j].BehaveScoreFrom.Value && rates[i].BehaveScoreFrom.Value <= rates[j].BehaveScoreTo.Value))
                        {
                            rates[i].OnValidationFailed("BehaveScoreFromRange", rates[i].CurrentRow);
                        }

                        if (rates[i].BehaveScoreTo.HasValue && rates[j].BehaveScoreTo.HasValue && rates[j].BehaveScoreFrom.HasValue &&
                          (rates[i].BehaveScoreTo.Value >= rates[j].BehaveScoreFrom.Value && rates[i].BehaveScoreTo.Value <= rates[j].BehaveScoreTo.Value))
                        {
                            rates[i].OnValidationFailed("BehaveScoreToRange", rates[i].CurrentRow);
                        }

                        if (rates[i].PurchaseInterestRate.HasValue && rates[j].PurchaseInterestRate.HasValue && rates[i].PurchaseInterestRate.Value == rates[j].PurchaseInterestRate.Value)
                        {
                            rates[i].OnValidationFailed("PurchaseInterestRateDup", rates[i].CurrentRow);
                        }
                    }
                }
            }
        }

        


        private bool ValidateAll(List<Blue.Cosacs.Shared.StoreCardRate> storeCardRates)
        {
            var ratelistvalidation = new List<Rate>();

            if (!CheckDefault())
            {
                errorProvider1.SetError(btn_makeDefault, "Please set a default rate.");
                return false;  
            }

            if (!CheckDupNamesValidation())
            {
                errorProvider1.SetError(btnSave, "Please change duplicate rate names.");
                return false;
            }
            SetDefaultName();

            foreach (var rate in storeCardRates)
            {
                ratelistvalidation = new List<Rate>();
                foreach (var ratedetails in rate.RateDetails)
                {
                  
                    ratelistvalidation.Add(new Rate { 
                                                        AppScoreFrom = ratedetails.AppScoreFrom, 
                                                        AppScoreTo = ratedetails.AppScoreTo,
                                                        BehaveScoreFrom = ratedetails.BehaveScoreFrom,
                                                        BehaveScoreTo = ratedetails.BehaveScoreTo,
                                                        PurchaseInterestRate = ratedetails.PurchaseInterestRate
                                                     });
                }

                foreach (var ratevalidation in ratelistvalidation)
                {
                    ratevalidation.Validate();
                    if (ratevalidation.Failed)
                        return false;
                }
            }
            return true;
        }

        private bool CheckDupNamesValidation()
        {
            RemoveDefaultName();
            for (var i = 0; i < bList.Count; i++)
            {
                for (var j = 0; j < bList.Count; j++)
                { 
                    if (i!=j)
                    {
                        if (bList[i].Name.ToUpper().Trim() == bList[j].Name.ToUpper().Trim())
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
           
        }

        private void btn_makeDefault_Click(object sender, EventArgs e)
        {
            errorProvider1.SetError(btn_makeDefault, "");
            if (currentlistindex != -1 && bList.Count > 0)
            {
                if (bList[currentlistindex].RateDetails.Count > 0)
                {
                    foreach (var rate in bList)
                    {
                        if (rate.IsDefaultRate)
                        {
                            rate.IsDefaultRate = false;
                            rate.Modified = true;
                        }
                    }
                    bList[currentlistindex].IsDefaultRate = true;
                    bList[currentlistindex].Modified = true;
                    SetDefaultName();

                    lbx_ratenames.DataSource = null;
                    lbx_ratenames.DisplayMember = "Name";
                    lbx_ratenames.ValueMember = "IsDefaultRate";
                    lbx_ratenames.DataSource = bList;
                }
                else
                {
                    errorProvider1.SetError(btn_makeDefault, "Default rate selected is empty.");
                }
            }
        }

        private bool CheckDefault()
        {
            foreach (var rate in bList)
            {
                if (Convert.ToBoolean(rate.IsDefaultRate) && rate.RateDetails.Count > 0)
                    return true;
            }
            return false;
        }


        private void dgv_ratesetup_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            if (loaded)
                ValidateCells();
        }

        private void dgv_ratesetup_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context == DataGridViewDataErrorContexts.Parsing)
            {
                dgv_ratesetup[e.ColumnIndex, e.RowIndex].ErrorText = "Please enter a valid decimal";
            }
        }

        private void chk_FixedRate_CheckedChanged(object sender, EventArgs e)
        {
            bList[currentlistindex].Modified = true;
        }
    }


    public class Rate
    {
        public decimal? AppScoreFrom { get; set; }
        public decimal? AppScoreTo { get; set; }
        public decimal? BehaveScoreFrom { get; set; }
        public decimal? BehaveScoreTo { get; set; }
        public decimal? PurchaseInterestRate { get; set; }
        public DataGridViewRow CurrentRow { get; set; }
        public bool Failed { get; set; }

        public delegate void ValidationEvent(string name, DataGridViewRow row);
        public event ValidationEvent ValidationFailed;

        public void OnValidationFailed(string validationid, DataGridViewRow row)
        {
            Failed = true;

            if (validationid != null && CurrentRow != null)
                this.ValidationFailed(validationid, row);
        }

        public bool Validate()
        {
            if(!(PurchaseInterestRate.HasValue && 
                 PurchaseInterestRate.Value > 0 &&
                 PurchaseInterestRate.Value <= 200))
                OnValidationFailed("PurchaseInterestRate", CurrentRow);

            if (AppScoreTo.HasValue && !AppScoreFrom.HasValue ||
                (AppScoreFrom.HasValue && AppScoreFrom.Value < 0))
                OnValidationFailed("AppScoreFrom", CurrentRow);

            if (!AppScoreTo.HasValue && AppScoreFrom.HasValue || 
                (AppScoreTo.HasValue && AppScoreTo.Value < 0) ||
                (AppScoreTo.HasValue && AppScoreFrom.HasValue && (AppScoreTo.Value < 0) || AppScoreTo < AppScoreFrom))
                OnValidationFailed("AppScoreTo", CurrentRow);

            if (BehaveScoreTo.HasValue && !BehaveScoreFrom.HasValue ||
                (BehaveScoreFrom.HasValue && BehaveScoreFrom < 0))
                OnValidationFailed("BehaveScoreFrom", CurrentRow);

            if (!BehaveScoreTo.HasValue && BehaveScoreFrom.HasValue ||
                (BehaveScoreTo.HasValue && BehaveScoreTo < 0) ||
                (BehaveScoreTo.HasValue && BehaveScoreFrom.HasValue && (BehaveScoreTo < BehaveScoreFrom)))
                OnValidationFailed("BehaveScoreTo", CurrentRow);

            if (!AppScoreFrom.HasValue && !BehaveScoreFrom.HasValue)
            {
                OnValidationFailed("AppScoreFrom", CurrentRow);
                OnValidationFailed("BehaveScoreFrom", CurrentRow);
            }
            return true;
        }

    }
     
}
