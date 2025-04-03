using System;
using STL.Common;
using System.Collections.Specialized;

namespace STL.Batch
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class MainClass
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main(string[] args)
		{
			try
			{
//#if (DEBUG)
	//"Default","99999","V"
                // IF YOU NEED THIS LINE FOR TESTING THEN REMOVE THE READ ONLY PROPERTY
                // FROM YOUR LOCAL FILE INSTEAD OF CHECKING OUT - SO WE DON'T CHECK IT BACK IN.
                //if (args.Length != 3)
                //args = new string[] { "Default", "99999", "A" };      
//#endif
				if (args.Length != 3)
				{
					string[] parameter = new string[1] {"configuration user country"};
					Console.WriteLine("Please provide parameters in the following format:");
					foreach (string s in parameter)
					{
						Console.WriteLine(s);
					}
				}
				else
				{
					EODConfiguration EODRun = new EODConfiguration(args[0], Convert.ToInt32(args[1]), args[2]);
				}
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}
		}
	}
}

