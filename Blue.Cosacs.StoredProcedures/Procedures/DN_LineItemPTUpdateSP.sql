SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemPTUpdateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemPTUpdateSP]
GO



CREATE PROCEDURE 	dbo.DN_LineItemPTUpdateSP
			@origbr smallint,
			@acctNo varchar(12),
			@agreementNo int,
			@itemNo varchar(8),
			@itemSuppText varchar(76),
			@quantity float,
			@delQty float,
			@stockLocn smallint,
			@price money,
			@orderValue money,
			@dateReqDel datetime,
			@timeReqDel varchar(12),
			@datePlanDel datetime,
			@delNoteBranch smallint,
			@qtyDiff char(1),
			@itemType varchar(1),
			@hasString smallint,
			@notes varchar(200),
			@taxAmount float,
			@parentItemNo varchar(8),
			@parentStockLocn smallint,
			@isKit smallint,
			@deliveryAddress char(2),
			@expectedreturndate datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE 	lineitem
	SET		origbr		=	@origbr,		
			acctno		=	@acctNo,		
			agrmtno		=	@agreementNo,		
			itemno		=	@itemNo,		
			itemsupptext	=	@itemSuppText,		
			quantity		=	quantity + @quantity,		
			delqty		=	delqty + @delQty,		
			stocklocn	=	@stockLocn,		
			price		=	@price,		
			ordval		=	ordval + @orderValue,		
			datereqdel	=	@dateReqDel,		
			timereqdel	=	@timeReqDel,		
			dateplandel	=	@datePlanDel,		
			delnotebranch	=	@delNoteBranch,		
			qtydiff		=	@qtyDiff,		
			itemtype		=	@itemType,		
			--hasstring	=	@hasString,		--IP - 09/03/11 - Removed hasstring
			notes		=	@notes,		
			taxamt		=	 taxamt + @taxAmount,		
			parentItemNo	=	@parentItemNo,		
			parentLocation	=	@parentStockLocn,		
			isKit		=	@isKit,
			deliveryAddress	=	@deliveryAddress,
			expectedreturndate = 	@expectedreturndate
	WHERE	acctno = @acctNo
	AND		agrmtno = @agreementNo
	AND		stocklocn = @stockLocn
	AND		itemno = @itemNo

	IF(@@rowcount=0)		--the line item doesn't exist
	BEGIN
		INSERT
		INTO		lineitem
				(origbr,
				acctno,
				agrmtno,
				itemno,
				itemsupptext,
				quantity,
				delqty,
				stocklocn,
				price,
				ordval,
				datereqdel,
				timereqdel,
				dateplandel,
				delnotebranch,
				qtydiff,
				itemtype,
				--hasstring, --IP - 09/03/11 - Removed hasstring
				notes,
				taxamt,
				parentItemNo,
				parentLocation,
				isKit,
				deliveryAddress,
				expectedreturndate)
		VALUES	(@origbr,	
				@acctNo ,	
				@agreementNo ,	
				@itemNo ,	
				@itemSuppText ,	
				@quantity ,	
				@delQty ,	
				@stockLocn ,	
				@price ,	
				@orderValue ,	
				@dateReqDel ,	
				@timeReqDel ,	
				@datePlanDel ,	
				@delNoteBranch ,	
				@qtyDiff ,	
				@itemType ,	
				--@hasString ,	--IP - 09/03/11 - Removed hasstring
				@notes ,	
				@taxAmount ,	
				@parentItemNo ,	
				@parentStockLocn ,	
				@isKit,
				@deliveryAddress,
				@expectedreturndate)
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

