SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

---------------------------------------------------------------------------------------------------

IF exists (select * from dbo.sysobjects where id = object_id(N'[dbo].[DN_ResetLineItemPrintOrder]') and objectproperty(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[DN_ResetLineItemPrintOrder]
GO

------------------------------------------------------------------------------------
-- Author : NM
-- CR 1048 
-- 16/10/2009
-- ---------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[DN_ResetLineItemPrintOrder]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_ResetLineItemPrintOrder.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Reset Line tem
-- Author       : ??
-- Date         : ??
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/05/11  jec RI Integration 
-- ================================================
	-- Add the parameters for the stored procedure here 
	@acctNo VARCHAR(12),
	@return Int OUTPUT
AS

SET NOCOUNT ON; 
SET @return = 0

DECLARE @itemCount SMALLINT, @index SMALLINT, @stockLocn SMALLINT, 
		@ItemID INT		-- RI

IF EXISTS(SELECT 1 FROM LineItem WHERE AcctNo = @acctNo AND (PrintOrder = 0 OR PrintOrder IS NULL))
BEGIN
	SELECT @itemCount = COUNT(*) FROM LineItem WHERE AcctNo = @acctNo
	
	UPDATE LineItem SET PrintOrder = @itemCount WHERE AcctNo = @acctNo
	
	SET @index = 1
	
	WHILE (EXISTS(SELECT 1 FROM LineItem WHERE AcctNo = @acctNo AND PrintOrder > @index))
	BEGIN 
		UPDATE TOP(1) LineItem SET PrintOrder = @index
		WHERE AcctNo = @acctNo AND PrintOrder > @index AND 
			--RTRIM(LTRIM(ISNULL(ParentItemNo, ''))) = '' AND 
			ParentItemID = 0 AND		-- RI
			ParentLocation <= 0
		
		IF(@@ROWCOUNT > 0)
		BEGIN
			SELECT @stockLocn = StockLocn, @ItemID=ItemID		-- RI
			FROM LineItem
			WHERE AcctNo = @acctNo AND PrintOrder = @index AND 
				--RTRIM(LTRIM(ISNULL(ParentItemNo, ''))) = '' AND 
				ParentItemID = 0 AND		-- RI
				ParentLocation <= 0
		
			WHILE( EXISTS( SELECT 1 FROM LineItem WHERE AcctNo = @acctNo AND PrintOrder > @index AND
														--ParentItemNo = @itemNo
														ParentItemID = @ItemID		-- RI 
														AND ParentLocation = @stockLocn )
				 )
			BEGIN
				SET	 @index = @index + 1
				
				UPDATE TOP(1) LineItem SET PrintOrder = @index
				WHERE AcctNo = @acctNo AND PrintOrder > @index AND 
					--ParentItemNo = @itemNo
					ParentItemID = @ItemID		-- RI 
					AND ParentLocation = @stockLocn
			END
		END
		
		SET	 @index = @index + 1
		
	END	
END

SET @return = @@ERROR

RETURN @return




