using System;
using STL.Common;

namespace STL.BLL
{
	/// <summary>
	/// This class will handle all access to the transact OUTPUT buffer.
	/// Access will be controlled using properties so that the 
	/// buffer is encapsulated and presents as a normal object with 
	/// member variables.
	/// </summary>
	public class BTransactBufferOut : StringBuffer
	{

		public BTransactBufferOut()
		{
			ClearBuffer(1500);
		}

		#region output buffer properties
		//to do: add properties for all the output buffer variables

		public int BehaviouralScore1
		{
			get	{return ExtractInt32(Offset.BehaviouralScore1, Length.BehaviouralScore1);	}
			set	{Append(Offset.BehaviouralScore1, Length.BehaviouralScore1, value);	}
		}

		public int BehaviouralScore2
		{
			get	{return ExtractInt32(Offset.BehaviouralScore2, Length.BehaviouralScore2);	}
			set	{Append(Offset.BehaviouralScore2, Length.BehaviouralScore2, value);	}
		}

		public int BehaviouralScore3
		{
			get	{return ExtractInt32(Offset.BehaviouralScore3, Length.BehaviouralScore3);	}
			set	{Append(Offset.BehaviouralScore3, Length.BehaviouralScore3, value);	}
		}

		public int A2BehaviouralScore1
		{
			get	{return ExtractInt32(Offset.A2BehaviouralScore1, Length.A2BehaviouralScore1);	}
			set	{Append(Offset.A2BehaviouralScore1, Length.A2BehaviouralScore1, value);	}
		}

		public int A2BehaviouralScore2
		{
			get	{return ExtractInt32(Offset.A2BehaviouralScore2, Length.A2BehaviouralScore2);	}
			set	{Append(Offset.A2BehaviouralScore2, Length.A2BehaviouralScore2, value);	}
		}

		public int A2BehaviouralScore3
		{
			get	{return ExtractInt32(Offset.A2BehaviouralScore3, Length.A2BehaviouralScore3);	}
			set	{Append(Offset.A2BehaviouralScore3, Length.A2BehaviouralScore3, value);	}
		}


		public string SystemDecision
		{
			get	{return ExtractString(Offset.SystemDecision, Length.SystemDecision);	}
			set	{Append(Offset.SystemDecision, Length.SystemDecision, value);	}
		}

		#endregion

		/// <summary>
		/// This structure contains all the offset definitions
		/// </summary>
		private struct Offset
		{
			#region Transact output buffer
			public const int AccountNumber = 9;
			public const int PreBureauScore = 41;
			public const int Score1 = 44;
			public const int Score1LowerCutoff = 47;
			public const int Score1UpperCutoff = 50;
			public const int Score2 = 53;
			public const int Score2LowerCutoff = 56;
			public const int Score2UpperCutoff = 59;
			public const int Score3 = 62;
			public const int Score3LowerCutoff = 65;
			public const int Score3UpperCutoff = 68;
			public const int Risk = 71;
			public const int Risk2 = 72;
			public const int Risk3 = 73;
			public const int SystemDecision = 74;
			public const int PolicyRule1 = 75;
			public const int PolicyRule2 = 78;
			public const int PolicyRule3 = 81;
			public const int PolicyRule4 = 84;
			public const int PolicyRule5 = 87;
			public const int PolicyRule6 = 90;
			public const int PolicyRule7 = 93;
			public const int PolicyRule8 = 96;
			public const int PolicyRule9 = 99;
			public const int PolicyRule10 = 102;
			public const int OverrideReason1 = 105;
			public const int OverrideReason2 = 108;
			public const int OverrideReason3 = 111;
			public const int OverrideReason4 = 114;
			public const int OverrideReason5 = 117;
			public const int PrimaryScorecard = 120;
			public const int SecondaryScorecard = 121;
			public const int TertiaryScorecard = 122;
			public const int OrigDecisionDate = 123;
			public const int UpdateDecisionDate = 131;
			public const int ParamDate = 139;
			public const int RecCreditLimit1 = 187;
			public const int RecCreditLimit2 = 201;
			public const int RecCreditLimit3 = 215;
			public const int RecMonthlyInstal1 = 229;
			public const int RecMonthlyInstal2 = 243;
			public const int RecMonthlyInstal3 = 257;
			public const int RecTerm1 = 271;
			public const int RecTerm2 = 273;
			public const int RecTerm3 = 275;
			public const int BehaviouralScore1 = 277;
			public const int BehaviouralScore2 = 280;
			public const int BehaviouralScore3 = 283;
			public const int A2BehaviouralScore1 = 286;
			public const int A2BehaviouralScore2 = 289;
			public const int A2BehaviouralScore3 = 292;
			public const int Error = 401;
			public const int Summary = 402; 
		#endregion
		}

		/// <summary>
		/// This structure contains all the field Lengths
		/// </summary>
		private struct Length
		{
			#region Transact output buffer
			public const int AccountNumber = 12;
			public const int PreBureauScore = 3;
			public const int Score1 = 3;
			public const int Score1LowerCutoff = 3;
			public const int Score1UpperCutoff = 3;
			public const int Score2 = 3;
			public const int Score2LowerCutoff = 3;
			public const int Score2UpperCutoff = 3;
			public const int Score3 = 3;
			public const int Score3LowerCutoff = 3;
			public const int Score3UpperCutoff = 3;
			public const int Risk = 1;
			public const int Risk2 = 1;
			public const int Risk3 = 1;
			public const int SystemDecision = 1;
			public const int PolicyRule1 = 3;
			public const int PolicyRule2 = 3;
			public const int PolicyRule3 = 3;
			public const int PolicyRule4 = 3;
			public const int PolicyRule5 = 3;
			public const int PolicyRule6 = 3;
			public const int PolicyRule7 = 3;
			public const int PolicyRule8 = 3;
			public const int PolicyRule9 = 3;
			public const int PolicyRule10 = 3;
			public const int OverrideReason1 = 3;
			public const int OverrideReason2 = 3;
			public const int OverrideReason3 = 3;
			public const int OverrideReason4 = 3;
			public const int OverrideReason5 = 3;
			public const int PrimaryScorecard = 1;
			public const int SecondaryScorecard = 1;
			public const int TertiaryScorecard = 1;
			public const int OrigDecisionDate = 8;
			public const int UpdateDecisionDate = 8;
			public const int ParamDate = 8;
			public const int RecCreditLimit1 = 14;
			public const int RecCreditLimit2 = 14;
			public const int RecCreditLimit3 = 14;
			public const int RecMonthlyInstal1 = 14;
			public const int RecMonthlyInstal2 = 14;
			public const int RecMonthlyInstal3 = 14;
			public const int RecTerm1 = 2;
			public const int RecTerm2 = 2;
			public const int RecTerm3 = 2;
			public const int BehaviouralScore1 = 3;
			public const int BehaviouralScore2 = 3;
			public const int BehaviouralScore3 = 3;
			public const int A2BehaviouralScore1 = 3;
			public const int A2BehaviouralScore2 = 3;
			public const int A2BehaviouralScore3 = 3;
			public const int Error = 1;
			public const int Summary = 1099; 
		#endregion
		}

	}
}
