using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Xml;
using Blue.Cosacs.Shared;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.StoreCard;
using STL.Common;
using STL.Common.Constants.FTransaction;
using STL.Common.Constants.TableNames;
using STL.Common.Static;
using STL.PL.StoreCard.Common;

namespace STL.PL.StoreCard
{

    public partial class StoreCardAccount : CommonForm
    {
        public delegate void InvokeDelegate();

        bool loading = false;
        BindingList<View_StoreCardWithPayments> BStoreCard;
        StoreCardAccountResult SCAR;
        bool newcard = false;
        private readonly System.EventHandler CardIndexChanged;
        private bool CanEditFinancial = false;

        public StoreCardAccount(string AccountNo, Form root = null, Form parent = null, int tabNo = 0)
        {
            loading = true;
            CardIndexChanged = new System.EventHandler(cmb_cardname_SelectedIndexChanged);
            InitializeComponent();
            PopulatePaymentCombos();
            CmbYearAddRange(cmbEndYear);
            CmbYearAddRange(cmbStartYear);
            FormRoot = root;
            FormParent = parent;
            AccountNo = AccountNo.Replace("-", "");
            tabMain.SelectedIndex = tabNo;
            LoadAccount(AccountNo);
            if (Convert.ToDouble(Country[CountryParameterNames.STReplacementFee]) > 0)
                toolTip1.SetToolTip(btnReplace, "Replacement Card cost: " + Convert.ToString(Country[CountryParameterNames.STReplacementFee]));
        }

        private void LoadAccount(string acctno)
        {
            Client.Call(new GetRequest { Acctno = acctno },
                response =>
                {
                    Response(response.StoreCardAccountResult);
                }, this);
        }

        private void Response(StoreCardAccountResult scar)
        {
            SCAR = scar;
            CheckExtraRoleRestriction();
            BStoreCard = new BindingList<View_StoreCardWithPayments>(SCAR.StoreCardWithPayments);
            PopulateAcctnoCmb();
            BindControls();
            storeCardActivation1.ActivateEvent += new Activation(storeCardActivation1_ActivateEvent);
            storeCardCancellation1.CancelEvent += new Cancellation(storeCardCancellation1_CancelEvent);
            storeCardPaymentDetails1.PrintStatementEvent += new PrintStatementEvent(storeCardPaymentDetails1_PrintStatementEvent);
            this.cmb_cardname.SelectedIndexChanged += CardIndexChanged;
            StoreCardTransactions1.Setup(SCAR.Fintransfers);
            storeCardPaymentDetails1.Setup(SCAR, Country, CanEditFinancial);
            txt_status.Text = StoreCardCardStatus_Lookup.FromString(BStoreCard[cmb_cardname.SelectedIndex].CardStatus).Description;
            loading = false;
            cmb_cardname_SelectedIndexChanged(cmb_cardname, null);
        }


        public void PrintStatement(Int32 Id)
        {
            LaunchWebBrowser("StoreCard/Statement?Id=" + Id);
        }

        public void btnPrintAgreement_Click(object sender, EventArgs e)
        {
            LaunchWebBrowser("StoreCard/agreement/?acctno=" + SCAR.Acct.acctno);

            Client.Call(new SaveDateNotePrintedRequest { AcctNo = SCAR.Acct.acctno, Empeeno = Credential.UserId },
             response =>
             {

                 MainForm.Current.ShowStatus("Note Printed");
             }, this);
        }

        private void LaunchWebBrowser(string url)
        {
            var browser = CreateBrowserArray(1);
            object x = "";
            browser[0].Navigate(Config.Url + url, ref x, ref x, ref x, ref x);
        }

        void storeCardPaymentDetails1_PrintStatementEvent(object sender, GenericEventHandler<Int32> args)
        {
            PrintStatement(args.Results);
            var Fee = Convert.ToDecimal(Country[CountryParameterNames.STStatementFee]);

            if (Fee > 0) // increase the balance by the fee just charged....
            {
                SCAR.Acct.outstbal += Fee;

                SCAR.StoreCardWithPayments[0].StoreCardAvailable -= Fee;
                SCAR.Fintransfers.Add(new view_FintranswithTransfers
                {
                    Value = Fee,
                    DateTrans = DateTime.Now,
                    Code = TransType.StoreCardCardStatementFee,
                    BranchNo = Convert.ToInt16(Config.BranchCode),
                    Empeeno = Credential.UserId,

                    Description = "Statement Fee"
                });
                storeCardPaymentDetails1.SetBalance(SCAR);
            }
            //eeff
            Client.Call(new FintransAddRequest
         {
             acctno = SCAR.Acct.acctno,
             value = Fee,
             code = TransType.StoreCardCardStatementFee,
             branch = Config.BranchCode,
             custid = SCAR.MainCustid
         }, r => { });

        }

