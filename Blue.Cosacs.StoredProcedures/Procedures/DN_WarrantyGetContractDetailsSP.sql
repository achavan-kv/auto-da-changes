SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_WarrantyGetContractDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_WarrantyGetContractDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_WarrantyGetContractDetailsSP
-- ===================================================================
-- Author:		?
-- Create date: ?
-- Description:	
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 22/09/11  IP  RI - #8228 - CR8201 - Warranty Contract print out - description needs to be: descr+brand+vendor style long
-- 01/11/13  IP  #15068 - Remove references to WarrantyBand
-- ==================================================================
			@acctno varchar(12),
			@agreementNo int, 
			@contractNo varchar(10),
			@return int OUTPUT
AS

	SET 	@return = 0			--initialise return code
	
	DECLARE @agrmttaxtype varchar(1),
			@origacctno varchar(12),
			@InvoiceNo varchar(25) = @acctno+' '+cast(@agreementno as varchar(8))

	SELECT @agrmttaxtype = agrmttaxtype 
	FROM country

	SELECT @origacctno = stockitemacctno
	FROM WarrantyRenewalPurchase
	WHERE acctno = @acctno

	IF(@origacctno IS NULL)
		SET @origacctno = @acctno

	SELECT	L.itemno as 'ItemNo',
			L2.itemno as 'warrantyno',
			A.branchno as 'branchno',
			B.branchname as 'branchname',
			AG.empeenosale as 'EmpeeNo',
			AG.dateagrmt as 'dateagrmt',
			S.itemdescr1 as 'itemdescr1', 
			S.itemdescr2 as 'itemdescr2',
			--WB.warrantylength as 'warrantylength',
			case when ws.WarrantyLength is null then ISNULL(S2.WarrantyLength,0) else ws.WarrantyLength end as warrantylength,		--#15068
			--L.price + L.taxamt / L.quantity as 'itemprice',		/* only add tax on if price does not include tax i.e. agrmttaxtype = E */
			--L2.price + L2.taxamt / L2.quantity as 'warrantyprice',	/* only add tax on if price does not include tax */
			
			--IP - 23/05/08 - UAT(455) V5.1 - Added 'AND L.quantity' and 'L2.quantity' > 0 as previously was
			--giving a 'Divide by zero error'when printing for 'Cash & Go Return'.
			itemprice = 	CASE
						WHEN @agrmttaxtype = 'E' AND L.quantity >0	THEN L.price + L.taxamt / L.quantity
						ELSE L.price
					END,	
			warrantyprice = 	CASE
						WHEN @agrmttaxtype = 'E'AND L2.quantity >0	THEN L2.price + L2.taxamt / L2.quantity
						ELSE L2.price
					END,			
			L2.datereqdel as 'DateReqDel',
			S2.itemdescr1 as 'warrantydesc1',
			S2.itemdescr2 as 'warrantydesc2',
			CP.FullName AS EmployeeName,
			A.termstype,
			rtrim(ltrim(isnull(s.VendorLongStyle,''))) as Style,			--IP - 22/09/11 - RI - #8228 - CR8201
			rtrim(ltrim(isnull(s.Brand,''))) as Brand						--IP - 22/09/11 - RI - #8228 - CR8201
	FROM lineitem L 
	INNER JOIN lineitem L2 ON
			L.acctno = @origacctno AND
			L.agrmtno = L2.agrmtno AND
			L.stocklocn = L2.parentLocation AND
			L.ItemID = L2.ParentItemID
	INNER JOIN acct A ON @acctno = A.acctno 
	INNER JOIN branch B ON A.branchno = B.branchno 
	INNER JOIN agreement AG ON L.agrmtno = AG.agrmtno AND @acctno = AG.acctno  
	INNER JOIN stockitem S ON L.ItemID = S.ItemID AND L.stocklocn = S.stocklocn 
	INNER JOIN stockitem S2 ON L2.ItemID = S2.ItemID AND L2.stocklocn = S2.stocklocn 
	--INNER JOIN warrantyband WB ON WB.ItemID = L2.ItemID				--#15068
	INNER JOIN Admin.[User] CP ON AG.empeenosale = CP.id
	left outer join Warranty.WarrantySale ws on ws.CustomerAccount=@acctno and ws.InvoiceNumber=@InvoiceNo and ws.WarrantyContractNo=@contractNo
	WHERE L2.acctno = @acctno AND		
		  L2.agrmtno = @agreementNo AND
		  L2.contractno = @contractNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO