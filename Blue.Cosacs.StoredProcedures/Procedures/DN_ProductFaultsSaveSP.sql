SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ProductFaultsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ProductFaultsSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_ProductFaultsSaveSP
			@acctno varchar(12), --IP 30/11/2007 - UAT(206) Changed to varchar
			@agreementno int,
			--@itemno varchar(8),
			@itemno varchar(18),						--IP - 01/08/11 - RI
			--@returnitemno varchar(8),
			@returnitemno varchar(18),					--IP - 01/08/11 - RI
			@notes varchar(1000),
			@reason varchar(3),
			@datecollection datetime,
			@elapsedmonths smallint,
			@newbuffno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @itemID int,								--IP - 01/08/11 - RI
			@retItemID int
			
	SET @itemID = isnull((select ID from StockInfo where iupc = @itemno and repossesseditem = 0),0)	--IP - 01/08/11 - RI
	SET @retItemID = isnull((select ID from StockInfo where iupc = @returnitemno and repossesseditem = 0),0)	--IP - 01/08/11 - RI
	
	UPDATE	productfaults
	SET		notes = @notes,	
			reason = @reason,
			datecollection = @datecollection,
			elapsedmonths = @elapsedmonths,
			newbuffno = @newbuffno
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	--AND		itemno = @itemno
	--AND		returnitemno = @returnitemno
	AND		ItemID = @itemID						--IP - 01/08/11 - RI					
	AND		RetItemID = @retItemID					--IP - 01/08/11 - RI

	IF(@@rowcount = 0)
	BEGIN
		INSERT 
		INTO		productfaults
				(acctno, agrmtno, itemno, returnitemno, 
				--notes, reason, datecollection, elapsedmonths, newbuffno)
					notes, reason, datecollection, elapsedmonths, newbuffno, ItemID, RetItemID)		--IP - 01/08/11 - RI
		--VALUES	(@acctno, @agreementno, @itemno, @returnitemno,
		--		@notes, @reason, @datecollection, @elapsedmonths, @newbuffno)
			VALUES	(@acctno, @agreementno, '', '',
				@notes, @reason, @datecollection, @elapsedmonths, @newbuffno, @itemID, @retItemID)	--IP - 01/08/11 - RI
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

