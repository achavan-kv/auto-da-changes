using System;
using STL.DAL;
using STL.Common;
using System.Data;
using System.Security.Cryptography;
using System.Data.SqlClient;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
//using TRANSACTLib;
using System.Configuration;
using System.Diagnostics;

namespace STL.BLL
{
	/// <summary>
	/// Credit Sanction scoring using the Scorex Transact scoring engine.
	/// This class provides the operations to retrieve the data to construct
	/// the transact input buffer, calls Transact to perform the score and
	/// saves the returned results buffer back to the database.
	/// 
	/// The low level construction of the Transact buffers using field offsets
	/// within strings is encapsulated by the TransactBuffer class under the 
	/// 'Common' namespace.
	/// </summary>
	public class BTransact : CommonObject
	{

		//private BTransactBufferIn  _TransactBufferIn;   // Input buffer
		//private BTransactBufferOut _TransactBufferOut;  // Result buffer

		[MarshalAs(UnmanagedType.LPStruct)]public Object serverHandle;


		public BTransact()
		{
			/*
			serverHandle = new Object();
			string ipAddress = ConfigurationSettings.AppSettings["transactIP"];
			int socket = Convert.ToInt32(ConfigurationSettings.AppSettings["transactPort"]);
			int recordLength = 5000;

			TRANSACTLib.TransObjectClass transact = new TransObjectClass();

			logMessage("ServerHandle: "+ serverHandle.ToString(), "99999", EventLogEntryType.Information);

			transact.Initialize();
			transact.AddServer(out serverHandle);


			logMessage("ServerHandle: "+ serverHandle.ToString(), "99999", EventLogEntryType.Information);
			logMessage("IPAddress: "+ ipAddress, "99999", EventLogEntryType.Information);
			logMessage("Socket: "+ socket.ToString(), "99999", EventLogEntryType.Information);

			transact.SetRecordLength((uint)serverHandle, recordLength);
			transact.SetServer((uint)serverHandle, ipAddress, socket);			

			transact.RemoveServer((uint)serverHandle);
			transact.DeInitialize();
			*/
			
			
		}

		/*

		public void SubmitToScore()
		{
			// Connect to the Transact Server and submit buffer for scoring
			// using private _TransactBufferIn ....
			
		}

		public void SaveResultBuffer(string AcctNo,
									 int	CurNumAcc,
									 char	CurHiEver,
									 int	SetNumAcc,
									 char	SetHiEver,
									 string	ProdCat,
									 string	ProdCode)
		{
			// Save to DB the results buffer and SProc calculations as separate fields
			// using private _TransactBufferOut ....

			// DUMMY: Save ACCEPT OR DECLINE without scoring
			byte[] random = new Byte[2];

			// Random number generator
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(random); 
			if (random[1] > random[2])
				this._TransactBufferOut.SystemDecision = "A";
			else
				this._TransactBufferOut.SystemDecision = "D";

		}

		public void SaveScorexExtract(string AcctNo)
		{
			// Save a copy of the input and result buffers for Scorex Extract
			// using private _TransactBufferIn and _TransactBufferOut ....
		}

		public void GetBureauData(string AcctNo, string FileName)
		{
			// Connect to Transact Server and retrieve Raw Bureau Data
			// ....
		}

		public void ScoreApplication(string AcctNo, char AppStatus)
		{
			// Values returned from SProc to be saved on DB
			int		CurNumAcc = 0;
			char	CurHiEver = ' ';
			int		SetNumAcc = 0;
			char	SetHiEver = ' ';
			string	ProdCat = " ";
			string	ProdCode = " ";

			// Retrieve the data to construct the Transact input buffer
			// using private _TransactBufferIn ....

			// Call sp_TransactData for calculated values
			// (This Stored Procedure currently contains the business logic
			// written as SQL statements.)
			// ...

			// Submit the input buffer for scoring
			this.SubmitToScore();

			// Save the result buffer and SProc calculated values
			this.SaveResultBuffer(AcctNo,
								  CurNumAcc,
							      CurHiEver,
								  SetNumAcc,
								  SetHiEver,
								  ProdCat,
								  ProdCode);
		
			// Save a copy of the buffers for Scorex Extract
			this.SaveScorexExtract(AcctNo);
		}
		*/

	}
}
