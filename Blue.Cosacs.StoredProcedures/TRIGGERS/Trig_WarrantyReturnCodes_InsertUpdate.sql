-- Trigger for Audit of Stockinfo table

IF EXISTS (SELECT * FROM sysobjects WHERE NAME= 'Trig_WarrantyReturnCodes_InsertUpdate')
DROP TRIGGER Trig_WarrantyReturnCodes_InsertUpdate
GO 

CREATE Trigger [dbo].[Trig_WarrantyReturnCodes_InsertUpdate] ON [dbo].[WarrantyReturnCodes]
For UPDATE, INSERT, Delete

AS

	INSERT INTO WarrantyReturnCodesAudit(ProductType, MonthSinceDelivery, ReturnCode, refundpercentfromAIG, 
			WarrantyMonths, ManufacturerMonths, DateChange, Deleted)
	SELECT ProductType, MonthSinceDelivery, MAX(ReturnCode),	MAX(refundpercentfromAIG), 
			WarrantyMonths, ManufacturerMonths, DateChange, ''
	From INSERTED I
	WHERE NOT EXISTS (SELECT * FROM WarrantyReturnCodesAudit W 
	WHERE w.ProductType = i.ProductType AND  w.MonthSinceDelivery = i.MonthSinceDelivery 
	AND w.WarrantyMonths = i.WarrantyMonths AND w.ManufacturerMonths= i.ManufacturerMonths
	AND w.DateChange = i.DateChange)
	GROUP BY ProductType, MonthSinceDelivery, WarrantyMonths, ManufacturerMonths, DateChange
	
	INSERT INTO WarrantyReturnCodesAudit(ProductType, MonthSinceDelivery, ReturnCode, refundpercentfromAIG, 
			WarrantyMonths, ManufacturerMonths, DateChange, Deleted)
	Select DISTINCT ProductType, MonthSinceDelivery, MAX(ReturnCode),	MAX(refundpercentfromAIG), 
			WarrantyMonths, ManufacturerMonths, GETDATE(), 'D'
	From Deleted D
	-- inhibit deletion audit for updates   jec 04/01/11
	where not exists(select * from WarrantyReturnCodes w
					where w.ProductType=d.productType 
				and w.WarrantyMonths=d.WarrantyMonths
				and w.ManufacturerMonths=d.ManufacturerMonths		
				and w.MonthSinceDelivery=d.MonthSinceDelivery)
   GROUP BY ProductType, MonthSinceDelivery, WarrantyMonths, ManufacturerMonths
GO 	 
-- End End End End End End End End End End End End End End End End End End End End End End End End