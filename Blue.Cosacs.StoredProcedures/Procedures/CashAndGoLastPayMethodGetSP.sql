
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CashAndGoLastPayMethodGetSP]') 
		AND OBJECTPROPERTY(id,N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CashAndGoLastPayMethodGetSP]
GO
-- =============================================
-- Author:		Ilyas Parker
-- Create date: 14/10/2009
-- Description:	Retrieves the details from Fintrans of the last payment made for a Cash & Go sale.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 14/10/09  IP  Created
-- 27/01/11 jec  Fix error when @Agrmtno =1 
-- =============================================
CREATE PROCEDURE [dbo].[CashAndGoLastPayMethodGetSP] 
    @AcctNo varchar(12),
	@AgrmtNo int,
	@return INT OUTPUT
AS
	
    SET @return = 0
	
declare @MaxDelTransRefNo int,
		@EmpeenoNotified int


set @MaxDelTransRefNo = (select max(d.transrefno) from delivery d
							where d.acctno = @AcctNo
							and d.agrmtno = @AgrmtNo
							and d.delorcoll = 'D')
	
set @EmpeenoNotified = (select  MAX(d.NotifiedBy) from delivery d			-- jec 27/01/11
							where d.acctno = @AcctNo
							and d.agrmtno = @AgrmtNo)

--Select the payment record for the Cash & Go sale (first payment record, after the delivery record for the Cash & Go Sale).
select f.acctno, f.datetrans, f.transrefno, f.empeeno, f.transtypecode, f.transvalue, b.bankname, f.bankacctno, f.chequeno, c.codedescript as PayMethod from fintrans f
left join code c 
on c.code = f.paymethod
left join bank b 
on f.bankcode = b.bankcode
where f.acctno = @AcctNo
and f.empeeno = @EmpeenoNotified
and f.transrefno = (select min(f2.transrefno) from fintrans f2
						where f2.acctno = @AcctNo
						and f2.empeeno = f.empeeno
						and f2.transrefno > @MaxDelTransRefNo
						and f2.transtypecode = 'PAY')
and c.category = 'FPM'		
			
	
IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
Go

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End


