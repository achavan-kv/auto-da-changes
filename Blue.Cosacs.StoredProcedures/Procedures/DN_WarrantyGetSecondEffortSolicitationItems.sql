
/****** Object:  StoredProcedure [dbo].[DN_WarrantyGetSecondEffortSolicitationItems]    Script Date: 11/06/2006 13:32:57 ******/
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[DN_WarrantyGetSecondEffortSolicitationItems]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_WarrantyGetSecondEffortSolicitationItems]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Peter Chong
-- Create date: 06-Nov-2006
-- Description:	List all warrantable items for a customer where a warranty has not been purchased. (Second Effort Solicitation) 

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 02/08/11  IP  RI - System Integration changes
-- ================================================
-- =============================================
CREATE PROCEDURE DN_WarrantyGetSecondEffortSolicitationItems 
(	
	@custId varchar(20) ,
	@numberofPrompts			int = 0, -- How many times second effort solicitation to be shown
	@warrantyAfterDeliveryDays	int = 0, -- How many days can have passed and still purchase a warranty
	@return int output
)
AS
BEGIN
	SET NOCOUNT ON;
		
	SELECT A.Acctno,
		--, SI.ItemNo
		SI.IUPC as ItemNo																						--IP - 02/08/11 - RI 	
		, SI.itemdescr1 + ' ' + SI.itemdescr2 [itemdescr1]
		--, (SELECT Isnull(Max(right(code,1)), '0') FROM acctcode WHERE AcctNo = A.AcctNo AND reference = LI.ItemNo AND LEFT(Code,3) = 'SSP') [NoOfPrompts]
		, (SELECT Isnull(Max(right(code,1)), '0') FROM acctcode WHERE AcctNo = A.AcctNo AND reference = SI.IUPC AND LEFT(Code,3) = 'SSP') [NoOfPrompts]			--IP - 02/08/11 - RI
	FROM 
		acct A JOIN 
		custacct CA ON A.AcctNo = CA.AcctNo		JOIN
		delivery D ON A.AcctNo = D.AcctNo		JOIN 
		--stockItem SI ON SI.ItemNo = D.ItemNo AND SI.stocklocn = D.stocklocn	JOIN
		stockItem SI ON SI.ID = D.ItemID AND SI.stocklocn = D.stocklocn	JOIN									--IP - 02/08/11 - RI
		--lineItem LI ON SI.ItemNo = LI.ItemNo AND SI.Stocklocn = LI.stocklocn AND LI.AcctNo = A.AcctNo 
		lineItem LI ON SI.ID = LI.ItemID AND SI.Stocklocn = LI.stocklocn AND LI.AcctNo = A.AcctNo				--IP - 02/08/11 - RI
		
	WHERE 
		CA.CustID = @custId AND
		Datediff(d, D.Datedel, getdate()) < @warrantyAfterDeliveryDays AND --must be this less than this many days after product delivered
		A.AcctType IN ('C', 'O', 'R') AND 
		--SI.Category NOT IN (12, 82) AND
		SI.Category NOT IN (select code from code where category = 'WAR') AND									--IP - 02/08/11 - RI
		CA.hldorjnt = 'H'	AND
		D.delorcoll = 'D'	AND 
		ISNULL(warrantable, 0) = 1 AND
		SI.RefCode <> 'ZZ' AND
		LI.Quantity > 0 AND
		NOT EXISTS (
			SELECT * 
			FROM LineItem 
			WHERE AcctNo = A.AcctNo AND 
				--ParentItemNo = LI.ItemNO AND 
				ParentItemID = LI.ItemID AND																	--IP - 02/08/11 - RI
				ParentLocation = LI.stockLocn AND
				Agrmtno = LI.Agrmtno AND 
				--IP - 08/05/08 - UAT(426) v 5.1 - prompt would not appear when an account was
				--revised, warranty added, then removed and account saved. This was due to an 
				--entry being inserted into 'Lineitem' table for the warranty.
				Quantity > 0 AND 
				Ordval > 0 
		) AND
		NOT EXISTS (					--Check for products that have been collected
			SELECT * 
			FROM Delivery D1 JOIN 
			Delivery D2 ON D1.AcctNo = D2.AcctNo 
				--AND D1.ItemNo = D2.ItemNo 
				AND D1.ItemID = D2.ItemID																		--IP - 02/08/11 - RI 
				AND D1.StockLocn = D2.StockLocn
				AND D1.DelorColl = 'D' AND D2.DelorColl = 'R'
				AND D1.DateDel >= D2.DateDel
			WHERE D1.AcctNo = A.AcctNo AND 
				--D1.ItemNo = LI.ItemNo AND
				D1.ItemID = LI.ItemID AND																		--IP - 02/08/11 - RI
				D1.StockLocn = LI.StockLocn  
		) AND 
		NOT EXISTS (
			SELECT * 
			FROM acctcode
			--WHERE reference = LI.ItemNo AND 
			WHERE reference = SI.iupc AND																		--IP - 02/08/11 - RI
				AcctNo = A.AcctNo AND 
				code = 'SSP' + convert(varchar(2), @numberofPrompts)
		)

	SET @return = @@error
END

GO

