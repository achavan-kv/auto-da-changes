using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
	/// <summary>
	/// Summary description for DScoring.
	/// </summary>
	public class DScoring : DALObject
	{
		DataSet operands = null;
		private DataTable _matrix = null;
		private DataTable _limits = null;
		public DataTable Matrix
		{
			get{return _matrix;}
		}
		private string _country = "";
		public new string Country
		{
			get{return _country;}
			set{_country = value;}
		}
		private string _region = "";
		public string Region
		{
			get{return _region;}
			set{_region = value;}
		}

		private string _rules = "";
		public string RulesXML
		{
			get{return _rules;}
			set{_rules = value;}
		}
		private DataTable _rulesTab = null;
		public DataTable RulesTable
		{
			get{return _rulesTab;}
			set{_rulesTab = value;}
		}

		private string _acctno = "";
		public string AccountNumber
		{
			get{return _acctno;}
			set{_acctno = value;}
		}
		public DataTable Limits
		{
			get
			{
				return _limits;
			}
		}
		private decimal _creditlimit = 0;
		public decimal CreditLimit
		{
			get{return _creditlimit;}
			set{_creditlimit = value;}
		}

		private string _custid = "";
		public string CustomerID
		{
			get{return _custid;}
			set{_custid = value;}
		}

		private DateTime _dateprop;
		public DateTime DateProp
		{
			get{return _dateprop;}
			set{_dateprop = value;}
		}
		private decimal _score = 0;
		public decimal Score
		{
			get{return _score;}
			set{_score = value;}
		}
		private DateTime _dateImported = DateTime.Now;
		public DateTime DateImported
		{
			get{return _dateImported;}
			set{_dateImported = value;}
		}
		private int _importedBy = 0;
		public int ImportedBy
		{
			get{return _importedBy;}
			set{_importedBy = value;}
		}
		private bool _newImport = false;
		public bool NewImport 
		{
			get{return _newImport;}
			set{_newImport = value;}
		}
		private string _filename = "";
		public string FileName
		{
			get{return _filename;}
			set{_filename = value;}
		}
        private char _scoretype = ' ';
        public char scoretype
        {
            get { return _scoretype; }
            set { _scoretype = value; }
        }


		public void GetRules()
		{
			try
			{
                _rulesTab = new DataTable(TN.Rules);
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
                parmArray[0].Value = this.Country;
                parmArray[1] = new SqlParameter("@scoretype", SqlDbType.Char, 1); //SC CR1034 Behavioural Scoring 15/02/2010
                parmArray[1].Value = this.scoretype;
                parmArray[2] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
                parmArray[2].Value = this.Region;

				this.RunSP("DN_ScoringRulesGetSP", parmArray, _rulesTab);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void DeleteMatrix(SqlConnection conn, SqlTransaction trans, string countryCode, char scoretype,
        string region)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
                parmArray[0].Value = countryCode;
                parmArray[1] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
                parmArray[1].Value = region;
                parmArray[2] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[2].Value = scoretype;

                this.RunSP(conn, trans, "DN_ScoringMatrixDeleteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }

        public void DeleteTermsTypeMatrix(SqlConnection conn, SqlTransaction trans, string countryCode, char scoretype)
        {
            try
            {
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@CountryCode", SqlDbType.Char, 2);
                parmArray[0].Value = countryCode;
                parmArray[1] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[1].Value = scoretype;

                this.RunSP(conn, trans, "DN_ScoreBandMatrixDeleteSP", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		public void SaveMatrixRow(SqlConnection conn, SqlTransaction trans, 
			string fileName,
			string countryCode,
            char scoretype,
			string region,
			int score,
			decimal fLimit,
			decimal eLimit,
			decimal income,
			int user,
			DateTime dateImported )
		{
			try
			{
				parmArray = new SqlParameter[10];
				parmArray[0] = new SqlParameter("@filename", SqlDbType.NChar,120);
				parmArray[0].Value = fileName;
				parmArray[1] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[1].Value = countryCode;
				parmArray[2] = new SqlParameter("@score", SqlDbType.Int);
				parmArray[2].Value = score;
				parmArray[3] = new SqlParameter("@flimit", SqlDbType.Money);
				parmArray[3].Value = fLimit;
				parmArray[4] = new SqlParameter("@elimit", SqlDbType.Money);
				parmArray[4].Value = eLimit;
				parmArray[5] = new SqlParameter("@region", SqlDbType.NVarChar,3);
				parmArray[5].Value = region;
				parmArray[6] = new SqlParameter("@income", SqlDbType.Money);
				parmArray[6].Value = income;
				parmArray[7] = new SqlParameter("@user", SqlDbType.Int);
				parmArray[7].Value = user;
				parmArray[8] = new SqlParameter("@dateimported", SqlDbType.DateTime);
				parmArray[8].Value = dateImported;
                parmArray[9] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[9].Value = scoretype;


				this.RunSP(conn, trans, "DN_ScoringMatrixSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


        public void SaveTermsTypeMatrixRow(SqlConnection conn, SqlTransaction trans, 
    		string countryCode,
            char scoretype,
    		string band,
    		int pointsFrom,
    		int pointsTo,
            decimal serviceCharge,
    		DateTime dateImported,
    		int importedBy,
    		string fileName)
		{
			try
			{
				parmArray = new SqlParameter[9];
				parmArray[0] = new SqlParameter("@CountryCode", SqlDbType.Char,2);
				parmArray[0].Value = countryCode;
				parmArray[1] = new SqlParameter("@Band", SqlDbType.VarChar,32);
                parmArray[1].Value = band;
				parmArray[2] = new SqlParameter("@PointsFrom", SqlDbType.SmallInt);
                parmArray[2].Value = pointsFrom;
                parmArray[3] = new SqlParameter("@PointsTo", SqlDbType.SmallInt);
                parmArray[3].Value = pointsTo;
                parmArray[4] = new SqlParameter("@ServiceCharge", SqlDbType.Float);
                parmArray[4].Value = serviceCharge;
                parmArray[5] = new SqlParameter("@DateImported", SqlDbType.DateTime);
                parmArray[5].Value = dateImported;
                parmArray[6] = new SqlParameter("@ImportedBy", SqlDbType.Int);
                parmArray[6].Value = importedBy;
                parmArray[7] = new SqlParameter("@FileName", SqlDbType.NVarChar,120);
                parmArray[7].Value = fileName;
                parmArray[8] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[8].Value = scoretype;


                this.RunSP(conn, trans, "DN_ScoreBandMatrixRowSaveSP", parmArray);
	
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}


        public void ApplyTermsTypeMatrix(SqlConnection conn, SqlTransaction trans, DateTime startDate, int user, char scorecard)
        {
            try
            {
                parmArray = new SqlParameter[3];
                parmArray[0] = new SqlParameter("@StartDate", SqlDbType.DateTime);
                parmArray[0].Value = startDate;
                parmArray[1] = new SqlParameter("@EmpeeNo", SqlDbType.Int);
                parmArray[1].Value = user;
                parmArray[2] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[2].Value = scorecard;

                this.RunSP(conn, trans, "DN_ScoreBandMatrixApplySP", parmArray);
  
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		public void GetMatrix()
		{
			try
			{
				_matrix = new DataTable(TN.ScoringMatrix);
				parmArray = new SqlParameter[3];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@region", SqlDbType.NVarChar,3);
				parmArray[1].Value = this.Region;
                parmArray[2] = new SqlParameter("@scoretype", SqlDbType.Char, 1);
                parmArray[2].Value = this.scoretype; //CR1034 SC 16/12/10


				this.RunSP("DN_ScoringMatrixGetSP", parmArray, _matrix);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public void GetTermsTypeMatrix()
        {
            try
            {
                _matrix = new DataTable(TN.TermsTypeMatrix);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@CountryCode", SqlDbType.Char, 2);
                parmArray[0].Value = this.Country;
                parmArray[1] = new SqlParameter("@ScoreType", SqlDbType.Char, 1);
                parmArray[1].Value = this.scoretype;

                this.RunSP("DN_ScoreBandMatrixGetSP", parmArray, _matrix); // SC CR1034 17-02-10
      
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
        }


		public void SaveRules(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@rules", SqlDbType.NText);
				parmArray[1].Value = this.RulesXML;
				parmArray[2] = new SqlParameter("@dateImported", SqlDbType.DateTime);
				parmArray[2].Value = this.DateImported;
				parmArray[3] = new SqlParameter("@importedBy", SqlDbType.Int);
				parmArray[3].Value = this.ImportedBy;
				parmArray[4] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
				parmArray[4].Value = this.Region;
				parmArray[5] = new SqlParameter("@newimport", SqlDbType.Bit);
				parmArray[5].Value = this.NewImport;
				parmArray[6] = new SqlParameter("@filename", SqlDbType.NVarChar, 100);
				parmArray[6].Value = this.FileName;

				this.RunSP(conn, trans, "DN_ScoringRulesSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		public int GetScoreLimits()
		{
			try
			{
				_limits= new DataTable("ScoreLimits");      
				parmArray = new SqlParameter[0];
				//parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				result = this.RunSP("DN_GetlatestscorecardSP", parmArray, _limits);
				
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return result;
		}
		public DataSet GetOperands()
		{
			try
			{
				operands = new DataSet();
				result = this.RunSP("DN_ScoringOperandGetSP", operands);
				if(operands.Tables.Count>1)
				{
					operands.Tables[0].TableName = TN.Operands;
					operands.Tables[1].TableName = TN.Operators;
                    operands.Tables[2].TableName = TN.EquifaxOperands;
                }
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return operands;
		}

		public DataTable GetScoreDetails()
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable(TN.ScoringDetails);
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP("DN_getscoredetailsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public DataTable GetScoreDetails(SqlConnection conn, SqlTransaction trans)
		{
			DataTable dt = null;
			try
			{
				dt = new DataTable();
				parmArray = new SqlParameter[1];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar,12);
				parmArray[0].Value = this.AccountNumber;

				this.RunSP(conn, trans, "DN_getscoredetailsSP", parmArray, dt);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
			return dt;
		}

		public void GetRFCreditLimit(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
	
				parmArray = new SqlParameter[7];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[2].Value = this.DateProp;
				parmArray[3] = new SqlParameter("@score", SqlDbType.Int);
				parmArray[3].Value = this.Score;
				parmArray[4] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
				parmArray[4].Value = this.Region;
				parmArray[5] = new SqlParameter("@creditlimit", SqlDbType.Money);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;
                parmArray[6] = new SqlParameter("@scorecard", SqlDbType.Char, 1);
                parmArray[6].Value = this.scoretype;
				result = this.RunSP(conn, trans, "DN_AccountGetRFCreditLimitSP", parmArray);
				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.CreditLimit = (decimal)parmArray[5].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}
		
		public void GetCategory(string acctno, ref string category)
		{
			try
			{
				parmArray = new SqlParameter[2];
				parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[0].Value = acctno;
				parmArray[1] = new SqlParameter("@category", SqlDbType.NChar, 1);
				parmArray[1].Value = "";
				parmArray[1].Direction = ParameterDirection.Output;

				result = this.RunSP("DN_ProposalGetCategorySP", parmArray);
				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[1].Value))
						category = (string)parmArray[1].Value;
				}

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetMinimumRFCreditLimit(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[2].Value = this.DateProp;
				parmArray[3] = new SqlParameter("@score", SqlDbType.Int);
				parmArray[3].Value = this.Score;
				parmArray[4] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
				parmArray[4].Value = this.Region;
				parmArray[5] = new SqlParameter("@creditlimit", SqlDbType.Money);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;

				result = this.RunSP(conn, trans, "DN_AccountSetMinimumRFCreditLimitSP", parmArray);
				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.CreditLimit = (decimal)parmArray[5].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void GetPotentialRFCreditLimit(SqlConnection conn, SqlTransaction trans)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar, 2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[1].Value = this.CustomerID;
				parmArray[2] = new SqlParameter("@dateprop", SqlDbType.DateTime);
				parmArray[2].Value = this.DateProp;
				parmArray[3] = new SqlParameter("@score", SqlDbType.Int);
				parmArray[3].Value = this.Score;
				parmArray[4] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
				parmArray[4].Value = this.Region;
				parmArray[5] = new SqlParameter("@creditlimit", SqlDbType.Money);
				parmArray[5].Value = 0;
				parmArray[5].Direction = ParameterDirection.Output;

				result = this.RunSP(conn, trans, "DN_AccountGetRFPotentialCreditLimitSP", parmArray);
				if(result == 0)
				{
					if(!Convert.IsDBNull(parmArray[5].Value))
						this.CreditLimit = (decimal)parmArray[5].Value;
				}
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

		public void SaveScoreDetails(SqlConnection conn, SqlTransaction trans, string custID,
					string acctNo, DateTime dateScored, string operandName,
					string operandValue, decimal points)
		{
			try
			{
				parmArray = new SqlParameter[6];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
				parmArray[0].Value = custID;
				parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
				parmArray[1].Value = acctNo;
				parmArray[2] = new SqlParameter("@datescored", SqlDbType.DateTime);
				parmArray[2].Value = dateScored;
				parmArray[3] = new SqlParameter("@operandname", SqlDbType.NVarChar, 150);
				parmArray[3].Value = operandName;
				parmArray[4] = new SqlParameter("@operandvalue", SqlDbType.NVarChar, 50);
				parmArray[4].Value = operandValue;
				parmArray[5] = new SqlParameter("@points", SqlDbType.Int);
				parmArray[5].Value = points;

				result = this.RunSP(conn, trans, "DN_SaveScoreDetailsSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

      public void SaveScoreDetails(SqlConnection conn, SqlTransaction trans, string custID,
               string acctNo, DateTime dateScored, string operandName,
               string operandValue, string points)
      {
         try
         {
            parmArray = new SqlParameter[6];
            parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
            parmArray[0].Value = custID;
            parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
            parmArray[1].Value = acctNo;
            parmArray[2] = new SqlParameter("@datescored", SqlDbType.DateTime);
            parmArray[2].Value = dateScored;
            parmArray[3] = new SqlParameter("@operandname", SqlDbType.NVarChar, 4000);
            parmArray[3].Value = operandName;
            parmArray[4] = new SqlParameter("@operandvalue", SqlDbType.NVarChar, 100);
            parmArray[4].Value = operandValue;
            parmArray[5] = new SqlParameter("@points", SqlDbType.NVarChar,100);
            parmArray[5].Value = points;

            result = this.RunSP(conn, trans, "DN_SaveScoreDetailsFromFrontEndSP", parmArray);
         }
         catch (SqlException ex)
         {
            LogSqlException(ex);
            throw ex;
         }
      }

		public void SaveAcceptReferScore(SqlConnection conn, SqlTransaction trans,
										int acceptScore, int referScore)
		{
			try
			{
				parmArray = new SqlParameter[5];
				parmArray[0] = new SqlParameter("@country", SqlDbType.NChar,2);
				parmArray[0].Value = this.Country;
				parmArray[1] = new SqlParameter("@region", SqlDbType.NVarChar, 3);
				parmArray[1].Value = this.Region;
				parmArray[2] = new SqlParameter("@datefrom", SqlDbType.DateTime);
				parmArray[2].Value = this.DateImported;
				parmArray[3] = new SqlParameter("@acceptscore", SqlDbType.Int);
				parmArray[3].Value = acceptScore;
				parmArray[4] = new SqlParameter("@referscore", SqlDbType.Int);
				parmArray[4].Value = referScore;

				this.RunSP(conn, trans, "DN_ScoringAcceptReferSaveSP", parmArray);
			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		}

        public DataTable ScoringGetApplicationTypes()
        {
            DataTable dt = new DataTable();

            try
            {
                this.RunSPNoReturn("ScoringGetApplicationTypes");
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }
		
		public DScoring()
		{

		}

        public DataSet GetScoreDetailsForScoreCard(SqlConnection conn, SqlTransaction trans)
        {
            DataSet ds = null;
            try
            {
                ds = new DataSet();
                parmArray = new SqlParameter[1];
                parmArray[0] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;

                this.RunSP(conn, trans, "DN_GetScoreDetailsForScoreCardSP", parmArray, ds);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return ds;
        }

        public decimal CalculateScoreForScoreCardByEquifax(SqlConnection conn, SqlTransaction trans, string customerage,
                         string employmentempmtstatus,
                     string proposalMaritalstat,
                     string proposaldependants,
                     string worktype,
                     string prodcode,
                     string custaddressresstatus,
                     string addresstime,
                     string employmenttime,
                     string proposalnationality,
                     string custaddresspocode,
                     string paddresstime,
                     string gender,
                     string mobilephone,
                     string NumAppsLst90)
        {
            decimal dt = 0;
            try
            {
                // dt = new DataTable();
                parmArray = new SqlParameter[19];
                parmArray[0] = new SqlParameter("@accountNo", SqlDbType.NVarChar, 12);
                parmArray[0].Value = this.AccountNumber;
                parmArray[1] = new SqlParameter("@customerID", SqlDbType.NVarChar, 50);
                parmArray[1].Value = this.CustomerID;
                parmArray[2] = new SqlParameter("@country", SqlDbType.NVarChar, 10);
                parmArray[2].Value = this.Country;
                parmArray[3] = new SqlParameter("@customerage", SqlDbType.NVarChar, 50);
                parmArray[3].Value = customerage;
                parmArray[4] = new SqlParameter("@employmentempmtstatus", SqlDbType.NVarChar, 50);
                parmArray[4].Value = employmentempmtstatus;
                parmArray[5] = new SqlParameter("@proposalMaritalstat", SqlDbType.NVarChar, 50);
                parmArray[5].Value = proposalMaritalstat;
                parmArray[6] = new SqlParameter("@proposaldependants", SqlDbType.NVarChar, 50);
                parmArray[6].Value = proposaldependants;
                parmArray[7] = new SqlParameter("@worktype", SqlDbType.NVarChar, 50);
                parmArray[7].Value = worktype;
                parmArray[8] = new SqlParameter("@prodcode", SqlDbType.NVarChar, 50);
                parmArray[8].Value = prodcode;
                parmArray[9] = new SqlParameter("@custaddressresstatus", SqlDbType.NVarChar, 50);
                parmArray[9].Value = custaddressresstatus;
                parmArray[10] = new SqlParameter("@addresstime", SqlDbType.NVarChar, 50);
                parmArray[10].Value = addresstime;
                parmArray[11] = new SqlParameter("@employmenttime", SqlDbType.NVarChar, 50);
                parmArray[11].Value = employmenttime;
                parmArray[12] = new SqlParameter("@proposalnationality", SqlDbType.NVarChar, 50);
                parmArray[12].Value = proposalnationality;
                parmArray[13] = new SqlParameter("@custaddresspocode", SqlDbType.NVarChar, 50);
                parmArray[13].Value = custaddresspocode;
                parmArray[14] = new SqlParameter("@paddresstime", SqlDbType.NVarChar, 50);
                parmArray[14].Value = paddresstime;
                parmArray[15] = new SqlParameter("@gender", SqlDbType.NVarChar, 50);
                parmArray[15].Value = gender;
                parmArray[16] = new SqlParameter("@mobilephone", SqlDbType.NVarChar, 50);
                parmArray[16].Value = mobilephone;
                parmArray[17] = new SqlParameter("@NumAppsLst90", SqlDbType.NVarChar, 50);
                parmArray[17].Value = NumAppsLst90;

                parmArray[18] = new SqlParameter("@SCORE", SqlDbType.Decimal);
                parmArray[18].Value = 0;
                parmArray[18].Direction = ParameterDirection.Output;

                this.RunSP("CalculateScoreForScoreCardByEquifax", parmArray);
                // this.RunSP("DN_AccountGetNoOfAgreementsSP", parmArray);

                if (parmArray[18].Value != DBNull.Value)
                    dt = (decimal)parmArray[18].Value;


                //if (result == 0)
                //{
                //    if (!Convert.IsDBNull(parmArray[3].Value))
                //        dt = (decimal)parmArray[3].Value;
                //}

                //this.RunSP(conn, trans, "CalculateScoreForScoreCardByEquifax", parmArray, dt);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return dt;
        }
        public int CalculateScore(
             string customerID
           , string accountNo
           , decimal InterceptScore
           , string CustType
           , string age
           , string employmentstatus_woe
           , string gender_woe
           , string maritalstatus_woe
           , string mobilenumber_woe
           , string occupation_woe
           , string postcode_woe
           , string residentialstatus_woe
           , string timecurrentemployment
           , string numberdependents
           , string timecurrentaddress
           , string avg_agreement_total_1m_sq
           , string avg_balance_arrears_1m_ln
           , string avg_balance_arrears_12m_ln
           , string balancearrears_pound_6m
           , string balancearrears_pound_6m_ln
           , string count_daysarrear_30more_17m_ln
           , string count_daysarrear_60more_17m_ln
           , string flag_customerstatus_his_woe
           , string daysarrears_pound_6m
           , string max_perc_outs_3m_sq
           , string max_perc_outsarrears_6m_ln
           , string newest_credit_sq
           , string number_account_17m
           , string number_account_opened_3m
           , string number_account_opened_3m_cr
           , string numberdependents_cr
           , string numberdependents_sq
           , string oldest_credit_ln
           , string ratio_ndependent_to_age
           , string ratio_tcurrentemploy_to_age
           , string timecurrentaddress_ln
           , string timecurrentemployment_ln
           , string timecurrentemployment_sr
           )
        {
            int score = 0;
            try
            {
                parmArray = new SqlParameter[38];

                parmArray[0] = new SqlParameter("@customerID", SqlDbType.VarChar, 20);
                parmArray[0].Value = customerID;
                parmArray[1] = new SqlParameter("@accountNo", SqlDbType.Char, 12);
                parmArray[1].Value = accountNo;
                parmArray[2] = new SqlParameter("@InterceptScore", SqlDbType.Float);
                parmArray[2].Value = InterceptScore;
                parmArray[3] = new SqlParameter("@CustType", SqlDbType.Char, 1);
                parmArray[3].Value = CustType;
                parmArray[4] = new SqlParameter("@rl_age", SqlDbType.Float);
                parmArray[4].Value = age;
                parmArray[5] = new SqlParameter("@rl_employmentstatus_woe", SqlDbType.Float);
                parmArray[5].Value = employmentstatus_woe;
                parmArray[6] = new SqlParameter("@rl_gender_woe", SqlDbType.Float);
                parmArray[6].Value = gender_woe;
                parmArray[7] = new SqlParameter("@rl_maritalstatus_woe", SqlDbType.Float);
                parmArray[7].Value = maritalstatus_woe;
                parmArray[8] = new SqlParameter("@rl_mobilenumber_woe", SqlDbType.Float);
                parmArray[8].Value = mobilenumber_woe;
                parmArray[9] = new SqlParameter("@rl_occupation_woe", SqlDbType.Float);
                parmArray[9].Value = occupation_woe;
                parmArray[10] = new SqlParameter("@rl_postcode_woe", SqlDbType.Float);
                parmArray[10].Value = postcode_woe;
                parmArray[11] = new SqlParameter("@rl_residentialstatus_woe", SqlDbType.Float);
                parmArray[11].Value = residentialstatus_woe;
                parmArray[12] = new SqlParameter("@rl_timecurrentemployment", SqlDbType.Float);
                parmArray[12].Value = timecurrentemployment;
                parmArray[13] = new SqlParameter("@rl_numberdependents", SqlDbType.Float);
                parmArray[13].Value = numberdependents;
                parmArray[14] = new SqlParameter("@rl_timecurrentaddress", SqlDbType.Float);
                parmArray[14].Value = timecurrentaddress;
                parmArray[15] = new SqlParameter("@rl_avg_agreement_total_1m_sq", SqlDbType.Float);
                parmArray[15].Value = avg_agreement_total_1m_sq;
                parmArray[16] = new SqlParameter("@rl_avg_balance_arrears_1m_ln", SqlDbType.Float);
                parmArray[16].Value = avg_balance_arrears_1m_ln;
                parmArray[17] = new SqlParameter("@rl_avg_balance_arrears_12m_ln", SqlDbType.Float);
                parmArray[17].Value = avg_balance_arrears_12m_ln;
                parmArray[18] = new SqlParameter("@rl_balancearrears_pound_6m", SqlDbType.Float);
                parmArray[18].Value = balancearrears_pound_6m;
                parmArray[19] = new SqlParameter("@rl_balancearrears_pound_6m_ln", SqlDbType.Float);
                parmArray[19].Value = balancearrears_pound_6m_ln;
                parmArray[20] = new SqlParameter("@rl_count_daysarrear_30more_17m_ln", SqlDbType.Float);
                parmArray[20].Value = count_daysarrear_30more_17m_ln;
                parmArray[21] = new SqlParameter("@rl_count_daysarrear_60more_17m_ln", SqlDbType.Float);
                parmArray[21].Value = count_daysarrear_60more_17m_ln;
                parmArray[22] = new SqlParameter("@rl_flag_customerstatus_his_woe", SqlDbType.Float);
                parmArray[22].Value = flag_customerstatus_his_woe;
                parmArray[23] = new SqlParameter("@rl_daysarrears_pound_6m", SqlDbType.Float);
                parmArray[23].Value = daysarrears_pound_6m;
                parmArray[24] = new SqlParameter("@rl_max_perc_outs_3m_sq", SqlDbType.Float);
                parmArray[24].Value = max_perc_outs_3m_sq;
                parmArray[25] = new SqlParameter("@rl_max_perc_outsarrears_6m_ln", SqlDbType.Float);
                parmArray[25].Value = max_perc_outsarrears_6m_ln;
                parmArray[26] = new SqlParameter("@rl_newest_credit_sq", SqlDbType.Float);
                parmArray[26].Value = newest_credit_sq;
                parmArray[27] = new SqlParameter("@rl_number_account_17m", SqlDbType.Float);
                parmArray[27].Value = number_account_17m;
                parmArray[28] = new SqlParameter("@rl_number_account_opened_3m", SqlDbType.Float);
                parmArray[28].Value = number_account_opened_3m;
                parmArray[29] = new SqlParameter("@rl_number_account_opened_3m_cr", SqlDbType.Float);
                parmArray[29].Value = number_account_opened_3m_cr;
                parmArray[30] = new SqlParameter("@rl_numberdependents_cr", SqlDbType.Float);
                parmArray[30].Value = numberdependents_cr;
                parmArray[31] = new SqlParameter("@rl_numberdependents_sq", SqlDbType.Float);
                parmArray[31].Value = numberdependents_sq;
                parmArray[32] = new SqlParameter("@rl_oldest_credit_ln", SqlDbType.Float);
                parmArray[32].Value = oldest_credit_ln;
                parmArray[33] = new SqlParameter("@rl_ratio_ndependent_to_age", SqlDbType.Float);
                parmArray[33].Value = ratio_ndependent_to_age;
                parmArray[34] = new SqlParameter("@rl_ratio_tcurrentemploy_to_age", SqlDbType.Float);
                parmArray[34].Value = ratio_tcurrentemploy_to_age;
                parmArray[35] = new SqlParameter("@rl_timecurrentaddress_ln", SqlDbType.Float);
                parmArray[35].Value = timecurrentaddress_ln;
                parmArray[36] = new SqlParameter("@rl_timecurrentemployment_ln", SqlDbType.Float);
                parmArray[36].Value = timecurrentemployment_ln;
                parmArray[37] = new SqlParameter("@rl_timecurrentemployment_sr", SqlDbType.Float);
                parmArray[37].Value = timecurrentemployment_sr;

                score = this.RunSP("usp_SCDE_CalculateScore", parmArray);
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return score;
        }
    }
}
