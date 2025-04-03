using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DProposalResult.
	/// </summary>
	public class DProposalResult: DALObject
	{
		private object _adcomment;
		private string _adreqd;
		private string _decision;
		private string _finaldec;
		private string _manualrefer;
		private string _override;
		private string _policyrule1;
		private string _policyrule2;
		private string _policyrule3;
		private string _policyrule4;
		private string _policyrule5;
		private string _policyrule6;
		private string _riskcat;
		private int _score;
		private string _sysrecommend;
		private object _uwcomment;
		private object _warning;
		private int _curnumber;
		private string _curworst;
		private int _setnumber;
		private string _setworst;
		private string _prodcat;
		private string _prodcode;
		private object _summarydata;

		public int GetProposalResult(string accountNumber)
		{
			try
			{
				parmArray = new SqlParameter[25];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNumber;
				parmArray[1] = new SqlParameter("@adcomment", SqlDbType.NVarChar,200);
				parmArray[1].Value = "";
				parmArray[2] = new SqlParameter("@adreqd", SqlDbType.NVarChar,1);
				parmArray[2].Value = "";
				parmArray[3] = new SqlParameter("@decision", SqlDbType.NVarChar,4);
				parmArray[3].Value = "";
				parmArray[4] = new SqlParameter("@finaldec", SqlDbType.NVarChar,4);
				parmArray[4].Value = "";
				parmArray[5] = new SqlParameter("@manualrefer", SqlDbType.NVarChar,1);
				parmArray[5].Value = "";
				parmArray[6] = new SqlParameter("@override", SqlDbType.NVarChar,4);
				parmArray[6].Value = "";
				parmArray[7] = new SqlParameter("@policyrule1", SqlDbType.NVarChar,4);
				parmArray[7].Value = "";
				parmArray[8] = new SqlParameter("@policyrule2", SqlDbType.NVarChar,4);
				parmArray[8].Value = "";
				parmArray[9] = new SqlParameter("@policyrule3", SqlDbType.NVarChar,4);
				parmArray[9].Value = "";
				parmArray[10] = new SqlParameter("@policyrule4", SqlDbType.NVarChar,4);
				parmArray[10].Value = "";
				parmArray[11] = new SqlParameter("@policyrule5", SqlDbType.NVarChar,4);
				parmArray[11].Value = "";
				parmArray[12] = new SqlParameter("@policyrule6", SqlDbType.NVarChar,4);
				parmArray[12].Value = "";
				parmArray[13] = new SqlParameter("@riskcat", SqlDbType.NVarChar,1);
				parmArray[13].Value = "";
				parmArray[14] = new SqlParameter("@score", SqlDbType.Int);
				parmArray[14].Value = 0;
				parmArray[15] = new SqlParameter("@sysrecommend", SqlDbType.NVarChar,1);
				parmArray[15].Value = "";
				parmArray[16] = new SqlParameter("@uwcomment", SqlDbType.NVarChar,200);
				parmArray[16].Value = "";
				parmArray[17] = new SqlParameter("@warning", SqlDbType.NVarChar,50);
				parmArray[17].Value = "";
				parmArray[18] = new SqlParameter("@curnumber", SqlDbType.Int);
				parmArray[18].Value = 0;
				parmArray[19] = new SqlParameter("@curworst", SqlDbType.NVarChar,1);
				parmArray[19].Value = "";
				parmArray[20] = new SqlParameter("@setnumber", SqlDbType.Int);
				parmArray[20].Value = 0;
				parmArray[21] = new SqlParameter("@setworst", SqlDbType.NVarChar,1);
				parmArray[21].Value = "";
				parmArray[22] = new SqlParameter("@prodcat", SqlDbType.NVarChar,3);
				parmArray[22].Value = "";
				parmArray[23] = new SqlParameter("@prodcode", SqlDbType.NVarChar,8);
				parmArray[23].Value = "";
				parmArray[24] = new SqlParameter("@summarydata", SqlDbType.NVarChar,999);
				parmArray[24].Value = "";

				foreach(SqlParameter parm in parmArray)
				{
					parm.Direction = ParameterDirection.Output;
				}
				parmArray[0].Direction = ParameterDirection.Input;			

				result = this.RunSP("DN_PropResultGetSP", parmArray);

				_adcomment = parmArray[1].Value;
				_adreqd = (string)parmArray[2].Value;
				_decision = (string)parmArray[3].Value;
				_finaldec = (string)parmArray[4].Value;
				_manualrefer = (string)parmArray[5].Value;
				_override = (string)parmArray[6].Value;
				_policyrule1 = (string)parmArray[7].Value;
				_policyrule2 = (string)parmArray[8].Value;
				_policyrule3 = (string)parmArray[9].Value;
				_policyrule4 = (string)parmArray[10].Value;
				_policyrule5 = (string)parmArray[11].Value;
				_policyrule6 = (string)parmArray[12].Value;
				_riskcat = (string)parmArray[13].Value;
				_score = (int)parmArray[14].Value;
				_sysrecommend = (string)parmArray[15].Value;
				_uwcomment = parmArray[16].Value;
				_warning = parmArray[17].Value;
				_curnumber = (int)parmArray[18].Value;
				_curworst = (string)parmArray[19].Value;
				_setnumber = (int)parmArray[20].Value;
				_setworst = (string)parmArray[21].Value;
				_prodcat = (string)parmArray[22].Value;
				_prodcode = (string)parmArray[23].Value;
				_summarydata = parmArray[24].Value;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public int GetProposalRefDetails(string accountNumber)
		{
			try
			{
				_proposalref = new DataTable(TN.ProposalRef);			
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,CW.AccountNo);
				parmArray[0].Value = accountNumber;

				result = this.RunSP("DN_PropResultGetDataSP", parmArray, _proposalref);
			
				if(result==0)
					result = (int)Return.Success;
				else
					result = (int)Return.Fail;
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}

		public void Save(SqlConnection conn, SqlTransaction trans, string accountNo)
		{
			//HACK this method currently only inserts a dummy row into propresult and
			//will need to be redone eventually
			try
			{
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = accountNo;						

				this.RunSP(conn, trans, "DN_PropResultSaveSP", parmArray);

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}

		}

		public DProposalResult()
		{
			
		}

		public object AdditionalDataComment
		{
			get
			{
				return _adcomment;
			}
			set
			{
				_adcomment = value;
			}
		}

		public string AdditionalDataRequired
		{
			get
			{
				return _adreqd;;
			}
			set
			{
				_adreqd = value;
			}
		}

		public string Decision
		{
			get
			{
				return _decision;
			}
			set
			{
				_decision = value;
			}
		}

		public string FinalDecision
		{
			get
			{
				return _finaldec;
			}
			set
			{
				_finaldec = value;
			}
		}

		public string ManualRefer
		{
			get
			{
				return _manualrefer;
			}
			set
			{
				_manualrefer = value;
			}
		}

		public string Override
		{
			get
			{
				return _override;
			}
			set
			{
				_override = value;
			}
		}

		public string PolicayRule1
		{
			get
			{
				return _policyrule1;
			}
			set
			{
				_policyrule1 = value;
			}
		}

		public string PolicyRule2
		{
			get
			{
				return _policyrule2;
			}
			set
			{
				_policyrule2 = value;
			}
		}

		public string PolicyRule3
		{
			get
			{
				return _policyrule3;
			}
			set
			{
				_policyrule3 = value;
			}
		}

		public string PolicyRule4
		{
			get
			{
				return _policyrule4;
			}
			set
			{
				_policyrule4 = value;
			}
		}

		public string PolicyRule5
		{
			get
			{
				return _policyrule5;
			}
			set
			{
				_policyrule5 = value;
			}
		}

		public string PolicyRule6
		{
			get
			{
				return _policyrule6;
			}
			set
			{
				_policyrule6 = value;
			}
		}

		public string RiskCategory
		{
			get
			{
				return _riskcat;
			}
			set
			{
				_riskcat = value;
			}
		}

		public int Score
		{
			get
			{
				return _score;
			}
			set
			{
				_score = value;
			}
		}

		public string SysRecommend
		{
			get
			{
				return _sysrecommend;
			}
			set
			{
				_sysrecommend = value;
			}
		}

		public object UWComment
		{
			get
			{
				return _uwcomment;
			}
			set
			{
				_uwcomment = value;
			}
		}

		public object Warning
		{
			get
			{
				return _warning;
			}
			set
			{
				_warning = value;
			}
		}

		public int CurNumber
		{
			get
			{
				return _curnumber;
			}
			set
			{
				_curnumber = value;
			}
		}

		public string CurWorst
		{
			get
			{
				return _curworst;
			}
			set
			{
				_curworst = value;
			}
		}

		public int SetNumber
		{
			get
			{
				return _setnumber;
			}
			set
			{
				_setnumber = value;
			}
		}

		public string SetWorst
		{
			get
			{
				return _setworst;
			}
			set
			{
				_setworst = value;
			}
		}

		public string ProductCategory
		{
			get
			{
				return _prodcat;
			}
			set
			{
				_prodcat = value;
			}
		}

		public string ProductCode
		{
			get
			{
				return _prodcode;
			}
			set
			{
				_prodcode = value;
			}
		}

		public object SummaryData
		{
			get
			{
				return _summarydata;
			}
			set
			{
				_summarydata = value;
			}
		}

		private DataTable _proposalref = null;
		public DataTable ProposalRef
		{
			get{return _proposalref;}
		}

	}
}
