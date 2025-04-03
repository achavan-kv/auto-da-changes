

-- **********************************************************************
-- Title:
-- Developer: Alex Ayscough
-- Date: 2006
-- Purpose: Trigger for insert of fintrans data 
-- 26 april 2007 changing so is an after trigger rather than instead of, this allows other triggers to fire

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 13/10/11 ip   CR1232 - Insert CLD (Cash Loan Disbursement) into fintrans_new_income for Cashiers Totals
-- 04/01/12 ip   #8778 - LW73615 - Duplicate key on insert error caused by code previously commented out.
-- 16/01/13 ip   CR11488 - #11762 - Merged from CoSACS 6.5
-- **********************************************************************
/*


All triggers combined as the instead of trigger seeemd to prevent the others firing 
Singapore Reverse DPY Transactions automatically*/
if exists (select * from sysobjects  where name =  'trig_fintrans_INSTEAD' )
drop trigger trig_fintrans_INSTEAD
go
create trigger trig_fintrans_INSTEAD
on fintrans INSTEAD OF INSERT 
as 
DECLARE @origbr smallint,
    @branchno smallint,
    @datetrans datetime,
    @empeeno int,
    @transvalue money,
    @bankcode varchar(20),
    @bankacctno varchar(20),
    @chequeno varchar(20),
    @ftnotes varchar(4),
    @paymethod smallint,
    @runno smallint,
    @acctno char(12),
    @transrefno int,
    @transtypecode varchar(3),
    @source varchar(10),
    @transprinted char(1),
    @transUPDATEd char(1), 
    @newtransrefno int,
    @num int,
	@agrmtno int 

SET NOCOUNT ON

SELECT @num = count(*) from inserted 
declare @countrycode varchar(2)
select @countrycode = countrycode from country
if @num = 1
begin
 SELECT
   @origbr = origbr,
   @branchno = branchno,
   @acctno = acctno,
   @transrefno = transrefno,
   @datetrans = datetrans,
   @transtypecode = transtypecode,
   @empeeno = empeeno,
   @transUPDATEd = transUPDATEd,
   @transprinted = transprinted,
   @transvalue = transvalue,
   @bankcode = bankcode,
   @bankacctno = bankacctno,
   @chequeno = chequeno,
   @ftnotes = ftnotes,
   @paymethod = paymethod,
   @runno = runno,
   @source = source,
   @agrmtno = agrmtno  
 FROM INSERTED

   delete from bdwpending where exists (select * from inserted f where
		f.acctno = bdwpending.acctno and f.transtypecode in ('PAY','DDN') and f.transvalue <0 )


--IP - 04/01/12 - #8778 - LW73615 - Re-instated the below previously commented out incorrectly. 
 if exists (SELECT * FROM fintrans 
           WHERE acctno =@acctno 
           AND branchno =@branchno 
           AND transrefno =@transrefno 
           AND datetrans =@datetrans)
 begin           
 
-- potential duplicate key --was happening in the delivery screens this is a hack really, but it does the job
-- this should not already exist -- AA removing as causing locking ouT - updating totals in trig_delivery_insertupdate instead
	SELECT @newtransrefno = transrefno FROM delivery 
	WHERE  acctno = @acctno AND transvalue = @transvalue  
	AND    not exists (SELECT * FROM fintrans f WHERE f.acctno = delivery.acctno 
			   AND f.branchno = delivery.branchno AND f.transrefno = delivery.transrefno )

   	if @newtransrefno is null
       		SELECT @newtransrefno = transrefno FROM delivery WHERE acctno = @acctno 
       		AND	not exists (SELECT * FROM fintrans f WHERE f.acctno = delivery.acctno 
				    AND f.transrefno = delivery.transrefno )
   	if @newtransrefno is null 
	   begin
	       update branch set hirefno = hirefno + 1 where branchno = @branchno

	       select  @newtransrefno = hirefno from branch where branchno= @branchno
	   end


   	set @transrefno= @newtransrefno
 end


 IF @transrefno IS NOT NULL
 BEGIN
	insert into fintrans (origbr,branchno,acctno,transrefno,
	    datetrans,    transtypecode,    empeeno,    transUPDATEd,
	    transprinted, transvalue,    bankcode,    bankacctno,
chequeno,     ftnotes,    paymethod,    runno,    source, agrmtno)  
	values    (@origbr,@branchno,@acctno,@transrefno,
	    @datetrans,    @transtypecode,    @empeeno,    @transUPDATEd,
	    @transprinted, @transvalue,    @bankcode,    @bankacctno,
	    @chequeno,     @ftnotes,    @paymethod,    @runno,    @source, @agrmtno)
  -- for cashier totals
  if exists	(select * from inserted where (transtypecode in ('PAY', 'REF', 'RET', 'COR','DPY' )) or   
  (transtypecode in ('STR','SCT') and acctno not like '___9%' )   )
  begin   
	 
     insert into fintrans_new_income (acctno, transvalue, transrefno, paymethod, 
												datetrans, empeeno, branchno, bankcode,
											   bankacctno,chequeno, transtypecode)
      select acctno, transvalue, transrefno, paymethod, 
          datetrans, empeeno , branchno ,bankcode,
			   bankacctno,chequeno, transtypecode
      from inserted
	   where  (transtypecode in ('PAY', 'REF', 'RET', 'COR','DPY' )) or  
  ( transtypecode in ('STR','SCT') and acctno not like '___9%' )
	   and not (paymethod =5 and @countrycode ='M') -- do not insert for standing order in Mauritius or performance will be adversly affected
   end
 END
