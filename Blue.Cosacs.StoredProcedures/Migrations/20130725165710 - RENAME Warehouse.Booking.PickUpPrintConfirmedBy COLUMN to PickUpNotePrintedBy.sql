-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
sp_RENAME '[Warehouse].[Booking].PickUpPrintConfirmedBy', 'PickUpNotePrintedBy ' , 'COLUMN'
GO
