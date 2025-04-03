SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetRelatedLineItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetRelatedLineItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_GetRelatedLineItemsSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_GetRelatedLineItemsSP
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve assocaited items for an item
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 24/02/11  ip  Sprint 5.10 - #3133 - Return a flag to identify an item as an Installation item.
-- 15/06/11 jec  CR1212 - use new Associated items table
-- 21/06/11 jec  #4053 add branch parameter
-- 07/07/11 ip   RI - #4037 - Return quantity available for associated item to determine if authorisation is required
--				 in New Sales Order screen for associated items that are out of stock.
-- ================================================
			@itemId int,
			@branchno SMALLINT,
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	
	declare @hierarchy TABLE (ProductGroup VARCHAR(3),Category INT,Class VARCHAR(5),SubClass VARCHAR(5))
	-- get heirarchy of selected item
	Insert into @hierarchy
	select case when c.Category = 'PCF'  then 'PCF'	
				ELSE 'PCE'
				end as ProductGroup
		,s.Category,ISNULL(Class,''),ISNULL(SubClass,'')
	from StockInfo s 
	LEFT OUTER JOIN code c 
		on CAST(c.code as INT) = s.category 
		and c.category in ('PCE','PCF','PCW','PCO')		
	where s.ID=@itemid

	-- Get related products
	Select associated.IUPC as 'ProductCode',
			itemdescr1 + ' 	'+ itemdescr2 as 'ProductDescription1',
			MAX(CreditPrice) as 'CreditPrice',
			MAX(CashPrice) as 'CashPrice',
			case 
				when c.code is not null then 1 
				else 0 
			end as Installation,	
			associated.ID as ItemId,
			q.qtyAvailable	as 'QtyAvailable'											--IP - 07/07/11 - RI - #4037
	From @hierarchy product
	 INNER JOIN Code 
		on code.code = product.category 
		and code.category in ('PCE', 'PCF', 'PCW', 'PCO')
	 INNER JOIN StockInfoAssociated a
		on (product.ProductGroup=a.ProductGroup or a.ProductGroup='Any')
		and (product.Category=a.Category  or a.Category =0) 
		and (product.Class=a.Class or a.Class='Any')
		and (product.SubClass=a.SubClass or a.SubClass='Any')
	INNER JOIN StockInfo associated 
		on a.AssocItemId=associated.id
	INNER JOIN Stockprice p 
		on associated.id=p.ID 
		and p.branchno=@branchno
	INNER JOIN StockQuantity q 
		on associated.id=q.id 
		and q.stocklocn=@branchno
	LEFT OUTER JOIN code c 
		on c.code = associated.IUPC	
		and c.category = 'INST'
	Where  associated.prodstatus != 'D'
		and ((associated.itemtype='S') OR p.CashPrice = 0 OR associated.itemtype='N')
		AND   associated.id != @itemid -- exclude original item number!!
	GROUP BY associated.IUPC
	,associated.itemdescr1
	, associated.itemdescr2
	, c.code
	, associated.ID
	, q.qtyAvailable		

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End