end
else -- multiple rows, don't check
begin
	insert into fintrans (origbr,branchno,acctno,transrefno,
	    datetrans,    transtypecode,    empeeno,    transUPDATEd,
	    transprinted, transvalue,    bankcode,    bankacctno,
chequeno,     ftnotes,    paymethod,    runno,    source, agrmtno)  
	select origbr,branchno,acctno,transrefno,
	   datetrans,    transtypecode,    empeeno,    transUPDATEd,
	   transprinted, transvalue,    bankcode,    bankacctno,
chequeno,     ftnotes,    paymethod,    runno,    source, agrmtno from inserted
  if exists	(select * from inserted where transtypecode in ('PAY', 'REF', 'RET', 'COR','DPY' ) or  
  ( transtypecode  in ('STR','SCT') and acctno not like '___9%' ))
  begin     -- for cashier totals
    insert into fintrans_new_income (acctno, transvalue, transrefno, paymethod, 
												datetrans, empeeno, branchno, bankcode,
											   bankacctno,chequeno, transtypecode)
    select acctno, transvalue, transrefno, paymethod, 
          datetrans, empeeno , branchno ,bankcode,
			   bankacctno,chequeno, transtypecode
    from inserted
    where  (transtypecode in ('PAY', 'REF', 'RET', 'COR','DPY' )) or  
  ( transtypecode in ('STR','SCT') and acctno not like '___9%' )
  end
end

if update (transvalue) 
begin
   declare @amount money,@maximum_value money,
    --,@transtypecode varchar (3), @paymethod smallint,
   @error varchar (128),
   --@acctno char(12),
   @code varchar (4),
   --@branchno smallint,@transvalue money,
   @total money,
   --@transrefno integer,
   @status integer
   --@chequeno varchar (20),@datetrans datetime
   set @status = 0   
   select @transtypecode = transtypecode ,      @transvalue = transvalue,
          @paymethod = paymethod,       @acctno = acctno,
          @transrefno = transrefno,     @chequeno = chequeno,
          @datetrans = datetrans
   from inserted 
   if not exists (select * from transtype where transtypecode =@transtypecode)-- and isdeposit = 0) This is shit. Doesn't work for CLD. Don't know why it's here and could possible break everything. Sorry if you are fixing this.
   if @transvalue != 0
   begin
     if abs(@transvalue)  >0.01
     begin
       set @error ='transaction type ' + @transtypecode + ' does not exist in transtype table-please insert (value)' + convert (varchar,@transvalue) + ' raised by trig_fintrans_instead '
       set @status = 1
       RAISERROR (@error, 16, 1)	
     end
   end
   select @branchno = branchno from inserted where not exists (select branchno from 
   branch where branch.branchno = inserted.branchno)
   if @@rowcount > 0
   begin
       set @error ='invalid branch ' + convert (varchar,@branchno) + ' does not exist in branch table' + ' raised by trig_fintrans_instead '
       set @status = 1
       RAISERROR (@error, 16, 1)	
   end


   select @code = code, @maximum_value = abs(convert (money,codedescript)) 
   from code where category ='MAXF' and code =@transtypecode
   if @code !='' and @code is not null
   begin
   select @amount = transvalue,@acctno = acctno from inserted where abs(transvalue)> @maximum_value
      if @@rowcount > 0
      begin
        set @error ='transaction value for ' + @transtypecode + ' and account number ' +@acctno + ' exceeds limit' + ' raised by trig_fintrans_instead '
        RAISERROR (@error, 16, 1)
        set @status = 1
      end
  end
  if @transtypecode in ('COR','REF','RET','SCX')  AND @acctno not like '___5%'
  begin

      select @total = isnull (sum (transvalue), 0) from fintrans where
      acctno =@acctno and transtypecode in (N'PAY',N'COR',N'XFR',N'JLX',N'SCX',N'REF',N'RET',N'DDE',N'DDN',N'DDR','OVE','DPY','ADX')
      if @total  > 0.01 
      begin
        set @error ='transaction value for ' + @transtypecode + ' and account number ' +@acctno + ' exceeds payment amount' + ' raised by trig_fintrans_instead '
        RAISERROR (@error, 16, 1)
        set @status = 1
      end
  end
  if (@transvalue < 0 and @transtypecode in ('COR','RET','REF')) OR
     (@transvalue > 0 and @transtypecode in ('PAY','DDN','DDR','DDE'))
  begin
        set @error ='transaction value for ' + @transtypecode + ' is incorrectly signed for ' +@acctno + ' raised by trig_fintrans_instead '
        RAISERROR (@error, 16, 1)
        set @status = 1
  end
  if @transtypecode in ('PAY', 'COR', 'REF') and @paymethod = 0
  begin
        set @error =' Paymethod for ' + @transtypecode + ' cannot be zero ' + @acctno  + ' raised by trig_fintrans_instead '
        RAISERROR (@error, 16, 1)
        set @status = 1
  end
  if @status = 0 and @transtypecode = 'ADD'-- For add tos 
  BEGIN
      if not exists (select * from fintransAddtos where acctnocheq=@chequeno and transrefno=@transrefno
		and acctno=@acctno and datetrans=@datetrans) --only insert on insert if update record will already be present
		BEGIN
		      insert into fintransAddtos (acctnocheq, transrefno, transvalue, acctno, datetrans)
			values (@chequeno,@transrefno,@transvalue,@acctno,@datetrans)	
		END
  END
  if @status = 0 and @transtypecode = 'RFN' AND @transvalue >0 -- refinance... 
  BEGIN
	 IF RIGHT(@acctno,1) ='2' -- this is a refinance serial 2 account number so cheq will be serial 1
	 BEGIN
	 	SET @chequeno  = LEFT(@acctno,11) + '1'
	 END
     if not exists (select * from fintransAddtos where acctnocheq=@chequeno and transrefno=@transrefno
		and acctno=@acctno and datetrans=@datetrans) --only insert on insert if update record will already be present
		BEGIN
		    insert into fintransAddtos (acctnocheq, transrefno, transvalue, acctno, datetrans)
			values (@acctno,@transrefno,@transvalue,@chequeno,@datetrans)	
		END
  end

