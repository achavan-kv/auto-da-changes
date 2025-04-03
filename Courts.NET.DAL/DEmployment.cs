using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using STL.Common;
using STL.Common.Constants.Enums;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;

namespace STL.DAL
{
    /// <summary>
    /// Summary description for DEmployment.
    /// </summary>
    public class DEmployment : DALObject
    {
        private short _origbr = 0;
        public short OrigBr
        {
            get { return _origbr; }
            set { _origbr = value; }
        }

        private DateTime _dateemployed = DateTime.MinValue;
        public DateTime DateEmployed
        {
            get { return _dateemployed; }
            set { _dateemployed = value; }
        }

        private string _empyrno = "";
        public string EmployerNo
        {
            get { return _empyrno; }
            set { _empyrno = value; }
        }

        private string _worktype = "";
        public string WorkType
        {
            get { return _worktype; }
            set { _worktype = value; }
        }

        private string _employmentstatus = "";
        public string EmploymentStatus
        {
            get { return _employmentstatus; }
            set { _employmentstatus = value; }
        }

        private string _fullorpart = "";
        public string FullOrPartTime
        {
            get { return _fullorpart; }
            set { _fullorpart = value; }
        }

        private string _temporperm = "";
        public string TempOrPerm
        {
            get { return _temporperm; }
            set { _temporperm = value; }
        }

        private string _custempeeno = "";
        public string CustomerEmployeeNo
        {
            get { return _custempeeno; }
            set { _custempeeno = value; }
        }

        private string _payfreq = "";
        public string PayFrequency
        {
            get { return _payfreq; }
            set { _payfreq = value; }
        }

        private object _annualgross;
        public object AnnualGross
        {
            get { return _annualgross; }
            set { _annualgross = value; }
        }

        private DateTime _dateleft = DateTime.MinValue;
        public DateTime DateLeft
        {
            get { return _dateleft; }
            set { _dateleft = value; }
        }

        private string _persdialcode = "";
        public string PersDialCode
        {
            get { return _persdialcode; }
            set { _persdialcode = value; }
        }

        private string _perstel = "";
        public string PersTel
        {
            get { return _perstel; }
            set { _perstel = value; }
        }

        private DateTime _pdateemployed = DateTime.MinValue;
        public DateTime PrevDateEmployed
        {
            get { return _pdateemployed; }
            set { _pdateemployed = value; }
        }

        private DateTime _pdateleft = DateTime.MinValue;
        public DateTime PrevDateLeft
        {
            get { return _pdateleft; }
            set { _pdateleft = value; }
        }

        private string _staffno = "";
        public string StaffNo
        {
            get { return _staffno; }
            set { _staffno = value; }
        }

        private string _custid = "";
        public string CustomerID
        {
            get { return _custid; }
            set { _custid = value; }
        }

        //CR 866 added occupation as jobtitle already existed - move old jobtitle to occupation
        private string _occupation = "";
        public string Occupation
        {
            get { return _occupation; }
            set { _occupation = value; }
        }

        //CR 866 Added Job title
        private string _jobtitle = "";
        public string JobTitle
        {
            get { return _jobtitle; }
            set { _jobtitle = value; }
        }

        //CR 866 Added Industry
        private string _industry = "";
        public string Industry
        {
            get { return _industry; }
            set { _industry = value; }
        }
        //CR 866 Added organisation
        private string _organisation = "";
        public string Organisation
        {
            get { return _organisation; }
            set { _organisation = value; }
        }

        //CR 866 Added EducationLevel
        private string _educationLevel = "";
        public string EducationLevel
        {
            get { return _educationLevel; }
            set { _educationLevel = value; }
        }

