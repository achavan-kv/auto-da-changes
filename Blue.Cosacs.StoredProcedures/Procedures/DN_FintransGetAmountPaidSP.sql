SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetAmountPaidSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetAmountPaidSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetAmountPaidSP
--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_FintransGetAmountPaidSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Sum Fintrans Amount Paid
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 22/04/09  jec CR976 Refinance Arrangements - cater for RFD transtype
-- 17/024/12 jec #9933 UAT155[UAT V6] - Error encountered upon returning cheque. Incorrectly including SCT transactions
--------------------------------------------------------------------------------

    -- Parameters 
			@acctno varchar(12),
			@amount money OUT,
			@return int OUTPUT

AS
/* AA 20/05/04  64849- Refund was not possible for transactions with ADX - now allowing this */
	SET 	@return = 0			--initialise return code

	if (@acctno like '___9%')
	begin
		SELECT 	@amount =  isnull(sum (transvalue),0) 
		FROM 		fintrans 
		WHERE 	acctno = @acctno
		AND 		transtypecode IN ('PAY','COR','XFR','JLX','SCX','REF','RET','DDE','DDN','DDR','OVE','DPY','ADX','STR')	-- #9933
	END
	
	ELSE
	BEGIN
		SELECT 	@amount =  isnull(sum (transvalue),0) 
		FROM 		fintrans 
		WHERE 	acctno = @acctno
		AND 		transtypecode IN ('PAY','COR','XFR','JLX','SCX','REF','RET','DDE','DDN','DDR','OVE','DPY','ADX','SCT','STR')	-- #9933
	END
	
	
	-- include RFD trans with Credit balance  - Refinance Deposit
	SELECT 	@amount =  @amount+ isnull(sum (transvalue),0) 
	FROM 		fintrans 
	WHERE 	acctno = @acctno
	AND 	transtypecode IN ('RFD') and transvalue<0



	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End

