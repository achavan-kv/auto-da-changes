using System;
using System.Drawing;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Using this class we can set the colour of individual cell in a datagrid
	/// which is otherwise not possible. 
	/// </summary>
	public class ColourCellEventArgs : EventArgs
	{
		private int _row;
		private int _col;
		private bool _colour;

		public ColourCellEventArgs(int row, int col, bool val)
		{
			_row = row;
			_col = col;
			_colour = val;
		}
		public int Col
		{
			get{ return _col;}
			set{ _col = value;}
		}
		public int Row
		{
			get{ return _row;}
			set{ _row = value;}
		}
		public bool Colour
		{
			get{ return _colour;}
			set{ _colour = value;}
		}
	}

	public delegate void ColourCellEventHandler(object sender, ColourCellEventArgs e);

	public class ColouredTextColumn : DataGridTextBoxColumn
	{
		public event ColourCellEventHandler CheckCellColour;

		public ColouredTextColumn(int column)
		{
			_col = column;
		}

		private int _col = 0;
		private int _acctCol = -1;
		private Brush _backBrush = Brushes.RosyBrown;
		private Brush _foreBrush = Brushes.WhiteSmoke;
		/// <summary>
		/// Gets or sets the brush used to paint the cell background
		/// </summary>
		public Brush BackColour
		{
			get{return _backBrush;}
			set{_backBrush = value;}
		}
		public Brush ForeColour
		{
			get{return _foreBrush;}
			set{_foreBrush = value;}
		}
		/// <summary>
		/// Sets the (zero-based) column number which should be formatted as an account number
		/// </summary>
		public int AccountColumn
		{
			get{return _acctCol;}
			set{_acctCol = value;}
		}

		/// <summary>
		/// This method will format a columns text box so that it appears
		/// as an account number
		/// </summary>
		/// <param name="cm"></param>
		/// <param name="rowNum"></param>
		/// <returns></returns>
		protected override object GetColumnValueAtRow(CurrencyManager cm, int rowNum)
		{
			//CommonForm c = new CommonForm();
			object val = base.GetColumnValueAtRow(cm, rowNum);

			if((_acctCol!=-1)&&(_col==_acctCol)&&(val.GetType()!=Convert.DBNull))
			{
				string acctNo = (string)val;
				CommonForm.FormatAccountNo(ref acctNo);
				return acctNo;
			}
			else
				return val;
		}

		//override the paint method so that it fires our custom event to 
		//check whether to change the background 
		protected override void Paint(System.Drawing.Graphics g, System.Drawing.Rectangle bounds, System.Windows.Forms.CurrencyManager source, int rowNum, System.Drawing.Brush backBrush, System.Drawing.Brush foreBrush, bool alignToRight)
		{
			bool colour = false;

			if(CheckCellColour != null)
			{
				ColourCellEventArgs e = new ColourCellEventArgs(rowNum, _col, colour);
				CheckCellColour(this, e);
				if(e.Colour)
				{
					backBrush = _backBrush;
					foreBrush = _foreBrush;
				}
			}			

			base.Paint(g, bounds, source, rowNum, backBrush, foreBrush, alignToRight);
		}
	}
}
