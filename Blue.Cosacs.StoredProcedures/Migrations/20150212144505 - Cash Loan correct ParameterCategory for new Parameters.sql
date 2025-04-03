-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @parameterCat int

set @parameterCat = (select ParameterCategory from CountryMaintenance where codename = 'CL_MaxLoanAmount')

UPDATE 
    CountryMaintenance
SET
    ParameterCategory = @parameterCat
WHERE
    CodeName in ('CL_NewCustMaxLoanAmount', 'CL_RecentCustMaxLoanAmount', 'CL_StaffCustMaxLoanAmount', 'CL_RecentCustAccountLength', 'CL_HighSettStatRecentCust',
                    'CL_RecentCustMaxArrearsLevel')