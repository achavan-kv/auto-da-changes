SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemBfCollectionSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemBfCollectionSaveSP]
GO

CREATE PROCEDURE dbo.DN_LineItemBfCollectionSaveSP
/***********************************************************************************************************
--
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemBfCollectionSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Description  : 
--				
-- Author       : ?
-- Date         : ?
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/06/11  IP  CR1212 - RI - #3806 - Use ItemID
************************************************************************************************************/
			@acctNo varchar(12),
			@agreementNo int,
			--@itemNo varchar(8),
			@itemID int,						--IP - 06/06/11 - CR1212 - RI - #3806
			@quantity float,
			@price money,
			@orderValue money,
			@contractNo varchar(10),
			@return int OUTPUT
AS
	SET 	@return = 0			--initialise return code

	UPDATE 	lineitembfcollection
	SET		quantity		=	@quantity,
			price		=	@price,		
			ordval		=	@orderValue		
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	--AND		itemno = @itemNo
	AND		ItemID = @itemID				--IP - 06/06/11 - CR1212 - RI - #3806
	AND		contractno = @contractNo

	IF(@@rowcount=0)		--the line item doesn't exist
	BEGIN
		INSERT
		INTO lineitembfcollection
				(acctno,
				agrmtno,
				itemno,
				quantity,
				price,
				ordval,
				contractno,
				ItemID)						--IP - 06/06/11 - CR1212 - RI - #3806
		VALUES	(@acctNo ,	
				@agreementNo ,	
				--@itemNo ,	
				'',							--IP - 06/06/11 - CR1212 - RI - #3806			
				@quantity ,	
				@price ,	
				@orderValue ,	
				@contractNo,
				@itemID)					--IP - 06/06/11 - CR1212 - RI - #3806		
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

