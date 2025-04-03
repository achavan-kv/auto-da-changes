/* 
jec 16/04/12  #9931 UAT154[UAT V6] - Financial Interface Report -CLD Transactions. duplicated 
*/

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GetInterfaceBreakdownTotalsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GetInterfaceBreakdownTotalsSP]
GO

CREATE PROCEDURE 	dbo.DN_GetInterfaceBreakdownTotalsSP
			@runno int,
			@branch int,
			@interfaceacctno varchar(10),
			@return int OUTPUT

AS
	select branchno,empeeno,acctno, datetrans, transtypecode, transvalue, transrefno, runno into #fintrans
   from fintrans
	where runno =@runno	
	and transtypecode not in ('CLD')			-- #9931
	
   -- potential problem is that you could notify a delivery with a date prior to its being securitised
   -- this would then be interfaced correctly but we need to work out whether the delivery occurred after securitisation
   -- in which case we need to set the delivery date to be the date of the previous summary run and export
   -- as this could be earlier than the securitisation run
   -- securitisation run 9-April 02:00 -account securitised
   -- delivery the following day but saved as delivered on 9-April 00:00
   -- this was picking up as before securitisation when in fact occurred after
   declare @dateSecinterface datetime,@datesummaryinterface datetime
   select @datesummaryinterface= datefinish from interfacecontrol where interface ='UPDSMRY' and runno =@runno

   create clustered index ix_fintrans_ser34cxv on #fintrans (acctno, transtypecode)
   -- here we are updating the delivery date to be the date notified on the system
   --this is so that we correctly calculate when actually delivered
   update #fintrans set datetrans =@datesummaryinterface
	where transtypecode IN ('del','grt') 

--   delivery.datedel = #fintrans.datetrans
--   and delivery.transvalue = #fintrans.transvalue
 

	DECLARE @branchno varchar(5), @split int

	if left (@interfaceacctno,3)='000' 
		set @split=0
   else
	   set @split = 1


	if left (@interfaceacctno,3) !='000' and @branch =0
   begin

		select @branch = branchno from branch where warehouseno = 
		case when SUBSTRING(@interfaceacctno,1,1) <> 0 and len(@interfaceacctno) > =7  THEN
		left (@interfaceacctno,3)
		ELSE right(left (@interfaceacctno,3),2)
		END


   end

	SET 	@return = 0
	SET	@branchno = convert(varchar, @branch) + '%'

	CREATE TABLE #tmpaccts(	transvalue money,
					code varchar(3),
					empeeno int,
					runno int,
					empeename varchar(101))	

   -- getting interface account #fintrans
	INSERT INTO 	#tmpaccts
	SELECT	SUM(f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a
	WHERE	a.acctno = f.acctno
	AND		f.runno = @runno
	AND		f.acctno like @branchno
	AND		(a.securitised != 'Y' or exists (select * from 
	 sec_account b where b.acctno = f.acctno and b.datesecuritised> f.datetrans))
	AND EXISTS(	SELECT transtypecode
			FROM	transtype t
			WHERE t.interfaceaccount = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode and isnull(t.branchsplit,@split) = @split )
	GROUP BY	f.transtypecode, f.empeeno, f.runno


   -- getting interface account SBL
	INSERT INTO 	#tmpaccts
	SELECT	SUM(f.transvalue),
						f.transtypecode,
			0,
			f.runno,
			''
	FROM		interface_financial f
	WHERE		f.runno = @runno
	and  F.transtypecode ='SBL'
	and f.interfaceaccount =@interfaceacctno
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacesecaccount = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode )
	GROUP BY	 f.runno,			f.transtypecode

   -- getting interface balancing SBL
INSERT INTO 	#tmpaccts
	SELECT	SUM(-f.transvalue),
			f.transtypecode,
			0,
			f.runno,
			''
	FROM		interface_financial f
	WHERE		f.runno = @runno
	and  F.transtypecode ='SBL'
	and f.interfaceaccount =@interfaceacctno
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacesecbalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode )
	GROUP BY	 f.runno,			f.transtypecode, f.interfaceaccount

   -- getting interface sec account #fintrans
	INSERT INTO 	#tmpaccts
	SELECT	SUM(f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a--, sec_account s
	WHERE	f.acctno = a.acctno
	--AND		f.acctno = s.acctno
	AND		f.runno = @runno
	AND		f.acctno like @branchno
	AND		a.securitised = 'Y'
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacesecaccount = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode and isnull(t.branchsplit,@split)= @split)
   and not exists (select * from sec_account s where f.acctno = s.acctno -- exclude transactions earlier than securitisation
			AND		f.datetrans <s.datesecuritised   and f.transtypecode not in ('INT','ADM') ) --except if interest/admin then include
	GROUP BY	f.transtypecode, f.empeeno, f.runno

	
	/*INSERT INTO 	#tmpaccts
	SELECT	SUM(f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a, sec_account s
	WHERE	f.acctno = a.acctno
	AND		f.acctno = s.acctno
	AND		f.runno = @runno
	AND		f.acctno like @branchno
	AND		a.securitised = 'Y'
	AND		f.datetrans < s.datesecuritised
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfaceaccount =  right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode)
	GROUP BY	f.transtypecode, f.empeeno, f.runno*/
   -- getting interface  account deposits
	INSERT INTO 	#tmpaccts
	SELECT	SUM(depositvalue),
			code,
			empeeno,
			runno,
			''
	FROM		cashierdeposits
	WHERE	runno = @runno
	AND		branchno like @branchno
	AND		voided != 'Y'
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfaceaccount =  right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND cashierdeposits.code =T.transtypecode and isnull(t.branchsplit,@split)= @split)

	GROUP BY	code, empeeno, runno
