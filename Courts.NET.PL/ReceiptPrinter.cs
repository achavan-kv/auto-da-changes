using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Collections;
using System.Globalization;
using POS.Devices;
using System.Threading;
using System.Windows.Forms;
using STL.AppUpdater;
using STL.Common.Static;
using STL.Common;
using STL.Common.Printing.CustomerCard;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;


namespace STL.PL
{
	/// <summary>
	/// Common printing routines for the slip printer, used for printing
	/// customer receipts and the customer payment card.
	/// </summary>
	public class ReceiptPrinter
	{
		protected OPOSPOSPrinter Printer = null;
        protected _IOPOSPOSPrinterEvents_StatusUpdateEventEventHandler SUEH;
        protected const int OPOS_SUCCESS = 0;
		
        //private static Hashtable conversion;
        //public static Hashtable Conversion 
        //{
        //    get
        //    {
        //        if(conversion==null)
        //            InitialiseUnicodeAsciiConversionTable();
        //        return conversion;
        //    }
        //}

		//declare printer command strings

		//rotation constants
		private const int PTR_RP_ROTATE180		= 0x0103;
		private const int PTR_RP_NORMAL			= 0x0001;
		private const int PTR_RP_RIGHT90		= 0x0101;
		private const int PTR_RP_LEFT90			= 0x0102;

		//printer station constants
        protected const int PTR_S_SLIP = 4;

		private string NEWLINE = Convert.ToString('\n');
		private string INITIALISE = "\x1b\x40";
		//private string NARROW = "\x1b\x21\x01";
		//private string WIDE = "\x1b\x21\x02";
		//private string LINESPACE_11 = "\x1b\x33\x0A";
		//private string LINESPACE_15 = "\x1b\x33\x0A";
		private string OPENDRAWER = "\x1b\x70\x0";

		private string INITFEED_290 = "1b64";
        protected string RELEASE_290 = "1b71";
		private string REVERSE_290 = "1b65";
		private string NARROW_290 = "1b2103";
		private string WIDE_290 = "1b2102";
		private string INITIALISE_290 = "18h";
		private string INITIALISE_290_2 = "1b40";
		private string INVERT = "1b7b01";
		private string PAPERSTATUSREQUEST = "1b76";
		private string PAPERSTATUSREQUEST_2 = "1B7601";

		private int PORT = 0;
		private int SPEED = 9600;
		private string PARITY = "E"; 
		private int WORD_LENGTH = 7; 
		private int STOPBITS = 1;

		//private readonly string rpOldModel = "290";
        protected readonly string rpNewModel = "295";

		private int lineSpace = 15;

		private int _open = 0;
		private string _error = "";
        protected string _card = "S";
        protected int rc = 0;


		/// <summary>
		/// Get or set the card property
		/// </summary>
		public string Card
		{
			get{return _card;}
			set{_card = value;}
		}
		/// <summary>
		/// Property to get any error message
		/// </summary>
		public string Error
		{
			get
			{return _error;}
		}

		/// <summary>
		/// Gets the property describing whether the comms port is open or not
		/// </summary>
		public bool Open
		{
			get{return Convert.ToBoolean(_open);}
		}

		public bool SlpEmpty
		{
			get 
			{
				if(Config.ReceiptPrinterModel == rpNewModel)
				{
					if (Printer != null) return Printer.SlpEmpty;
					else return false;
				}
				else
					return IsPaperOut();
			}
		}

		private bool _inverted = false;
		protected CommonForm _sender=null;

		[DllImport("SLIP.DLL")]
		public static extern int CoslOpenCommsPort(int port, int speed, string parity, 
													int word_length, int stopbits);  
		
		[DllImport("SLIP.DLL")]
		public static extern int CoslCloseCommsPort(int port);  

		[DllImport("SLIP.DLL")]
		public static extern int CoslWriteHex(string hexValue);  

		[DllImport("SLIP.DLL")]
		public static extern int CoslWriteCommsPort(string txtValue, int txtLength);  

		[DllImport("SLIP.DLL")]
		public static extern int CoslReadCharTimed();  

		[DllImport("SLIP.DLL")]
		public static extern int CoslWriteDec(string lines);

