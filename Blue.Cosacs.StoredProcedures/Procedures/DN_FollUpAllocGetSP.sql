SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocGetSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocGetSP
			@acctno varchar(12),
			@allocno int OUT,
			@empeeno int OUT,
			@datealloc datetime OUT,
			@datedealloc datetime OUT,
			@allocarrears money OUT,
			@bailfee float OUT,
			@allocprtflag char(1) OUT,
			@empeenoalloc int OUT,
			@empeenodealloc int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@allocno = allocno,
			@empeeno = empeeno,
			@datealloc = datealloc,
			@datedealloc = datedealloc,
			@allocarrears = allocarrears,
			@bailfee = bailfee,
			@allocprtflag = allocprtflag,
			@empeenoalloc = empeenoalloc,
			@empeenodealloc = empeenodealloc
	FROM		follupalloc 
	WHERE	isnull(datealloc, '1/1/1900') != '1/1/1900'
	AND		isnull(datealloc, '1/1/1900') <= getdate()
	AND		(isnull(datedealloc, '1/1/1900') = '1/1/1900'
	OR		isnull(datedealloc, '1/1/1900') >= getdate())
	AND		acctno = @acctno

      declare @paymentdate datetime
      
      if @empeeno is null
      begin
            
            select @paymentdate = isnull(max(datetrans), dateacctopen)
            from acct a
            left outer join fintrans f
                  on a.acctno = f.acctno 
            where a.acctno = @acctno 
             and isnull(f.transtypecode, 'PAY') = 'PAY'
            group by dateacctopen
            
             --RM changes for work list when not allocated based on last action
            
            select @empeeno = empeeno,
            @datealloc = b.dateadded,
            @allocno = b.allocno
            from bailaction b
            where acctno = @acctno
            and dateadded > @paymentdate
            AND b.dateadded = (SELECT MAX(bc.dateadded)
            from bailaction bc
            where bc.acctno = @acctno
            and bc.dateadded > @paymentdate)
            order by dateadded DESC 
            
            -- get arrears at time of allocation       
            SELECT @allocarrears = d.arrears
            FROM ArrearsDaily d WHERE acctno= @acctno      
            AND d.datefrom  =(SELECT MAX(p.datefrom) FROM ArrearsDaily p 
            WHERE p.Acctno= @acctno AND p.datefrom < @datealloc)
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

