using System;
using System.Data;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Web.Services.Protocols;
using STL.PL.WS9;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common;
using STL.Common.Static;
using Crownwood.Magic.Menus;
//using Blue.Cosacs.Repositories;


namespace STL.PL
{
    public partial class EODPreviousRuns : CommonForm
    {
        string eodOption = "";
        int reRunNo = 0;
        
        public EODPreviousRuns(Form root, Form parent, string option, string descr)
        {
            InitializeComponent();

            FormRoot = root;
            FormParent = parent;

            txtError.BackColor = SystemColors.Window;
            lDescription.Text = "Interface: " + descr;
            eodOption = option;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                DataSet ds = EodManager.GetInterfaceControl(eodOption, "", false, out Error);     //UAT1010 jec 09/07/10
                if (Error.Length > 0)
                {
                    ShowError(Error);
                    return;
                }

                if (ds == null)
                    return;

                dgPrevious.DataSource = ds.Tables["Table1"].DefaultView;
                ApplyDgPreviousTableStyle(ds.Tables["Table1"].TableName);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void ApplyDgPreviousTableStyle(string mappingName)
        {
            if (dgPrevious.TableStyles.Count != 0)
                return;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = mappingName;
            dgPrevious.TableStyles.Add(tabStyle);

            tabStyle.GridColumnStyles[CN.Interface].Width = 0;
            tabStyle.GridColumnStyles[CN.Interface].HeaderText = GetResource("T_INTERFACE");

            tabStyle.GridColumnStyles[CN.Runno].Width = 75;
            tabStyle.GridColumnStyles[CN.Runno].HeaderText = GetResource("T_RUNNO");

            tabStyle.GridColumnStyles[CN.DateStart].Width = 130;
            tabStyle.GridColumnStyles[CN.DateStart].HeaderText = GetResource("T_DATESTART");
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateStart]).Format = "dd/MM/yyyy HH:mm";

            tabStyle.GridColumnStyles[CN.DateFinish].Width = 130;
            tabStyle.GridColumnStyles[CN.DateFinish].HeaderText = GetResource("T_DATEFINISH");
            ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.DateFinish]).Format = "dd/MM/yyyy HH:mm";

            tabStyle.GridColumnStyles[CN.Result].Width = 100;
            tabStyle.GridColumnStyles[CN.Result].HeaderText = GetResource("T_RESULT");

            tabStyle.GridColumnStyles["CanReRun"].Width = 0;
        }

        private void dgPrevious_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                DataView dv = (DataView)dgPrevious.DataSource;
                for (int i = dv.Count - 1; i >= 0; i--)
                {
                    if (!dgPrevious.IsSelected(i))
                        continue;

                    if (e.Button == MouseButtons.Right)
                    {
                        //int index = dgPrevious.CurrentRow.Index;
                        DataGrid ctl = (DataGrid)sender;

                        MenuCommand m1 = new MenuCommand("ReRun Interface");

                        m1.Click += new System.EventHandler(this.cmenuReRun_Click);                        

                        PopupMenu popup = new PopupMenu();

                        if (Convert.ToInt32(dv[i]["CanReRun"])==1)      //Can Rerun
                        {
                            reRunNo = Convert.ToInt32(dv[i][CN.Runno]);
                            popup.MenuCommands.Add(m1);
                        }
                        

                        //popup.MenuCommands.AddRange(new MenuCommand[] { m1 });
                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                    int runNo = Convert.ToInt32(dv[i][CN.Runno]);
                    var startdate = Convert.ToDateTime(dv[i][CN.DateStart]);        //jec 06/04/11
                    DataSet ds = EodManager.GetInterfaceError(eodOption, runNo, startdate, out Error);

                    if (Error.Length > 0)
                    {
                        ShowError(Error);
                    }
                    else if (ds != null)
                    {
                        dgErrors.DataSource = ds.Tables["Table1"].DefaultView;
                        ApplyDgErrorsTableStyle(ds.Tables["Table1"].TableName);
                        DataView dve = (DataView)dgErrors.DataSource;
                        for (int ie = dve.Count - 1; ie >= 0; ie--)
                        {
                            // Check/remove carriageReturn linefeeds as causes error text to show blank
                            var error1 = Convert.ToString(dve[ie][CN.ErrorText]);
                            var error2 = error1.Split(new[] { "\r\n\r\n" }, StringSplitOptions.None)[0];
                            if(error2 == "")
                                dve[ie][CN.ErrorText] = error1.Split(new[] { "\r\n\r\n" }, StringSplitOptions.None)[1];
                        }
                    }

                    break;
                }

                txtError.Text = "";                
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void ApplyDgErrorsTableStyle(string mappingName)
        {
            if (dgErrors.TableStyles.Count != 0)
                return;

            DataGridTableStyle tabStyle = new DataGridTableStyle();
            tabStyle.MappingName = mappingName;
            dgErrors.TableStyles.Add(tabStyle);

            tabStyle.GridColumnStyles[CN.ErrorText].Width = 450;
            tabStyle.GridColumnStyles[CN.ErrorText].HeaderText = GetResource("P_ERRORTEXT");

            tabStyle.GridColumnStyles[CN.Severity].Width = 0;
            tabStyle.GridColumnStyles[CN.ErrorDate].Width = 0;
            tabStyle.GridColumnStyles[CN.Interface].Width = 0;
            tabStyle.GridColumnStyles[CN.Runno].Width = 0;
        }

        private void dgErrors_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgErrors.DataSource == null)
                return;

            DataView dv = (DataView)dgErrors.DataSource;
            for (int i = dv.Count - 1; i >= 0; i--)
            {
                if (dgErrors.IsSelected(i))
                {
                    //var error1 = Convert.ToString(dv[i][CN.ErrorText]);
                    //var error2 = error1.Split(new[] { "\r\n\r\n" }, StringSplitOptions.None)[1];
                    var error = Convert.ToString(dv[i][CN.ErrorText]);
                    //var error = error2;
                    txtError.Text = error.Split(new[] { "#System Message#" }, StringSplitOptions.None)[0];
                    break;
                }
            }
        }

        //jec 05/04/11 Set Rerun Number
        private void cmenuReRun_Click(object sender, System.EventArgs e)
        {
            try
            {
                Function = "cmenuReRun_Click";
                Wait();

                var ui = FormParent as EODUserInterface;
                var mainForm = FormRoot as MainForm;

                if (ui != null)
                {
                    ui.SetReRunNo(reRunNo);       //Sets the ReRun number in EODInterface screen
                    mainForm.FocusIfExists(ui);
                }                
               
                CloseTab(this);                
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
    }
}