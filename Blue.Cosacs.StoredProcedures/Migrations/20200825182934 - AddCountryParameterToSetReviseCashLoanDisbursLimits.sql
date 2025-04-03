-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- *************************************************************************************************
-- Developer:    Rahul Sonawane
-- Date:         25 Aug 2020
-- Purpose:      Include setting for- "Allow Revise Cash Loan Disbursement Limits" in CountryMaintenance.
-- *************************************************************************************************

		DECLARE @CountryCode CHAR(2)
		SELECT TOP 1 @CountryCode = countrycode FROM country WITH(NOLOCK)   

		IF NOT EXISTS (SELECT * FROM [CountryMaintenance] WITH(NOLOCK) WHERE CodeName='ReviseCashLoanDisbursLimits')
		BEGIN            
				INSERT INTO CountryMaintenance (CountryCode,ParameterCategory,Name,Value,[Type],[Precision],OptionCategory,OptionListName,[Description],CodeName)
				VALUES ( @CountryCode
							,(SELECT code FROM code WHERE codedescript = 'Cash Loans' AND category = 'CMC')
							,'Allow Revise Cash Loan Disbursement Limits'
							,'False'
							,'checkbox'
							,0
							,''
							,''
							,'Set as true to get cash loan upto Maximum %RF Spend Limit Allocated'
							,'ReviseCashLoanDisbursLimits');
		END