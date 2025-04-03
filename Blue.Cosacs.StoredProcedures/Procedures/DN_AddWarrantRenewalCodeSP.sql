SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AddWarrantRenewalCodeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AddWarrantRenewalCodeSP]
GO

CREATE PROCEDURE 	dbo.DN_AddWarrantRenewalCodeSP
			@acctNo varchar(12),
			@empno int,
			@contractno varchar(10),
			@return int OUTPUT

AS

	DECLARE @lastWR varchar(10)
	SET 	@return = 0			--initialise return code
	SET     @lastWR = 'WR'

	SELECT  TOP 1 @lastWR = code
	FROM	acctcode
	WHERE	acctno = @acctNo 
	AND     reference = @contractno
	AND		(code = 'WR1' OR code='WR2')
	ORDER BY datecoded DESC

	select @lastWR 

	SELECT @lastWR = 
		CASE @lastWR
		WHEN 'WR'  THEN 'WR1'
		WHEN 'WR1' THEN 'WR2'
		WHEN 'WR2' THEN 'WR3'
		ELSE 'WR1'
	END	
	select @lastWR
		INSERT INTO acctcode(acctno, code, datecoded, empeenocode, reference)
		VALUES (@acctNo, @lastWR, GETDATE(), @empno, @contractno)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO