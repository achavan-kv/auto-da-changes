using Blue.Cosacs.Repositories;
using STL.Common.Constants.AccountTypes;
using STL.Common.Constants.Enums;
using STL.DAL;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Unicomer.Cosacs.Repository.DbCommands.Customers
{
    public class CosacsAccountNumberGenerateCommand
    {
        private int[] array1;
        private int[] array2;
        private int hiAllowed;
        private int hiAllocated;
        private short _branchNo;
        private string _accountType;
        private DAccountNumber acctNo;
        private string acctInd;
        private string _newAccountNo = "000000000000";
        private bool created = false;
        private int mod11 = 11;

        public string GenerateAccountNumber(SqlConnection conn, SqlTransaction trans, string countryCode, short branchNumber, string accountType)
        {
            _branchNo = branchNumber;
            _accountType = accountType;
            string newAccountNo = "000000000000";
            countryCode = countryCode.Trim();

            array1 = new int[11];
            array2 = new int[12];

            if ((countryCode == "A" ||
                countryCode == "M" ||
                countryCode == "N" ||
                countryCode == "P") &&
                AT.IsCreditType(accountType))
            {   //HP weightings
                array1[0] = 9;
                array1[1] = 8;
                array1[2] = 4;
                array1[3] = 10;
                array1[4] = 7;
                array1[5] = 1;
                array1[6] = 6;
                array1[7] = 3;
                array1[8] = 5;
                array1[9] = 2;
                array1[10] = 1;
            }
            else
            {   //Option weightings
                array1[0] = 9;
                array1[1] = 8;
                array1[2] = 4;
                array1[3] = 10;
                array1[4] = 1;
                array1[5] = 6;
                array1[6] = 3;
                array1[7] = 5;
                array1[8] = 2;
                array1[9] = 7;
                array1[10] = 1;
            }

            acctNo = new DAccountNumber();

            if (acctNo.GetAccountControl(conn, trans, countryCode, branchNumber, accountType) == (int)Return.Success)
            {
                DataTable dt = acctNo.AccountControlTable;

                //loop through the records. Usually only one
                foreach (DataRow row in dt.Rows)
                {
                    hiAllocated = (int)row["hiallocated"];
                    hiAllowed = (int)row["hiallowed"];
                    acctInd = (string)row["acctind"];

                    if (++hiAllocated >= hiAllowed)
                    {
                        new AccountRepository().ResetAcctnos(conn, trans, branchNumber, accountType);
                        hiAllocated = 1;
                    }

                    //increment hiAllocated trying each time to create
                    //an account number based on it.
                    while (hiAllocated++ < hiAllowed)
                    {
                        //update first so that we don't clash with other
                        //users
                        acctNo.UpdateAccountControl(conn, trans, branchNumber, accountType, hiAllocated, hiAllowed);
                        if (createNumber())
                        {
                            //When an account number has been successfully 
                            //created break from the loop
                            created = true;
                            break;
                        }
                    }
                    if (created)
                    {
                        //No need to carry on looking
                        break;
                    }
                }
            }

            if (created)
            {
                newAccountNo = _newAccountNo;
            }
            else
            {
                newAccountNo = String.Empty;
            }

            return newAccountNo;
        }

        /// <summary>
        /// Attempts to create a new account number using the 
        /// current allocated number
        /// </summary>
        /// <returns>success or failure</returns>
        private bool createNumber()
        {
            bool succeed = false;
            string branchStr = Convert.ToString(_branchNo);
            string hiAllocStr = Convert.ToString(hiAllocated);
            for (int i = 0; i < array2.Length; i++)
                array2[i] = 0;

            for (int i = branchStr.Length; i > 0; i--)
            {
                array2[i - 1] = Convert.ToInt32(branchStr.Substring(i - 1, 1));
            }

            array2[3] = Convert.ToInt32(acctInd);

            //put hiAllocated in position 
            int y = 9;
            for (int i = hiAllocStr.Length; i > 0; i--, y--)
            {
                array2[y] = Convert.ToInt32(hiAllocStr.Substring(i - 1, 1));
            }

            //Calculate the check digit
            int modCalc = 0;
            for (int i = 0; i < 10; i++)
            {
                modCalc += array1[i] * array2[i];
            }

            modCalc = mod11 - (modCalc % mod11);

            if (modCalc != 10)
            {
                if (modCalc == 11)
                    modCalc = 0;
                array2[10] = modCalc;
                if (AT.IsCashType(_accountType))
                    array2[11] = 0;
                else
                    array2[11] = 1;

                _newAccountNo = "";
                foreach (int letter in array2)
                {
                    _newAccountNo += Convert.ToString(letter);
                }
                //Check to see if the account already exists
                if (!Convert.ToBoolean(acctNo.IsDuplicate(_newAccountNo)))
                {
                    succeed = true;
                }
            }
            return succeed;
        }
    }
}