--aa 24 mar 2005
   -- getting interface  balancing deposits
	INSERT INTO 	#tmpaccts
	SELECT	SUM(-depositvalue),
			code,
			empeeno,
			runno,
			''
	FROM		cashierdeposits
	WHERE	runno = @runno
	AND		branchno like @branchno
	AND		voided != 'Y'
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacebalancing =  right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND cashierdeposits.code =T.transtypecode and isnull(t.branchsplitbalancing,@split) = @split)

	GROUP BY	code, empeeno, runno
--aa 24 mar 2005
   -- getting interface  balancing #fintrans income
	INSERT INTO 	#tmpaccts
	SELECT	SUM(-f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a
	WHERE	a.acctno = f.acctno
	AND		f.runno = @runno
	--AND		f.acctno like @branchno
	AND		(a.securitised != 'Y' or exists (select * from 
	 sec_account b where b.acctno = f.acctno and b.datesecuritised> f.datetrans))
	AND		f.branchno = @branch
	AND		f.transtypecode IN('PAY', 'COR', 'DPY', 'RET', 'REF','SHO','OVE')
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacebalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode and isnull(t.branchsplitbalancing,@split) = @split)
	GROUP BY	f.transtypecode, f.empeeno, f.runno
   -- getting interface  balancing #fintrans non income
   -- balancing based on account number rather than branch number as no physical cash actually taken
	INSERT INTO 	#tmpaccts
	SELECT	SUM(-f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a
	WHERE	a.acctno = f.acctno
	AND		f.runno = @runno
	AND		f.acctno like @branchno
	AND		(a.securitised != 'Y' or exists (select * from 
	 sec_account b where b.acctno = f.acctno and (b.datesecuritised> f.datetrans and f.transtypecode not in ('INT','ADM'))))
	--AND		f.branchno = @branch
	AND		f.transtypecode NOT IN('PAY', 'COR', 'DPY', 'RET', 'REF','SHO','OVE')
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacebalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode and isnull(t.branchsplitbalancing,@split)= @split)
	GROUP BY	f.transtypecode, f.empeeno, f.runno

   -- getting interfacesecbalancing #fintrans income
	INSERT INTO 	#tmpaccts
	SELECT	SUM(-f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a
	WHERE	a.acctno = f.acctno
	AND		f.runno = @runno
	--AND		f.acctno like @branchno
	AND		a.securitised = 'Y'
	AND		f.branchno = @branch
	AND		f.transtypecode IN('PAY', 'COR', 'DPY', 'RET', 'REF','SHO','OVE')
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacesecbalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        		AND F.transtypecode =T.transtypecode and isnull(t.branchsplitbalancing,@split) = @split)
   and not exists (select * from sec_account s where f.acctno = s.acctno
			AND		f.datetrans <s.datesecuritised and (f.transtypecode not in ('INT','ADM') and f.runno !=0))--except if interest/admin then include)

	GROUP BY	f.transtypecode, f.empeeno, f.runno

   -- getting interfacesecbalancing #fintrans non-income
	INSERT INTO 	#tmpaccts
	SELECT	SUM(-f.transvalue),
			f.transtypecode,
			f.empeeno,
			f.runno,
			''
	FROM		#fintrans f, acct a
	WHERE	a.acctno = f.acctno
	AND		f.runno = @runno
	AND		f.acctno like @branchno
	AND		a.securitised = 'Y'
	--AND		f.branchno = @branch
	AND		f.transtypecode NOT IN('PAY', 'COR', 'DPY', 'RET', 'REF','SHO','OVE')
	AND EXISTS(	SELECT transtypecode
			FROM transtype t
			WHERE t.interfacesecbalancing = right(@interfaceacctno, len(@interfaceacctno) - 3)
                        AND F.transtypecode =T.transtypecode and isnull(t.branchsplitbalancing,@split) = @split)
   and not exists (select * from sec_account s where f.acctno = s.acctno
			AND		f.datetrans <s.datesecuritised and f.transtypecode not in ('INT','ADM') )
	GROUP BY	f.transtypecode, f.empeeno, f.runno
       if @interfaceacctno = '0009998' --this is the error account retrieve details from interface_account cannot be bothered with the employee number
       begin
	INSERT INTO 	#tmpaccts (	transvalue,
					code ,
					empeeno ,
					runno ,
					empeename )
	select sum (f.transvalue), f.transtypecode, 0, f.runno ,''
        from interface_financial f
  	 where	f.runno = @runno and f.interfaceaccount= @interfaceacctno 
         group by f.transtypecode,  f.runno
      end


	UPDATE	#tmpaccts
	SET		empeename = c.Fullname
	FROM 		Admin.[User] c
	WHERE	c.Id = #tmpaccts.empeeno
        --final check to make sure that the transaction was actually interfaced
        delete from #tmpaccts where not exists (select * from 
          interface_financial f where f.runno =   #tmpaccts.runno and f.interfaceaccount=@interfaceacctno)
	SELECT	empeename + ' (' + convert(varchar(8), empeeno) + ') ' as EmployeeName,
			code,
			transvalue,
			runno,
			empeeno
	FROM		#tmpaccts

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
