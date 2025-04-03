SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountFindNameSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountFindNameSP]
GO

CREATE PROCEDURE 	dbo.DN_AccountFindNameSP
-- ============================================================================================
-- Author:		?
-- Create date: ?
-- Description:	
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 15/02/12  IP  #8819 - CR1234 - Transfering overage/shortage - return account balance
-- ============================================================================================
			@acct varchar(12),
			@cust varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	if exists (select 1 from custacct where acctno = @acct AND LTRIM(RTRIM(AcctNo)) != '')
	begin
		select 	CA.custid as CustomerID, 
			isnull(C.firstname,'') as firstname,
			C.name,
			CA.acctno as AccountNo,
			A.outstbal as Balance						--IP - 15/02/12 - #8819 - CR1234
		from	custacct CA, customer C, acct A			--IP - 15/02/12 - #8819 - CR1234
		where	CA.acctno = @acct
		and	CA.custid = C.custid
		and	CA.hldorjnt = 'H'
		and CA.acctno = A.acctno						--IP - 15/02/12 - #8819 - CR1234
	end
	else
	begin
		select 	top 1 C.custid as CustomerID, 
			isnull(C.firstname,'') as firstname,
			C.name,
			ISNULL(CA.acctno,'') as AccountNo
		from	customer C
		LEFT OUTER JOIN custacct CA ON CA.custid = C.custid
		where	C.custid = @cust
	
	end

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO