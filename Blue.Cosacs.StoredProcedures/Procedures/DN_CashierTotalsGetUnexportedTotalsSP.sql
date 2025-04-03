SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsGetUnexportedTotalsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsGetUnexportedTotalsSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsGetUnexportedTotalsSP
			@branchno smallint,
			@return int OUTPUT

AS
   set nocount on
	SET 	@return = 0			--initialise return code
--print ' stage one '
	-- unexported cashier totals by payment method
	select 	code.codedescript, 
		code.code,
		isnull(sum(isnull (cb.usertotal,0) + isnull (cb.deposit,0)),0) as 'CashierTotals'
	into 	#totals
	from 	code left outer join cashiertotalsbreakdown cb
	on	code.code = cb.paymethod 
   --left outer 
   join cashiertotals c
	on	cb.cashiertotalid = c.id
	where 	code.category = 'FPM' 
	and	code.code != '0'
	and	isnull(c.runno,0) = 0
	and	isnull(c.branchno, @branchno) = @branchno 
	group by code.codedescript, code.code
	--print 'stage to '
	--unexported cashier deposits by payment method
	select 	code.codedescript, 
		code.code,
		isnull(sum(cd.depositvalue),0) as 'CashierDeposits'
	into	#deposits
	from 	code left outer join cashierdeposits cd
	on	code.code = cd.paymethod  and cd.voided !='Y'
	where 	code.category = 'FPM' 
	and	code.code != '0'
	and	isnull(cd.runno,0) = 0
	and	isnull(cd.branchno, @branchno) = @branchno 
	group by code.category, code.codedescript, code.code
   -- print	'stage three '
	select	d.codedescript, d.code, d.CashierDeposits, t.CashierTotals
	from	#deposits d inner join #totals t
	on	d.code = t.code
	where	CashierDeposits != CashierTotals 


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