		private string _employer = "";
		public string Employer
		{
			get{return _employer;}
			set{_employer = value;}
		}
        //CR1066 - added employer address
        private string _employeradd1 = "";
        public string employeradd1
        {
            get { return _employeradd1; }
            set { _employeradd1 = value; }
        }
        private string _employeradd2 = "";
        public string employeradd2
        {
            get { return _employeradd2; }
            set { _employeradd2 = value; }
        }
        private string _employeradd3 = "";
        public string employeradd3
        {
            get { return _employeradd3; }
            set { _employeradd3 = value; }
        }
        private string _employerpocode = "";
        public string employerpocode
        {
            get { return _employerpocode; }
            set { _employerpocode = value; }
        }

        
		private DataTable _empDetails = null;
		public DataTable EmpDetails
		{
			get{return _empDetails;}
		}

        
        private string _department = "";
        public string Department
        {
            get { return _department; }
            set { _department = value; }
        }

        public DataTable GetRow(string name)
        {
            DataTable dt = new DataTable(name);
            //CR 866 3 New Columns Added [PC]
            dt.Columns.AddRange(new DataColumn[] { new DataColumn(CN.CustomerID),
													 new DataColumn(CN.DateEmployed, Type.GetType("System.DateTime")),
													 new DataColumn(CN.WorkType), //CR 866 Changed this to worktype
													 new DataColumn(CN.EmploymentStatus),
													 new DataColumn(CN.PayFrequency),
													 new DataColumn(CN.AnnualGross, Type.GetType("System.Double")),
													 new DataColumn(CN.PersDialCode),
													 new DataColumn(CN.PersTel),
													 new DataColumn(CN.PrevDateEmployed, Type.GetType("System.DateTime")),
													 new DataColumn(CN.StaffNo),
													 new DataColumn(CN.JobTitle),
			                                         new DataColumn(CN.Employer),
			                                         new DataColumn(CN.FullOrPartTime),
													 new DataColumn(CN.Department),
													 new DataColumn(CN.Occupation), //CR 866 new col
													 new DataColumn(CN.Industry), //CR 866 new col
													 new DataColumn(CN.Organisation), //CR 866 new col 
													 new DataColumn(CN.EducationLevel), //CR 866 new col 
													 new DataColumn(CN.EmpyrAddr1), //CR 1066 
													 new DataColumn(CN.EmpyrAddr2), //CR 1066 
													 new DataColumn(CN.EmpyrAddr3), //CR 1066 
													 new DataColumn(CN.EmpyrPOCode), //CR 1066 
												 });

            //CR 866 4 New Columns Added [PC]
            dt.Rows.Add(new object[] { this.CustomerID, 
										this.DateEmployed,
										this.WorkType,
										this.EmploymentStatus,
										this.PayFrequency,
										this.AnnualGross,
										this.PersDialCode,
										this.PersTel,
										this.PrevDateEmployed,
										this.StaffNo,
										this.JobTitle,
										this.Employer,
			                            this.FullOrPartTime,
										this.Department,
										this.Occupation, //CR 866
										this.Industry,	 //CR 866
										this.Organisation, //CR 866
										this.EducationLevel, //CR 866
	                                    this.employeradd1, //CR1066
                                        this.employeradd2, //CR1066
                                        this.employeradd3, //CR1066
                                        this.employerpocode //CR1066
            }); 
			return dt;
		}

