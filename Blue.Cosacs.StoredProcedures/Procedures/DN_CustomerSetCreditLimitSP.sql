SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSetCreditLimitSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSetCreditLimitSP]
GO





CREATE PROCEDURE 	dbo.DN_CustomerSetCreditLimitSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerSetCreditLimitSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        :
-- Author       : ??
-- Date         : ??
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/11/11  jec #8732 Unblock Cashloan if Blocked
-- 16/05/13  jec #13449 Storecard Limit
--------------------------------------------------------------------------------
			@custid varchar(20),
			@limit money,
			@type char(1),
			@return int OUTPUT


AS

	SET 	@return = 0			--initialise return code

/*
16/10/07 rdb lw346 - when an account has line items and is taken
over the RF spend limit, referred and rejected the RFLimit was set to 0.
We will now no longer set it to 0 if the customer has other valid RF accounts

6/11/07 rdb lw346 - this has caused a problem, it wont set a new RF Limit
have added @limit to if clause

28/05/09 RM LW70933 - If a customer has an RF account rejected, but has a other 
valid RF accounts, the credit limit will be set to the total agreement value of
other accounts so that the available spend will be set to 0.

*/

	DECLARE @StorecardPercent DECIMAL
	SELECT @StorecardPercent = CONVERT(DECIMAL,value)  FROM CountryMaintenance WHERE CodeName = 'StorecardPercent'

if @limit > 0 or not exists
(
	select 1 -- count(*) 
		from acct a
		inner join proposal p
			on a.acctno = p.acctno
		where p.custid = @custId
			and a.accttype = 'R'
			and (p.propresult not in ( 'X' , 'D')  
					or a.outstbal >0 )
)
BEGIN

	UPDATE	customer
	SET		RFCreditLimit = @limit,
			--StoreCardLimit = @limit * @StorecardPercent/100,
			CashLoanBlocked = case when CashLoanBlocked='B' then 'U' else CashLoanBlocked end,		-- #8732
			LimitType = @type
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END

ELSE

BEGIN
	declare @RFlimit money, @RFAvailable money


	EXEC DN_CustomerGetRFLimitSP @custID, '', @RFLimit OUT, @RFAvailable OUT, @return OUT

	SELECT @limit=(sum(a.outstbal)-(sum(a.outstbal)+@RFAvailable-@RFlimit))
	FROM acct a
	INNER JOIN proposal p on a.acctno = p.acctno
	INNER JOIN Customer c on c.custid=p.custid
	WHERE p.custid = @custId
	

	UPDATE	customer
	SET		RFCreditLimit = @limit,
			--StoreCardLimit = @limit * @StorecardPercent/100,
			CashLoanBlocked = case when CashLoanBlocked='B' then 'U' else CashLoanBlocked end,		-- #8732
			LimitType = @type
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END

	if exists(select * from custacct ca where custid = @custid and SUBSTRING(acctno,4,1)='9')		-- #13449 - Storecard 
	Begin
		UPDATE	customer
			SET		StoreCardLimit = @limit * @StorecardPercent/100
			WHERE	custid = @custid
	End		
	else
	Begin
		UPDATE	customer
			SET		StoreCardLimit = null,StoreCardAvailable=null
			WHERE	custid = @custid
	End

GO


-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
