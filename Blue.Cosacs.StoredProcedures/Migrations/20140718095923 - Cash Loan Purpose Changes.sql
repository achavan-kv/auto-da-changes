-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from codecat where category = 'CLP')
BEGIN
	insert into codecat (origbr, category, catdescript, codelgth, forcenum, forcenumdesc, usermaint, CodeHeaderText, DescriptionHeaderText, SortOrderHeaderText, ReferenceHeaderText, AdditionalHeaderText, ToolTipText, Additional2HeaderText)
	select 0, 'CLP', 'Cash Loan Purpose', 5, 'N', 'N', 'Y', NULL, 'Description', NULL, NULL, NULL, 'Cash Loan Purpose', NULL
END


declare @parameterCat int

set @parameterCat = (select ParameterCategory from CountryMaintenance where codename = 'CL_MaxLoanAmount')

IF NOT EXISTS(select * from countrymaintenance where codename = 'CL_EnablePurposeDropDown')
BEGIN
	
	insert into countrymaintenance(CountryCode, ParameterCategory, Name, Value, Type, Precision, OptionCategory, OptionListName, Description, CodeName)
	select c.CountryCode, @parameterCat, 'Enable Cash Loan Purpose drop down', 'True', 'checkbox', 0, '', '', 'Enable / Disable Cash Loan Purpose drop down on the Cash Loan Application Screen', 'CL_EnablePurposeDropDown'
	from country c
END


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'CashLoanPurpose')
BEGIN
	ALTER TABLE CashLoan Add CashLoanPurpose varchar(25) null
END