        void storeCardActivation1_ActivateEvent(object sender, GenericEventHandler<ActivateRequest> args)
        {
            this.cmb_cardname.SelectedIndexChanged -= CardIndexChanged;
            var act = args.Results;
            var scp = SCAR.StoreCardWithPayments.Find(delegate(View_StoreCardWithPayments scwp) { return scwp.CardNumber == act.CardNumber; });


            Client.Call(act, Response =>
            {
                SCAR.History.Insert(0, new View_StoreCardHistory
                                         {
                                             DateChanged = act.Date,
                                             CardNumber = act.CardNumber,
                                             StatusCode = StoreCardCardStatus_Lookup.Active.Code,
                                             Status = StoreCardCardStatus_Lookup.Active.Description,
                                             Reason = act.Reason,
                                             EmployeeName = Response.Empeename
                                         });

                scp.ProofAddNotes = act.ProofAddNotes;
                scp.ProofID = act.ProofID;
                scp.ProofAddress = act.ProofAddress;
                scp.ProofIDNotes = act.ProofIDNotes;
                scp.SecurityA = act.SecurityA;
                scp.SecurityQ = act.SecurityQ;
                scp.CardStatusDateChanged = act.Date;
                scp.CardStatus = StoreCardCardStatus_Lookup.Active.Code;
                SCAR.AcceptedAgreement = true;

                txt_status.Text = Response.StoreCardStatus.StatusName;
                storeCardActivation1.Setup(SCAR, act.CardNumber, false);
                this.cmb_cardname.SelectedIndexChanged += CardIndexChanged;
                cmb_cardname_SelectedIndexChanged(cmb_cardname, null);
                MainForm.Current.ShowStatus("Details Saved");
            }, this);
        }


        private void storeCardCancellation1_CancelEvent(object sender, GenericEventHandler<CancelRequest> args)
        {
            //this.cmb_cardname.SelectedIndexChanged -= CardIndexChanged;
            Client.Call(args.Results, Response =>
            {
                SCAR.History = Response.StoreCardHistory;
                SCAR.StoreCardWithPayments = Response.View_StoreCardWithPayments;
                txt_status.Text = StoreCardCardStatus_Lookup.Cancelled.Description;
                storeCardCancellation1.Setup(SCAR, args.Results.StoreCardHistory.CardNumber);
                cmb_cardname_SelectedIndexChanged(cmb_cardname, null);
                MainForm.Current.ShowStatus("The card has been cancelled");
                //this.cmb_cardname.SelectedIndexChanged += CardIndexChanged;
            }, this);

        }

        private void PopulateAcctnoCmb()
        {
            cmb_cardname.DisplayMember = "CardNameCombined";
            cmb_cardname.ValueMember = "CardNumber";
            cmb_cardname.DataSource = BStoreCard;


        }

        private void CheckExtraRoleRestriction()
        {
            DataRow[] foundrows = { };
            DataSet permissions = StaticDataManager.GetDynamicMenus(Credential.UserId, "StoreCardAccount", out Error);
            if (permissions.Tables.Count > 0)
            {
                foundrows = permissions.Tables["Menus"].Select("Control = 'FinancialChanges' and Enabled = 1");
                if (foundrows.Length > 0)
                {
                    CanEditFinancial = true;
                }
            }
        }


        private void BindControls()
        {
            ClearBindings();
            txtCardNumber.DataBindings.Add("Text", BStoreCard, "CardNumber");
            cmbStartMonth.DataBindings.Add("Text", BStoreCard, "IssueMonth");
            cmbStartYear.DataBindings.Add("Text", BStoreCard, "IssueYear");
            cmbEndMonth.DataBindings.Add("Text", BStoreCard, "ExpirationMonth");
            cmbEndYear.DataBindings.Add("Text", BStoreCard, "ExpirationYear");

        }

        private void ClearBindings()
        {
            txtCardNumber.DataBindings.Clear();
            cmbStartMonth.DataBindings.Clear();
            cmbStartYear.DataBindings.Clear();
            cmbEndMonth.DataBindings.Clear();
            cmbEndYear.DataBindings.Clear();
            txt_status.DataBindings.Clear();
        }

