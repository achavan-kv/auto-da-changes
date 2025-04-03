-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE NONCLUSTERED INDEX IX_WarrantyPromotion_WarrantyId_BranchNumber_BranchType ON Warranty.WarrantyPromotion
(
	WarrantyId DESC,
	BranchNumber,
	BranchType
) 
WITH
( 
	PAD_INDEX = ON, 
	FILLFACTOR = 70, 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) 
