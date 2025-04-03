using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace STL.PL
{
	public class AlphabetEventArgs : EventArgs 
	{  
		private readonly string _clicked ;      

		public AlphabetEventArgs(string clicked) 
		{
			this._clicked = clicked;
		}
      
		public string Clicked
		{     
			get { return _clicked;}      
		}
	}

	public delegate void AlphabetEventHandler(object sender, AlphabetEventArgs e);

	/// <summary>
	/// Summary description for Alphabet.
	/// </summary>
	public class Alphabet : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Button bNum;
		private System.Windows.Forms.Button bA;
		private System.Windows.Forms.Button bB;
		private System.Windows.Forms.Button bC;
		private System.Windows.Forms.Button bD;
		private System.Windows.Forms.Button bE;
		private System.Windows.Forms.Button bF;
		private System.Windows.Forms.Button bG;
		private System.Windows.Forms.Button bH;
		private System.Windows.Forms.Button bI;
		private System.Windows.Forms.Button bJ;
		private System.Windows.Forms.Button bK;
		private System.Windows.Forms.Button bL;
		private System.Windows.Forms.Button bM;
		private System.Windows.Forms.Button bN;
		private System.Windows.Forms.Button bO;
		private System.Windows.Forms.Button bP;
		private System.Windows.Forms.Button bQ;
		private System.Windows.Forms.Button bR;
		private System.Windows.Forms.Button bS;
		private System.Windows.Forms.Button bT;
		private System.Windows.Forms.Button bU;
		private System.Windows.Forms.Button bV;
		private System.Windows.Forms.Button bW;
		private System.Windows.Forms.Button bX;
		private System.Windows.Forms.Button bY;
		private System.Windows.Forms.Button bZ;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;	
	
		public event AlphabetEventHandler Clicked;

		protected virtual void OnClicked(AlphabetEventArgs e)
		{
			if (Clicked != null) 
			{
				// Invokes the delegates. 
				Clicked(this, e);
			}
		}


		public Alphabet()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.bNum = new System.Windows.Forms.Button();
			this.bA = new System.Windows.Forms.Button();
			this.bB = new System.Windows.Forms.Button();
			this.bC = new System.Windows.Forms.Button();
			this.bD = new System.Windows.Forms.Button();
			this.bE = new System.Windows.Forms.Button();
			this.bF = new System.Windows.Forms.Button();
			this.bG = new System.Windows.Forms.Button();
			this.bH = new System.Windows.Forms.Button();
			this.bI = new System.Windows.Forms.Button();
			this.bJ = new System.Windows.Forms.Button();
			this.bK = new System.Windows.Forms.Button();
			this.bL = new System.Windows.Forms.Button();
			this.bM = new System.Windows.Forms.Button();
			this.bN = new System.Windows.Forms.Button();
			this.bO = new System.Windows.Forms.Button();
			this.bP = new System.Windows.Forms.Button();
			this.bQ = new System.Windows.Forms.Button();
			this.bR = new System.Windows.Forms.Button();
			this.bS = new System.Windows.Forms.Button();
			this.bT = new System.Windows.Forms.Button();
			this.bU = new System.Windows.Forms.Button();
			this.bV = new System.Windows.Forms.Button();
			this.bW = new System.Windows.Forms.Button();
			this.bX = new System.Windows.Forms.Button();
			this.bY = new System.Windows.Forms.Button();
			this.bZ = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// bNum
			// 
			this.bNum.Name = "bNum";
			this.bNum.Size = new System.Drawing.Size(32, 23);
			this.bNum.TabIndex = 0;
			this.bNum.Tag = "0";
			this.bNum.Text = "0-9";
			this.bNum.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bA
			// 
			this.bA.Location = new System.Drawing.Point(32, 0);
			this.bA.Name = "bA";
			this.bA.Size = new System.Drawing.Size(24, 23);
			this.bA.TabIndex = 1;
			this.bA.Tag = "A";
			this.bA.Text = "A";
			this.bA.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bB
			// 
			this.bB.Location = new System.Drawing.Point(56, 0);
			this.bB.Name = "bB";
			this.bB.Size = new System.Drawing.Size(24, 23);
			this.bB.TabIndex = 2;
			this.bB.Tag = "B";
			this.bB.Text = "B";
			this.bB.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bC
			// 
			this.bC.Location = new System.Drawing.Point(80, 0);
			this.bC.Name = "bC";
			this.bC.Size = new System.Drawing.Size(24, 23);
			this.bC.TabIndex = 3;
			this.bC.Tag = "C";
			this.bC.Text = "C";
			this.bC.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bD
			// 
			this.bD.Location = new System.Drawing.Point(104, 0);
			this.bD.Name = "bD";
			this.bD.Size = new System.Drawing.Size(24, 23);
			this.bD.TabIndex = 4;
			this.bD.Tag = "D";
			this.bD.Text = "D";
			this.bD.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bE
			// 
			this.bE.Location = new System.Drawing.Point(128, 0);
			this.bE.Name = "bE";
			this.bE.Size = new System.Drawing.Size(24, 23);
			this.bE.TabIndex = 5;
			this.bE.Tag = "E";
			this.bE.Text = "E";
			this.bE.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bF
			// 
			this.bF.Location = new System.Drawing.Point(152, 0);
			this.bF.Name = "bF";
			this.bF.Size = new System.Drawing.Size(24, 23);
			this.bF.TabIndex = 10;
			this.bF.Tag = "F";
			this.bF.Text = "F";
			this.bF.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bG
			// 
			this.bG.Location = new System.Drawing.Point(176, 0);
			this.bG.Name = "bG";
			this.bG.Size = new System.Drawing.Size(24, 23);
			this.bG.TabIndex = 9;
			this.bG.Tag = "G";
			this.bG.Text = "G";
			this.bG.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bH
			// 
			this.bH.Location = new System.Drawing.Point(200, 0);
			this.bH.Name = "bH";
			this.bH.Size = new System.Drawing.Size(24, 23);
			this.bH.TabIndex = 8;
			this.bH.Tag = "H";
			this.bH.Text = "H";
			this.bH.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bI
			// 
			this.bI.Location = new System.Drawing.Point(224, 0);
			this.bI.Name = "bI";
			this.bI.Size = new System.Drawing.Size(24, 23);
			this.bI.TabIndex = 7;
			this.bI.Tag = "I";
			this.bI.Text = "I";
			this.bI.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bJ
			// 
			this.bJ.Location = new System.Drawing.Point(248, 0);
			this.bJ.Name = "bJ";
			this.bJ.Size = new System.Drawing.Size(24, 23);
			this.bJ.TabIndex = 6;
			this.bJ.Tag = "J";
			this.bJ.Text = "J";
			this.bJ.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bK
			// 
			this.bK.Location = new System.Drawing.Point(272, 0);
			this.bK.Name = "bK";
			this.bK.Size = new System.Drawing.Size(24, 23);
			this.bK.TabIndex = 20;
			this.bK.Tag = "K";
			this.bK.Text = "K";
			this.bK.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bL
			// 
			this.bL.Location = new System.Drawing.Point(296, 0);
			this.bL.Name = "bL";
			this.bL.Size = new System.Drawing.Size(24, 23);
			this.bL.TabIndex = 19;
			this.bL.Tag = "L";
			this.bL.Text = "L";
			this.bL.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bM
			// 
			this.bM.Location = new System.Drawing.Point(320, 0);
			this.bM.Name = "bM";
			this.bM.Size = new System.Drawing.Size(24, 23);
			this.bM.TabIndex = 18;
			this.bM.Tag = "M";
			this.bM.Text = "M";
			this.bM.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bN
			// 
			this.bN.Location = new System.Drawing.Point(344, 0);
			this.bN.Name = "bN";
			this.bN.Size = new System.Drawing.Size(24, 23);
			this.bN.TabIndex = 17;
			this.bN.Tag = "N";
			this.bN.Text = "N";
			this.bN.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bO
			// 
			this.bO.Location = new System.Drawing.Point(368, 0);
			this.bO.Name = "bO";
			this.bO.Size = new System.Drawing.Size(24, 23);
			this.bO.TabIndex = 16;
			this.bO.Tag = "O";
			this.bO.Text = "O";
			this.bO.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bP
			// 
			this.bP.Location = new System.Drawing.Point(392, 0);
			this.bP.Name = "bP";
			this.bP.Size = new System.Drawing.Size(24, 23);
			this.bP.TabIndex = 15;
			this.bP.Tag = "P";
			this.bP.Text = "P";
			this.bP.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bQ
			// 
			this.bQ.Location = new System.Drawing.Point(416, 0);
			this.bQ.Name = "bQ";
			this.bQ.Size = new System.Drawing.Size(24, 23);
			this.bQ.TabIndex = 14;
			this.bQ.Tag = "Q";
			this.bQ.Text = "Q";
			this.bQ.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bR
			// 
			this.bR.Location = new System.Drawing.Point(440, 0);
			this.bR.Name = "bR";
			this.bR.Size = new System.Drawing.Size(24, 23);
			this.bR.TabIndex = 13;
			this.bR.Tag = "R";
			this.bR.Text = "R";
			this.bR.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bS
			// 
			this.bS.Location = new System.Drawing.Point(464, 0);
			this.bS.Name = "bS";
			this.bS.Size = new System.Drawing.Size(24, 23);
			this.bS.TabIndex = 12;
			this.bS.Tag = "S";
			this.bS.Text = "S";
			this.bS.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bT
			// 
			this.bT.Location = new System.Drawing.Point(488, 0);
			this.bT.Name = "bT";
			this.bT.Size = new System.Drawing.Size(24, 23);
			this.bT.TabIndex = 11;
			this.bT.Tag = "T";
			this.bT.Text = "T";
			this.bT.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bU
			// 
			this.bU.Location = new System.Drawing.Point(512, 0);
			this.bU.Name = "bU";
			this.bU.Size = new System.Drawing.Size(24, 23);
			this.bU.TabIndex = 25;
			this.bU.Tag = "U";
			this.bU.Text = "U";
			this.bU.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bV
			// 
			this.bV.Location = new System.Drawing.Point(536, 0);
			this.bV.Name = "bV";
			this.bV.Size = new System.Drawing.Size(24, 23);
			this.bV.TabIndex = 24;
			this.bV.Tag = "V";
			this.bV.Text = "V";
			this.bV.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bW
			// 
			this.bW.Location = new System.Drawing.Point(560, 0);
			this.bW.Name = "bW";
			this.bW.Size = new System.Drawing.Size(24, 23);
			this.bW.TabIndex = 23;
			this.bW.Tag = "W";
			this.bW.Text = "W";
			this.bW.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bX
			// 
			this.bX.Location = new System.Drawing.Point(584, 0);
			this.bX.Name = "bX";
			this.bX.Size = new System.Drawing.Size(24, 23);
			this.bX.TabIndex = 22;
			this.bX.Tag = "X";
			this.bX.Text = "X";
			this.bX.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bY
			// 
			this.bY.Location = new System.Drawing.Point(608, 0);
			this.bY.Name = "bY";
			this.bY.Size = new System.Drawing.Size(24, 23);
			this.bY.TabIndex = 21;
			this.bY.Tag = "Y";
			this.bY.Text = "Y";
			this.bY.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// bZ
			// 
			this.bZ.Location = new System.Drawing.Point(632, 0);
			this.bZ.Name = "bZ";
			this.bZ.Size = new System.Drawing.Size(24, 23);
			this.bZ.TabIndex = 26;
			this.bZ.Tag = "Z";
			this.bZ.Text = "Z";
			this.bZ.Click += new System.EventHandler(this.OnButtonClick);
			// 
			// Alphabet
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.bZ,
																		  this.bU,
																		  this.bV,
																		  this.bW,
																		  this.bX,
																		  this.bY,
																		  this.bF,
																		  this.bG,
																		  this.bH,
																		  this.bI,
																		  this.bJ,
																		  this.bK,
																		  this.bL,
																		  this.bM,
																		  this.bN,
																		  this.bO,
																		  this.bP,
																		  this.bQ,
																		  this.bR,
																		  this.bS,
																		  this.bT,
																		  this.bE,
																		  this.bD,
																		  this.bC,
																		  this.bB,
																		  this.bA,
																		  this.bNum});
			this.Name = "Alphabet";
			this.Size = new System.Drawing.Size(656, 24);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnButtonClick(object sender, System.EventArgs e)
		{
			AlphabetEventArgs ev = new AlphabetEventArgs((string)((Button)sender).Tag);
			OnClicked(ev);
		}

	}
}
