using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using STL.Common.Constants.ColumnNames;


namespace STL.PL
{
	/// <summary>
	///  DataGridMultipleEditColumn class to override the Edit event on a
	///  DataGrid cell so that the cell is read only when a corresponding
	///  bit is set in a masking column. This allows a single 32 bit masking
	///  column to control the read only property of up to 32 columns of
	///  this type in a DataGrid.
	/// </summary>
	public class DataGridMultipleEditColumn : DataGridTextBoxColumn
	{
		private string _colName  = "";
		//private string _colValue = "";
		private int _colNumber = 0;
		private double _binary = 2;
		
		public DataGridMultipleEditColumn(string colName, int colNumber)
		{
			_colName = colName;		/* name of the column containing the mask */
			_colNumber = colNumber; /* ordinal position of the column to set as read only */
		}

		//public DataGridMultipleEditColumn(string colName, string colValue)
		//{
		//	_colName = colName;
		//	_colValue = colValue;
		//}

		protected override void Edit(
			System.Windows.Forms.CurrencyManager source,
			int rowNum,
			System.Drawing.Rectangle bounds,
			bool readOnly,
			string instantText,
			bool cellIsVisible) 
		{ 
			/* find out if colNumber is set to be read only
			 * by checking the bits in colName */
			int mask = (int)((DataRowView)source.Current).DataView[rowNum][_colName];
			double power = Convert.ToDouble(_colNumber-1);
			int x = Convert.ToInt32(Math.Pow(_binary, power));
			if( (mask & x) == x )
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
				/*
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
				*/
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
