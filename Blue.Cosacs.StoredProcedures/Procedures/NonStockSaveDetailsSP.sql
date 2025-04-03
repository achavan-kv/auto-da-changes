SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE id = object_id(N'[dbo].NonStockSaveDetailsSP') and OBJECTPROPERTY(id, N'IsProcedure') = 1)

BEGIN
    DROP PROCEDURE NonStockSaveDetailsSP
END
GO

CREATE PROCEDURE dbo.NonStockSaveDetailsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : NonStockSaveDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Non Stock Save Item details
-- Author       : John Croft
-- Date         : 10 December 2010
--
-- This procedure will get the Non Stock Item details
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/05/11  jec CR1212 RI Integration 
-- ================================================
	-- Add the parameters for the stored procedure here
	@itemNo VARCHAR(8),			-- RI
	@itemdescr1 VARCHAR(25),
	@itemdescr2 VARCHAR(40),
	@suppliername VARCHAR(40),
	@suppliercode VARCHAR(18),
	@category INT,
	@taxrate FLOAT,	
	@deleted CHAR(1),	
	@endDate DATETIME,
	@itemId INT,			-- RI
	@return INT output

As
	set @return=0
	-- Update basic stock Info
	if exists (select * from stockInfo where itemno=@itemNo)
	Begin
	-- item exists
	UPDATE StockInfo
		set itemdescr1=@itemdescr1, itemdescr2=@itemdescr2, Supplier=@suppliername,suppliercode=@suppliercode,
			category=@category, taxrate=@taxrate, prodstatus=case when @deleted='Y' then 'D' else prodstatus end
				Where ID=@itemID		-- RI	itemno=@itemNo
		and (itemdescr1!=@itemdescr1 or itemdescr2!=@itemdescr2 or Supplier!=@suppliername
			or suppliercode!=@suppliercode or category!=@category or taxrate!=@taxrate
			or prodstatus!=case when @deleted='Y' then 'D' else '' end)
			
	End
	else
	Begin
        declare @maxid int

        select @maxid = Max(id) from stockinfo
        where id < 100000

		-- new item
		insert into StockInfo (id,itemno, itemdescr1, itemdescr2, category, Supplier, prodstatus,
				 suppliercode, warrantable, itemtype, warrantyrenewalflag, leadtime,
				 assemblyrequired, taxrate,SKU,IUPC			-- RI
				 )			
		
		Values (@maxid + 1, @itemNo, @itemdescr1, @itemdescr2, @category, @suppliername, case when @deleted='Y' then 'D' else 'L' end,
				 @suppliercode, 0, 'N', 'N', 0,
				 0, @taxrate, @itemNo, @itemNo				-- RI
				 )
		
	End
	
	set @return=@@ERROR
	-- Update Deleted
	if @return=0
	Begin
		
		if exists (select top 1 itemno from StockQuantity where ID=@itemID)			-- RI itemno=@itemNo)	
		BEGIN
			-- item exists 
			UPDATE stockQuantity
				set deleted = @deleted		
			Where ID=@itemID			-- RI itemno=@itemNo
				and deleted!=@deleted 		
			
		END	
		else
		Begin
			
			Insert into StockQuantity (itemno, stocklocn, qtyAvailable, stock, stockonorder,
				 stockdamage, leadtime, dateupdated, deleted, LastOperationSource,ID)
			Select '', BranchNo, 0, 0, 0,					-- RI itemno not required
				 0, 60, GETDATE(), @deleted, '',s.ID			-- RI
			From Branch, 
					StockInfo s			-- RI
			Where s.IUPC=@itemNo		-- RI
			
		End
		
	End
	
	set @return=@@ERROR
	-- Update Deletion dates
	if @return=0
	Begin
		
		if exists (select top 1 itemno from NonStockDeletionDates where itemno=@itemNo)	
		BEGIN
			-- item exists 
			UPDATE NonStockDeletionDates
				set DeletionDate = @EndDate		
			Where itemno=@itemNo
				and DeletionDate != @EndDate 		
			
		END	
		else
		Begin	
				
			Insert into NonStockDeletionDates (Branchno,itemno, DeletionDate)
			Select 0,@itemno, @EndDate
			Where @Enddate !='1900-01-01'
		End
		
	End
go	
-- end end end end end end end end end end end end end end end end end end end end end end end 