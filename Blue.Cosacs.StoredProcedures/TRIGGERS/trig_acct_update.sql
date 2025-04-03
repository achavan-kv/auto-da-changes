--EXEC sp_helptext trig_acct_update

/* 69470 Debtors reconciliation Mauritius*/
IF EXISTS(SELECT name FROM sysobjects WHERE name = 'trig_acct_update' 
  	  		AND type = 'TR')

BEGIN
	DROP TRIGGER trig_acct_update
END
GO
CREATE TRIGGER trig_acct_update
ON  acct 
FOR update
AS
	
   DECLARE @acctno   char(12),
	   @agreementtotal money,
	   @error varchar (256),@arrears money,@oldarrears money, @count INT, @accttype CHAR(1),@balance MONEY,
	   @oldaccttype CHAR(1), @newtermstype VARCHAR(4), @oldtermstype VARCHAR(4),
	   @oldtotal MONEY, @newtotal MONEY , @lastupdatedby INT 
	   

   SELECT  @acctno = acctno,
           @agreementtotal = agrmttotal,@arrears = arrears,
		   @accttype = accttype,@balance =outstbal,
		   @newtermstype = termstype ,
		   @lastupdatedby = lastupdatedby,
		   @newtotal = agrmttotal
   FROM    inserted
   --69470 Prevent account type changing as causes problems with reconciliation - summary update control report
   SELECT @oldaccttype= accttype,
   @oldtermstype=termstype , @oldtotal = agrmttotal
   FROM deleted
   IF @accttype !=@oldaccttype AND ISNULL(@balance,0) <>0
   BEGIN
   	  -- we are only interested if account type changes between S, C and a credit account
	  IF @accttype NOT IN ('C','S') 
	     SET @accttype ='R' -- just call it generic credit account
	
	  IF @oldaccttype NOT IN ('C','S') 
	     SET @oldaccttype ='R'
	  -- now verify if changed
	  IF @accttype !=@oldaccttype
	  BEGIN
	  	 SET @error =' Account type of accounts with balances should not be changed raised by trigger trig_acct_update'
		 ROLLBACK
 		 RAISERROR(@error, 16, 1) 
	  END
   END
  
   select @count = count(*) from inserted
   SELECT  @oldarrears = arrears
   FROM    deleted  where acctno =@acctno

   if @oldarrears !=@arrears and (@arrears > 0 or @oldarrears > 0) and @count <2
   begin
      --  recording all changes in arrears  marking previous records dateto as current  date
      update ArrearsDaily  set dateto = dbo.StripTimeMinusSecond(GETDATE())
      where acctno =@acctno  and dateto  is null

      delete from arrearsdaily where dateto < datefrom and acctno = @acctno
      delete from arrearsdaily where datefrom =  dbo.StripTimeMinusSecond(getdate()) and acctno = @acctno
      insert into ArrearsDaily ( acctno, arrears,datefrom,dateto) 
      values (@acctno,@arrears,dbo.StripTimeMinusSecond(getdate()), NULL )
   end

   IF( @agreementtotal < -.01 and @acctno not like '___5%')  --Issue Number: 73 Version: 3.5.1.0 Title: UAT 36 - Legacy Cash & Go Return screen allow negative agrmttotals for special accounts as meaningless figure anyway on the account table

   BEGIN
         ROLLBACK
         SET @error =' agreement total should not be < 0 ' + @acctno + ' raised by trigger trig_acct_update'
 	 RAISERROR(@error, 16, 1) 
   END
  	--#14236 - The agreementAudit trail was updated incorrectly
GO
