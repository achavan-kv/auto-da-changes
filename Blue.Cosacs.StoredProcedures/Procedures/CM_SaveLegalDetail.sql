
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveLegalDetail]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveLegalDetail]
GO

-- =============================================================================================
-- Author:		Nasmi Mohamed & Ilyas Parker 
-- Create date: 06/01/2009
-- Description:	The procedure will insert records into CMLegalDetail for ExtraTelephone Actions	
-- =============================================================================================

CREATE PROCEDURE 	[dbo].[CM_SaveLegalDetail]
			@acctno	          char(12),
			@empeeno	      int,
			@solicitorNo      varchar(20),
			@auctionProceeds  money,
			@auctionDate	  datetime,
			@auctionAmount	  money,
			@courtDeposit	  money,
			@courtAmount	  money,
			@courtDate	      datetime,
			@caseClosed	      bit,	
			@mentionDate	  datetime,	
			@mentionCost	  money,	
			@paymentRemittance	money,	
			@judgement	        varchar(300),	
			@legalAttachmentDate  datetime,	
			@legalInitiatedDate	  datetime,	
			@defaultedDate	    datetime,	
			@userNotes	        varchar(300),	
			@return int OUTPUT

AS

	SET @return = 0			--initialise return code

	DECLARE @actionno smallint, @allocno int, @updateRecord bit
	
	SELECT	@allocno = allocno
	FROM	follupalloc 
	WHERE	datedealloc is null
	AND	acctno = @acctno

	SELECT	@actionno = isnull(MAX(actionno),1)   -- To get last updated actionno from bailaction table
	FROM	bailaction
	WHERE	acctno = @acctno
	AND code = 'LEG'						-- LEG for Legal (Extra Telephone Action)
	AND	allocno = isnull(@allocno, 0)
	AND	empeeno = @empeeno
	
	--NM & IP - 08/01/09 - CR976 - If a record exists for the account
	--then we only want to update the record and NOT insert a new record.
	IF EXISTS(SELECT * FROM CMLegalDetail WHERE acctno = @acctno)
	BEGIN
		SET  @updateRecord = 1
	END
	ELSE
	BEGIN
		SET	@updateRecord = 0
	END

	IF(@updateRecord = 0)
	BEGIN
		INSERT INTO CMLegalDetail
			(acctno,allocno,actionno,empeeno,
			solicitorNo,auctionProceeds,auctionDate,auctionAmount,
			courtDeposit,courtAmount,courtDate,caseClosed,
			mentionDate,mentionCost,paymentRemittance,judgement,
			legalAttachmentDate,legalInitiatedDate,defaultedDate,userNotes,	
			dateadded)
		VALUES(@acctno,isnull(@allocno, 0),@actionno,@empeeno,
			@solicitorNo,@auctionProceeds,@auctionDate,@auctionAmount,
			@courtDeposit,@courtAmount,@courtDate,@caseClosed,
			@mentionDate,@mentionCost,@paymentRemittance,@judgement,
			@legalAttachmentDate,@legalInitiatedDate,@defaultedDate,@userNotes,
			GETDATE())
	END
	ELSE
	BEGIN
		UPDATE CMLegalDetail
		SET allocno = isnull(@allocno, 0),
			actionno = @actionno,
			empeeno = @empeeno,
			solicitorNo = @solicitorNo,
			auctionProceeds = @auctionProceeds,
			auctionDate = @auctionDate,
			auctionAmount = @auctionAmount,
			courtDeposit = @courtDeposit,
			courtAmount = @courtAmount,
			courtDate = @courtDate,
			caseClosed = @caseClosed,
			mentionDate = @mentionDate,
			mentionCost = @mentionCost,
			paymentRemittance = @paymentRemittance,
			judgement = @judgement,
			legalAttachmentDate = @legalAttachmentDate,
			legalInitiatedDate = @legalInitiatedDate,
			defaultedDate = @defaultedDate,
			userNotes = @userNotes,
			datechanged = GETDATE() 
		WHERE acctno = @acctno
	END
	
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END








