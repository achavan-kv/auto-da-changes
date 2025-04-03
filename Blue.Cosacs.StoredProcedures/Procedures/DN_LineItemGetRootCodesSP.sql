SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetRootCodesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemGetRootCodesSP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemGetRootCodesSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemGetRootCodesSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 16/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed to use ParentItemID rather than parentitemno
-- ================================================
			@acctNo varchar(12),
			@agreementNo int,
			@invoiceversion int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code	

	IF(@invoiceversion != 0)

	BEGIN	
	IF EXISTS(Select * from InvoiceDetails where acctno=@acctNo and agrmtno=@agreementNo and invoiceversion=@invoiceversion and returnquantity IS NOT NULL)
	BEGIN			
		SELECT	 i.RetItemNo, i.stocklocn,  i.itemId ,ISNULL(i.contractno,'') as 'ContractNo'
		FROM		 Invoicedetails i  Inner Join  StockInfo s ON i.ItemID = s.ID
		WHERE	i.acctno = @acctNo
		AND		i.agrmtno = @agreementNo
		AND i.invoiceversion = @invoiceversion
		AND i.returnquantity IS NOT NULL

				AND		(i.ParentItemID = 0 OR					--IP - 16/05/11 - CR1212 - #3627
				 EXISTS(SELECT	* 
						FROM	warrantyrenewalpurchase
						WHERE	acctno = @acctNo))						
		
	END
	ELSE					
	BEGIN	
	SELECT	 s.IUPC, i.stocklocn,  i.itemId ,ISNULL(i.contractno,'') as 'ContractNo'
		FROM		 Invoicedetails i  Inner Join  StockInfo s ON i.ItemID = s.ID
		WHERE	i.acctno = @acctNo
		AND		i.agrmtno =@agreementNo
		AND i.invoiceversion = @invoiceversion
		AND i.Quantity !=0		
		--AND	(isnull(parentItemNo,'') = '' OR
		AND		(i.ParentItemID = 0 OR					--IP - 16/05/11 - CR1212 - #3627
				 EXISTS(SELECT	* 
						FROM	warrantyrenewalpurchase
						WHERE	acctno = @acctNo))						
						END
						
	
	END 
	ELSE
	BEGIN		
		SELECT	s.IUPC, l.stocklocn, l.contractno, l.itemId, '' as 'delorcoll'
	FROM        lineitem l INNER JOIN StockInfo s ON l.ItemID = s.ID
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	AND		(l.ParentItemID = 0 OR					--IP - 16/05/11 - CR1212 - #3627
				 EXISTS(SELECT	* 
						FROM	warrantyrenewalpurchase
						WHERE	acctno = @acctNo))
END


	
	
