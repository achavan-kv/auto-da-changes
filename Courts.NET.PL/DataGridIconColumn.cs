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
	///  DataGridIconColumn class to override Edit and Paint events on a
	///  DataGrid cell so that the cell is either blank or displays an 
	///  icon depending on a value in another column.
	///  The constructor for this class is overloaded to display two
	///  different icons (eg a tick or a cross) depending on two different
	///  values in another column.
	/// </summary>
	public class DataGridIconColumn : DataGridTextBoxColumn
	{
		private Image _image1;
		private Image _image2;
		private string _colName="";
		private string _colValue1="";
		private string _colValue2="";
		
		public DataGridIconColumn(Image image1, string colName, string colValue1)
		{
			_image1 = image1;
			_colName = colName;
			_colValue1 = colValue1;
		}

		public DataGridIconColumn(Image image1, Image image2, string colName, string colValue1, string colValue2)
		{
			_image1 = image1;
			_image2 = image2;
			_colName = colName;
			_colValue1 = colValue1;
			_colValue2 = colValue2;
		}


		protected override void Edit(System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Rectangle bounds, bool readOnly, string instantText, bool cellIsVisible) 
		{ 
			//do not allow the unbound cell to become active
			if(this.MappingName == "Icon")
				return; 
 
			base.Edit(source, rowNum, bounds, readOnly, instantText, cellIsVisible); 
		} 

		protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Brush backBrush, System.Drawing.Brush foreBrush, bool alignToRight)
		{
			try
			{
				//erase background
				g.FillRectangle(backBrush, bounds);

				if (_colValue2 == "")
				{
					if ((((DataRowView)source.Current).DataView[rowNum][_colName]).ToString() != _colValue1)
						g.DrawImage(this._image1, bounds);
				}
				else
				{
					if ((((DataRowView)source.Current).DataView[rowNum][_colName]).ToString() == _colValue1)
						g.DrawImage(this._image1, bounds);
					else if ((((DataRowView)source.Current).DataView[rowNum][_colName]).ToString() == _colValue2)
						g.DrawImage(this._image2, bounds);
				}

			}
			catch{ /* empty catch */ }
		}
	}
}
