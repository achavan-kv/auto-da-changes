-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

sp_RENAME 'Sales.[order].[IsDutyFree]' , 'IsDutyFreeSale', 'COLUMN'
go

sp_RENAME 'Sales.[order].[IsTaxFree]' , 'IsTaxFreeSale', 'COLUMN'
go