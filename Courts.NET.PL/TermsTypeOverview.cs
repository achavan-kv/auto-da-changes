using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using STL.Common.Static;
using STL.Common.Constants;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;

namespace STL.PL
{
    public partial class TermsTypeOverview : CommonForm
    {
        private string _error = "";
        private DateTime _serverDate = Date.blankDate;


        public TermsTypeOverview()
        {
            InitializeComponent();
        }

        public TermsTypeOverview(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;
        }


        private void TermsTypeOverview_Load(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.LoadBandsOverview();
            }
            catch (Exception ex)
            {
                Catch(ex, "tpBands_Enter");
            }
            finally
            {
                StopWait();
            }
        }


        private void LoadBandsOverview()
        {
            this._serverDate = StaticDataManager.GetServerDate();
            this.dtAdjustDate.Value = this._serverDate;
            this.dtAdjustDate.MinDate = this._serverDate;

            DataSet ttOverviewSet = StaticDataManager.TermsTypeBandsOverview(out _error);
            if (_error.Length > 0)
                ShowError(_error);
            else
            {
                foreach (DataTable dt in ttOverviewSet.Tables)
                {
                    if (dt.TableName == TN.TTOverview)
                    {
                        this.dgOverview.DataSource = dt;
                        dgOverview.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgOverview.Columns[CN.TermsType].HeaderText = GetResource("T_TERMS");
                        dgOverview.Columns[CN.TermsType].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgOverview.Columns[CN.Description].HeaderText = GetResource("T_DESCRIPTION");
                        dgOverview.Columns[CN.InsPcent].HeaderText = GetResource("T_INSPC");

                        // Format the dyanmic % columns
                        foreach (DataGridViewColumn col in dgOverview.Columns)
                        {
                            if (col.Name != CN.TermsType && col.Name != CN.Description)
                            {
                                col.DefaultCellStyle.Format = "F3";
                                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            }
                        }

                        dgOverview.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.DisplayedCells);
                    }
                }
            }
        }


        private void CheckEnabled()
        {
            this.numAdjustIns.Enabled = this.cbAdjustIns.Checked;
            this.numAdjustSC.Enabled = this.cbAdjustSC.Checked;

            this.dtAdjustDate.Enabled = this.btnAdjustBands.Enabled =
                ((cbAdjustIns.Checked && numAdjustIns.Value != 0) ||
                 (cbAdjustSC.Checked && numAdjustSC.Value != 0));
        }


        private void cbAdjustIns_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.CheckEnabled();
            }
            catch (Exception ex)
            {
                Catch(ex, "cbAdjustIns_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void cbAdjustSC_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.CheckEnabled();
            }
            catch (Exception ex)
            {
                Catch(ex, "cbAdjustSC_CheckedChanged");
            }
            finally
            {
                StopWait();
            }
        }


        private void numAdjustIns_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.CheckEnabled();
            }
            catch (Exception ex)
            {
                Catch(ex, "numAdjustIns_ValueChanged");
            }
            finally
            {
                StopWait();
            }
        }

        private void numAdjustSC_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Wait();
                this.CheckEnabled();
            }
            catch (Exception ex)
            {
                Catch(ex, "numAdjustSC_ValueChanged");
            }
            finally
            {
                StopWait();
            }
        }


        private void btnAdjustBands_Click(object sender, EventArgs e)
        {
            try
            {
                Wait();

                decimal adjustIns = (cbAdjustIns.Checked ? numAdjustIns.Value : 0);
                decimal adjustSC = (cbAdjustSC.Checked ? numAdjustSC.Value : 0);

                // Uncheck the adjustments in case the user clicks again
                this.cbAdjustSC.Checked = false;
                this.cbAdjustIns.Checked = false;

                // Adjust all terms that currently have an end date AFTER the adjustment date
                StaticDataManager.TermsTypeBandsAdjust(dtAdjustDate.Value, adjustIns, adjustSC, out _error);
                if (_error.Length > 0)
                    ShowError(_error);
                else
                {
                    this.LoadBandsOverview();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnAdjustBands_Click");
            }
            finally
            {
                StopWait();
            }
        }

    }
}
