using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace STL.PL
{
	/// <summary>
	/// Using this class we can set the colour of individual cell in a datagrid
	/// which is otherwise not possible. 
	/// </summary>
	public class AccountCellEventArgs : EventArgs
	{
		private int _row;
		private int _col;

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
	}

	/// <summary>
	/// Text box column in a DataGrid customised to use the 000-0000-0000-0
	/// template for account numbers listed in a DataGrid column.
	/// </summary>
	public class AccountTextColumn : DataGridTextBoxColumn
	{
		public AccountTextColumn(int column)
		{
			_col = column;
		}

		private int _col = 0;
		private int _acctCol = -1;

		/// <summary>
		/// Sets the (zero-based) column number which should be formatted as an account number
		/// </summary>
		public int AccountColumn
		{
			get{return _acctCol;}
			set{_acctCol = value;}
		}
		protected override void Paint(Graphics g, Rectangle bounds, CurrencyManager source,
		int rowNum, Brush backBrush, Brush foreBrush, bool alignToRight)
		{
			var idCellValue = ((DataRowView)source.List[rowNum])["IsMambuAccount"];
			var brush = backBrush;
			if (idCellValue != DBNull.Value && ((bool)idCellValue) == true)
				brush = Brushes.Yellow;
			base.Paint(g, bounds, source, rowNum, brush, foreBrush, alignToRight);
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

	}
}
