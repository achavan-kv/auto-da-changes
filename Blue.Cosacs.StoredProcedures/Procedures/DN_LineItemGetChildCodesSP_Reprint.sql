if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemGetChildCodesSP_Reprint]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_LineItemGetChildCodesSP_Reprint] 
GO

/****** Object:  StoredProcedure [dbo].[DN_LineItemGetChildCodesSP_Reprint]    Script Date: 04-01-2019 11:19:11 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Suvidha
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[DN_LineItemGetChildCodesSP_Reprint]
		-- Add the parameters for the stored procedure here
		@acctNo varchar(12),
		@itemId int,
		@location smallint,
		@agreementNo int,
		@invoiceversion int,
		@return int OUTPUT
AS
BEGIN

	SET 	@return = 0		

	IF EXISTS(Select acctno from InvoiceDetails where acctno = @acctNo and agrmtno = @agreementNo and invoiceversion = @invoiceversion 
			and returnquantity IS NOT NULL)--for Win Return case
		BEGIN			
			SELECT	i.itemno, i.stocklocn, i.contractno, i.itemId, i.lineItemID
			--FROM	lineitem l 
			FROM	Invoicedetails i 
			LEFT JOIN stockinfo s ON i.ItemId = s.ID
			WHERE	acctno = @acctNo AND Invoiceversion = @invoiceversion
			AND		ParentItemID = @itemId
			--AND		stocklocn = @location //commented by Ashish : this condition is commented because child items can have diffrent location
			AND		agrmtno = @agreementNo
			AND		returnquantity IS NOT NULL
			order by s.itemtype desc
		END
	ELSE					
		BEGIN	
			SELECT	i.itemno, i.stocklocn, i.contractno, i.itemId, i.lineItemID
			FROM	Invoicedetails i 
			LEFT JOIN stockinfo s ON i.ItemId = s.ID
			WHERE	acctno = @acctNo AND Invoiceversion = @invoiceversion
			AND		ParentItemID = @itemId
			--AND		stocklocn = @location // commented by Ashish : this condition is commented because child items can have diffrent location
			AND		agrmtno = @agreementNo
			order by s.itemtype desc						
	
		END
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

END

GO


