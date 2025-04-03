SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (SELECT * FROM dbo.sysobjects WHERE id = object_id('[dbo].[DN_SpaSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SpaSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_SpaSaveSP
			@acctno varchar(12),
			@allocno int,
			@actionno int,
			@employeeno int,
			@empeenospa int,
			@dateadded datetime,
			@reasoncode varchar(4),
			@dateexpiry datetime,
			@spainstal float,
			@return int OUTPUT

AS
   declare @balance money
	SET 	@return = 0			--initialise return code

	INSERT
	INTO		Spa
			(origbr, acctno, allocno, actionno, empeeno, empeenospa,
			dateadded, code, spainstal, dateexpiry)
	VALUES	(0, @acctno, @allocno, @actionno, @employeeno, @empeenospa, 
			@dateadded, @reasoncode, @spainstal, @dateexpiry)
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
   -- if SPA added and is expiring within the last month then remove any pending write-offs for this account
   IF EXISTS (SELECT * FROM bdwpending WHERE acctno =@acctno)
   BEGIN
		SELECT @balance = outstbal FROM acct WHERE acctno =@acctno
  
      INSERT INTO bdwrejection
      (acctno, empeeno, code, rejectcode, balance, rejectdate)
      SELECT 
      P.acctno, 0,P.CODE,'SPA',@balance, getdate()
      FROM bdwpending P WHERE acctno =@acctno
      AND @dateexpiry >dateadd(month, -1,getdate())
   
	   IF (@@error != 0)
    	BEGIN
		  SET @return = @@error
    	END
   END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