        public ReceiptPrinter() { }
		public ReceiptPrinter(CommonForm sender)
		{
			try
			{
				_sender = sender;
				Printer = new OPOSPOSPrinterClass();

				SUEH =	new _IOPOSPOSPrinterEvents_StatusUpdateEventEventHandler(PrinterStatusUpdateHandler );
				Printer.StatusUpdateEvent += SUEH;
			}
			catch(COMException)
			{
				//_sender.ShowInfo("M_OPOSDOWNLOADING");
				//((MainForm)_sender.FormRoot).lDownloading.Visible = true;
				//((MainForm)_sender.FormRoot).pbDownloading.Visible = true;
				//Start a new thread to download the controls required for the slip printer
				//Thread download = new Thread(new ThreadStart(DownloadOPOSControls));
				//download.Start();
                //download.Join(5000); //5 seconds
				throw new SlipPrinterException("Slip Printer Not Installed.");
			}
		}

        //private void DownloadOPOSControls()
        //{
        //    try
        //    {
        //        string dest = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        //        dest += "\\STL\\Courts.NET\\Client\\";
        //        int files = WebFileLoader.CopyDirectory(Config.Url + "OPOS/", dest);
        //        System.Diagnostics.Process.Start(dest + "disk1\\setup.exe", "/a\"" + dest + "disk1\\OposData.reg\"");

        //        ((MainForm)_sender.FormRoot).lDownloading.Visible = false;
        //        ((MainForm)_sender.FormRoot).pbDownloading.Visible = false;
        //    }
        //    catch (System.ComponentModel.Win32Exception ex)
        //    {
        //        _sender.logException(ex, Credential.User, "DownloadOPOSControls");
        //        _sender.ShowInfo("M_OPOSDOWNLOADERROR");
        //    }
        //    catch (IOException ex)
        //    {
        //        _sender.logException(ex, Credential.User, "DownloadOPOSControls");
        //        _sender.ShowError("File Access Error");
        //    }
        //    catch (Exception ex)
        //    {
        //        _sender.logException(ex, Credential.User, "DownloadOPOSControls");
        //        _sender.ShowError("General Error");
        //    }
        //}


        //private void CreateDirectories(List<STL.PL.WSUpdater.UpdateFile> files, int lastpostition, string startpath)
        //{
        //    string clientdir = "";
        //    DirectoryInfo newdir = new DirectoryInfo("C:\\");
        //    foreach (STL.PL.WSUpdater.UpdateFile file in files)
        //    {
        //        if (file.dir == true && file.fullpath.Length > lastpostition)
        //        {
        //            clientdir = file.fullpath.Substring(lastpostition, file.fullpath.Length - lastpostition);
        //            newdir = new DirectoryInfo(startpath + @"\OPOS" + clientdir);
        //            if (!newdir.Exists)
        //            {
        //                newdir.Create();
        //            }
        //        }
        //    }

        //    if (newdir != null)
        //    {
        //        newdir = null;
        //    }
        //}

		protected void PrinterStatusUpdateHandler(int nStatus)
		{
			((MainForm)_sender.FormRoot).statusBar1.Text = "Printer status: "+ nStatus.ToString();
		}

		public void OpenPrinter()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				//open the printer
				rc = Printer.Open("SlipPrinter");
				if ( rc == OPOS_SUCCESS )
				{
					rc = Printer.ClaimDevice(1000);
					// If succeeded, then enable.
					if ( rc == OPOS_SUCCESS )
					{
						_open = 1;
						Printer.DeviceEnabled = true;
						rc = Printer.ResultCode;
					}
					else
                        _error = CommonForm.GetResource("M_PRINTERCLAIMFAIL", new Object[] { rc.ToString() });
				}
				else
                    _error = CommonForm.GetResource("M_PRINTEROPENFAIL", new Object[] { rc.ToString() });

