IF OBJECT_ID('CacheSets') IS NOT NULL
BEGIN
DROP TRIGGER CacheSets
END
GO

CREATE TRIGGER CacheSets
   ON  dbo.sets 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetSetsForTNameBranch'
END
GO

IF OBJECT_ID('CacheSetByBranch') IS NOT NULL
BEGIN
DROP TRIGGER CacheSetByBranch
END
GO

CREATE TRIGGER CacheSetByBranch
   ON  dbo.SetByBranch 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetSetsForTNameBranch'
END
GO


IF OBJECT_ID('Admin.CacheAdminUser') IS NOT NULL
BEGIN
DROP TRIGGER Admin.CacheAdminUser
END
GO

CREATE TRIGGER CacheAdminUser
   ON  [Admin].[User] 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetDynamicMenus'
END
GO


IF OBJECT_ID('Admin.CacheAdminRolePermission') IS NOT NULL
BEGIN
DROP TRIGGER Admin.CacheAdminRolePermission
END
GO

CREATE TRIGGER [Admin].CacheAdminRolePermission
   ON  [Admin].RolePermission 
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetDynamicMenus'
END
GO

IF OBJECT_ID('Admin.CacheAdminUserRole') IS NOT NULL
BEGIN
DROP TRIGGER Admin.CacheAdminUserRole
END
GO

CREATE TRIGGER [Admin].CacheAdminUserRole
   ON  [Admin].UserRole
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetDynamicMenus'
END
GO

IF OBJECT_ID('CacheTermsTypeTable') IS NOT NULL
BEGIN
DROP TRIGGER CacheTermsTypeTable
END
GO

CREATE TRIGGER CacheTermsTypeTable
   ON  TermsTypeTable
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'TermsType'
END
GO


IF OBJECT_ID('CacheIntrateHistory') IS NOT NULL
BEGIN
DROP TRIGGER CacheIntrateHistory
END
GO

CREATE TRIGGER CacheIntrateHistory
   ON  IntrateHistory
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'TermsType'
END
GO

IF OBJECT_ID('CacheTermsTypeBandList') IS NOT NULL
BEGIN
DROP TRIGGER CacheTermsTypeBandList
END
GO

CREATE TRIGGER CacheTermsTypeBandList
   ON  TermsTypeBand
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'TermsTypeBandList'
END
GO

IF OBJECT_ID('CacheTermsTypeBand') IS NOT NULL
BEGIN
DROP TRIGGER CacheTermsTypeBand
END
GO

CREATE TRIGGER CacheTermsTypeBand
   ON  TermsTypeBand
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'TermsTypeBand'
END
GO

IF OBJECT_ID('CacheCodeSOA') IS NOT NULL
BEGIN
DROP TRIGGER CacheCodeSOA
END
GO

CREATE TRIGGER CacheCodeSOA
   ON  Code
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName IN ('SourceOfAttraction','MethodOfPayment','CustomerCodes','AccountCodes','UserTypes','AddressType','ApplicationType','ProductCategories','NonDeposits','WriteOffCodes','CountryParameterCategories','InsuranceTypes','CashLoanDisbursementMethods')
END
GO


IF OBJECT_ID('Admin.CacheAdminUserStaffSummary') IS NOT NULL
BEGIN
DROP TRIGGER [Admin].CacheAdminUserStaffSummary
END
GO

CREATE TRIGGER CacheAdminUserStaffSummary
   ON  [Admin].[User]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName IN ('SalesStaff','AllStaff','SalesCommStaff')
END
GO


IF OBJECT_ID('Admin.CacheAdminUserRoleStaffSummary') IS NOT NULL
BEGIN
DROP TRIGGER Admin.CacheAdminUserRoleStaffSummary
END
GO

CREATE TRIGGER Admin.CacheAdminUserRoleStaffSummary
   ON  [Admin].[UserRole]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName IN ('SalesStaff','AllStaff')
END
GO

IF OBJECT_ID('CacheCommStaff') IS NOT NULL
BEGIN
DROP TRIGGER CacheCommStaff
END
GO

CREATE TRIGGER CacheCommStaff
   ON  salesCommission
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName IN ('CommStaff','SalesCommStaff')
END
GO

IF OBJECT_ID('CacheAcctnoAcctype') IS NOT NULL
BEGIN
DROP TRIGGER CacheAcctnoAcctype
END
GO

CREATE TRIGGER CacheAcctnoAcctype
   ON  acctnoctrl
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'AccountType'
END
GO


IF OBJECT_ID('CacheBranchAccountType') IS NOT NULL
BEGIN
DROP TRIGGER CacheBranchAccountType
END
GO

IF OBJECT_ID('CacheAccttypeAccountType') IS NOT NULL
BEGIN
DROP TRIGGER CacheAccttypeAccountType
END
GO
IF OBJECT_ID('Admin.CachePermissionUserFunctions') IS NOT NULL
BEGIN
DROP TRIGGER [Admin].CachePermissionUserFunctions
END
GO

CREATE TRIGGER [Admin].CachePermissionUserFunctions
   ON  [Admin].[permission]
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'UserFunctions'
END
GO

IF OBJECT_ID('CacheBankGetBankCodes') IS NOT NULL
BEGIN
DROP TRIGGER CacheBankGetBankCodes
END
GO

CREATE TRIGGER CacheBankGetBankCodes
   ON  Bank
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'Bank'
END
GO

IF OBJECT_ID('CacheDDDueDay') IS NOT NULL
BEGIN
DROP TRIGGER CacheDDDueDay
END
GO

CREATE TRIGGER CacheDDDueDay
   ON  DDDueDay
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'DDDueDate'
END
GO

IF OBJECT_ID('CacheEndPeriods') IS NOT NULL
BEGIN
DROP TRIGGER CacheEndPeriods
END
GO

CREATE TRIGGER CacheEndPeriods
   ON  RebateForecast_PeriodEndDates
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'EndPeriods'
END
GO

IF OBJECT_ID('CacheEodConfiguration') IS NOT NULL
BEGIN
DROP TRIGGER CacheEodConfiguration
END
GO

CREATE TRIGGER CacheEodConfiguration
   ON  EodConfiguration
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'EODConfigurations'
END
GO


IF OBJECT_ID('CacheStockInfo') IS NOT NULL
BEGIN
DROP TRIGGER CacheStockInfo
END
GO

CREATE TRIGGER CacheStockInfo
   ON  StockInfo
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'InstallationItemCat'
END
GO


IF OBJECT_ID('CacheCountryMaintenance') IS NOT NULL
BEGIN
DROP TRIGGER CacheCountryMaintenance
END
GO

CREATE TRIGGER CacheCountryMaintenance
   ON  CountryMaintenance
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	UPDATE CacheTableChange
	SET ChangedOn = GETDATE()
	WHERE TableName = 'GetCountryMaintenanceParameters'
END
GO
