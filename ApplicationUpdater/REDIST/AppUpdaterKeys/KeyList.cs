using System;

namespace Microsoft.Samples.AppUpdater
{
	/// <summary>
	/// Summary description for AppUpdater_Keys
	/// </summary>
	public class KeyList
	{
		public static byte[][] Keys;
		public static string[] ExceptionList;

		static KeyList()
		{
			//Add the list of public keys your app uses here
			Keys = new byte[][] {
				new byte[] { 0, 36, 0, 0, 4, 128, 0, 0, 148, 0, 0, 0, 6, 2, 0, 0, 0, 36, 0, 0, 82, 83, 65, 49, 0, 4, 0, 0, 1, 0, 1, 0, 95, 81, 246, 90, 218, 186, 162, 97, 166, 49, 31, 81, 219, 192, 9, 180, 14, 174, 24, 158, 138, 225, 14, 38, 226, 192, 31, 171, 74, 47, 210, 255, 104, 31, 90, 175, 172, 246, 149, 141, 132, 248, 25, 166, 64, 102, 240, 89, 239, 22, 97, 54, 233, 217, 7, 155, 23, 87, 172, 111, 39, 104, 48, 97, 200, 50, 155, 32, 37, 42, 212, 167, 201, 220, 50, 119, 84, 201, 191, 19, 164, 227, 94, 9, 44, 79, 115, 18, 25, 236, 73, 169, 16, 14, 84, 241, 175, 110, 112, 223, 214, 81, 111, 220, 222, 16, 224, 208, 204, 65, 108, 207, 171, 88, 52, 149, 212, 147, 62, 9, 112, 118, 105, 25, 24, 161, 235, 213 }
			};

			//Add the list of files that don't need to be signed, but are allowed to be downloaded even if not signed.
			//PDB files would be a good example of this type of file.
			ExceptionList = new string[] {"simpleform.dll", "simpleform.pdb"};
		}
	}
}
