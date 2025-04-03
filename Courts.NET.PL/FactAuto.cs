using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common;
using STL.Common.Static;
using STL.Common.Constants.TableNames;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.EOD;
using System.Web.Services.Protocols;
using System.Xml;
using System.Collections.Specialized;
using mshtml;
using System.Threading;
using System.Text.RegularExpressions;

namespace STL.PL
{
    public partial class FactAuto : CommonForm
    {
        private string err = "";

        public FactAuto(Form root, Form parent)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            dtEffectiveDate.MinDate = DateTime.Today;
            dtEffectiveDate.Value = DateTime.Today;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                bool valid = true;
                Wait();

                Regex regExp = new Regex("[Y]|[y]|[N]|[n]");

                if (!regExp.IsMatch(txtProduct.Text))
                {
                    valid = false;
                    errorProvider1.SetError(txtProduct, GetResource("M_ENTERVALIDVALUE"));
                }
                else
                    errorProvider1.SetError(txtProduct, "");

                if (valid)
                {
                    if (!regExp.IsMatch(txtCint.Text))
                    {
                        valid = false;
                        errorProvider1.SetError(txtCint, GetResource("M_ENTERVALIDVALUE"));
                    }
                    else
                        errorProvider1.SetError(txtCint, "");
                }

                if (valid)
                {
                    string excludeZeroStock = chxZeroStock.Checked ? "Y" : "N";
                    string processEOD = rbEOD.Checked ? "Y" : "N";
                    string processEOW = rbEOW.Checked ? "Y" : "N";
                    string processEOP = rbEOP.Checked ? "Y" : "N";

                    EodManager.SaveFACT2000Options(dtEffectiveDate.Value, txtProduct.Text.ToUpper(), excludeZeroStock,
                                            processEOD, processEOW, processEOP, txtCint.Text.ToUpper(), out err);

                    if (err.Length > 0)
                        ShowError(err);
                    else
                        ShowInfo("M_SAVED");  
                        CloseTab();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
            finally
            {
                StopWait();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dtEffectiveDate.Value = DateTime.Today;
            txtProduct.Text = "";
            chxZeroStock.Checked = false;
            rbEOD.Checked = true;
            rbEOW.Checked = false;
            rbEOP.Checked = false;
            txtCint.Text = "";
        }
    }
}