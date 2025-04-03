SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryScheduleGetCustomerSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryScheduleGetCustomerSP]
GO
CREATE PROCEDURE 	dbo.DN_DeliveryScheduleGetCustomerSP
			@loadno smallint, 
			@branchno smallint,
			@datedel datetime,
			@return int OUTPUT

AS


	SET 	@return = 0			--initialise return code

	CREATE TABLE #accts
	(	
		acctno varchar(12),
		buffbranchno smallint,
		buffno int,
		dateprinted datetime, --IP - 01/08/08 -UAT5.1 - UAT(508)
		customername varchar(95),
		custid varchar(20),
		alias varchar(25),
		origBuffBranchNo smallint	-- UAT(5.2) - 568	
	)

	INSERT INTO #accts
	(
		acctno,
		buffbranchno,
		buffno,
		dateprinted, --IP - 01/08/08 -UAT5.1 - UAT(508)
		customername,
		custid,
		alias,
		origBuffBranchNo 
	)
	SELECT 	DISTINCT 
		s.acctno,
		-- UAT 219 --ISNULL(s.retstocklocn, s.stocklocn) as buffbranchno,
		CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END as buffbranchno,
		s.buffno,
		s.dateprinted, --IP - 01/08/08 - UAT5.1 - UAT(508)
		'',
		'',
		'',
		ISNULL(s.buffbranchno, s.stocklocn) -- UAT(5.2) - 568	
	FROM  	schedule s, deliveryload d
        WHERE  	d.datedel = @dateDel
        AND  	d.loadno = @loadno
        AND  	d.branchno = @branchno
        AND     s.datedelplan = d.datedel
        AND  	s.buffno = d.buffno
        AND     d.buffbranchno = (CASE WHEN ISNULL(s.retstocklocn,0) = 0 THEN s.stocklocn ELSE s.retstocklocn END)
        AND  	s.loadno = d.loadno

	UPDATE	#accts
	SET		customername = cu.firstname + ' ' + cu.name,
			custid = c.custid,
			alias = ISNULL(cu.alias, '')
	FROM	acct a, custacct c, customer cu
	WHERE	a.acctno = #accts.acctno
	AND		a.acctno = c.acctno
	AND		c.custid = cu.custid
	and		c.hldorjnt='H'

	SELECT	acctno,
			buffbranchno,
			buffno,
			customername,
			buffbranchno,
			buffno,
			dateprinted, --IP - 01/08/08 - UAT5.1 - UAT(508)
			customername,
			custid,
			alias,
			origBuffBranchNo -- UAT(5.2) - 568	
	FROM	#accts


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO



