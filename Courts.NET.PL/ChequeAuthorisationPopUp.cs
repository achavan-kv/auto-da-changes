using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using STL.Common.Static;

namespace STL.PL
{
    public partial class ChequeAuthorisationPopUp : CommonForm
    {
        private bool _cancelled = false;
        private bool _authorised = false;
        private int _authorisingUser = 0;
        private string _error = "";
        private Control _control;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="root"></param>
        /// <param name="parent"></param>
        public ChequeAuthorisationPopUp(Form root, Form parent)
        {
            InitializeComponent();

            //Enable controls based on the current user on this form
            base.FormRoot = root;
            base.FormParent = parent;

            _control = new Label();
            _control.Name = "lConfirm";


            dynamicMenus = new Hashtable();
            dynamicMenus[this.Name + ":" + this._control.Name] = this._control;

            base.ApplyRoleRestrictions();

            //Continue will not be enabled 
            btnContinue.Enabled = false;
            //Fill in the user automatically if they have permission to authorise returned cheques.
            if (btnContinue.Enabled)
            {
                //this functionality removed as of failed testing
            }

            // Don't need the entry screens if the user has permission to authorise
            this.groupBox1.Enabled = !btnContinue.Enabled;
        }

        /// <summary>
        /// The cancel button was clicked
        /// </summary>
        public bool Cancelled
        {
            get { return this._cancelled; }
        }

        /// <summary>
        /// A user with permission to authorise check payments entered their username
        /// </summary>
        public bool Authorised
        {
            get { return this._authorised; }
        }
        /// <summary>
        /// The user that authorises the cheque
        /// </summary>
        public int AuthorisingUser
        {
            get { return this._authorisingUser; }
        }

        /// <summary>
        /// set the datasource of the returned cheques list
        /// </summary>
        public DataSet ReturnedCheques
        {
            set
            {
                this.LoadData(value);
            }
        }

        /// <summary>
        /// Click to authorise cheques
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnContinue_Click(object sender, EventArgs e)
        {
            //This button will only be enabled when a user has been authorised
            base.Function = "btnContinue_Click()";

            this._authorised = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this._cancelled = true;
            this.Close();
        }

        private void LoadData(DataSet ds)
        {
            base.Function = "LoadData()";
            try
            {
                DataGridTableStyle tabStyle = new DataGridTableStyle();
                tabStyle.MappingName = TN.ReturnedCheques;

                dgReturnedCheques1.TableStyles.Clear();
                dgReturnedCheques1.TableStyles.Add(tabStyle);
                dgReturnedCheques1.DataSource = ds;
                dgReturnedCheques1.DataMember = TN.ReturnedCheques;

                tabStyle.GridColumnStyles[CN.acctno].Width = 100;
                tabStyle.GridColumnStyles[CN.acctno].ReadOnly = true;
                tabStyle.GridColumnStyles[CN.acctno].HeaderText = GetResource("T_ACCOUNTNO");

                tabStyle.GridColumnStyles[CN.CurrStatus].Width = 50;
                tabStyle.GridColumnStyles[CN.CurrStatus].ReadOnly = true;
                tabStyle.GridColumnStyles[CN.CurrStatus].HeaderText = GetResource("T_STATUS");

                tabStyle.GridColumnStyles[CN.TransValue].Width = 100;
                tabStyle.GridColumnStyles[CN.TransValue].ReadOnly = true;
                tabStyle.GridColumnStyles[CN.TransValue].HeaderText = GetResource("T_AMOUNT1");
                ((DataGridTextBoxColumn)tabStyle.GridColumnStyles[CN.TransValue]).Format = base.DecimalPlaces;


                tabStyle.GridColumnStyles[CN.DateTrans].Width = 70;
                tabStyle.GridColumnStyles[CN.DateTrans].ReadOnly = true;
                tabStyle.GridColumnStyles[CN.DateTrans].HeaderText = GetResource("T_DATE");
            }
            catch (Exception ex)
            {
                Catch(ex, base.Function);
            }


        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                base.Function = "txtPassword_Validating";
                Wait();

                if (txtEmpNo.Text.Length != 0)
                {
                    var userId = StaticDataManager.ControlPermissionPasswordCheck(txtEmpNo.Text, txtPassword.Text, this.Name, _control.Name);
                    if (userId.HasValue)
                    {
                        this._authorisingUser = userId.Value;
                        btnContinue.Enabled = this._control.Enabled;
                        if (btnContinue.Enabled)
                            btnContinue.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                Catch(ex, base.Function);
            }
            finally
            {
                base.StopWait();
            }

        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Function = "txtPassword_KeyPress";


                if (e.KeyChar == (char)13)
                    txtPassword_Validating(null, null);
            }
            catch (Exception ex)
            {
                Catch(ex, Function);
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void ChequeAuthorisationPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            this._cancelled = true;
        }



    }
}