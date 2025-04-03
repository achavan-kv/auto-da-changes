SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ItemGetPurchaseOrdersSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ItemGetPurchaseOrdersSP]
GO

CREATE PROCEDURE 	dbo.DN_ItemGetPurchaseOrdersSP
			@itemId INT,
			@branchno smallint,
			@return int OUTPUT

AS

	DECLARE	@region varchar(3),
			@days int
	
	SET 	@return = 0			--initialise return code
	
	SELECT	@region = warehouseregion
	FROM	branch
	WHERE	branchno = @branchno
	
	SELECT	@days = convert(integer,value)
	FROM	countrymaintenance
	WHERE	codename = 'warehousetime'

	SELECT	S.IUPC AS itemno,
			DATEADD(DAY, @days, p.expectedreceiptdate) as receiptdate,
			p.supplierno,
			p.quantityavailable as quantity,
			p.stocklocn,
			p.purchaseordernumber,
			p.ItemID
	FROM	PurchaseOrderOutstanding p
			INNER JOIN dbo.StockInfo S ON p.ItemID = S.ID,
			branch b, branchregion br
	WHERE	p.ItemID = @itemId
	AND		p.stocklocn = b.branchno
	AND		b.branchno = br.branchno
	AND		br.region = @region
	AND		p.quantityavailable > 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

