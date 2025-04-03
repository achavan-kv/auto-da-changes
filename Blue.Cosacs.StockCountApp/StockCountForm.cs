using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Symbol.Barcode2;

namespace Blue.Cosacs.StockCountApp
{
    public partial class StockCountForm : Form
    {
        private readonly SimpleStockCountViewModel _stockCount;
        private readonly StockCountRepository _repo;
        private bool dirty;

        public StockCountForm(SimpleStockCountViewModel stockCount)
        {
            InitializeComponent();
            _stockCount = stockCount;
            _repo = new StockCountRepository();
            barcode21.EnableScanner = true;
        }

        private void StockCountForm_Load(object sender, EventArgs e)
        {
            dirty = false;           
            this.UIThread(() => { 
                buttonConfirm.Enabled = false;
                this.Text = string.Format("Stock Count {0}", _stockCount.Id);
            });
        }        

        private void buttonConfirm_Click(object sender, EventArgs e)
        {
            ValidateAndSave(textBoxSku.Text);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {            
            this.DialogResult = DialogResult.Yes;               
            this.Close();          
        }

        private bool SaveCount(SimpleStockCountProductViewModel stockProduct)
        {
            ShowLoader();
            bool result = false;
            
            var countString = string.IsNullOrEmpty(textBoxCount.Text) ? "1" : textBoxCount.Text;
            var count = ValidateCount(countString);            
            stockProduct.Count += count;
            _repo.SaveStockCountProduct(stockProduct);
            result = true;

            // Reset view
            ResetView();
            HideLoader();
            return result;
        }

        private void ResetView()
        {
            this.UIThread(() =>
            {
                textBoxSku.Text = string.Empty;
                textBoxCount.Text = string.Empty;
                buttonConfirm.Enabled = false;
                labelError.Visible = false;
            });
            dirty = false;
        }

        private SimpleStockCountProductViewModel FindStockProduct(string key)
        {
            var stockProduct = _repo.FindStockCountProductBySkuOrBarcode(_stockCount.Id, key);
            if (stockProduct == null)
            {
                this.UIThread(() =>
                {
                    labelError.Text = "SKU or Barcode not found";
                    labelError.Visible = true;
                    buttonConfirm.Enabled = false;
                });
                return null;
            }

            return stockProduct;
        }

        private bool ValidateKey(string key)
        {
            // Check key
            if (string.IsNullOrEmpty(key))
            {
                this.UIThread(() =>
                {
                    labelError.Text = "SKU or Barcode is required";
                    labelError.Visible = true;
                    buttonConfirm.Enabled = false;
                });
                return false;
            }

            return true;
        }


        private int ValidateCount(string countString)
        {            
            // Check count
            int count = 1;
            try
            {
                count = Int32.Parse(countString);
            }
            catch
            {
                count = 1;
            }            

            return count;
        }

        private void ValidateAndSave(string key)
        {
            if (ValidateKey(key))
            {
                var stockProduct = FindStockProduct(key);
                if (stockProduct != null)
                {
                    SaveCount(stockProduct);
                }
            }
            dirty = false;
        }

        private void barcode21_OnScan(ScanDataCollection scanDataCollection)
        {
            barcode21.EnableScanner = false;

            // If the asynchronous scanning is done in the buffered mode, 
            // there may be more than one scanned data in the collection
            foreach (ScanData data in scanDataCollection)
            {                
                if (data.Result == Results.SUCCESS)
                {
                    if (dirty)
                    {
                        labelMessage.Text = string.Empty;
                        ValidateAndSave(data.Text);                        
                    }

                    var product = FindStockProduct(data.Text);
                    if (ValidateKey(data.Text) && product != null)
                    {
                        dirty = true;
                    }                   

                    this.UIThread(() =>
                    {
                        labelMessage.Text = product.LongDescription;
                        textBoxSku.Text = data.Text;
                        textBoxCount.Text = "1";
                        textBoxCount.Focus();
                    });
                    HideLoader();
                }
                else
                {                    
                    this.UIThread(() =>
                    {
                        labelError.Text = string.Format("Scanner error ({0})", data.Result);
                        labelError.Visible = true;
                    });
                }
                HideLoader();
            }

            barcode21.EnableScanner = true;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!buttonConfirm.Enabled)
            {
                this.UIThread(() =>
                {
                    buttonConfirm.Enabled = true;
                    labelError.Visible = false;
                });         
            }
        }

        private void ShowLoader()
        {
            this.UIThread(() => labelLoading.Visible = true);            
        }

        private void HideLoader()
        {
            this.UIThread(() => labelLoading.Visible = false);            
        }

        private void CheckEnterKeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                // Enter key pressed
                ValidateAndSave(textBoxSku.Text);
            }
        }

        private void StockCountForm_Closing(object sender, CancelEventArgs e)
        {
            barcode21.EnableScanner = false;
            barcode21.Dispose();
        }

        private void textBoxSku_LostFocus(object sender, EventArgs e)
        {
            var product = FindStockProduct(textBoxSku.Text);
            if (product != null)
            {
                labelMessage.Text = product.LongDescription;
            }
            else
            {
                labelMessage.Text = string.Empty;
            }
        } 
    }
}