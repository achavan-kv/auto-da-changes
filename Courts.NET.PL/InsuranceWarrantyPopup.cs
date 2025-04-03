using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants;
using STL.Common;
using STL.Common.Static;

namespace STL.PL
{
    public partial class InsuranceWarrantyPopup : CommonForm
    {
        private string _AccountNo; // Account number to pass to the form
        private DataSet _WarrantyReturnDS = null;

        public DataSet WarrantyReturnDataSet 
        {
            get { return _WarrantyReturnDS; }
        }

        public InsuranceWarrantyPopup(Form root, Form parent, string accountNo)
        {
            InitializeComponent();
            
            this._AccountNo = accountNo;
            base.FormRoot   = root;
            base.FormParent = parent;

            this.LoadData();
        }

        private void LoadData()
        {
            base.Function = "LoadData()";
            string err = "";
            try
            {

                DataSet ds =  AccountManager.GetWarrantyProductsByAccount(this._AccountNo, out err);
                
                if (err.Length > 0)
                    throw new Exception(err);

                
                
                //Set Grid style here

                DataView dv = new DataView(ds.Tables[TN.WarrantyList]);
                
                //Cheque country parameters to find out if we are required to populate return codes automatically
                bool autoReturnCode = Convert.ToBoolean(Country[CountryParameterNames.AutoReturnCodes]);
                if (!autoReturnCode)
                {
                    foreach (DataRow r in dv.Table.Rows)
	                {
                        r[CN.ReturnCode] = String.Empty;
	                }  
                }
                
                dgProductWarrantyList.DataSource = dv;
                 //dgProductWarrantyList.Columns[CN.Claim].CellTemplate
                dgProductWarrantyList.Columns[CN.Claim].Width = 50;
                dgProductWarrantyList.Columns[CN.Claim].ReadOnly = false;
                dgProductWarrantyList.Columns[CN.Claim].HeaderText = GetResource("T_CLAIM");

                //DataGridViewCell c = new DataGridViewCell();
                //c.InitializeEditingControl( 

                dgProductWarrantyList.Columns[CN.ReturnCode].Width = 50;
                dgProductWarrantyList.Columns[CN.ReturnCode].ReadOnly = false;  //   Should this be set to to read only ?
                dgProductWarrantyList.Columns[CN.ReturnCode].HeaderText = GetResource("T_RETURNCODE");
                
                dgProductWarrantyList.Columns[CN.StockLocn].Width = 50;
                dgProductWarrantyList.Columns[CN.StockLocn].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.StockLocn].HeaderText = GetResource("T_STOCKLOCN");

                dgProductWarrantyList.Columns[CN.ItemDescr1].Width = 100;
                dgProductWarrantyList.Columns[CN.ItemDescr1].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.ItemDescr1].HeaderText = GetResource("T_ITEM_DESCRIPTION");

                dgProductWarrantyList.Columns[CN.ItemDescr2].Width = 200;
                dgProductWarrantyList.Columns[CN.ItemDescr2].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.ItemDescr2].HeaderText = GetResource("T_ITEM_DESCRIPTION2");
                //((DataGridTextBoxColumn)dgProductWarrantyList.Columns[CN.TransValue]).Format = base.DecimalPlaces;

