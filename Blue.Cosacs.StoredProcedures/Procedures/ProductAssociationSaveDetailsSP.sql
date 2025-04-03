IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductAssociationSaveDetailsSP]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ProductAssociationSaveDetailsSP]
GO

SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER OFF
GO


CREATE PROCEDURE [dbo].[ProductAssociationSaveDetailsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : ProductAssociationSaveDetailsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save Product Associations
-- Date         : 09 June 2011
--
-- This procedure will save the  Product Associations
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
-- ================================================
			@productGroup VARCHAR(3),
			@category INT,
			@class VARCHAR(3),
			@subclass VARCHAR(5),
			@itemid INT,
			@delete CHAR(1),
			@return INT out
As	
	set @return=0
	
	UPDATE StockInfoAssociated
		set AssocItemId=@itemid
	Where ProductGroup=@productGroup and Category=@category and Class=@class and SubClass=@subclass and AssocItemId=@itemid
		and @delete='N'
	
	if @@ROWCOUNT=0
		if @delete='Y'
			delete StockInfoAssociated
			Where ProductGroup=@productGroup and Category=@category and Class=@class and SubClass=@subclass and AssocItemId=@itemid
		else
		
			Insert into StockInfoAssociated (ProductGroup, Category, Class, SubClass, AssocItemId)
			select @productGroup,@category,@class,@subclass,@itemid
	
	set @return=@@ERROR

go
	
-- End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End  End 