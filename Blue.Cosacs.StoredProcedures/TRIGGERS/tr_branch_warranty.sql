
-- **********************************************************************
-- Title: tr_branch_warranty
-- Developer: Pa Njie
-- Date: 27 Aug 2003
-- Purpose: Create trigger tr_branch_warranty to add a row in branch warranty

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 12/01/10  ip  UAT(966) - Added missing columns when inserting into StockQuantity table.
-- 25/07/11  ip  RI System Integration changes.
--***********************************************************************

IF EXISTS(SELECT name FROM sysobjects
	  WHERE name = 'tr_branch_warranty' 
  	  AND type = 'TR')

DROP TRIGGER tr_branch_warranty
go

CREATE TRIGGER tr_branch_warranty
ON branch
FOR INSERT
AS 
	DECLARE @branchno integer,
			@itemID integer				--IP - 25/07/11 - RI

	SELECT @branchno = branchno 
	FROM inserted

	INSERT INTO branchwarranty
	(
	    branchno,
	    warrantyno
	)
	VALUES
	(
	    @branchno,
	    0
	)

--IP - 25/07/11 - RI - select the itemID of the ADDDR and ADDCR from stockinfo
set @itemID = (select ID from stockinfo where iupc = 'ADDCR' and repossesseditem = 0)
/* 68191 KEF Also need to add adddr and addcr items for new branches as these aren't in FACT2000 */
    insert into stockquantity
           (itemno, stocklocn,qtyavailable, stock,  stockonorder, stockdamage,
           --leadtime, dateupdated, deleted, LastOperationSource) --IP - 12/01/10 - UAT(966) - Added deleted, LastOperationSource
            leadtime, dateupdated, deleted, LastOperationSource, ID) --IP - 25/07/11 - RI --IP - 12/01/10 - UAT(966) - Added deleted, LastOperationSource
    select  'ADDCR', @branchno, 0.0,
           --0.0, 0.0, 0.0,  0.0, getdate(), 'N', ''
             0.0, 0.0, 0.0,  0.0, getdate(), 'N', '', @itemID	--IP - 25/07/11 - RI	
    
set @itemID = (select ID from stockinfo where iupc = 'ADDDR' and repossesseditem = 0)
       
               insert into stockquantity
           (itemno, stocklocn,qtyavailable, stock,  stockonorder, stockdamage,
           --leadtime, dateupdated, deleted, LastOperationSource)
           leadtime, dateupdated, deleted, LastOperationSource, ID)--IP - 25/07/11 - RI	
    select  'ADDDR', @branchno, 0.0,
           --0.0, 0.0, 0.0, 0.0, getdate(), 'N', ''
           0.0, 0.0, 0.0, 0.0, getdate(), 'N', '', @itemID			--IP - 25/07/11 - RI


GO