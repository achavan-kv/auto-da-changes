SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocGetCustDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocGetCustDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocGetCustDetailsSP
			@empeeno int,
			@datealloc datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
    	SELECT	f.AcctNo,
		f.empeeno,
		f.datealloc,
		c.firstname + ' ' + c.name as CustomerName
	from 	acct a, follupalloc f, custacct cu, customer c
	where 	a.acctno = f.acctno 
	and 	cu.acctno = a.acctno
	and 	c.custid = cu.custid 
	and 	cu.hldorjnt = 'H'
	and 	f.empeeno = @empeeno
	and 	f.datealloc = @datealloc
	and	datedealloc is null		-- only select accounts that are currenty allocated (UAT 5.0 iss 20 jec 28/03/07)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO