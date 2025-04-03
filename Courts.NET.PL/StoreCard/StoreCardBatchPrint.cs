using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using Crownwood.Magic.Menus;
using STL.Common.Static;
using System.Globalization;

namespace STL.PL.StoreCard
{
    public partial class StoreCardBatchPrint : CommonForm
    {
        private int index { get; set; }
        private bool reprint { get; set; }

        public StoreCardBatchPrint(Form root, Form parent)
        {
            FormRoot = root;
            FormParent = parent;

            InitializeComponent();

            LoadStatementRuns();

        }

        public void LoadStatementRuns()
        {

            Client.Call(new GetStatementRunsRequest(),
                            response =>
                            {
                                var statementRuns = response.StatementRuns;

                                if (statementRuns.Tables[0].Rows.Count > 0) //#12382
                                {
                                    dgStatementRuns.DataSource = statementRuns.Tables[0];
                                }

                            },
                                this);
        }

        private void dgStatementRuns_MouseUp(object sender, MouseEventArgs e)
        {
            if (dgStatementRuns.CurrentRow!= null)
            {
                index = dgStatementRuns.CurrentRow.Index;

                if (index >= 0)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        var datePrinted = ((DataTable)dgStatementRuns.DataSource).Rows[index]["DatePrinted"];

                        PopupMenu popup = new PopupMenu();

                        DataGridView ctl = (DataGridView)sender;

                        if (datePrinted.ToString().Length > 0)
                        {
                            reprint = true;

                            MenuCommand reprintBatch = new MenuCommand("Re-Print Batch");

                            reprintBatch.Click += new System.EventHandler(this.OnPrintBatch);

                            popup.MenuCommands.Add(reprintBatch);

                        }
                        else
                        {
                            reprint = false;

                            MenuCommand printBatch = new MenuCommand("Print Batch");

                            printBatch.Click += new System.EventHandler(this.OnPrintBatch);

                            popup.MenuCommands.Add(printBatch);

                        }


                        MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                    }
                }
            }
        }

        private void LaunchWebBrowser(string url)
        {
            var browser = CreateBrowserArray(1);
            object x = "";
            browser[0].Navigate(Config.Url + url, ref x, ref x, ref x, ref x);
        }

        private void OnPrintBatch(object sender, System.EventArgs e)
        {

            var batchNos = ((DataTable)dgStatementRuns.DataSource).Rows[index]["BatchNo"];
            var runNo = ((DataTable)dgStatementRuns.DataSource).Rows[index]["RunNo"];
            var batchDateRun = DateTime.UtcNow.ToString("s",CultureInfo.InvariantCulture); //#12390

            LaunchWebBrowser("StoreCard/BatchStatement?batchNos=" + batchNos + "&batchDateRun=" + batchDateRun.ToString() + "&reprint=" + Convert.ToString(reprint) + "&runNo=" + Convert.ToString(runNo));

            //Update the Date Printed 
            if (!reprint) //#12383
            {
                ((DataTable)dgStatementRuns.DataSource).Rows[index]["DatePrinted"] = Convert.ToDateTime(batchDateRun).ToLocalTime(); //#12390
            }

        }
    }
}