        public int Save(SqlConnection conn, SqlTransaction trans, string customerID)
        {
            try
            {
                //CR 866 Changed this to 21
                parmArray = new SqlParameter[21];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar,20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@dateemployed", SqlDbType.DateTime);
				if(this.DateEmployed == DateTime.MinValue)
					parmArray[1].Value = DBNull.Value;
				else
					parmArray[1].Value = this.DateEmployed;
				parmArray[2] = new SqlParameter("@empyrno", SqlDbType.NVarChar, 6);
				parmArray[2].Value = this.EmployerNo;
				parmArray[3] = new SqlParameter("@worktype", SqlDbType.NVarChar, 2);
				parmArray[3].Value = this.WorkType;
				parmArray[4] = new SqlParameter("@empmtstatus", SqlDbType.NChar, 1);
				parmArray[4].Value = this.EmploymentStatus;
				parmArray[5] = new SqlParameter("@fullorpart", SqlDbType.NChar, 1);
				parmArray[5].Value = this.FullOrPartTime;
				parmArray[6] = new SqlParameter("@temporperm", SqlDbType.NChar, 1);
				parmArray[6].Value = this.TempOrPerm;
				parmArray[7] = new SqlParameter("@custempeeno", SqlDbType.NVarChar, 12);
				parmArray[7].Value = this.CustomerEmployeeNo;
				parmArray[8] = new SqlParameter("@payfreq", SqlDbType.NChar, 1);
				parmArray[8].Value = this.PayFrequency;
				parmArray[9] = new SqlParameter("@annualgross", SqlDbType.Float);
				parmArray[9].Value = this.AnnualGross;
				parmArray[10] = new SqlParameter("@dateleft", SqlDbType.DateTime);
				if(this.DateLeft == DateTime.MinValue)
					parmArray[10].Value = DBNull.Value;
				else
					parmArray[10].Value = this.DateLeft;
				parmArray[11] = new SqlParameter("@persdialcode", SqlDbType.NChar, 8);
				parmArray[11].Value = this.PersDialCode;
				parmArray[12] = new SqlParameter("@perstel", SqlDbType.NChar, 13);
				parmArray[12].Value = this.PersTel;
				parmArray[13] = new SqlParameter("@pdateemployed", SqlDbType.DateTime);
				if(this.PrevDateEmployed == DateTime.MinValue)
					parmArray[13].Value = DBNull.Value;
				else
					parmArray[13].Value = this.PrevDateEmployed;
				parmArray[14] = new SqlParameter("@pdateleft", SqlDbType.DateTime);
				if(this.PrevDateLeft == DateTime.MinValue)
					parmArray[14].Value = DBNull.Value;
				else
					parmArray[14].Value = this.PrevDateLeft;
				parmArray[15] = new SqlParameter("@staffno", SqlDbType.NChar, 20);      //UAT16 jec 07/04/10
				parmArray[15].Value = this.StaffNo;
				
				parmArray[16] = new SqlParameter("@department", SqlDbType.NChar, 20);
				parmArray[16].Value = this.Department;

                //CR 866a Change column widths to constants
                //CR 866 adding these fields
                parmArray[17] = new SqlParameter("@jobtitle", SqlDbType.NVarChar, CW.JobTitle);
                parmArray[17].Value = this.JobTitle;
                parmArray[18] = new SqlParameter("@industry", SqlDbType.NVarChar, CW.Industry);
                parmArray[18].Value = this.Industry;
                parmArray[19] = new SqlParameter("@organisation", SqlDbType.NVarChar, CW.Organisation);
                parmArray[19].Value = this.Organisation;
                parmArray[20] = new SqlParameter("@EducationLevel", SqlDbType.NVarChar, CW.EducationLevel);
                parmArray[20].Value = this.EducationLevel;

                //End CR 866

                result = this.RunSP(conn, trans, "DN_EmploymentSaveSP", parmArray);

                if (result == 0)
                    result = (int)Return.Success;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public void GetEmployment(string customerID)
        {
            try
            {
                CustomerID = customerID;
                //CR 866 changed to 24
				parmArray = new SqlParameter[28];
				parmArray[0] = new SqlParameter("@custid", SqlDbType.VarChar,20);
				parmArray[0].Value = customerID;
				parmArray[1] = new SqlParameter("@origbr", SqlDbType.SmallInt);
				parmArray[1].Value = this.OrigBr;
				parmArray[2] = new SqlParameter("@dateemployed", SqlDbType.DateTime);
				parmArray[2].Value = this.DateEmployed;
				parmArray[3] = new SqlParameter("@empyrno", SqlDbType.VarChar, 6);
				parmArray[3].Value = this.EmployerNo;
				parmArray[4] = new SqlParameter("@worktype", SqlDbType.VarChar, 2);
				parmArray[4].Value = this.WorkType;
				parmArray[5] = new SqlParameter("@empmtstatus", SqlDbType.Char, 1);
				parmArray[5].Value = this.EmploymentStatus;
				parmArray[6] = new SqlParameter("@fullorpart", SqlDbType.Char, 1);
				parmArray[6].Value = this.FullOrPartTime;
				parmArray[7] = new SqlParameter("@temporperm", SqlDbType.Char, 1);
				parmArray[7].Value = this.TempOrPerm;
				parmArray[8] = new SqlParameter("@custempeeno", SqlDbType.VarChar, 12);
				parmArray[8].Value = this.CustomerEmployeeNo;
				parmArray[9] = new SqlParameter("@payfreq", SqlDbType.Char, 1);
				parmArray[9].Value = this.PayFrequency;
				parmArray[10] = new SqlParameter("@annualgross", SqlDbType.Float);
				parmArray[10].Value = this.AnnualGross;
				parmArray[11] = new SqlParameter("@dateleft", SqlDbType.DateTime);
				parmArray[11].Value = this.DateLeft;
				parmArray[12] = new SqlParameter("@persdialcode", SqlDbType.Char, 8);
				parmArray[12].Value = this.PersDialCode;
				parmArray[13] = new SqlParameter("@perstel", SqlDbType.Char, 13);
				parmArray[13].Value = this.PersTel;
				parmArray[14] = new SqlParameter("@pdateemployed", SqlDbType.DateTime);
				parmArray[14].Value = this.PrevDateEmployed;
				parmArray[15] = new SqlParameter("@pdateleft", SqlDbType.DateTime);
				parmArray[15].Value = this.PrevDateLeft;
                parmArray[16] = new SqlParameter("@staffno", SqlDbType.VarChar, 20);       //UAT16 jec 07/04/10
				parmArray[16].Value = this.StaffNo;
				parmArray[17] = new SqlParameter("@jobtitle", SqlDbType.VarChar, CW.JobTitle); //CR 866a changed to col width constant
				parmArray[17].Value = this.JobTitle;
				parmArray[18] = new SqlParameter("@employer", SqlDbType.VarChar, 20);
				parmArray[18].Value = this.Employer;
				parmArray[19] = new SqlParameter("@department", SqlDbType.VarChar, 20);
				parmArray[19].Value = this.Department;
                //CR 866a
                //CR 866 adding these fields
                //CR 866a  
                parmArray[20] = new SqlParameter("@occupation", SqlDbType.VarChar, 20);
                parmArray[20].Value = this.Occupation;
                parmArray[21] = new SqlParameter("@industry", SqlDbType.VarChar, CW.Industry);
                parmArray[21].Value = this.Industry;
                parmArray[22] = new SqlParameter("@organisation", SqlDbType.VarChar, CW.Organisation);
                parmArray[22].Value = this.Organisation;
                parmArray[23] = new SqlParameter("@EducationLevel", SqlDbType.VarChar, CW.EducationLevel);
                parmArray[23].Value = this.EducationLevel;
                //End CR 866#
                // CR1066 added address
                parmArray[24] = new SqlParameter("@employeradd1", SqlDbType.VarChar, 26);
                parmArray[24].Value = this.employeradd1;
                parmArray[25] = new SqlParameter("@employeradd2", SqlDbType.VarChar, 26);
                parmArray[25].Value = this.employeradd2;
                parmArray[26] = new SqlParameter("@employeradd3", SqlDbType.VarChar, 26);
                parmArray[26].Value = this.employeradd3;
                parmArray[27] = new SqlParameter("@employerpocode", SqlDbType.VarChar, 26);
                parmArray[27].Value = this.employerpocode;


                foreach (SqlParameter p in parmArray)
                    p.Direction = ParameterDirection.Output;
                parmArray[0].Direction = ParameterDirection.Input;

                this.RunSP("DN_EmploymentGetSP", parmArray);

				if(!Convert.IsDBNull(parmArray[1].Value))
					this.OrigBr = (short)parmArray[1].Value;
				if(!Convert.IsDBNull(parmArray[2].Value))
					this.DateEmployed = (DateTime)parmArray[2].Value;
				if(!Convert.IsDBNull(parmArray[3].Value))
					this.EmployerNo = (string)parmArray[3].Value;
				if(!Convert.IsDBNull(parmArray[4].Value))
					this.WorkType = (string)parmArray[4].Value;
				if(!Convert.IsDBNull(parmArray[5].Value))
					this.EmploymentStatus = (string)parmArray[5].Value;
				if(!Convert.IsDBNull(parmArray[6].Value))
					this.FullOrPartTime = (string)parmArray[6].Value;
				if(!Convert.IsDBNull(parmArray[7].Value))
					this.TempOrPerm = (string)parmArray[7].Value;
				if(!Convert.IsDBNull(parmArray[8].Value))
					this.CustomerEmployeeNo = (string)parmArray[8].Value;
				if(!Convert.IsDBNull(parmArray[9].Value))
					this.PayFrequency = (string)parmArray[9].Value;
				//if(!Convert.IsDBNull(parmArray[10].Value))
					this.AnnualGross = parmArray[10].Value;
				if(!Convert.IsDBNull(parmArray[11].Value))
					this.DateLeft = (DateTime)parmArray[11].Value;
				if(!Convert.IsDBNull(parmArray[12].Value))
					this.PersDialCode = (string)parmArray[12].Value;
				if(!Convert.IsDBNull(parmArray[13].Value))
					this.PersTel = (string)parmArray[13].Value;	
				if(!Convert.IsDBNull(parmArray[14].Value))
					this.PrevDateEmployed = (DateTime)parmArray[14].Value;
				if(!Convert.IsDBNull(parmArray[15].Value))
					this.PrevDateLeft = (DateTime)parmArray[15].Value;
				if(!Convert.IsDBNull(parmArray[16].Value))
					this.StaffNo = ((string)parmArray[16].Value).Trim();
				if(!Convert.IsDBNull(parmArray[17].Value))
					this.JobTitle = (string)parmArray[17].Value;
				if(!Convert.IsDBNull(parmArray[18].Value))
					this.Employer = (string)parmArray[18].Value;
				if(!Convert.IsDBNull(parmArray[19].Value))
					this.Department = (string)parmArray[19].Value;

                //CR 866 New fields
                if (!Convert.IsDBNull(parmArray[20].Value))
                    this.Occupation = parmArray[20].Value.ToString();
                if (!Convert.IsDBNull(parmArray[21].Value))
                    this.Industry = parmArray[21].Value.ToString();
                if (!Convert.IsDBNull(parmArray[22].Value))
                    this.Organisation = parmArray[22].Value.ToString();
                if (!Convert.IsDBNull(parmArray[23].Value))
                    this.EducationLevel = parmArray[23].Value.ToString();

                //End new fields
                //C1066 new address fields
                if (!Convert.IsDBNull(parmArray[24].Value))
                    this.employeradd1 = parmArray[24].Value.ToString();
                if (!Convert.IsDBNull(parmArray[25].Value))
                    this.employeradd2 = parmArray[25].Value.ToString();
                if (!Convert.IsDBNull(parmArray[26].Value))
                    this.employeradd3 = parmArray[26].Value.ToString();
                if (!Convert.IsDBNull(parmArray[27].Value))
                    this.employerpocode = parmArray[27].Value.ToString();

			}
			catch(SqlException ex)
			{
				LogSqlException(ex);
				throw ex;
			}
		

            
        }

        public int GetEmployerDetails(string acctNo)
        {
            try
            {
                _empDetails = new DataTable(TN.Employment);
                parmArray = new SqlParameter[2];
                parmArray[0] = new SqlParameter("@custid", SqlDbType.NVarChar, 20);
                parmArray[0].Value = this.CustomerID;
                parmArray[1] = new SqlParameter("@acctno", SqlDbType.NVarChar, 12);
                parmArray[1].Value = acctNo;

                result = this.RunSP("DN_EmployerGetSP", parmArray, _empDetails);

                if (result == 0)
                    result = (int)Return.Success;
                else
                    result = (int)Return.Fail;
            }
            catch (SqlException ex)
            {
                LogSqlException(ex);
                throw ex;
            }
            return result;
        }

        public DEmployment()
        {
            //
            // TODO: Add constructor logic here
            //
        }
    }
}
