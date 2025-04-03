
IF OBJECT_ID('CacheTableChange') IS NOT NULL
BEGIN
	DROP TABLE CacheTableChange
END
GO

CREATE TABLE [CacheTableChange](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [varchar](50) NOT NULL,
	[ChangedOn] [datetime] NOT NULL,
 CONSTRAINT [PK_TableChange] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)




INSERT INTO [dbo].[CacheTableChange]
(TableName, ChangedOn)
SELECT 'GetSetsForTNameBranch', GETDATE() UNION
SELECT 'GetDynamicMenus', GETDATE() UNION
SELECT 'TermsType', GETDATE() UNION
SELECT 'TermsTypeBandList', GETDATE() UNION
SELECT 'TermsTypeBand', GETDATE() UNION
SELECT 'SourceOfAttraction', GETDATE() UNION
SELECT 'SalesStaff', GETDATE() UNION
SELECT 'AllStaff', GETDATE() UNION
SELECT 'CommStaff', GETDATE() UNION
SELECT 'SalesCommStaff', GETDATE() UNION
SELECT 'MethodOfPayment', GETDATE() UNION
SELECT 'AccountType', GETDATE() UNION
SELECT 'BranchNumber', GETDATE() UNION
SELECT 'CustomerCodes', GETDATE() UNION
SELECT 'AccountCodes', GETDATE() UNION
SELECT 'UserTypes', GETDATE() UNION
SELECT 'UserFunctions', GETDATE() UNION
SELECT 'AddressType', GETDATE() UNION
SELECT 'Bank', GETDATE() UNION
SELECT 'ApplicationType', GETDATE() UNION
SELECT 'DDDueDate', GETDATE() UNION
SELECT 'ProductCategories', GETDATE() UNION
SELECT 'Deposits', GETDATE() UNION
SELECT 'NonDeposits', GETDATE() UNION
SELECT 'GeneralTransactions', GETDATE() UNION
SELECT 'WriteOffCodes', GETDATE() UNION
SELECT 'EndPeriods', GETDATE() UNION
SELECT 'CountryParameterCategories', GETDATE() UNION
SELECT 'EODConfigurations', GETDATE() UNION
SELECT 'InsuranceTypes', GETDATE() UNION
SELECT 'InstallationItemCat', GETDATE() UNION
SELECT 'CashLoanDisbursementMethods', GETDATE() UNION
SELECT 'GetCountryMaintenanceParameters', GETDATE() 
