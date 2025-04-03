using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace STL.PL
{
	/// <summary>
	///  DataGridEditColumn class to override Edit and Paint events on a
	///  DataGrid cell so that the cell is read only and displayed in a 
	///  different colour depending on a value in another column
	/// </summary>
	public class DataGridEditColumn : DataGridTextBoxColumn
	{
		private string _colName  = "";
		private string _colValue = "";
		
		public DataGridEditColumn(string colName, string colValue)
		{
			_colName = colName;
			_colValue = colValue;
		}

		protected override void Edit(
			System.Windows.Forms.CurrencyManager source,
			int rowNum,
			System.Drawing.Rectangle bounds,
			bool readOnly,
			string instantText,
			bool cellIsVisible) 
		{ 
			// Prevent an individual cell from being edited
			if (((DataRowView)source.Current).DataView[rowNum][_colName].ToString() == _colValue)
				return; 
 
			base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible); 
		} 

		protected override void Paint(
			System.Drawing.Graphics g,
			System.Drawing.Rectangle bounds,
			System.Windows.Forms.CurrencyManager source,
			int rowNum,
			System.Drawing.Brush backBrush,
			System.Drawing.Brush foreBrush,
			bool alignToRight)
		{
			try
			{
				// Change cell colour when the cell cannot be edited
				if (((DataRowView)source.Current).DataView[rowNum][_colName].ToString() == _colValue)
				{
					//backBrush = new SolidBrush(Color.LightGray);
					foreBrush = new SolidBrush(Color.Gray);
				}
				else
				{
					backBrush = new SolidBrush(Color.AntiqueWhite);  
					foreBrush = new SolidBrush(Color.Black); 
				}

			}
			catch{ /* empty catch */ }
			finally
			{ 
				// make sure the base class gets called to do the drawing with 
				// the possibly changed brushes 
				base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight); 
			} 
		}
	}
}
