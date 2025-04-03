SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS ON
go

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[RIPurchaseOrderLoadSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[RIPurchaseOrderLoadSP]
GO

CREATE PROCEDURE RIPurchaseOrderLoadSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RIPurchaseOrderLoadSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : RI Interface - Purchase Order Data Load
-- Date         : 15 March 2010
--
-- This procedure will load the Purchase Order detail from the RI interface file.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/06/11  jec Change date format to 110 (caters for yyyymmdd and yyyy-mm-dd) 
-- 09/09/11  ip  Sprint 8 - RI - #8135 - StockOnOrder was being updated for all branches for an item, needed to also join on StockLocn
-- 16/09/11  ip  #8206 - UAT47 - Add label POD File to the beginning of error text
-- ================================================
	-- Add the parameters for the stored procedure here
		@interface varchar(10),
		@runno int,
		@rerun BIT,
        @return int OUTPUT
AS
BEGIN
    SET 	@return = 0			--initialise return code
    DECLARE @statement SQLText
    
	UPDATE PurchaseOrderOutstanding
	SET 
		warehousenumber = b.warehouseno,
		supplierno = VendorNumber,	
		expectedreceiptdate = CONVERT(DATETIME,POReceiptDate,110),
		quantityonorder = POOrderQty,
		quantityavailable = POOrderQty
	From PurchaseOrderOutstanding po 
	INNER JOIN  RItemp_RawPOload r on po.itemno = ItemIUPC and po.stocklocn = r.StockLocn and supplierno = VendorNumber
	INNER JOIN Branch b on b.BranchNo = r.StockLocn

	--IF @@ROWCOUNT = 0
	
    INSERT INTO PurchaseOrderOutstanding
    ( 
		warehousenumber, itemno, stocklocn,
		supplierno, purchaseordernumber, expectedreceiptdate,
		quantityonorder, quantityavailable,ItemID		
	)	
	Select b.WarehouseNo,ItemIUPC,r.StockLocn,VendorNumber,'NotAvail',
			CONVERT(DATETIME,POReceiptDate,110),POOrderQty,POOrderQty,s.ID
	
	From RItemp_RawPOload r INNER JOIN Branch b on b.BranchNo = r.StockLocn
							INNER JOIN StockInfo s on r.ItemIUPC = s.IUPC AND RepossessedItem = 0
							INNER JOIN StockQuantity q on q.StockLocn = r.StockLocn and s.id = q.id
	Where NOT EXISTS (select * from PurchaseOrderOutstanding po2 where itemno = r.ItemIUPC and 
						po2.stocklocn = r.StockLocn) --and supplierno = VendorNumber
					--INNER JOIN Branch b on b.BranchNo=r2.StockLocn)
	
	UPDATE s
	SET stockonorder = POO.quantityavailable
	FROM StockQuantity s
	INNER JOIN stockinfo ON s.ID = StockInfo.ID
	INNER JOIN PurchaseOrderOutstanding POO ON POO.ItemID = StockInfo.ID
	AND s.stocklocn = POO.StockLocn												--IP - 09/09/11 - RI - #8135
	
	-- Flag errors
	INSERT INTO Interfaceerror(interface, runno,errordate,severity,errortext)   
	select @interface, @runno, getdate(),'W',
			'POD file: Item: ' + ItemIUPC + ' , StockLocn:' + CAST(r.StockLocn as VARCHAR(3)) +			--IP - 16/09/11 - #8206 - UAT47
			' , Quantity: ' + CAST(POOrderQty as VARCHAR(5)) + ' - Item/StockLocn not found in CoSACS stock.'
	from RItemp_RawPOload r
	where Not exists (select * from stockitem s where r.ItemIUPC=s.IUPC and r.StockLocn=s.Stocklocn) 
			
    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
SET ANSI_WARNINGS Off
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