end

   DECLARE @new_source varchar(10),
   	   @new_acctno   char(12),
	   @new_transrefno int

   SELECT  @new_source = source,
           @new_acctno   = acctno,
	   @new_transrefno = transrefno
   FROM    inserted

   --Check if source is not equal to COSACS or COASTER (all upper case)
   --RM REMOVE AS THIS IS ALREADY IN CONSTRAINT -- ip - 16/01/13
   --IF  	not (  (@new_source = 'COSACS' and ascii(right(left(@new_source,2),1)) = 79)
   -- 	    or (@new_source = 'COASTER'))
   --BEGIN 
   --      ROLLBACK
   --      SET @error = 'Please report this error to CoSACS Support Centre, ensure you supply the script that has just been run. ' +
		 --     'Source must be COSACS or the SUCR breakdown will be incorrect. Raised by trigger: trig_fintrans_instead.'
 	 --RAISERROR(@error, 16, 1) 
   --END
-- Now we are going to reverse the transaction if DPY and singapore.....
declare    @name varchar(60), @custid varchar (20)
select @countrycode = countrycode from country
select @transtypecode = transtypecode ,
       @acctno = acctno
from inserted
declare @statement varchar (500)
if (@transtypecode in ('PAY','RET','XFR','REF','COR') AND @acctno like '___5%') OR @transtypecode ='DPY'  
begin
  if @transtypecode ='DPY' and @acctno not like '___5%'
  begin
      set @statement ='DPY recovery transactions only allowed for special accounts error raised by trig_fintrans_dpyreverse'
      RAISERROR (@statement, 16, 1)	   
  end
  select @custid = custid from custacct where acctno =@acctno
  if @custid not like 'PAID & TAKEN%' -- for performance
     select @name = name from customer where custid =@custid
  ELSE
     set @name =''
  if @custid IN ('BDWRECOVER', 'BDWSECRECOVER') OR @NAME ='BAD DEBTS RECOVERY' -- Post Reversing transaction
  begin
     
     if @countrycode in ('H','S')
     begin
        select
          @origbr = origbr,
          @branchno = branchno,
          @acctno = acctno,
          @transrefno = transrefno,
          @datetrans = datetrans,
          @empeeno = empeeno,
          @transupdated = transupdated,
          @transprinted = transprinted,
          @transvalue = transvalue,
          @bankcode = bankcode,
          @bankacctno = bankacctno,
          @chequeno = chequeno,
          @ftnotes = ftnotes,
          @paymethod = paymethod,
          @runno = runno,
          @source = source,
		  @agrmtno = agrmtno
        from inserted
 --       Removing as causing locking problems.....   
 --       update branch set hirefno = hirefno + 1 where branchno = @branchno
 --       select @transrefno = hirefno from branch where branchno = @branchno
        insert into fintrans
        (     origbr,    branchno,    acctno,    transrefno,
          datetrans,    transtypecode,    empeeno,    transupdated,
          transprinted,    transvalue,    bankcode,    bankacctno,
          chequeno,    ftnotes,    paymethod,    runno,
          source, agrmtno)
        values
        (    @origbr,    @branchno,    @acctno,    @transrefno,
          dateadd(minute,1,@datetrans),    'DPR',    @empeeno,    @transupdated,  -- now using same transrefno but add 1 minute to transaction to prevent duplicate key on insert
          @transprinted,    -@transvalue/* reversed */,    @bankcode,    @bankacctno,
          @chequeno,    @ftnotes,    @paymethod,    @runno,
          @source, @agrmtno)
     end
  end
end



go
