SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LoadCollectionDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LoadCollectionDetailsSP]
GO


CREATE PROCEDURE 	dbo.DN_LoadCollectionDetailsSP
			@stocklocn smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	A.AcctNo,
			A.AgrmtNo,
			A.ParentItemNo,
			B.IUPC as ItemNo,
			C.Quantity,
			A.DelQty,
			A.StockLocn,
			C.RetStockLocn,
			A.Price,
			A.OrdVal,
			B.ItemDescr1,
			CAST(c.CreatedBy as VARCHAR(5)) + ' ' + u.FullName as 'GRT Created By', --IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			c.datecreated as 'GRT Created On', --IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			B.Itemdescr2,
			B.SupplierCode,
			B.Supplier,		-- IP - 09/02/10 - CR1048 (Ref:3.1.2) Merged - Malaysia Enhancements (CR1072)
			A.DatePlanDel,
			C.DelorColl,
			CA.CustID,
			C.RetItemNo,
			C.RetVal,
			AG.EmpeeNoSale,
			A.Notes,
			C.GRTnotes, -- UAT 158 merge.
			C.BuffNo,
			A.DeliveryAddress,
			A.ItemType
	FROM    LINEITEM A, STOCKITEM B, SCHEDULE C 
	LEFT outer JOIN Admin.[User] u ON u.id = c.createdBy--IP - 10/02/10 - CR1048 (Ref:3.1.4 & 3.1.5) Merged - Malaysia Enhancements (CR1072)
			, CUSTACCT CA, AGREEMENT AG
	WHERE   A.ItemID = B.ItemID
	AND     A.ItemID = C.ItemID
	AND     A.AcctNo = C.AcctNo
	AND     A.AgrmtNo = C.AgrmtNo
	AND     A.StockLocn = C.StockLocn
	AND     A.StockLocn = B.StockLocn
	AND		A.AcctNo = CA.AcctNo
	AND		A.AcctNo = AG.AcctNo
	AND     (C.DateDelPlan = '' OR C.DateDelPlan is null)
	AND     C.Quantity < 0
	AND     C.RetStockLocn = @stocklocn
	AND     A.Iskit=0
	AND 	CA.hldorjnt = 'H'
	--AND		B.category NOT IN(12,82,36,37,38, 39, 46,47,48,86,87,88)
	AND		B.category NOT IN(select distinct code from code where category in ('WAR', 'PCDIS')) --IP - 29/10/09 - CoSACS Improvement - Remove hardcoded discounts & warranties
	AND		C.dateprinted IS NULL
	ORDER BY A.AcctNo ASC

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

