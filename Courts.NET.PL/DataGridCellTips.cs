using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Diagnostics;

namespace STL.PL
{
	/// <summary>
	/// Allows a DataGrid column to be specified as the text for a ToolTip.
	/// As the mouse is moved over the DataGrid the text from the specified 
	/// column on the row pointed to by the mouse will appear as a ToolTip.
	/// </summary>
	public class DataGridCellTips : DataGrid  
	{  
		private int hitRow;  
		private int displayCol=0;
		private int delay=300;

		private System.Windows.Forms.ToolTip toolTip1; 

		[Browsable(true)]
		public int ToolTipColumn
		{
			get{return displayCol;}
			set{displayCol = value;}
		}
		[Browsable(true)]
		public int ToolTipDelay
		{
			get{return delay;}
			set{delay = value;}
		}
 
		public DataGridCellTips()  
		{  
			hitRow = -1;  
			this.toolTip1 = new System.Windows.Forms.ToolTip();  
			this.toolTip1.InitialDelay = delay;  
			this.MouseMove += new MouseEventHandler(HandleMouseMove);  
		} 
 
        [DebuggerHidden]
		private void HandleMouseMove(object sender, MouseEventArgs e)  
		{  
			DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));  
			if(hti.Type == DataGrid.HitTestType.Cell    
				&& ( hti.Row != hitRow ))
			{     //new hit row 
 
				hitRow = hti.Row;   
				if(this.toolTip1 != null && this.toolTip1.Active)  
					this.toolTip1.Active = false; //turn it off 

				if(this[hitRow, displayCol].ToString().Length!=0)
				{ 
					this.toolTip1.SetToolTip(this, this[hitRow, displayCol].ToString());  
					this.toolTip1.Active = true; //make it active so it can show itself 
				}
			}  
		} 
	} 
}
