SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[GetCashLoanItemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[GetCashLoanItemSP]
GO

CREATE PROCEDURE 	dbo.GetCashLoanItemSP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : GetCashLoanItemSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :  
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/10/11 ip  #3921 - CR1232 - Procedure to return just the Cash LOAN item to process the delivery for this item
--				seperate from any service charges, fee's.
-- ================================================
			@acctNo varchar(12),
			@agreementNo int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	s.IUPC, l.stocklocn, l.contractno, l.itemId
	FROM		lineitem l INNER JOIN StockInfo s ON l.ItemID = s.ID
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	AND		s.IUPC = 'LOAN'						

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

