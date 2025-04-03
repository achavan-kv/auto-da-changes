using System;
using System.Text.RegularExpressions;

namespace Blue.Cosacs.Warehouse
{
	public class PasswordValidator
	{
		public byte MinimunPasswordLenght
		{
			get;
			set;
		}

		public byte MinimunNonalfanumericChars
		{
			get;
			set;
		}

		public bool IsValid(string valueToChech)
		{
			var returnValue = false;
			var maxLenghtCheck = new Regex("{" + this.MinimunPasswordLenght.ToString() + ",255}");
			var minNonAlfanumericCheck = new Regex(@"\W|_");

			returnValue = maxLenghtCheck.IsMatch(valueToChech) && minNonAlfanumericCheck.Matches(valueToChech).Count >= this.MinimunNonalfanumericChars;

			return returnValue;
		}
	}
}
