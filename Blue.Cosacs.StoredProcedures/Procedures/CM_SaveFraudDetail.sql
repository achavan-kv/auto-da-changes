IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'CM_SaveFraudDetail')
DROP PROCEDURE CM_SaveFraudDetail
GO 
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================================================================
-- Author:		Nasmi Mohamed & Ilyas Parker 
-- Create date: 06/01/2009
-- Description:	The procedure will insert records into CMFraudDetail for ExtraTelephone Actions	
-- =============================================================================================

CREATE PROCEDURE 	[dbo].[CM_SaveFraudDetail]
			@acctno	            char(12),
			@empeeno	        int,
			@fraudInitiatedDate	datetime,
			@isResolved			bit,
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
	AND code = 'FRD'						-- FRD for Fraud (Extra Telephone Action)
	AND	allocno = isnull(@allocno, 0)
	AND	empeeno = @empeeno

	--NM & IP - 08/01/09 - CR976 - If a record exists for the account
	--then we only want to update the record and NOT insert a new record.
	IF EXISTS(SELECT * FROM CMFraudDetail WHERE acctno = @acctno)
	BEGIN
		SET  @updateRecord = 1
	END
	ELSE
	BEGIN
		SET	@updateRecord = 0
	END

	IF(@updateRecord = 0)
	BEGIN
		INSERT INTO CMFraudDetail
			(acctno,allocno,actionno,empeeno,
			fraudInitiatedDate,isResolved,userNotes,dateadded)
		VALUES(@acctno,isnull(@allocno, 0),@actionno,@empeeno,
			@fraudInitiatedDate,@isResolved, @userNotes,GETDATE())
	END
	ELSE
	BEGIN
		UPDATE CMFraudDetail
		SET allocno = isnull(@allocno, 0),
			actionno = @actionno,
			empeeno = @empeeno,
			fraudInitiatedDate = @fraudInitiatedDate,
			isResolved = @isResolved,
			userNotes = @userNotes,
			datechanged = GETDATE() 
		WHERE acctno = @acctno
	END
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





