using System;
using System.Data;
using System.Windows.Forms;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.Utils;

namespace STL.PL.StoreCard
{

    public delegate void Cancellation(object sender, GenericEventHandler<CancelRequest> args);


    public partial class StoreCardCancellation : UserControl
    {
        public event Cancellation CancelEvent;


        private StoreCardAccountResult Res;
        private long CardNo;

        public StoreCardCancellation()
        {
            InitializeComponent();
            SetupDGV();
        }


        private void SetupDGV()
        {
            dgv_history.ColumnStyleInit();
            //dgv_history.ColumnStylePreLoad("CardNumber"); Not required
            dgv_history.ColumnStylePreLoad("DateChanged", null, 135, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);
            dgv_history.ColumnStylePreLoad("Status");
            dgv_history.ColumnStylePreLoad("Reason");
            dgv_history.ColumnStylePreLoad("EmployeeName");
            dgv_history.ColumnStylePreLoad("Notes", null, 333, null, DataGridViewColumnSortMode.Automatic, DataGridViewContentAlignment.MiddleLeft);

        }

        public void Setup(StoreCardAccountResult scar, long cardno)
        {
            Res = scar;

            CardNo = cardno;
            drpCancel.Set(((DataTable)StaticData.Tables[TN.StoreCardCancelReasons]).AddBlankCode(), CN.CodeDescription, CN.Code);

            var status = scar.GetCardStatusCodeByCard(cardno);


            if (!StoreCardAccountStatus_Lookup.Cancelled.Equals(status))
            {
                txtNotes.Enabled = true;
                btnCancel.Enabled = true;
            }
            else
            {
                txtNotes.Enabled = false;
                btnCancel.Enabled = false;

            }

            var history = scar.History.FindAll(delegate(View_StoreCardHistory h) { return h.CardNumber == cardno; });
            dgv_history.DataSourceSafe(history);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

            if (CancelEvent != null)// && ValidateForm())
            {
                var SCHistory = new View_StoreCardHistory()
                    {
                        CardNumber = CardNo,
                        Code = drpCancel.SelectedValue.ToString(),
                        Acctno = Res.Acct.acctno,

                        Empeeno = Credential.UserId,
                        StatusCode = StoreCardAccountStatus_Lookup.Cancelled.Code,
                        DateChanged = DateTime.Now,
                        Notes = txtNotes.Text
                    };
                CancelEvent(this, new GenericEventHandler<CancelRequest>
                (
                new CancelRequest()
                {
                    StoreCardHistory =
                        SCHistory
                }
                ));

                txtNotes.Text = "";
                txtNotes.Enabled = false;
                btnCancel.Enabled = false;
            }


        }
    }
}
