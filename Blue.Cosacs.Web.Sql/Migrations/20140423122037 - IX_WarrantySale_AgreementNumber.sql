-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE NONCLUSTERED INDEX IX_WarrantySale_AgreementNumber ON Warranty.WarrantySale
(
	AgreementNumber DESC
) WITH
( 
	PAD_INDEX = OFF, 
	FILLFACTOR = 20, 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
GO
