using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Blue.Cosacs.StockCountApp.Models;
using System.Net;
using Symbol.Barcode2;
using System.IO;

namespace Blue.Cosacs.StockCountApp
{
    public partial class StockCountListForm : Form
    {
        private List<SimpleStockCountViewModel> _counts;
        private readonly StockCountRepository _repo;
        private int _selectedIndex;
        private Timer _messageTimer;

        public StockCountListForm()
        {
            InitializeComponent();
            Settings.Init();

            try
            {
                _repo = new StockCountRepository();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Initialisation failed. Please restart application. ({0})", ex.Message));
            }

            _selectedIndex = 0;
            _messageTimer = new Timer();
            _messageTimer.Interval = 3000;
            _messageTimer.Tick += (s, e) =>
            {
                labelLoading.Visible = false;
                _messageTimer.Enabled = false;
            };
            labelIndex.Text = "Hit 'Update/Send Counts' to refresh";

        }

        private void StockCountListForm_Load(object sender, EventArgs e)
        {
            barcode21.EnableScanner = true;

            if (Settings.IsValid)
            {
                Initialise();
            }
            else
            {
                this.UIThread(() =>
                {
                    var settingsForm = new SettingsForm();
                    settingsForm.ShowDialog();
                    Initialise();
                });
            }
        }

        private void Initialise() {
            this.UIThread(() => buttonBegin.Enabled = false);
            ShowLoader();
            try
            {
                _counts = _repo.GetStockCounts(true);
            }
            catch (Exception)
            {
                _counts = new List<SimpleStockCountViewModel>();
                this.UIThread(() => MessageBox.Show("There was a problem loading stock count data. Please update list to refresh."));
            }

            BindCounts();
            HideLoader();
        }

        private void BindCounts()
        {
            try
            {
                this.UIThread(() => {
                    buttonPrev.Enabled = (_selectedIndex > 0);
                    buttonNext.Enabled = (_selectedIndex < (_counts.Count - 1));
                });

                if (_counts.Count > 0)
                {                    
                    var thisCount = _counts[_selectedIndex];

                    this.UIThread(() =>
                    {
                        labelIdValue.Text = thisCount.Id.ToString();
                        labelIndex.Text = string.Format("{0} / {1}", (_selectedIndex + 1).ToString(), _counts.Count.ToString());
                        labelLocationValue.Text = thisCount.Location;
                        labelTypeValue.Text = thisCount.Type;   
                    });
                    
                    SetDetailActive(true);                 
                }
                else
                {
                    this.UIThread(() =>
                    {
                        labelIndex.Text = "Hit 'Update/Send Counts' to refresh";
                    });
                    SetDetailActive(false);
                }
            }
            catch (Exception)
            {
                this.UIThread(() => MessageBox.Show("There was a problem displaying, please try again."));
                SetDetailActive(false);
            }          
        }

