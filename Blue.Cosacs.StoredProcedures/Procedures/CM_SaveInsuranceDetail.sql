
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveInsuranceDetail]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveInsuranceDetail]
GO

-- =============================================================================================
-- Author:		Nasmi Mohamed & Ilyas Parker 
-- Create date: 06/01/2009
-- Description:	The procedure will insert records into CMInsuranceDetail for ExtraTelephone Actions	
-- =============================================================================================

CREATE PROCEDURE 	[dbo].[CM_SaveInsuranceDetail]
			@acctno	          char(12),
			@empeeno	      int,
			@initiatedDate	  datetime,
			@fullOrPartClaim  char(4),
			@insAmount		  money,
			@insType		  varchar(12),
			@isApproved	      bit,
			@userNotes	      varchar(300),
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
	AND code = 'IND'						-- INS for Insurance (Extra Telephone Action)
	AND	allocno = isnull(@allocno, 0)
	AND	empeeno = @empeeno

	--NM & IP - 08/01/09 - CR976 - If a record exists for the account
	--then we only want to update the record and NOT insert a new record.
	IF EXISTS(SELECT * FROM CMInsuranceDetail WHERE acctno = @acctno)
	BEGIN
		SET  @updateRecord = 1
	END
	ELSE
	BEGIN
		SET	@updateRecord = 0
	END

	IF(@updateRecord = 0)
	BEGIN
		INSERT INTO CMInsuranceDetail
			(acctno,allocno,actionno,empeeno,
			initiatedDate,fullOrPartClaim,insAmount,insType,
			isApproved,userNotes,dateadded)
		VALUES(@acctno,isnull(@allocno, 0),@actionno,@empeeno,
			   @initiatedDate,@fullOrPartClaim,@insAmount,@insType,
			   @isApproved,@userNotes,GETDATE())
	END
	ELSE
	BEGIN
		UPDATE CMInsuranceDetail
		SET	allocno = isnull(@allocno, 0),
			actionno = @actionno,
			empeeno = @empeeno,
			initiatedDate = @initiatedDate,
			fullOrPartClaim = @fullOrPartClaim,
			insAmount = @insAmount,
			insType = @insType,
			isApproved = @isApproved,
			userNotes = @userNotes,
			datechanged = GETDATE() 
		WHERE acctno = @acctno
	END
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
