SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FinXfrInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FinXfrInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_FinXfrInsertSP
-- ============================================================================================
-- Author:		?
-- Create date: ?
-- Description:	
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/02/12  IP  #8819 - CR1234 - Transfering overage/shortage save the CashierTotalID
-- 20/02/12  IP  #9633 - CR1234 - Saving the transrefno of the transaction being transferred.
-- ============================================================================================
			@acctno varchar(12),
			@transrefno int,
			@datetrans datetime,
			@acctnoxfr varchar(12),
			@agrmtno int,				--IP - 30/11/2010 - StoreCard
			@storecardno bigint = null,	--IP - 30/11/2010 - StoreCard
			@cashierTotID varchar(10),	--IP - 14/02/12 - #8819 - CR1234
			@origTransRefNo int,		--IP - 20/02/12 - #9633 - CR1234		
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE 	@name varchar(60)

	IF(@acctnoxfr != 'JOURNAL')
	BEGIN
		SELECT	@name = C.name
		FROM		customer C  INNER JOIN
				custacct CA ON CA.custid = C.custid 
		WHERE	CA.acctno = @acctnoxfr
		AND		CA.hldorjnt = 'H'
	END
	ELSE
	BEGIN
		SET		@name = 'JOURNAL'
	END

	--IP - 14/02/12 - #8819 - CR1234
	if(@cashierTotID != '')
	begin
		if(cast(@cashierTotID as int) > 0)
		begin
			set @name = convert(varchar, @cashierTotID)
		end
	end
	
	UPDATE	finxfr
	SET		acctname = @name
	WHERE	acctno = @acctno
	AND		transrefno = @transrefno
	AND		datetrans = @datetrans
	AND		acctnoxfr = @acctnoxfr

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		finxfr
				(origbr, acctno, transrefno, datetrans, acctnoxfr, acctname, agrmtno, storecardno, OrigTransRefNo) --IP - 20/02/12 - #9633 - CR1234 - added OrigTransRefNo --IP - 30/11/2010 - StoreCard - added agrmtno and storecardno
		VALUES	(null, @acctno, @transrefno, @datetrans, @acctnoxfr, @name, @agrmtno, @storecardno, @origTransRefNo) --IP - 20/02/12 - #9633 - CR1234 - added @origTransRefNo
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

