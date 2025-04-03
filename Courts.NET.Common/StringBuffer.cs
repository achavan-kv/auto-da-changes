using System;
using System.Text;

namespace STL.Common
{
	/// <summary>
	/// This class will handles access to a generic string buffer to
	/// contain fixed width fields at fixed offsets.
	/// Access will be controlled using properties so that the buffer
	/// is encapsulated and presents a normal object with member variables.
	/// </summary>
	public class StringBuffer
	{
		protected StringBuilder _buffer;

		public StringBuffer()
		{
		}

		public StringBuffer(int BufferLength)
		{
			ClearBuffer(BufferLength); 
		}

		public void ClearBuffer(int BufferLength)
		{
			// To be called from derived class constructors
			// (Constructors cannot be inherited)
			_buffer = new StringBuilder(BufferLength, BufferLength);
			for(int i=0; i<BufferLength; i++)
				_buffer.Append(" ");
		}

		protected void Append(int offset, int length, string data)
		{
			data = data.Substring(0, length);
			_buffer.Remove(offset, length);
			_buffer.Insert(offset, data);
		}

		protected void Append(int offset, int length, int data)
		{
			string dataStr = data.ToString();
			dataStr = dataStr.PadLeft(length, '0');
			dataStr = dataStr.Substring(0, length);
			_buffer.Remove(offset, length);
			_buffer.Insert(offset, dataStr);
		}

		protected void Append(int offset, int length, DateTime data)
		{
			string dataStr = data.ToString("ddMMyyyy");
			dataStr = dataStr.Substring(0, length);
			_buffer.Remove(offset, length);
			_buffer.Insert(offset, dataStr);
		}
	
		protected void Append(int offset, int length, decimal data)
		{
			data *= 100;
			string dataStr = data.ToString("F0");
			dataStr = dataStr.PadLeft(length, ' ');
			dataStr = dataStr.Substring(0, length);
			_buffer.Remove(offset, length);
			_buffer.Insert(offset, dataStr);
		}

		protected void AppendW(int offset, int length, decimal data)
		{
			string dataStr = data.ToString("F1");
			dataStr = dataStr.PadLeft(length, ' ');
			dataStr = dataStr.Substring(0, length);
			_buffer.Remove(offset, length);
			_buffer.Insert(offset, dataStr);
		}

		protected string ExtractString(int offset, int length)
		{
			return _buffer.ToString(offset, length);
		}

		protected int ExtractInt32(int offset, int length)
		{
			return Convert.ToInt32(_buffer.ToString(offset, length));
		}

		protected DateTime ExtractDate(int offset, int length)
		{
			string dataStr = _buffer.ToString(offset, length);
			int dd = Convert.ToInt32(dataStr.Substring(0, 2));
			int mm = Convert.ToInt32(dataStr.Substring(2, 2)); 
			int yy = Convert.ToInt32(dataStr.Substring(4, 4));
			return new DateTime(yy, mm, dd);
		}

		protected decimal ExtractDecimal(int offset, int length)
		{
			string dataStr = _buffer.ToString(offset, length);
			return (Convert.ToDecimal(dataStr)/100);
		}

		protected decimal ExtractWDecimal(int offset, int length)
		{
			string dataStr = _buffer.ToString(offset, length);
			return Convert.ToDecimal(dataStr);
		}

	}
}
