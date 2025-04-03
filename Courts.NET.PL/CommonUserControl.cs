using System;
using System.Collections.Specialized;
using System.Collections;
using STL.Common.Static;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS5;
using System.Data;
using STL.Common.Constants.TableNames;
using System.ComponentModel;

namespace STL.PL
{
	/// <summary>
	/// All of the user controls in the Presentation Layer (PL) project are
	/// derived from this base class that contains common functions for:
	///  - Country Parameters
	///  - Language Translation
	/// </summary>
	public class CommonUserControl : System.Windows.Forms.UserControl
	{
		private string Error = "";

		public CommonUserControl()
		{
		}
		public StringCollection ListLabels()
		{
			StringCollection s = new StringCollection();			
			ListLabels(this.Controls, ref s);
			return s;
		}

		public void ListLabels(System.Windows.Forms.Control.ControlCollection controls, ref StringCollection s)
		{
			foreach(Control c in controls)
			{
				
				switch((c.GetType()).Name)
				{
					case "DataGrid":	if(((DataGrid)c).CaptionText.Length!=0)
											s.Add(((DataGrid)c).CaptionText);
						break;
					case "TabPage":		if(((Crownwood.Magic.Controls.TabPage)c).Title.Length!=0)
											s.Add(((Crownwood.Magic.Controls.TabPage)c).Title);
						break;
					case "ComboBox":		//Do not need translating
						break;
					case "TextBox":			//Do not need translating
						break;
					case "AccountTextBox":	//Do not need translating
						break;
					case "DateTimePicker":	//Do not need translating
						break;
					default:			if(c.Text.Length!=0)
											s.Add(c.Text);
						break;
				}
				if(c.Controls.Count>0)
					ListLabels(c.Controls, ref s);
			}
		}

		public void TranslateControls()
		{			
			TranslateControls(this.Controls);
		}

		/// <summary>
		/// Method to loop through the controls on a form and translate them into the 
		/// language represented by the current culture setting. Will be called by each 
		/// page constructor but can also be called at other times (i.e. when the 
		/// culture is changed
		/// </summary>
		public void TranslateControls(System.Windows.Forms.Control.ControlCollection controls)
		{
			foreach(Control c in controls)
			{
				
				switch((c.GetType()).Name)
				{
					case "DataGrid":	if(((DataGrid)c).CaptionText.Length!=0)
											((DataGrid)c).CaptionText = Translate(((DataGrid)c).CaptionText);
						break;
					case "TabPage":		if(((Crownwood.Magic.Controls.TabPage)c).Title.Length!=0)
											((Crownwood.Magic.Controls.TabPage)c).Title = Translate(((Crownwood.Magic.Controls.TabPage)c).Title);
						if(c.Controls.Count>0)
							TranslateControls(c.Controls);
						break;
					case "ComboBox":		//Do not need translating
						break;
					case "TextBox":			//Do not need translating
						break;
					case "AccountTextBox":	//Do not need translating
						break;
					case "DateTimePicker":	//Do not need translating
						break;
					default:			if(c.Text.Length!=0)
											c.Text = Translate(c.Text);
						if(c.Controls.Count>0)
							TranslateControls(c.Controls);
						break;
				}
			}
		}

		private string Translate(string text)
		{
			//look up the string in the hash table which corresponds to the current 
			//culture and return the result
			object translation = null;
			Hashtable dictionary  = (Hashtable)StaticData.Dictionaries[Config.Culture];
			if(dictionary!=null)
			{
				translation = dictionary[text];
				text = translation==null ? text:translation.ToString();
			}
			return text;
		}

		public string GetResource(string msgName, object[] parms)
		{
			//if this is an english culture use the string in the resource file
			//otherwise look up the string in the dictionary
			string complete = String.Format((string)Messages.List[msgName], parms);

			string trans = String.Format(Translate(msgName), parms);
			complete = trans==msgName?complete:trans;	

			string[] lines = complete.Split('\\');
			string msg="";
			foreach(string s in lines)
				msg+=s+"\n";
			msg = msg.Substring(0, msg.Length-1);
			return msg;
		}

		public string GetResource(string msgName)
		{
			string complete = (string)Messages.List[msgName];

			string trans = Translate(msgName);
			complete = trans==msgName?complete:trans;	

			string[] lines = complete.Split('\\');
			string msg="";
			foreach(string s in lines)
				msg+=s+"\n";
			msg = msg.Substring(0, msg.Length-1);
			return msg;
		}

		private CountryParameterCollection _country = null;

        //Don't list this in the properties of the control
        [Browsable(false)] 
        public CountryParameterCollection Country
		{
			get
			{
			    if(_country == null)
			    {
				    WStaticDataManager ws = new WStaticDataManager(true);
				    DataSet ds = ws.GetCountryMaintenanceParameters(Config.CountryCode, out Error);
				    if(Error.Length>0)
					    MessageBox.Show(Error);
				    else
				    {
					    _country = new CountryParameterCollection(ds.Tables[TN.CountryParameters]);
				    }
			    }
			    return _country;
			}
		}
	}
}