				if(rc!=OPOS_SUCCESS)
					throw new SlipPrinterException(_error);
			}
			else
			{
				rc = CoslOpenCommsPort(PORT, SPEED, PARITY, WORD_LENGTH, STOPBITS);
				rc = CoslCloseCommsPort(PORT);
				rc = CoslOpenCommsPort(PORT, SPEED, PARITY, WORD_LENGTH, STOPBITS);

				rc = CoslCloseCommsPort(PORT);

				rc = CoslOpenCommsPort(PORT, SPEED, PARITY, WORD_LENGTH, STOPBITS);
				rc = CoslCloseCommsPort(PORT);
				rc = CoslOpenCommsPort(PORT, SPEED, PARITY, WORD_LENGTH, STOPBITS);

				if ( rc == OPOS_SUCCESS )
					_open = 1;
				else
                    _error = CommonForm.GetResource("M_PRINTEROPENFAIL", new Object[] { rc.ToString() });

				if(rc!=OPOS_SUCCESS)
					throw new SlipPrinterException(_error);
			}
		}

		public void PrintString(bool checkPaper, string text)
		{
			/* split the string into an array of strings based on the 
			 * newline constant and then print each string individually.
			 * Otherwise the PaperOut warning may not work */
			//string[] lines = text.Split(Environment.NewLine.ToCharArray());
			//if(lines.Length>1)
			//	foreach(string line in lines)
			//		PrintString(line);

			string nxtLine = "";
			int curStart = 0;
			int curLength = text.IndexOf(Environment.NewLine, curStart) + 1;
			while (curLength > 0)
			{
				nxtLine = text.Substring(curStart, curLength - 1);
				PrintLine(checkPaper, nxtLine);
				curStart += curLength + 1;
				if (curStart <= text.Length - 1)
					curLength = text.IndexOf(Environment.NewLine, curStart) - curStart + 1;
				else
					curLength = 0;
			}

			// There might not be a trailing newline
			if (curStart <= text.Length - 1)
			{
				nxtLine = text.Substring(curStart, text.Length - curStart);
				PrintLine(checkPaper, nxtLine);
			}

		}

		public void PrintString(string text)
		{
			PrintString(true, text);
		}

        protected virtual void PrintLine(bool checkPaper, string text)
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if (Open)
				{
					// Currently have to reopen printer to refresh paper out sensor
					// TO DO : too slow this.ReOpenPrinter();
					if(checkPaper)
					{
						while (Printer.SlpEmpty)
						{
                            if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
								"Printer Error", MessageBoxButtons.RetryCancel,
								MessageBoxIcon.Information))
								Thread.Sleep(100);
							else
								throw new SlipPrinterException("Cancel");
						}	
					}

					/* JJ as long as regional settings are set correctly 
					* this translation step is unnecessary. Sickeningly unnecessary.
					char[] txt = text.ToCharArray();
					string hex="";
					foreach(char c in txt)
					{					
						if(Char.GetUnicodeCategory(c)==UnicodeCategory.OtherLetter)
							hex += (string)ReceiptPrinter.Conversion[c];
						else
							hex += c.ToString();
					}
					rc = Printer.PrintNormal(PTR_S_SLIP, hex+Environment.NewLine);
					*/
					rc = Printer.PrintNormal(PTR_S_SLIP, text+Environment.NewLine);
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERPRINTFAIL", new Object[] { rc.ToString() }));
				}
				else
                    throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERCLOSED"));
			}
			else
			{
				if (Open)
				{
					// Currently have to reopen printer to refresh paper out sensor
					// TO DO : too slow this.ReOpenPrinter();
					if(checkPaper)
					{
						while (SlpEmpty)
						{
                            if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
								"Printer Error", MessageBoxButtons.RetryCancel,
								MessageBoxIcon.Information))
								Thread.Sleep(100);
							else
								throw new SlipPrinterException("Cancel");
						}	
					}
					
					text += "\n";
					rc = CoslWriteCommsPort(text, text.Length);
				}
				else
                    throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERCLOSED"));
			}
		}


        public virtual void ClosePrinter()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					rc = Printer.Close();
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERCLOSEFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				if(Open)
				{
					rc = CoslCloseCommsPort(PORT);
					if(rc != OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERCLOSEFAIL", new Object[] { rc.ToString() }));
				}
			}
		}

		public void ReOpenPrinter ()
		{
			int lineSpace = this.LineSpacing;
			int lineChars = this.LineChars;
			bool invertToggle = this._inverted;

			this.ClosePrinter();
			this.OpenPrinter();

			// Set the spacing back to as it was
			this.LineSpacing = lineSpace;
			this.LineChars	= lineChars;
			// Toggle the invert position back to as it was
			this._inverted = !invertToggle;
			this.Invert();
		}

		public void Flush()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					rc = Printer.PrintNormal(PTR_S_SLIP, "\n");
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERFLUSHFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				if(Open)
				{
					rc = CoslWriteCommsPort("\n", 1);
				}
			}
		}

		public void Narrow()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
				this.LineChars = 42;
			else
				CoslWriteHex(NARROW_290);
		}

		public void Wide()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
				this.LineChars = 35;
			else
				CoslWriteHex(WIDE_290);
		}

		private int LineChars
		{
			get
			{
				return Printer.SlpLineChars;
			}
			set
			{
				if(Open)
				{
					lineSpace = Printer.SlpLineSpacing;
					Printer.SlpLineChars = value;
					Printer.SlpLineSpacing = lineSpace;
				}
			}
		}

		public int LineSpacing
		{
			get
			{
				return Printer.SlpLineSpacing;
			}
			set
			{
				Printer.SlpLineSpacing = lineSpace = value;
			}
		}

		public virtual void Release()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					Printer.BeginRemoval(100);
				}
			}
			else
			{
				CoslWriteHex(RELEASE_290);
			}

		}	

		public void Feed(int lines)
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					// Currently have to reopen printer to refresh paper out sensor
					// TO DO : too slow this.ReOpenPrinter();
					while(Printer.SlpEmpty)
					{
                        if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
							"Printer Error", MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Information))
							Thread.Sleep(100);
						else
							throw new SlipPrinterException("Cancel");
					}

					string txt="";
					for(int i=0; i<lines; i++)
						txt+="\n";	   
					rc = Printer.PrintNormal(PTR_S_SLIP, txt);
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERFEEDFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				/*while(SlpEmpty)
				{
					if(DialogResult.Retry == MessageBox.Show(_sender.GetResource("M_ADDPAPER"),
						"Printer Error", MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Information))
						Thread.Sleep(100);
					else
						throw new SlipPrinterException("Cancel");
				}*/

				string lines_text = "";

				rc = CoslWriteHex(INITFEED_290);

				if(rc == OPOS_SUCCESS)
				{
					if(lines < 10)
						lines_text = "0" + lines.ToString();
					else
						lines_text = lines.ToString();

					rc = CoslWriteDec(lines_text);
				}

				if(rc!=OPOS_SUCCESS)
                    throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERFEEDFAIL", new Object[] { rc.ToString() }));
			}
		}

		public void ReverseFeed(string hex, int startLine)
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					while(Printer.SlpEmpty)
					{
                        if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
							"Printer Error", MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Information))
							Thread.Sleep(100);
						else
							throw new SlipPrinterException("Cancel");
					}
					string txt="\x1b\x65"+hex;	
					rc = Printer.PrintNormal(PTR_S_SLIP, txt);
					//if(rc!=OPOS_SUCCESS)
					//	throw new SlipPrinterException(_sender.GetResource("M_PRINTERFEEDFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
				FeedCheck(startLine);
		}


		public void Feed(string hex, int lines)
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					// Currently have to reopen printer to refresh paper out sensor
					// TO DO : too slow this.ReOpenPrinter();
					while(Printer.SlpEmpty)
					{
                        if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
							"Printer Error", MessageBoxButtons.RetryCancel,
							MessageBoxIcon.Information))
							Thread.Sleep(100);
						else
							throw new SlipPrinterException("Cancel");
					}
					string txt="\x1b\x64"+hex;	
					rc = Printer.PrintNormal(PTR_S_SLIP, txt);
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERFEEDFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				while(SlpEmpty)
				{
                    if (DialogResult.Retry == MessageBox.Show(CommonForm.GetResource("M_ADDPAPER"),
						"Printer Error", MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Information))
						Thread.Sleep(100);
					else
						throw new SlipPrinterException("Cancel");
				}
				
				this.Feed(lines);
			}
		}

		public void Init()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					rc = Printer.PrintNormal(PTR_S_SLIP, this.INITIALISE);
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERINITFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				if(Open)
				{
					rc = CoslWriteHex(INITIALISE_290);
					if(rc == OPOS_SUCCESS)
						rc = CoslWriteHex(INITIALISE_290_2);
					
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERINITFAIL", new Object[] { rc.ToString() }));
					else
						this.SetPaymentCard();
				}
			}
		}

		public void OpenDrawer()
		{
			if(Open)
			{
				rc = Printer.PrintNormal(PTR_S_SLIP, this.OPENDRAWER);
				if(rc!=OPOS_SUCCESS)
                    throw new SlipPrinterException(CommonForm.GetResource("M_PRINTEROPENDRAWERFAIL", new Object[] { rc.ToString() }));
			}
		}

		public void Invert()
		{
			if(Config.ReceiptPrinterModel == rpNewModel)
			{
				if(Open)
				{
					if(!_inverted)
						rc = Printer.RotatePrint(4, PTR_RP_ROTATE180);
					else
						rc = Printer.RotatePrint(4, PTR_RP_NORMAL);
					_inverted = _inverted==true?false:true;
					if(rc!=OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERINVERTFAIL", new Object[] { rc.ToString() }));
				}
			}
			else
			{
				if(Open)
				{
					rc = CoslWriteHex(INVERT);
					if(rc != OPOS_SUCCESS)
                        throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERINVERTFAIL", new Object[] { rc.ToString() }));
				}
			}
		}

		public bool IsPaperOut()
		{
			bool paperOut = false;

			rc = CoslWriteHex(PAPERSTATUSREQUEST);
			if(rc != OPOS_SUCCESS)
				paperOut = true;

			if(!paperOut)
			{
				if(CoslReadCharTimed() != 0)
				{
					rc = CoslWriteHex(PAPERSTATUSREQUEST_2);
					if(CoslReadCharTimed() != 0)
						paperOut = true;
					else
						paperOut = false;
				}
			}
			
			return paperOut;
		}

		public void FeedCheck(int startLine)
		{
			this.Init();

			if(Card == CardType.typeLong)
			{
				if(startLine > 1)
				{
					this.ReverseFeed(3);
					Thread.Sleep(3000);
					this.Feed(startLine - 1);
				}
				else if(startLine == 1)
					this.ReverseFeed(3);
			}
			else if(Card == CardType.typeShort)
			{
				if(startLine > 5)
					this.Feed(startLine - 5);
				else
					this.ReverseFeed(5 - startLine + 1);
			}
		}

		public void ReverseFeed(int lines)
		{
			if(Open)
			{
				/*while(SlpEmpty)
				{
					if(DialogResult.Retry == MessageBox.Show(_sender.GetResource("M_ADDPAPER"),
						"Printer Error", MessageBoxButtons.RetryCancel,
						MessageBoxIcon.Information))
						Thread.Sleep(100);
					else
						throw new SlipPrinterException("Cancel");
				}*/

				string lines_text = "";

				if(lines < 10)
					lines_text = "0" + lines.ToString();
				else
					lines_text = lines.ToString();

				rc = CoslWriteHex(REVERSE_290 + lines_text);
				if(rc != OPOS_SUCCESS)
                    throw new SlipPrinterException(CommonForm.GetResource("M_PRINTERFEEDFAIL", new Object[] { rc.ToString() }));
			}
		}

		public void SetPaymentCard()
		{
			this.Narrow();

			if(Card == CardType.typeLong)
				CoslWriteHex("1b330F");
			else
				CoslWriteHex("1b330B");
		}
		
        //private static void InitialiseUnicodeAsciiConversionTable()
        //{
        //    /* all unneccessary with the regional settings set correctly
        //    if(conversion==null)
        //        conversion = new Hashtable();

        //    conversion.Add('\u0E01', "\xA1");
        //    conversion.Add('\u0E02', "\xA2");
        //    conversion.Add('\u0E03', "\xA3");
        //    conversion.Add('\u0E04', "\xA4");
        //    conversion.Add('\u0E05', "\xA5");
        //    conversion.Add('\u0E06', "\xA6");
        //    conversion.Add('\u0E07', "\xA7");
        //    conversion.Add('\u0E08', "\xA8");
        //    conversion.Add('\u0E09', "\xA9");
        //    conversion.Add('\u0E0A', "\xAA");
        //    conversion.Add('\u0E0B', "\xAB");
        //    conversion.Add('\u0E0C', "\xAC");
        //    conversion.Add('\u0E0D', "\xAD");
        //    conversion.Add('\u0E0E', "\xAE");
        //    conversion.Add('\u0E0F', "\xAF");

        //    conversion.Add('\u0E11', "\xB1");
        //    conversion.Add('\u0E12', "\xB2");
        //    conversion.Add('\u0E13', "\xB3");
        //    conversion.Add('\u0E14', "\xB4");
        //    conversion.Add('\u0E15', "\xB5");
        //    conversion.Add('\u0E16', "\xB6");
        //    conversion.Add('\u0E17', "\xB7");
        //    conversion.Add('\u0E18', "\xB8");
        //    conversion.Add('\u0E19', "\xB9");
        //    conversion.Add('\u0E1A', "\xBA");
        //    conversion.Add('\u0E1B', "\xBB");
        //    conversion.Add('\u0E1C', "\xBC");
        //    conversion.Add('\u0E1D', "\xBD");
        //    conversion.Add('\u0E1E', "\xBE");
        //    conversion.Add('\u0E1F', "\xBF");

        //    conversion.Add('\u0E21', "\xC1");
        //    conversion.Add('\u0E22', "\xC2");
        //    conversion.Add('\u0E23', "\xC3");
        //    conversion.Add('\u0E24', "\xC4");
        //    conversion.Add('\u0E25', "\xC5");
        //    conversion.Add('\u0E26', "\xC6");
        //    conversion.Add('\u0E27', "\xC7");
        //    conversion.Add('\u0E28', "\xC8");
        //    conversion.Add('\u0E29', "\xC9");
        //    conversion.Add('\u0E2A', "\xCA");
        //    conversion.Add('\u0E2B', "\xCB");
        //    conversion.Add('\u0E2C', "\xCC");
        //    conversion.Add('\u0E2D', "\xCD");
        //    conversion.Add('\u0E2E', "\xCE");
        //    conversion.Add('\u0E2F', "\xCF");

        //    conversion.Add('\u0E31', "\xD1");
        //    conversion.Add('\u0E32', "\xD2");
        //    conversion.Add('\u0E33', "\xD3");
        //    conversion.Add('\u0E34', "\xD4");
        //    conversion.Add('\u0E35', "\xD5");
        //    conversion.Add('\u0E36', "\xD6");
        //    conversion.Add('\u0E37', "\xD7");
        //    conversion.Add('\u0E38', "\xD8");
        //    conversion.Add('\u0E39', "\xD9");
        //    conversion.Add('\u0E3A', "\xDA");
        //    conversion.Add('\u0E3B', "\xDB");
        //    conversion.Add('\u0E3C', "\xDC");
        //    conversion.Add('\u0E3D', "\xDD");
        //    conversion.Add('\u0E3E', "\xDE");
        //    conversion.Add('\u0E3F', "\xDF");

        //    conversion.Add('\u0E41', "\xE1");
        //    conversion.Add('\u0E42', "\xE2");
        //    conversion.Add('\u0E43', "\xE3");
        //    conversion.Add('\u0E44', "\xE4");
        //    conversion.Add('\u0E45', "\xE5");
        //    conversion.Add('\u0E46', "\xE6");
        //    conversion.Add('\u0E47', "\xE7");
        //    conversion.Add('\u0E48', "\xE8");
        //    conversion.Add('\u0E49', "\xE9");
        //    conversion.Add('\u0E4A', "\xEA");
        //    conversion.Add('\u0E4B', "\xEB");
        //    conversion.Add('\u0E4C', "\xEC");
        //    conversion.Add('\u0E4D', "\xED");
        //    conversion.Add('\u0E4E', "\xEE");
        //    conversion.Add('\u0E4F', "\xEF");

        //    conversion.Add('\u0E51', "\xF1");
        //    conversion.Add('\u0E52', "\xF2");
        //    conversion.Add('\u0E53', "\xF3");
        //    conversion.Add('\u0E54', "\xF4");
        //    conversion.Add('\u0E55', "\xF5");
        //    conversion.Add('\u0E56', "\xF6");
        //    conversion.Add('\u0E57', "\xF7");
        //    conversion.Add('\u0E58', "\xF8");
        //    conversion.Add('\u0E59', "\xF9");
        //    conversion.Add('\u0E5A', "\xFA");
        //    conversion.Add('\u0E5B', "\xFB");
        //    */
			
        //}		
	}
}


