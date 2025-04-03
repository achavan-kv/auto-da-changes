-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.Call 
	ADD MobileNumber varchar(26) NULL,
	MobileExtension varchar(6) SPARSE  NULL,
	MobileDialCode varchar(8) SPARSE  NULL,
	LandLinePhone varchar(26) NULL,
	LandLineExtension varchar(6) SPARSE  NULL,
	LandLineDialCode varchar(8) SPARSE  NULL
GO

ALTER TABLE SalesManagement.Call
	DROP COLUMN Phone
GO