        private void SetDetailActive(bool isActive) {
            this.UIThread(() =>
            {
                buttonBegin.Enabled = isActive;
                labelId.Visible = isActive;
                labelLocation.Visible = isActive;
                labelType.Visible = isActive;
                labelIdValue.Visible = isActive;
                labelLocationValue.Visible = isActive;
                labelTypeValue.Visible = isActive;
            });
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {           
                // Save current stock counts and pull down changes              
                try
                {
                    _counts = _repo.GetStockCounts(true);
                }
                catch (Exception)
                {
                    _counts = new List<SimpleStockCountViewModel>();
                }
                SyncList();
        }

        private void SyncList()
        {
            try
            {
                ShowLoader();
                Application.DoEvents();

                var payload = JsonConvert.SerializeObject(_counts.SelectMany(c => c.Products)
                        .Where(p => p.Count > 0)
                        .ToArray());
                var result = RequestManager.MakeRequest("/Cosacs/StockCount/SyncCounts", "POST", payload);
                var counts = JsonConvert.DeserializeObject<SimpleResponse<List<SimpleStockCountViewModel>>>(result).Data;

                // Clear current cache and update
                _repo.ClearAll();
                foreach (var count in counts)
                {
                    _repo.SaveStockCount(count);
                }

                _selectedIndex = 0;
                _counts = counts;
                BindCounts();
            }
            catch (UnauthorizedAccessException ex)
            {
                var loginFrm = new LoginForm();
                if (loginFrm.ShowDialog() == DialogResult.Yes)
                {
                    SyncList();
                }                
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Response is System.Net.HttpWebResponse)
                {
                    var webEx = ex.Response as System.Net.HttpWebResponse;
                    switch (webEx.StatusCode)
                    {
                        case HttpStatusCode.Unauthorized:
                            var loginFrm = new LoginForm();
                            if (loginFrm.ShowDialog() == DialogResult.Yes)
                            {
                                SyncList();
                            }
                            break;
                        case HttpStatusCode.Forbidden:
                            HandlePermissionException();   
                            break;
                        default:
                            var resp = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                            if (resp.Contains("PermissionException")) 
                            {
                                HandlePermissionException();                            
                            }
                            else
                            {
                                HandleRequestException(ex);
                            }
                            break;
                    }
                }
                else
                {
                    HandleRequestException(ex);
                }
            }
            catch (Exception ex)
            {
                HandleRequestException(ex);
            }
            finally
            {
                HideLoader();
            }
        }

        private void HandlePermissionException()
        {
            this.UIThread(() =>
            {
                MessageBox.Show("You have insufficient privileges.");
                buttonBegin.Enabled = false;
            });      
        }

        private void HandleRequestException(Exception ex)
        {
            this.UIThread(() => MessageBox.Show(string.Format("Connection failed: {0} {1}", ex.Message,
                    ex.InnerException == null ? "" : "- " + ex.InnerException.Message)
                    , "Connection Failed"));
        }

        private void barcode21_OnScan(ScanDataCollection scanDataCollection)
        {
            var scanError = false;
            foreach (ScanData data in scanDataCollection)
            {
                if (data.Result == Results.SUCCESS)
                {
                    try {
                        var id = int.Parse(data.Text.Replace("SC$", ""));
                        var count = _repo.GetStockCount(id);
                        if (count != null)
                        {                            
                            this.UIThread(() =>
                            {
                                barcode21.EnableScanner = false;
                                var stockCountForm = new StockCountForm(count);
                                stockCountForm.ShowDialog();
                                barcode21.EnableScanner = true;
                            });
                        }
                        else
                        {
                            scanError = true;
                        }
                    } catch (Exception) {
                        scanError = true;
                    }
                }
            }

            if (scanError)
            {
                this.UIThread(() =>
                {                    
                    labelLoading.Text = "Invalid stock count barcode";
                    labelLoading.Visible = true;
                    _messageTimer.Enabled = true;
                });   
            }
        }

        private void buttonBegin_Click(object sender, EventArgs e)
        {          
            var model = _counts[_selectedIndex];
            barcode21.EnableScanner = false;
            this.UIThread(() => {
                var stockCountForm = new StockCountForm(model);
                stockCountForm.ShowDialog();
                barcode21.EnableScanner = true;
            });
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            _selectedIndex--;
            BindCounts();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            _selectedIndex++;
            BindCounts();
        }

        private void ShowLoader()
        {
            this.UIThread(() =>
            {
                labelLoading.Visible = true;
                labelLoading.Text = "Updating";
                labelLoading.Refresh();                
            });            
        }

        private void HideLoader()
        {
            this.UIThread(()=> 
            {           
                labelLoading.Text = "Update complete";       
                _messageTimer.Enabled = true;
            });           
        }

        private void StockCountListForm_Closing(object sender, CancelEventArgs e)
        {
            barcode21.EnableScanner = false;
            barcode21.Dispose();
            this.Close();
        }

        private void menuItemSettings_Click(object sender, EventArgs e)
        {
            this.UIThread(() =>
            {
                var settingsForm = new SettingsForm();
                settingsForm.ShowDialog();
                Initialise();
            });
        }
    }
}