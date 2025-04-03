-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parameterCat int

set @parameterCat = (select ParameterCategory from CountryMaintenance where codename = 'CL_MaxLoanAmount')

IF NOT EXISTS(select * from countrymaintenance where codename = 'CL_EarlySettPenaltyPeriod')
BEGIN
	
	insert into 
		countrymaintenance(CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	select 
		   c.CountryCode, 
		   @parameterCat, 
		   'T & T Early Settlement Penalty Period',
		   '6',
		   'numeric', 
		   0,
		   '',
		   '',
		 'If a Cash Loan is settled within this number of months in Trinidad and Tobago, then the rebate is calculated according to the rule defined by the Rebate Calculation Rule Country Parameter. If a Cash Loan account is settled early, in greater than this number of months, the rebate is calculated according to the rule 78. (The standard Rebate Calculation Rule is 78-2.)',
		  'CL_EarlySettPenaltyPeriod'
	from country c
END


