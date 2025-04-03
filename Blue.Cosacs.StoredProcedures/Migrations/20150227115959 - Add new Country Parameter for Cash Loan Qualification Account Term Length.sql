-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @parameterCat int

set @parameterCat = (select ParameterCategory from CountryMaintenance where codename = 'CL_MaxLoanAmount')

IF NOT EXISTS(select * from CountryMaintenance where CodeName = 'CL_SettledMinTermLength')
BEGIN
    insert into 
        CountryMaintenance
    select 
        c.countrycode, @parameterCat, 'Minimum settled account Term Length', 3, 'numeric', 0, '', '', 'For Existing Customers, settled accounts must have a Term Length greater than or equal to the value set in the parameter', 'CL_SettledMinTermLength'
    from 
        country c
END
GO