SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSetOverrideCreditLimitSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSetOverrideCreditLimitSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerSetOverrideCreditLimitSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_CustomerSetOverrideCreditLimitSP.PRC
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
-- 31/05/13  ip  #13449 Storecard Limit
--------------------------------------------------------------------------------
			@custid varchar(20),
			@limit money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @StorecardPercent DECIMAL
	SELECT @StorecardPercent = CONVERT(DECIMAL,value)  FROM CountryMaintenance WHERE CodeName = 'StorecardPercent'

	UPDATE	customer
	SET		RFCreditLimit = @limit,
			OldRFCreditLimit = @limit,
			CashLoanBlocked = case when CashLoanBlocked='B' then 'U' else CashLoanBlocked end--,		-- #8732
			--StoreCardLimit = @limit * @StorecardPercent/100											-- #13449 
	WHERE	custid = @custid

	if @@error = 0 --recording the fact that overwridden so if rescored by system do not change
   begin

    insert into customerRFlimitoverride (custid,newlimit,datechanged)
	 values (@custid,@limit, getdate())
   end

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
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
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End