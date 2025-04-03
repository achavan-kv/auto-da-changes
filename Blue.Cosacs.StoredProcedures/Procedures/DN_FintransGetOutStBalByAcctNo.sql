SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_FintransGetOutStBalByAcctNo' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_FintransGetOutStBalByAcctNo
END
GO


CREATE PROCEDURE DN_FintransGetOutStBalByAcctNo

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_FintransGetOutStBalByAcctNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load outstanding balance for cancellation
-- Author       : Rupal Desai
-- Date         : 26 June 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/06/06  RD  68237 Initial creation to resolve issue
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo		VARCHAR(12),
    @outstbal		money OUT,
    @Return         	INTEGER OUTPUT

AS
BEGIN

	SET @Return = 0;

	-- Get sum of all the transactions from fintrans 
     IF EXISTS (SELECT 'A' FROM ACCT WHERE acctno = @piAcctNo and isAmortized =1 and IsAmortizedOutStandingBal =1)
	BEGIN 
		-------Calculate outstanding balance------------------------
		Declare @outstandingBalance DECIMAL(15,4)
		EXEC CLAmortizationCalcDailyOutstandingBalance @piAcctNo,@outstandingBalance OUT  --SP returning new Oustanding balance
		SET @outstbal = @outstandingBalance
	
	END
	ELSE

	BEGIN


		SELECT @outstbal = ISNULL(SUM(round(TransValue,2)) ,0)
		FROM   Fintrans  
		WHERE  AcctNo = @piAcctNo

			IF (@@error != 0)
		BEGIN
			SET @return = @@error
		END
   END 
END
GO

GRANT EXECUTE ON DN_FintransGetOutStBalByAcctNo TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
