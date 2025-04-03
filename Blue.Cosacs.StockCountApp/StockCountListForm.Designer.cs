namespace Blue.Cosacs.StockCountApp
{
    partial class StockCountListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StockCountListForm));
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.buttonBegin = new System.Windows.Forms.Button();
            this.labelIndex = new System.Windows.Forms.Label();
            this.panelDetails = new System.Windows.Forms.Panel();
            this.labelTypeValue = new System.Windows.Forms.Label();
            this.labelType = new System.Windows.Forms.Label();
            this.labelLocationValue = new System.Windows.Forms.Label();
            this.labelLocation = new System.Windows.Forms.Label();
            this.labelIdValue = new System.Windows.Forms.Label();
            this.labelId = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonNext = new System.Windows.Forms.Button();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.labelLoading = new System.Windows.Forms.Label();
            this.barcode21 = new Symbol.Barcode2.Design.Barcode2();
            this.mainMenu = new System.Windows.Forms.MainMenu();
            this.menuItemSettings = new System.Windows.Forms.MenuItem();
            this.panelDetails.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonUpdate.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonUpdate.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.buttonUpdate.Location = new System.Drawing.Point(0, 425);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(638, 30);
            this.buttonUpdate.TabIndex = 1;
            this.buttonUpdate.Text = "Update / Send Counts";
            this.buttonUpdate.Click += new System.EventHandler(this.buttonUpdate_Click);
            // 
            // buttonBegin
            // 
            this.buttonBegin.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonBegin.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonBegin.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.buttonBegin.Location = new System.Drawing.Point(0, 395);
            this.buttonBegin.Name = "buttonBegin";
            this.buttonBegin.Size = new System.Drawing.Size(638, 30);
            this.buttonBegin.TabIndex = 2;
            this.buttonBegin.Text = "Begin Selected";
            this.buttonBegin.Click += new System.EventHandler(this.buttonBegin_Click);
            // 
            // labelIndex
            // 
            this.labelIndex.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.labelIndex.Location = new System.Drawing.Point(0, 47);
            this.labelIndex.Name = "labelIndex";
            this.labelIndex.Size = new System.Drawing.Size(311, 19);
            this.labelIndex.Text = "0 / 0";
            this.labelIndex.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panelDetails
            // 
            this.panelDetails.BackColor = System.Drawing.Color.Transparent;
            this.panelDetails.Controls.Add(this.labelTypeValue);
            this.panelDetails.Controls.Add(this.labelType);
            this.panelDetails.Controls.Add(this.labelLocationValue);
            this.panelDetails.Controls.Add(this.labelLocation);
            this.panelDetails.Controls.Add(this.labelIdValue);
            this.panelDetails.Controls.Add(this.labelId);
            this.panelDetails.Location = new System.Drawing.Point(3, 69);
            this.panelDetails.Name = "panelDetails";
            this.panelDetails.Size = new System.Drawing.Size(308, 71);
            // 
            // labelTypeValue
            // 
            this.labelTypeValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelTypeValue.Location = new System.Drawing.Point(94, 53);
            this.labelTypeValue.Name = "labelTypeValue";
            this.labelTypeValue.Size = new System.Drawing.Size(201, 18);
            // 
            // labelType
            // 
            this.labelType.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelType.Location = new System.Drawing.Point(3, 53);
            this.labelType.Name = "labelType";
            this.labelType.Size = new System.Drawing.Size(85, 18);
            this.labelType.Text = "Type:";
            this.labelType.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelLocationValue
            // 
            this.labelLocationValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelLocationValue.Location = new System.Drawing.Point(94, 21);
            this.labelLocationValue.Name = "labelLocationValue";
            this.labelLocationValue.Size = new System.Drawing.Size(214, 32);
            // 
            // labelLocation
            // 
            this.labelLocation.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelLocation.Location = new System.Drawing.Point(5, 21);
            this.labelLocation.Name = "labelLocation";
            this.labelLocation.Size = new System.Drawing.Size(83, 28);
            this.labelLocation.Text = "Location:";
            this.labelLocation.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // labelIdValue
            // 
            this.labelIdValue.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelIdValue.Location = new System.Drawing.Point(94, 1);
            this.labelIdValue.Name = "labelIdValue";
            this.labelIdValue.Size = new System.Drawing.Size(99, 20);
            // 
            // labelId
            // 
            this.labelId.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular);
            this.labelId.Location = new System.Drawing.Point(5, 1);
            this.labelId.Name = "labelId";
            this.labelId.Size = new System.Drawing.Size(83, 28);
            this.labelId.Text = "Id:";
            this.labelId.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.buttonNext);
            this.panel1.Controls.Add(this.buttonPrev);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 365);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 30);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonNext.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonNext.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.buttonNext.Location = new System.Drawing.Point(478, 0);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(160, 30);
            this.buttonNext.TabIndex = 4;
            this.buttonNext.Text = ">";
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // buttonPrev
            // 
            this.buttonPrev.BackColor = System.Drawing.Color.Gainsboro;
            this.buttonPrev.Dock = System.Windows.Forms.DockStyle.Left;
            this.buttonPrev.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold);
            this.buttonPrev.Location = new System.Drawing.Point(0, 0);
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.Size = new System.Drawing.Size(160, 30);
            this.buttonPrev.TabIndex = 3;
            this.buttonPrev.Text = "<";
            this.buttonPrev.Click += new System.EventHandler(this.buttonPrev_Click);
            // 
            // labelLoading
            // 
            this.labelLoading.Location = new System.Drawing.Point(3, 26);
            this.labelLoading.Name = "labelLoading";
            this.labelLoading.Size = new System.Drawing.Size(308, 21);
            this.labelLoading.Text = "Please wait...";
            // 
            // barcode21
            // 
            this.barcode21.Config.DecoderParameters.CODABAR = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.CODABARParams.ClsiEditing = false;
            this.barcode21.Config.DecoderParameters.CODABARParams.NotisEditing = false;
            this.barcode21.Config.DecoderParameters.CODABARParams.Redundancy = true;
            this.barcode21.Config.DecoderParameters.CODE128 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.CODE128Params.EAN128 = true;
            this.barcode21.Config.DecoderParameters.CODE128Params.ISBT128 = true;
            this.barcode21.Config.DecoderParameters.CODE128Params.Other128 = true;
            this.barcode21.Config.DecoderParameters.CODE128Params.Redundancy = false;
            this.barcode21.Config.DecoderParameters.CODE39 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.CODE39Params.Code32Prefix = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.Concatenation = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.ConvertToCode32 = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.FullAscii = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.Redundancy = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.ReportCheckDigit = false;
            this.barcode21.Config.DecoderParameters.CODE39Params.VerifyCheckDigit = false;
            this.barcode21.Config.DecoderParameters.CODE93 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.CODE93Params.Redundancy = false;
            this.barcode21.Config.DecoderParameters.D2OF5 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.D2OF5Params.Redundancy = true;
            this.barcode21.Config.DecoderParameters.EAN13 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.EAN8 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.EAN8Params.ConvertToEAN13 = false;
            this.barcode21.Config.DecoderParameters.I2OF5 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.I2OF5Params.ConvertToEAN13 = false;
            this.barcode21.Config.DecoderParameters.I2OF5Params.Redundancy = true;
            this.barcode21.Config.DecoderParameters.I2OF5Params.ReportCheckDigit = false;
            this.barcode21.Config.DecoderParameters.I2OF5Params.VerifyCheckDigit = Symbol.Barcode2.Design.I2OF5.CheckDigitSchemes.Default;
            this.barcode21.Config.DecoderParameters.KOREAN_3OF5 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.KOREAN_3OF5Params.Redundancy = true;
            this.barcode21.Config.DecoderParameters.MSI = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.MSIParams.CheckDigitCount = Symbol.Barcode2.Design.CheckDigitCounts.Default;
            this.barcode21.Config.DecoderParameters.MSIParams.CheckDigitScheme = Symbol.Barcode2.Design.CheckDigitSchemes.Default;
            this.barcode21.Config.DecoderParameters.MSIParams.Redundancy = true;
            this.barcode21.Config.DecoderParameters.MSIParams.ReportCheckDigit = false;
            this.barcode21.Config.DecoderParameters.UPCA = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.UPCAParams.Preamble = Symbol.Barcode2.Design.Preambles.Default;
            this.barcode21.Config.DecoderParameters.UPCAParams.ReportCheckDigit = true;
            this.barcode21.Config.DecoderParameters.UPCE0 = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.DecoderParameters.UPCE0Params.ConvertToUPCA = false;
            this.barcode21.Config.DecoderParameters.UPCE0Params.Preamble = Symbol.Barcode2.Design.Preambles.Default;
            this.barcode21.Config.DecoderParameters.UPCE0Params.ReportCheckDigit = false;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.AimDuration = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.AimMode = Symbol.Barcode2.Design.AIM_MODE.AIM_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.AimType = Symbol.Barcode2.Design.AIM_TYPE.AIM_TYPE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.BeamTimer = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.DPMMode = Symbol.Barcode2.Design.DPM_MODE.DPM_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.FocusMode = Symbol.Barcode2.Design.FOCUS_MODE.FOCUS_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.FocusPosition = Symbol.Barcode2.Design.FOCUS_POSITION.FOCUS_POSITION_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.IlluminationMode = Symbol.Barcode2.Design.ILLUMINATION_MODE.ILLUMINATION_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.ImageCaptureTimeout = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.ImageCompressionTimeout = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.Inverse1DMode = Symbol.Barcode2.Design.INVERSE1D_MODE.INVERSE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.LinearSecurityLevel = Symbol.Barcode2.Design.LINEAR_SECURITY_LEVEL.SECURITY_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.PicklistMode = Symbol.Barcode2.Design.PICKLIST_MODE.PICKLIST_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.PointerTimer = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.PoorQuality1DMode = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFFeedback = Symbol.Barcode2.Design.VIEWFINDER_FEEDBACK.VIEWFINDER_FEEDBACK_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFFeedbackTime = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFMode = Symbol.Barcode2.Design.VIEWFINDER_MODE.VIEWFINDER_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFPosition.Bottom = 0;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFPosition.Left = 0;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFPosition.Right = 0;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.ImagerSpecific.VFPosition.Top = 0;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.AimDuration = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.AimMode = Symbol.Barcode2.Design.AIM_MODE.AIM_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.AimType = Symbol.Barcode2.Design.AIM_TYPE.AIM_TYPE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.BeamTimer = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.BeamWidth = Symbol.Barcode2.Design.BEAM_WIDTH.DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.BidirRedundancy = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.ControlScanLed = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.DBPMode = Symbol.Barcode2.Design.DBP_MODE.DBP_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.KlasseEinsEnable = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.LinearSecurityLevel = Symbol.Barcode2.Design.LINEAR_SECURITY_LEVEL.SECURITY_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.PointerTimer = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.RasterHeight = -1;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.RasterMode = Symbol.Barcode2.Design.RASTER_MODE.RASTER_MODE_DEFAULT;
            this.barcode21.Config.ReaderParameters.ReaderSpecific.LaserSpecific.ScanLedLogicLevel = Symbol.Barcode2.Design.DisabledEnabled.Default;
            this.barcode21.Config.ScanParameters.BeepFrequency = 2670;
            this.barcode21.Config.ScanParameters.BeepTime = 200;
            this.barcode21.Config.ScanParameters.CodeIdType = Symbol.Barcode2.Design.CodeIdTypes.Default;
            this.barcode21.Config.ScanParameters.LedTime = 3000;
            this.barcode21.Config.ScanParameters.ScanType = Symbol.Barcode2.Design.SCANTYPES.Default;
            this.barcode21.Config.ScanParameters.WaveFile = "";
            this.barcode21.DeviceType = Symbol.Barcode2.DEVICETYPES.FIRSTAVAILABLE;
            this.barcode21.EnableScanner = true;
            this.barcode21.OnScan += new Symbol.Barcode2.Design.Barcode2.OnScanEventHandler(this.barcode21_OnScan);
            // 
            // mainMenu
            // 
            this.mainMenu.MenuItems.Add(this.menuItemSettings);
            // 
            // menuItemSettings
            // 
            this.menuItemSettings.Text = "Settings";
            this.menuItemSettings.Click += new System.EventHandler(this.menuItemSettings_Click);
            // 
            // StockCountListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(638, 455);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelDetails);
            this.Controls.Add(this.labelIndex);
            this.Controls.Add(this.buttonBegin);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.labelLoading);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Menu = this.mainMenu;
            this.MinimizeBox = false;
            this.Name = "StockCountListForm";
            this.Text = "Available Stock Counts";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.StockCountListForm_Load);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.StockCountListForm_Closing);
            this.panelDetails.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Button buttonBegin;
        private System.Windows.Forms.Label labelIndex;
        private System.Windows.Forms.Panel panelDetails;
        private System.Windows.Forms.Label labelIdValue;
        private System.Windows.Forms.Label labelId;
        private System.Windows.Forms.Label labelTypeValue;
        private System.Windows.Forms.Label labelType;
        private System.Windows.Forms.Label labelLocationValue;
        private System.Windows.Forms.Label labelLocation;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelLoading;
        private Symbol.Barcode2.Design.Barcode2 barcode21;
        private System.Windows.Forms.MainMenu mainMenu;
        private System.Windows.Forms.MenuItem menuItemSettings;
    }
}