                dgProductWarrantyList.Columns[CN.waritemno].Width = 50;
                dgProductWarrantyList.Columns[CN.waritemno].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.waritemno].HeaderText = GetResource("T_WARRANTY_NO");

                dgProductWarrantyList.Columns[CN.ContractNo].Width = 70;
                dgProductWarrantyList.Columns[CN.ContractNo].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.ContractNo].HeaderText = GetResource("T_CONTRACTNO");

                dgProductWarrantyList.Columns[CN.StockItemNo].Width = 50;
                dgProductWarrantyList.Columns[CN.StockItemNo].ReadOnly = true;
                dgProductWarrantyList.Columns[CN.StockItemNo].HeaderText = GetResource("T_STOCKITEMNO");
            
                //Hide these columns
                dgProductWarrantyList.Columns[CN.AgrmtNo].Visible = false;
                dgProductWarrantyList.Columns[CN.BuffNo].Visible = false;

                dgProductWarrantyList.Columns[CN.ItemId].Visible = false;                  //IP - 08/06/11 - CR1212 - RI
                dgProductWarrantyList.Columns[CN.WarrantyId].Visible = false;              //IP - 08/06/11 - CR1212 - RI 
               
            }   
            catch (Exception ex)
            {
                Catch(ex, base.Function);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            //Check all return codes to make sure a code is present for each selected grid item
            foreach (DataGridViewRow r in dgProductWarrantyList.Rows)
            {
                if (Convert.ToBoolean(r.Cells[CN.Claim].Value))
                {
                    if (r.Cells[CN.ReturnCode].Value.ToString().Trim().Equals(""))                 
                    {
                        ShowInfo("M_RETURNCODEREQUIREDFORWARRANTY"); 
                        return;
                    }
                }
            }
            
            DataSet ds = new DataSet();
            DataTable dt = new DataTable(TN.Deliveries); 
            dt.Columns.Add(CN.acctno);
            dt.Columns.Add(CN.AgrmtNo);
            dt.Columns.Add(CN.ItemNo);
            dt.Columns.Add(CN.StockLocn);
            dt.Columns.Add(CN.BuffNo);
            dt.Columns.Add(CN.ContractNo);
            dt.Columns.Add(CN.EmployeeNo);
            dt.Columns.Add(CN.ReturnCode);
            dt.Columns.Add(CN.WarrantyId);                                              //IP - 08/06/11 - CR1212 - RI
            
            //Process selected warranties
            foreach (DataGridViewRow r in dgProductWarrantyList.Rows)
            {
                //Check to see if item selected
                if (Convert.ToBoolean(r.Cells[CN.Claim].Value))
                {
                    DataRow dr = dt.NewRow();
                    dr[CN.acctno]   = this._AccountNo ;
                    dr[CN.AgrmtNo]  = r.Cells[CN.AgrmtNo].Value.ToString();
                    dr[CN.ItemNo]   = r.Cells[CN.waritemno].Value.ToString();
                    dr[CN.StockLocn] = r.Cells[CN.StockLocn].Value.ToString();
                    dr[CN.BuffNo]     = r.Cells[CN.BuffNo].Value.ToString();
                    dr[CN.ContractNo] = r.Cells[CN.ContractNo].Value.ToString();
                    dr[CN.EmployeeNo] = Credential.UserId.ToString();
                    dr[CN.ReturnCode] = r.Cells[CN.ReturnCode].Value.ToString();
                    dr[CN.WarrantyId] = r.Cells[CN.WarrantyId].Value.ToString();        //IP - 08/06/11 - CR1212 - RI 
                    dt.Rows.Add(dr);
                }
            }
            ds.Tables.Add(dt);
            this._WarrantyReturnDS = ds;
            this.Close();

          
            //
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InsuranceWarrantyPopup_Load(object sender, EventArgs e)
        {

        }

        private void dgProductWarrantyList_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {


            if (!e.FormattedValue.ToString().Trim().Equals(string.Empty) && e.ColumnIndex == dgProductWarrantyList.Columns[CN.ReturnCode].Index)
            {
                //int rowCount = AccountManager.GetItemCount(e.FormattedValue.ToString(),
                //    Convert.ToInt16(dgProductWarrantyList[CN.StockLocn, e.RowIndex].Value), out Error);
                int rowCount = AccountManager.GetItemCount(0,
                    Convert.ToInt16(dgProductWarrantyList[CN.StockLocn, e.RowIndex].Value), out Error);         //TODO
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    if (rowCount == 0)
                    {
                        ShowInfo("M_RETSTOCKINVALID");
                        e.Cancel = true ;
                    }
                }
            }
							
        }
        
    }
}