        private void CmbYearAddRange(ComboBox c)
        {
            for (var i = 2010; i < DateTime.Now.Year + 10; i++)
            {
                c.Items.Add(i.ToString());
            }
        }

        public void TextStatusUpdate(string Status)
        {
            txt_status.Text = Status;
        }


        private void PopulatePaymentCombos()
        {
            XmlDocument dropDowns = new XmlDocument();
            dropDowns.LoadXml("<DROP_DOWNS></DROP_DOWNS>");

            if (StaticData.Tables[TN.StoreCardPaymentOption] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StoreCardPaymentOption, new string[] { "STPO", "L" }));
            if (StaticData.Tables[TN.StoreCardPaymentMethod] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StoreCardPaymentMethod, new string[] { "STPM", "L" }));

            if (StaticData.Tables[TN.StoreCardSecurityQuestion] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StoreCardSecurityQuestion, new string[] { "STQ", "L" }));


            if (StaticData.Tables[TN.StoreCardContactMethod] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StoreCardContactMethod, new string[] { "STCM", "L" }));


            if (StaticData.Tables[TN.StoreCardCancelReasons] == null)
                dropDowns.DocumentElement.AppendChild(xml.CreateDropDownNode(dropDowns, TN.StoreCardCancelReasons, new string[] { "STCR", "L" }));

            if (dropDowns.DocumentElement.ChildNodes.Count > 0)
            {
                DataSet ds = StaticDataManager.GetDropDownData(dropDowns.DocumentElement, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        StaticData.Tables[dt.TableName] = dt;
                    }
                }
            }
        }

        private void cmb_cardname_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                long cardno = Convert.ToInt64(cmb_cardname.SelectedValue.ToString());
                storeCardActivation1.Setup(SCAR, cardno, newcard);
                storeCardCancellation1.Setup(SCAR, cardno);

                string cardName = ((View_StoreCardWithPayments)cmb_cardname.SelectedItem).CardName;
                var status = SCAR.GetCardStatusCodeByCard(cardno);
                txt_status.Text = StoreCardCardStatus_Lookup.FromString(status).Description;
                var custid = BStoreCard[cmb_cardname.SelectedIndex].custid;

                var storeCardCardCreateCheck = new StoreCardCreateCheck(StoreCardAccountStatus_Lookup.FromString(SCAR.AccountStatus), SCAR.StoreCardWithPayments,
                                                                            custid, SCAR.AcceptedAgreement, Convert.ToInt32(Country[CountryParameterNames.StoreCardMaxNoJointCards]));

                btnReplace.Enabled = storeCardCardCreateCheck.ReplaceCheck();
                btnNewStoreCard.Enabled = storeCardCardCreateCheck.AddNewCardCheck();
            }
        }



        private void btnNewStoreCard_Click(object sender, EventArgs e)
        {
            if (!newcard)
                NewStoreCardCreate();
            else
                NewStoreCardCancel();
        }



        private void NewStoreCardCreate()
        {
            CustomerSearch cust = new CustomerSearch(true);
            cust.FormRoot = this.FormRoot;
            cust.FormParent = this;
            ((MainForm)this.FormRoot).AddTabPage(cust, 9);
            cust.CustidSelected += new RecordIDHandler<StoreCardCustDetails>(cust_CustidSelected);
            btnNewStoreCard.Text = "Cancel";
            cmb_cardname.Enabled = false;
            cmb_cardname.SelectedItem = BStoreCard[BStoreCard.Count - 1];
            MainForm.Current.ShowStatus("New Store Card Created.");
            tabMain.SelectTab(0);
            newcard = true;
        }

        private void NewStoreCardCancel()
        {
            BStoreCard.RemoveAt(BStoreCard.Count - 1);
            cmb_cardname.Enabled = true;
            btnNewStoreCard.Text = "New Card";
            newcard = false;
        }



        void cust_CustidSelected(object sender, RecordIDEventArgs<StoreCardCustDetails> args)
        {
            var found = SCAR.StoreCardWithPayments.FindAll(delegate(View_StoreCardWithPayments p) { return p.custid == args.RecordID.Custid; });

            if (found != null && found.Count > 0)
            {
                MessageBox.Show("Please use the replace button to issue new card", "Cardholder already added.", MessageBoxButtons.OK);
                btnNewStoreCard.Text = "New Card";
                newcard = false;
            }
            else
                SaveLinkedCard(args.RecordID);
        }

        public void SaveLinkedCard(StoreCardCustDetails details)
        {
            this.cmb_cardname.SelectedIndexChanged -= CardIndexChanged;
            if (newcard)
            {
                Client.Call(new CreateRequest
                {
                    storeCardNew = new StoreCardNew
                        {
                            AcctNo = BStoreCard[0].Acctno,
                            CustId = details.Custid,
                            Source = "Additional",
                            User = Credential.UserId                 // #9542 jec 27/01/12
                        }
                },
              response =>
              {
                  MainForm.Current.ShowStatus("New StoreCard created.");

                  AddNewCard(response, details);
                  this.cmb_cardname.SelectedIndexChanged += CardIndexChanged;
                  cmb_cardname.SelectedIndex = cmb_cardname.Items.Count - 1;
                  LaunchWebBrowser(String.Format("StoreCard/NewCardAgreement/?acctno={0}&&newcustid={1}", BStoreCard[0].Acctno, details.Custid));
              }, this);

            }

        }

        private void btnReplace_Click(object sender, EventArgs e)
        {
            tabMain.SelectTab(0);

            this.cmb_cardname.SelectedIndexChanged -= CardIndexChanged;
            Client.Call(new CreateRequest
            {
                storeCardNew = new StoreCardNew
                    {
                        AcctNo = SCAR.Acct.acctno,
                        CustId = BStoreCard[cmb_cardname.SelectedIndex].custid,
                        Source = "Replacement",
                        User = Credential.UserId
                    }
            },
            response =>
            {
                var cust = SCAR.Customers.Find(c => c.custid == BStoreCard[cmb_cardname.SelectedIndex].custid);

                AddNewCard(response,
                            new StoreCardCustDetails
                            {
                                FirstName = cust.firstname,
                                Title = cust.title,
                                Custid = cust.custid,
                                LastName = cust.name
                            }, true
            );
                this.cmb_cardname.SelectedIndexChanged += CardIndexChanged;
                cmb_cardname.SelectedIndex = BStoreCard.Count - 1;
                MainForm.Current.ShowStatus("New StoreCard created.");
            }, this);
        }

        private void AddNewCard(CreateResponse response, StoreCardCustDetails details, bool replace = false)
        {
            var expDate = DateTime.Now.AddMonths(Convert.ToInt32(Country[CountryParameterNames.StoreCardDefaultCardMonths]));
            var SCWP = new View_StoreCardWithPayments
            {
                custid = details.Custid,
                CardNumber = response.CardNumber,
                SPDStatus = StoreCardAccountStatus_Lookup.CardToBeIssued.Code,
                CardName = details.NameConCat(),
                NewCardCustid = details.Custid,
                IssueMonth = Convert.ToByte(DateTime.Now.Month),
                IssueYear = Convert.ToInt16(DateTime.Now.Year),
                ExpirationMonth = Convert.ToByte(expDate.Month),
                ExpirationYear = Convert.ToInt16(expDate.Year),
                CardStatus = StoreCardCardStatus_Lookup.CardToBeIssued.Code,
                CardStatusDateChanged = DateTime.Now
            };
            SCAR.StoreCardWithPayments.Add(SCWP);
            BStoreCard.Add(SCWP);

            if (!replace)
            {
                SCAR.Customers.Add(new Customer
                {
                    custid = details.Custid,
                    name = details.LastName,
                    firstname = details.FirstName,
                    title = details.Title,
                    dateborn = response.DOB
                });

                if (response.CustAddress != null)
                {
                    SCAR.Addresses.Add(response.CustAddress);
                }
            }
            else
            {
                var Fee = Convert.ToDecimal(Country[CountryParameterNames.STReplacementFee]);
                if (Fee > 0) // increase the balance by the fee just charged....
                {
                    SCAR.Acct.outstbal += Fee;
                    SCAR.StoreCardWithPayments[0].StoreCardAvailable -= Fee;
                    SCAR.Fintransfers.Add(new view_FintranswithTransfers
                    {
                        Value = Fee,
                        DateTrans = DateTime.Now,
                        Code = TransType.StoreCardCardReplaceFee,
                        BranchNo = Convert.ToInt16(Config.BranchCode),
                        Empeeno = Credential.UserId,
                        Description = "Replacement Fee"

                    });
                    storeCardPaymentDetails1.SetBalance(SCAR);
                }

            }

            cmb_cardname.Enabled = true;
            btnNewStoreCard.Text = "New Card";
            newcard = false;
        }

        private void storeCardActivation1_Load(object sender, EventArgs e)
        {

        }
    }
}
