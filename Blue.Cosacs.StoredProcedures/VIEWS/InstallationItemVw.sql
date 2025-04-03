SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[InstallationItemVw]'))
	DROP VIEW [dbo].[InstallationItemVw]
GO

CREATE VIEW [dbo].[InstallationItemVw] AS ( 
    SELECT 
		LI.acctno AS AcctNo, LI.agrmtno AS AgreementNo, 
		LI.ItemID AS ItemId, SI.IUPC AS ItemNo, SI.itemno AS CourtsCode,
		SI.itemdescr1 AS ProductDescription1, SI.itemdescr2 AS ProductDescription2,
		LI_INST.ItemID AS InstItemId, SI_INST.IUPC AS InstItemNo, SI_INST.itemno AS InstCourtsCode,
		ISNULL(LI_INST.OrdVal,0) AS InstValue, LI.quantity AS Quantity, 
		LI.delqty AS DelQty, LI.stocklocn AS StockLocation,
		LI.datereqdel AS DateReqDel, LI.dateplandel AS DatePlanDel, LI.delnotebranch AS DelNoteBranch,
		LI.deliveryaddress AS DelAddressType,LI.deliveryprocess AS DelProcess , 
		LI.ParentItemID AS ParentItemId, LI.parentlocation AS ParentLocation,			
		SI.Supplier, SI.suppliercode AS SupplierCode,
		StockCode.category AS StockCategory
		--INST.InstallationNo as InstNo,
		--INST.InstallationDate AS InstDate,
		--ISNULL(INST.[Status], 'Unknown') AS InstallationStatus  -- Same as Blue.Cosacs.Shared.InstallationStatus enum
	FROM dbo.lineitem LI_INST  --Installation Item
    INNER JOIN dbo.lineitem LI ON LI_INST.ParentItemID = LI.ItemID AND LI_INST.parentlocation = LI.stocklocn AND 
									LI_INST.acctno = LI.acctno AND LI_INST.agrmtno = LI.agrmtno
    INNER JOIN dbo.StockInfo SI ON LI.ItemID = SI.ID	
    INNER JOIN dbo.StockInfo SI_INST ON LI_INST.ItemID = SI_INST.ID
    LEFT JOIN (SELECT category, code FROM dbo.code WHERE category IN ('PCW', 'PCE', 'PCF')) StockCode 
				ON CONVERT(VARCHAR, SI.category) = StockCode.code    
    --LEFT OUTER JOIN [dbo].[Installation] AS INST ON LI.acctno = INST.AcctNo AND LI.ItemID = INST.ItemId AND 
				--							LI.agrmtno = INST.AgreementNo AND LI.stocklocn = INST.StockLocation
    WHERE LI_INST.quantity > 0 
		and si_inst.category in (select code from code c2 where c2.category in('PCW', 'PCE', 'PCF') and c2.codedescript='Installation')
		AND LI.quantity  > 0
)
GO
