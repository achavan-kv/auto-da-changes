IF exists ( SELECT * FROM sysobjects WHERE name like 'dn_ArrearslettersAndCharges')
		drop procedure dn_ArrearslettersAndCharges
go

create procedure dn_ArrearslettersAndCharges 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : dn_ArrearslettersAndCharges.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Generate Arrears Letters and Charges
-- Author       : Alex Ayscough
-- Date         :??
--
-- This procedure UPDATEs the chargesdata table to store interest and admin charges. 
-- It then generates letters for accounts in arrears and apply interest and admin charges monthly
-- to the fintrans table. Interest charges are accumulated and stored weekly on the database tables.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 10/07/08  RDB uat274 round all interest calcs 
-- 68933     AA  Status codes not getting UPDATEd on the accounts as downstatus wasn't updating the account
--               or INSERTing INTO the status tables
-- 20/06/07 Jec  CR907 Instant Credit Approvals. UPDATE Customer.InstantCredit to 'N' IF conditions not met.
-- 05/09/07 Jec  CR906 Loan QualIFication. UPDATE Customer.LoanQualIFied to '0' IF Instant Credit conditions not met.
-- 22/11/07 AA   UAT 178 - removing duplicate status insert record.
-- 16/01/07 AA   --69500 Arrears Processing - reset the datenext due of accounts where they have become out of synch - also fixing year end problem
-- 14/08/08 AA   Preventing divide by zero error on test. 
-- 27/08/08 IP   (70038) - The INT charges applied by the letters and charges routine will now be rounded to (2dp).
-- 11/11/08 jec  70409 Include 69620 change from 5.0.5.8 
-- 27/06/09 RM   71474 using afterArrears (0) instead of arrears
-- 21/04/10 IP   UAT(20) UAT5.2 - Two new Country Parameters added which determine if charges and letters are generated or not.
-- 22/09/11 jec  #3296 LW73285 only decrease status if agreement term has not expired.
-- ================================================
	-- Add the parameters for the stored procedure here

			@runno int,
			@return int OUTPUT
as
declare @rundate datetime
set nocount ON-- change this to on  later

set @return =0
declare @lastrunno int,@debug smallint,@rowcount INT

--IP - 14/01/2008 - (69490) store the value of the Country Parameter (Hide Cents)
DECLARE @nocents VARCHAR(8)
SELECT @nocents = value FROM CountryMaintenance WHERE CodeName = 'nocents'

--IP - 21/04/10 - UAT(20) UAT5.2
DECLARE @GenCharges VARCHAR(8)
SELECT @GenCharges = VALUE FROM dbo.CountryMaintenance WHERE codename = 'GenCharges'

DECLARE @GenLetters VARCHAR(8)
SELECT @GenLetters = VALUE FROM dbo.CountryMaintenance WHERE codename = 'GenLetters'

-- doing warranty renewals......
exec DN_AccountGetForRenewalSP @acctno ='',  @iscurrentsettled =0, 	     @ismenucall =0, @custid =''


set @debug =1 -- 1 for debug (messages) 0 for live

if @debug= 0
  set nocount on
select @lastrunno =lastchargesweekno from country

IF @lastrunno = 0 -- as we are updating with previous week if new year then needs to be set to last week of previous year i.e. normally 52 69500
   SELECT @lastrunno=lastyrlastweekno FROM country
-- remove the current run if it failed
delete from chargesdata where runno = @runno 
delete from downstatus where runno = @runno			-- 69620 
delete from addtoletter where runno = @runno		-- 69620	

declare @oldestrunno int, @rundaily varchar(8) ,@lastyearno int
--select @rundaily =value from countrymaintenance where name ='Letters and Charges Daily Run'
select @rundaily =value from countrymaintenance where CodeName ='lettersdailyorweekly'		-- jec 22/09/11 should be using codename
if @rundaily ='True' -- daily
begin
   set @oldestrunno =@runno -20
   if @oldestrunno > 10 -- want to make sure we haven't just passed a year end
   begin --
       delete from chargesdata where runno <=@oldestrunno
       delete from addtoletter where runno <=@oldestrunno
       delete from downstatus where runno <=@oldestrunno
   end
end
else
begin -- weekly process
    set @oldestrunno = @runno - 10
	-- 69620 change copied from 5.0.5.8	
   -- if @lastrunno < 1 -- remove last years runs
   -- begin
   --     select @lastyearno=lastyrlastweekno from country
   --     set @oldestrunno = @lastyearno-@oldestrunno
   --    delete from chargesdata where runno >=@oldestrunno --so remove last years week numbers....
   --    delete from addtoletter where runno >=@oldestrunno
   --     delete from downstatus where runno >=@oldestrunno
   -- end
end

declare @usecurrentdate varchar(8)
--select @usecurrentdate =value from countrymaintenance where name ='Use Current Date & Time for Letters and Charges'
select @usecurrentdate =value from countrymaintenance where CodeName ='letterscurrentdateandtime'		-- jec 22/09/11 should be using codename
if @usecurrentdate='True' OR @rundaily ='True' 
	set @rundate = getdate()
else
begin  --using previous Sunday's date as the old OpenROAD version used to do.
	select @rundate =dateadd(week,@runno-1,datechargesstart) from country
        if dateadd(day,-2,@rundate)  > getdate()
        begin
        
            INSERT INTO interfaceerror
				    (interface, runno, errordate, severity,ErrorText)
				    SELECT 'Charges',@runno, GETDATE(),'F','Letters and Charges run too early.  Aborted.'
    		
		    RAISERROR('Letters and Charges run too early.  Aborted.', 16, 1)
		    RETURN 1
        end
end


/* to do addtoletter         status = addtoletter.updatefromcharges(weekno = weekno,
                  branchno = :branchset[i].branchno,
                  @lastrunno = @lastrunno,
                 rundate = rundate) */



/*" Interest will be calculated daily and accumulated separately.
The rate charged will be MPR/4.33 (4.33 being the number of weeks per month).The Accumulated Interest will be added to the Intchargesdue table and
the Intchargescumul column will be increased for this account by from the previous run's intchargescumul + the value of the latest week overdue.
Accumulated Interest = ($100 x 2/100) / 4.33 = $0.46 on 8/2/99 */

              /* Hp Accounts */
   declare @mindays smallint
   select @mindays = isnull(convert(smallint,value),7) from  countrymaintenance
   where name ='Minimum number of days in Arrears before Letter'


   --69500 Arrears Processing - reset the datenext from last week where they have become out of synch. 
    UPDATE  chargesdata 
    SET datenextdue = @rundate
    FROM instalplan i,acct a 
    WHERE 
    i.acctno =  chargesdata.acctno AND a.acctno = chargesdata.acctno AND
     (( a.currstatus = '1' AND afterarrears > i.instalamount AND a.arrears > i.instalamount) 
      OR (a.currstatus = '2' AND afterarrears > i.instalamount*2 AND a.arrears > i.instalamount*2) 
      OR (a.currstatus = '3' AND afterarrears > i.instalamount*3 AND a.arrears > i.instalamount*3)) 
      and chargesdata.runno = @lastrunno AND datenextdue > @rundate


   declare @datefirstdue datetime
   set @datefirstdue = dateadd(day,@mindays,@rundate)

    insert into chargesdata
        (ACCTNO, Lettercode ,  Arrears ,     AfterArrears, 
         instalment,    oldstatus,       Currstatus,    datenextdue, 
         runno  ,   IntChargesDue , IntChargesCumul,     IntChargesApplied, 
         AdmChargesApplied,    TrcChargesApplied, Agrmttotal,    mpr, 
         instalno,    isnew, instalamount,    deposit     )  
       select 
      a.acctno, '',   a.arrears,      0, 
     i.fininstalamt,      a.currstatus, '' , @datefirstdue , 
     @runno ,      0,   0,      0,  
     0,      0,  a.agrmttotal - g.deposit - g.servicechg,      0, 
     i.instalno,      1,  i.instalamount,      g.deposit 
     from acct a, instalplan i, agreement g, country c where
     a.outstbal > 0.001 and a.arrears >0.01 AND  a.currstatus not in ( '6','9','S' ) 
     AND  (a.arrears + .1* i.instalamount > i.instalamount or 
     @rundate    > i.datelast) AND  
     i.acctno = a.acctno AND  
     g.acctno =a.acctno AND  i.instalamount>0 AND 
     g.agrmtno = 1 AND i.agrmtno = 1 and g.acctno like '___0%'
     set @rowcount = @@rowcount
if @debug =1
    Print 'Added ' + convert(varchar,@rowcount)+ 'Hp Accounts in arrears'



    insert into chargesdata
        (ACCTNO, Lettercode ,  Arrears ,     AfterArrears, 
         instalment,    oldstatus,       Currstatus,    datenextdue, 
         runno  ,   IntChargesDue , IntChargesCumul,     IntChargesApplied, 
         AdmChargesApplied,    TrcChargesApplied, Agrmttotal,    mpr, 
         instalno,    isnew, instalamount,    deposit     )  
     select a.acctno,'',a.arrears,0,
          0,a.currstatus,'', @datefirstdue,
        @runno,0,0,0,
        0,0,a.agrmttotal,0,
        0,1,0,0
         from acct a, agreement g,  country c where 
         a.outstbal > 0.001 and a.arrears >0.01  AND 
         g.agrmtno = 1 AND 
         g.acctno = a.acctno AND 
         a.acctno like '___4%'
set @rowcount = @@rowcount
if @debug =1
         Print 'Added ' + convert(varchar,@rowcount)+ 'Cash Accounts'


       update chargesdata set deposit = chargesdata.instalamount,
                      agrmttotal = chargesdata.agrmttotal - chargesdata.instalamount 
         from acct , termstype
          where 
            acct.acctno = chargesdata.acctno and 
            termstype.termstype = acct.termstype and
            termstype.instalpredel = 'Y' and 
            chargesdata.deposit = 0 and 
            chargesdata.acctno like  '___0%' and
            chargesdata.runno = @runno

set @rowcount = @@rowcount
if @debug =1
    Print 'updated ' + convert(varchar,@rowcount) + ' agrtotal on accounts with non-deposits '

IF(@GenCharges) = 'True' --IP - 21/04/10 - UAT(20) UAT5.2
BEGIN
     update chargesdata set mpr = isnull(TM_Account.mpr ,chargesdata.mpr)
                 from   TM_Account
 				 where chargesdata.mpr < .01 and CONVERT(Decimal(10,4),TM_Account.mpr) > 0.00 and  
                 chargesdata.acctno = TM_Account.account_number 

      -- recalculating mpr where it is too high or where revise agreement has changed mpr values
      ----update chargesdata set mpr = 0 where (runno = @lastrunno and  mpr > .05 ) or exists 
      ----(select acctno from fintrans where fintrans.acctno = chargesdata.acctno and 
      ----    datetrans > dateadd (month, - 1, getdate()) and transtypecode in ('DEL','GRT','ADD' ))

      update 
	      chargesdata 
	  set 
		  mpr = 0 
	  where 
          runno = @lastrunno and  mpr > .05 
	  
	  ;WITH AccountsMprUpdate 
		AS
		(
			select distinct f.acctno
			from fintrans f
			inner join chargesdata c1 on f.acctno = c1.acctno
			where f.datetrans > dateadd (month, - 1, getdate()) and f.transtypecode in ('DEL','GRT','ADD' )
		)
		
		update 
            chargesdata
		set 
            mpr = 0
		from 
        (
            SELECT c.acctno, c.runno
            FROM AccountsMprUpdate a inner join  chargesdata c on a.acctno = c.acctno 
        ) Data
        WHERE
            chargesdata.acctno = Data.acctno AND chargesdata.runno = Data.runno

       -- updatding the details from last week
-- uat274 rdb 10/07/08 round all interest calcs 
       update c set
            datenextdue = v.datenextdue,
             intchargescumul =ROUND( v.intchargescumul,2),
            arrears = v.afterarrears,
             mpr = v.mpr,
             isnew = 0
      from chargesdata v,chargesdata c  where  c.runno  =@runno and
            c.acctno = v.acctno and v.runno =@lastrunno
END --IP - 21/04/10 - UAT(20) UAT5.2



--        select mpr,datenextdue from chargesdata where mpr >0 and datenextdue <getdate() and runno = @runno
set @rowcount = @@rowcount

if @debug =1
    Print 'updated ' + convert(varchar,@rowcount) + ' date next due from last week'
         
    -- arrears used to be based on the lesser of either last week's arrears or the current arrears
    -- the idea being that the account should have been in arrears for a whole week.               
       update c set
          arrears =  v.afterarrears
        from chargesdata c, chargesdata  v where  v.runno  = @lastrunno and
         c.acctno = v.acctno and 
            c.runno = @runno 
          and  c.arrears >=v.afterarrears

    -- now what we are going to do is to update datenext due based on the current datemoved into arrears from the arrearsdaily table
    -- typically the account should be due a minimum amount from the first time he went into arrears
    -- this would mean that the accounts would qualify for a letter the first time that they went into arrears
 if @rundaily = 'true'
 begin
    if @debug = 1
        print 'daily run adding minimum days to datenextdue from arrearsdaily'
    update chargesdata set datenextdue =dateadd(day,@mindays ,a.datefrom ),isnew=0
    from arrearsdaily a
    where a.acctno = chargesdata.acctno and a.dateto is null and runno =@runno and isnew = 1
 end
         
        /* Giving accounts an extra 2 weeks to pay if they have  a bankers draft/direct debit set up
        Other accounts have seven days already.
        */
        update  chargesdata 
         set datenextdue = dateadd(day,7,datenextdue) 
         where isnew = 1 and 
         runno =@runno and acctno in ( SELECT acctno from ddmandate where (endDate is null or enddate > getdate()))

	set @rowcount = @@rowcount
	if @debug =1
        	Print 'Removed ' + convert(varchar,@rowcount) + ' Accounts only just in arrears with Direct Debits'
        /*updating if accounts for Guyana to be due immediately*/
       declare @countrycode char(1)
       select @countrycode =countrycode from country
       if @countrycode ='Y'
       begin
         update  chargesdata 
         set datenextdue = dateadd(day,-7,datenextdue) 
         where    isnew = 1 and 
         runno = @runno  and 
         exists ( SELECT 
              acctno from acct where termstype ='IF' and acct.acctno = chargesdata.acctno)
        if @debug =1
        	Print 'Reduced ' + convert(varchar,@rowcount) + ' If accounts datenext due '
       end  
        /*updating to be due immediately where arrears is above normal threshold 
         shouldn't really happenunless system stuff up or possibly large cheque bounced*/
         update  chargesdata 
             set datenextdue = dateadd(day,-14,datenextdue) 
             from acct a, instalplan i where 
             i.acctno =  a.acctno and a.acctno =  chargesdata.acctno and 
             ((a.currstatus = '1' and a.arrears > i.instalamount*2) 
             or (a.currstatus = '2' and a.arrears > i.instalamount*3) 
             or (a.currstatus = '3' and a.arrears > i.instalamount*4)) 
             and chargesdata.runno = @runno

set @rowcount = @@rowcount
if @debug =1
        Print 'Update ' + convert(varchar,@rowcount) + ' overdue Accounts to be due a letter immediately '        
 
   --69296 Problem with letters/status codes not being generated swiftly enough in Jamaica. 
    -- seems due date not being updated swiftly enough so now we are checking to see if
    -- any arrears movement in the last seven days and if so we will set the datedue to now so 
    -- will check status of account and send letter/change status of account as approriate
   DECLARE @lastdaterun datetime
   SELECT @lastdaterun=ISNULL(datefinish,DATEADD(DAY,-6,GETDATE())) --use 6 days if letters have not been run recently
   FROM interfacecontrol 
   WHERE interface='CHARGES' AND runno =@lastrunno
   AND datefinish > DATEADD(DAY,-8,GETDATE())
   if @lastdaterun IS NULL 
       SET @lastdaterun = DATEADD(DAY,-6,GETDATE()) 
   -- Removing the below 69432 
   /*UPDATE chargesdata SET datenextdue = @rundate -- so these will now be due
   WHERE runno = @runno 
   AND EXISTS (SELECT * FROM ArrearsDaily d WHERE d.Acctno = chargesdata.acctno AND d.datefrom >@lastdaterun)
   AND isnew = 0 -- don't do this for new accounts this week who always get a weeks grace
*/
    -- now calculate monthly percentage rate this is the average monthly percentage used for calculating 
    -- charges based on this and the arrears         
        --table variable 

IF(@GenCharges) = 'True' --IP - 21/04/10 - UAT(20) UAT5.2
BEGIN
        declare @mprtemp table (acctno char(12), mpr float )
        declare @acctno char(12), @mpr float,@counter int,@total int
        set @counter=0
        SELECT @total =count(a.acctno)
        FROM chargesdata  a 
        where runno = @runno and mpr <.0001 and a.agrmttotal >0 and a.acctno like '___0%'
        set nocount on
        DECLARE mpr_cursor CURSOR 
      	FOR SELECT 
        --top 500
        acctno
        FROM chargesdata  a 
        where runno = @runno and mpr <.0001 and a.agrmttotal >0 and a.acctno like '___0%'
        OPEN mpr_cursor
        FETCH NEXT FROM mpr_cursor INTO @acctno

        WHILE (@@fetch_status <> -1)
        BEGIN
           IF (@@fetch_status <> -2)
           BEGIN
            execute DN_mprcalcSP @acctno = @acctno,
                        @mpr = @mpr OUT,
                        @return = @return OUT

			-- Issue 69261 - 09/10/07
			-- Check if MPR is negative. If so set to 0 and write to interface error.
	
            IF @mpr < 0 
			BEGIN

				SET @mpr = 0	
 
				INSERT INTO interfaceerror
				(interface, runno, errordate, severity,ErrorText)
				SELECT 'Charges',@runno, GETDATE(),'w','Negative mpr calculation error - ' + @acctno 
			END


            insert into @mprtemp (acctno,mpr) values (@acctno,@mpr)
            set @counter=@counter +1
            if @counter%100 =0
            begin
                if @debug =1
                    Print 'mpr done ' + convert(varchar,@counter) + ' of ' + convert(varchar,@total) + ' ' + convert(varchar,@counter/@total * 100) + ' percent '
                
            end
       END
       FETCH NEXT FROM mpr_cursor INTO @acctno
       END
       CLOSE mpr_cursor
       DEALLOCATE mpr_cursor
        set nocount off   

            update chargesdata set mpr = m.mpr 
            from @mprtemp m
            where chargesdata.acctno = m.acctno and runno = @runno


     -- now loop round and do what exactly -- forgotten
/*        declare @acctno char(12), @mpr float,@return int
        DECLARE mpr_cursor CURSOR 
  	    FOR SELECT acctno
        FROM chargesdata  a 
        where runno = @runno and mpr <.0001 and a.agrmttotal >0 and a.acctno like '___0%'
        OPEN mpr_cursor
        FETCH NEXT FROM mpr_cursor INTO @acctno

        WHILE (@@fetch_status <> -1)
        BEGIN
           IF (@@fetch_status <> -2)
           BEGIN
                execute DN_mprcalcSP @acctno = @acctno
                        @mpr = @mpr OUT
                        @return = @return
                update chargesdata set mpr = @mpr where acctno = @acctno and runno = @runno
       END
       FETCH NEXT FROM mpr_cursor INTO @acctno
   END
   CLOSE mpr_cursor
   DEALLOCATE mpr_cursor
*/

   

/* This method actually applies charges to the database - into the fintrans table and
 updates the acct table with the balance.
1. For Cash Accounts applies the monthly interest
rate charge where date due has past and acctno still in arrears
2. For HP Accounts applies the cumulative charges then sets them to zero.
*/
/* calculate interest on accounts */
    exec  orintdailycharges @runno= @runno, @branchno =0,@rundate=@rundate
-- interest suspended or account frozen will cause interest charges due to be set to 0.
   -- vbranchno = varchar(branchno) + '0%'
   -- cbranchno = varchar(branchno) + '4%'
   
END --IP - 21/04/10 - UAT(20) UAT5.2

    /*insert into downstatus for all thouse accounts which are not in arrears
    - they will not be in the chargesdata table */
    insert into downstatus
        (acctno,
        newstatus,
        oldstatus,
        runno)
    select a.acctno,
        '',
        a.currstatus,
        @RUNNO
     from acct a, instalplan i
     where (a.arrears ) < i.instalamount and
          i.acctno = a.acctno and
          i.instalamount > 0.001 and
          a.currstatus in ('2','3','4','5') and
          (@rundate < i.datelast or a.outstbal < .01) 

-- reducing status 
     update downstatus set newstatus ='1' 
     from instalplan , acct
     where 
     acct.currstatus ='2' and
     acct.arrears  + .1 * instalamount < instalplan.instalamount and 
     acct.acctno = instalplan.acctno and instalplan.agrmtno = 1 and
     acct.acctno = downstatus.acctno and 
     runno = @runno and
     (@rundate  <instalplan.datelast  or acct.outstbal < .001)   
         
      
      update downstatus set newstatus ='1'  
      from instalplan , acct 
      where  
       acct.currstatus in ('3','4') and 
       acct.arrears + .1 * instalplan.instalamount  < instalplan.instalamount  and  
       acct.acctno = instalplan.acctno and  
       acct.acctno = downstatus.acctno and  
       newstatus = '' and  
       runno = @runno 
       and ( @rundate   <instalplan.datelast or acct.outstbal < .001 )

      declare @downSC5 varchar(6)         
      select @downSC5 =value from countrymaintenance where codename ='AutoDownSC5'
      
      if @downSC5='True'  -- reducing status code where in SC5 automatically
      begin
        update downstatus set newstatus ='2'  
         from instalplan , acct 
         where  
          acct.currstatus ='5' and 
          acct.arrears   < instalplan.instalamount  and  
          acct.acctno = instalplan.acctno and  
          acct.acctno = downstatus.acctno and  
            newstatus = '' and  
           runno =  @runno and ( 
           @rundate  <instalplan.datelast  
          or acct.outstbal < .001) 
      end      
          
   
      update acct 
      set currstatus =d.newstatus 
      from downstatus d
      where d.acctno =acct.acctno and d.runno =@runno  and d.newstatus !='' and acct.currstatus !=d.newstatus
      set @return =@@error

      /* AA - System UAT 178 removing this as status record would be added by trigger
      *insert into status (origbr,acctno,datestatchge,empeenostat,statuscode,daysinstatus,currentstatus)
      *select 0,acctno,getdate(),99999,d.newstatus,1,d.newstatus
      *from downstatus d
      *where d.runno =@runno  and d.newstatus !=''*/

 

set @rowcount = @@rowcount
if @debug =1
     Print 'HP No 2a' + convert(varchar,@rowcount) 
         
      update chargesdata set
      lettercode =''     , 
      currstatus = '2' 
      from acct , instalplan  where acct.currstatus ='4' and  /* used to be status in  3 and 4 */
      acct.arrears/2 < chargesdata.instalamount and 
      acct.outstbal > 0.001 and 
      chargesdata.runno = @runno and 
      acct.acctno = chargesdata.acctno    and 
      instalplan.acctno = chargesdata.acctno and
      @rundate < instalplan.datelast and 
      chargesdata.currstatus !='2' and
      isnew = 0 and
      instalplan.datelast>@rundate 
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 4' + convert(varchar,@rowcount)
    
-- returned cheques
      select max(f.datetrans) as fdate, 
      f.acctno, 
      max(a.datetrans) as rdate 
      into  #ftret
      from fintrans f, chargesdata c , fintrans a 
      where a.acctno = c.acctno 
      and f.acctno = c.acctno 
      and c.runno = @runno
      and a.transtypecode =  'RET'
      and f.transtypecode !=  'RET' 
      and c.isnew =0 
      group by f.acctno 
      -- Q letter second warning         
      update chargesdata set  
      currstatus = '3' , lettercode = 'Q'  
      from acct  
      where  
       chargesdata.oldstatus in ('2','4') and  
       acct.acctno =chargesdata.acctno and  
      (acct.arrears + .1 * chargesdata.instalamount)/3 < chargesdata.instalamount and  
      (acct.arrears +.1 * chargesdata.instalamount)/2 >=chargesdata.instalamount and  
       acct.outstbal > 0.001 and  
       chargesdata.runno =  @runno   and  
       chargesdata.datenextdue <=  @rundate   and  
       isnew = 0 and  
       exists    (select acctno from   #ftret f   where f.acctno = chargesdata.acctno and rdate > fdate)
set @rowcount = @@rowcount
if @debug =1
      Print 'HP No 5' + convert(varchar,@rowcount)

-- 69703 Mauritiaus arrears anomoly - date due in 8 weeks time

declare @firstofnextmonth datetime,@firstofthismonth datetime
set @firstofthismonth = dateadd(day,-datepart(day,@rundate)+1,@rundate)
set  @firstofnextmonth = dateadd(month,1,@firstofthismonth)

        --first letter for those who have missed one instalment 
        --their due date will be the day after next month's instalment
        update chargesdata set 
            currstatus = '2', 
            Lettercode= 'U' , 
            datenextdue = 
				case when -- if the datefirst day of this month is in less than 2 weeks time 
						dateadd(day,datepart(day,datefirst),@firstofthismonth)<dateadd(week,2,@rundate)
					then   -- use datefirst day of next month
						dateadd(day,datepart(day,datefirst),@firstofnextmonth)
					else -- use datefirst day of this moneh
						dateadd(day,datepart(day,datefirst),@firstofthismonth)
					end
        from acct , agreement,instalplan i
        where  acct.currstatus in ('1','U','0') and 
        (acct.arrears + .1* chargesdata.instalamount) >= chargesdata.instalamount and  
        acct.outstbal > 0.001 and  
        chargesdata.runno =  @runno  and  
        acct.acctno = chargesdata.acctno and acct.accttype <> 'R' and  
        agreement.agrmtno = 1 and  
        agreement.acctno = chargesdata.acctno and  
        chargesdata.datenextdue <=   @rundate 
       and  datediff(day,agreement.datedel,  @rundate  ) >=59 
       and i.acctno = agreement.acctno and i.agrmtno =1 
       and isnew=0
       
        update chargesdata set 
          currstatus = '2', 
          Lettercode= 'UF' , 
            datenextdue = case when -- if the datefirst day of this month is in less than 2 weeks time 
						dateadd(day,datepart(day,datefirst),@firstofthismonth)<dateadd(week,2,@rundate)
					then   -- use datefirst day of next month
						dateadd(day,datepart(day,datefirst),@firstofnextmonth)
					else -- use datefirst day of this moneh
						dateadd(day,datepart(day,datefirst),@firstofthismonth)
					end
           from acct , agreement  ,instalplan i
           where  
               acct.currstatus in ('1','U','0') and 
             (acct.arrears + .1* chargesdata.instalamount) >= chargesdata.instalamount and  
               acct.outstbal > 0.001 and  
               chargesdata.runno =  @runno  and  
               acct.acctno = chargesdata.acctno and acct.accttype = 'R' and  
               agreement.agrmtno = 1 and  
               agreement.acctno = chargesdata.acctno and  
               chargesdata.datenextdue <=   @rundate and  
               datediff(day,agreement.datedel,  @rundate  ) >=59 
	        and i.acctno = agreement.acctno and i.agrmtno =1        


       -- reducing status for customers in arrears who have made a payment
         update chargesdata set 
             currstatus = '2', 
             Lettercode= ''  
                from acct , agreement ,instalplan 
                where  
            acct.currstatus in ('3','4') and 
            (acct.arrears + (.1* chargesdata.instalamount))>= chargesdata.instalamount 
             and acct.arrears <= chargesdata.instalamount 
             and acct.outstbal > 0.001 
             and chargesdata.runno = @runno 
             and acct.acctno = chargesdata.acctno 
             and chargesdata.currstatus <> '2' 
             and agreement.agrmtno = 1
             and agreement.acctno = chargesdata.acctno
             and instalplan.acctno = chargesdata.acctno			-- #3296
             and instalplan.datelast >= @rundate				-- #3296
             


            /*This could happen any time c.datenextdue <=date(@rundate) */
set @rowcount = @@rowcount
if @debug =1
        Print 'HP No 6a' + convert(varchar,@rowcount)
         update chargesdata set
               currstatus = '2',
               Lettercode= 'U' 
         from acct , agreement 
         where acct.currstatus in ('1','0','U') and
          (acct.arrears + .1 *chargesdata.instalamount) > chargesdata.instalamount and 
           acct.outstbal > 0.001 and 
           chargesdata.runno = @runno and 
           acct.acctno = chargesdata.acctno and acct.accttype <> 'R' and 
           agreement.agrmtno=1 and
           agreement.acctno = chargesdata.acctno and 
           datediff(day,agreement.datedel,  @rundate)         >=59  and 
           chargesdata.datenextdue <  @rundate  
set @rowcount = @@rowcount
if @debug =1
            Print 'HP No 6b' + convert(varchar,@rowcount)
            
        update chargesdata set
            currstatus = '2',
            Lettercode= 'UF' 
        from acct , agreement 
        where acct.currstatus in ('1','U','0') and
          (acct.arrears + .1 *chargesdata.instalamount) > chargesdata.instalamount and 
           acct.outstbal > 0.001 and acct.accttype= 'R' and 
           chargesdata.runno = @runno and 
           acct.acctno = chargesdata.acctno and 
           agreement.agrmtno=1 and
           agreement.acctno = chargesdata.acctno and 
           datediff(day,agreement.datedel,  @rundate)         >=59  and 
           chargesdata.datenextdue <  @rundate  and 
           isnew = 0
set @rowcount = @@rowcount
if @debug =1
       Print 'HP No 6b' + convert(varchar,@rowcount)
       -- first letter for those who have got into arrears within 3 months of delivery RF           
       update chargesdata set
                     currstatus = '2',
                     Lettercode= 'JF' ,
         datenextdue = case when -- if the datefirst day of this month is in less than 2 weeks time 
						dateadd(day,datepart(day,datefirst),@firstofthismonth)<dateadd(week,2,@rundate)
					then   -- use datefirst day of next month
						dateadd(day,datepart(day,datefirst),@firstofnextmonth)
					else -- use datefirst day of this moneh
						dateadd(day,datepart(day,datefirst),@firstofthismonth)
					end
                 from acct , agreement ,instalplan i
                  where acct.currstatus in ('1','U','0') and 
                      (acct.arrears + .1 * chargesdata.instalamount) >= chargesdata.instalamount and 
                      acct.outstbal > 0.001 and 
                      chargesdata.runno = @runno and 
                      acct.acctno = chargesdata.acctno and acct.accttype = 'R' and  
                      agreement.agrmtno=1 and
                      agreement.acctno = chargesdata.acctno and 
                      chargesdata.datenextdue <= @rundate  and 
                       datediff(day,agreement.datedel,@rundate)  <59
                and i.acctno = agreement.acctno and i.agrmtno =1 

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 7 ' + convert(varchar,@rowcount)
       -- first letter for those who have got into arrears within 3 months of delivery  non RF
           update chargesdata set
                     currstatus = '2',
                     Lettercode= 'J' ,
         datenextdue = case when -- if the datefirst day of this month is in less than 2 weeks time 
						dateadd(day,datepart(day,datefirst),@firstofthismonth)<dateadd(week,2,@rundate)
					then   -- use datefirst day of next month
						dateadd(day,datepart(day,datefirst),@firstofnextmonth)
					else -- use datefirst day of this moneh
						dateadd(day,datepart(day,datefirst),@firstofthismonth)
					end
           from acct , agreement ,instalplan i
                  where acct.currstatus in ('1','U','0') and 
                      (acct.arrears + .1 * chargesdata.instalamount) >= chargesdata.instalamount and  
                      acct.outstbal > 0.001 and acct.accttype <>'R' and 
                      chargesdata.runno = @runno and 
                      acct.acctno = chargesdata.acctno and 
                      agreement.agrmtno=1 and
                      agreement.acctno = chargesdata.acctno and 
                      chargesdata.datenextdue <=@rundate  and 
                       datediff(day,agreement.datedel,@rundate)  <59
                   and i.acctno = agreement.acctno and i.agrmtno =1
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 7 B' + convert(varchar,@rowcount)
-- Arrears letter for employees/staff in arrears    
         update chargesdata 
        set   Lettercode= 'X' 
         from acct a, agreement b 
        where a.currstatus = '9' and 
        (a.arrears + .1* instalamount) >= instalamount and 
         a.outstbal > 0.001 and 
         chargesdata.runno = @runno  and 
         a.acctno = chargesdata.acctno and 
          b.agrmtno=1 and 
          b.acctno = chargesdata.acctno  and 
          chargesdata.datenextdue <= @rundate 
          and isnew = 0  
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 8' +  convert(varchar,@rowcount)

     update chargesdata set    currstatus = '3', Lettercode= 'Q' 
      from acct , agreement 
      where   acct.currstatus ='2' and
           (acct.arrears + .1 *chargesdata.instalamount)/2 >= chargesdata.instalamount and
            acct.outstbal > 0.001 and 
           chargesdata.runno = @runno  and 
          acct.acctno = chargesdata.acctno and 
                     agreement.agrmtno=1    and 
                     agreement.acctno = chargesdata.acctno and 
                       chargesdata.datenextdue <= @rundate  
   
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 9 ' + convert(varchar,@rowcount)
    
   update chargesdata set
   currstatus = '4',Lettercode= 'K' 
   from acct , agreement 
   where acct.currstatus in ('3','2') and 
                         acct.arrears/2   >= chargesdata.instalamount and 
                         acct.arrears/3  < chargesdata.instalamount and 
                         acct.outstbal > 0.001 and 
                         agreement.agrmtno=1 and 
                         chargesdata.acctno = acct.acctno and 
                         chargesdata.runno = @runno  and 
                         chargesdata.acctno = chargesdata.acctno    and 
                         agreement.acctno = chargesdata.acctno and 
                         datediff(month, agreement.datedel, @rundate ) <6 
                         and chargesdata.datenextdue <= @rundate
        
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 10' + convert(varchar,@rowcount)
      -- status code 5 reposession letter
     update chargesdata set currstatus = '5',
                     Lettercode= 'R' 
                     from acct , agreement 
                      where acct.currstatus ='4' and 
                           (acct.arrears    + .1*chargesdata.instalamount)/3 >= chargesdata.instalamount and 
                           acct.outstbal > 0.001 and 
                           chargesdata.runno = @runno  and 
                           acct.acctno = chargesdata.acctno and 
                           agreement.agrmtno=1 and 
                           agreement.acctno = chargesdata.acctno and 
                           chargesdata.datenextdue <= @rundate  


        
    -- k letter 3 instalments in arrears and has received arrears letter in last 90 days????
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 11' + convert(varchar,@rowcount)
       update chargesdata set
               currstatus = '4',
               Lettercode= 'K' 
               from acct , agreement 
                where acct.currstatus in ('2','3') and
               (acct.arrears + .1*chargesdata.instalamount)/3 >= chargesdata.instalamount and 
               acct.outstbal > 0.001 and 
               chargesdata.runno = @runno  and 
               acct.acctno = chargesdata.acctno and 
               agreement.agrmtno=1    and 
               agreement.acctno = chargesdata.acctno and 
               chargesdata.datenextdue <= @rundate  and 
               chargesdata.isnew =0 and 
               chargesdata.acctno in 
                   (select acct.acctno from letter , chargesdata 
                       where letter.acctno = chargesdata.acctno and 
                     datediff(day,dateacctlttr, @rundate ) <90 
                   and  chargesdata.runno =  @runno ) 

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 12' + convert(varchar,@rowcount)
--status code 3 without any letter seems strange?
        update chargesdata set    currstatus = '3',
                         Lettercode= '' 
             from acct , agreement 
               where acct.currstatus = '2' and
             (acct.arrears + .1 *chargesdata.instalamount)/3 >=chargesdata.instalamount  and 
             acct.outstbal > 0.001 and 
             chargesdata.runno = @runno  and 
             acct.acctno = chargesdata.acctno and 
             agreement.agrmtno=1 and 
             agreement.acctno = chargesdata.acctno and 
             datediff(month,agreement.datedel, @rundate ) <6 and 
             (chargesdata.datenextdue <= @rundate  ) and 
             lettercode ='' 

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 13' + convert(varchar,@rowcount)
    declare @smallbalance money
    select @smallbalance=convert(money,value) from countrymaintenance where name like 'Small Balance Amount'   
    -- l Letter is for expiry. Only get if arrears/balance >smallbalance parameter
        update chargesdata set    Lettercode= 'L',
                   currstatus = convert(varchar(2), convert(integer, acct.currstatus) +1)						     
                     from acct , instalplan 
                      where acct.currstatus in('1','2') and
                     acct.arrears  >=  @smallbalance  and 
                     acct.outstbal > 0.001 and 
                     chargesdata.runno = @runno  and 
                     acct.acctno = chargesdata.acctno    and 
                     instalplan.acctno = chargesdata.acctno and 
                      @rundate >= instalplan.datelast and 
                     chargesdata.datenextdue <= @rundate and chargesdata.instalment > 0 

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 14' + convert(varchar,@rowcount)
   -- still expired after a month movethemup to four instalments in arrears
        update chargesdata set    Lettercode= 'K',
                         currstatus = '4'
                     from acct , instalplan 
                      where acct.currstatus ='3' and
                     acct.outstbal > 0.001 and 
                     chargesdata.runno = @runno  and 
                     acct.acctno = chargesdata.acctno    and 
                     acct.arrears >  @smallbalance  and 
                     instalplan.acctno = chargesdata.acctno and 
                     datediff(day,instalplan.datelast, @rundate ) >= 29 
                     and (chargesdata.datenextdue <= @rundate  ) 

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 15' + convert(varchar,@rowcount)
         update chargesdata set    Lettercode= 'R',
                         currstatus = '5'
                     from acct , instalplan 
                      where acct.currstatus ='4' and
                     acct.arrears >=  @smallbalance   and 
                     acct.outstbal > 0.001 and 
                     chargesdata.runno = @runno  and 
                     acct.acctno = chargesdata.acctno    and 
                     instalplan.acctno = chargesdata.acctno and 
                     datediff(day,instalplan.datelast, @rundate  )  >=29 
                     and (chargesdata.datenextdue <= @rundate  ) 


       

set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 16' + convert(varchar,@rowcount)
    
/*update Guyana accounts to be due for repossession immediately if in arrears*/
         update chargesdata set    Lettercode= 'R',
                         currstatus = '5'
                     from acct , instalplan 
                      where acct.termstype='IF' and
                     acct.arrears >=  @smallbalance    and 
                     acct.outstbal > 0.001 and 
                     chargesdata.runno = @runno  and 
                     acct.acctno = chargesdata.acctno    and 
                     instalplan.acctno = chargesdata.acctno and 
                    (chargesdata.datenextdue <= @rundate  ) 
   -- privelege club customers get a more gentle reminder
    update chargesdata set lettercode = '8'
    where runno = @runno
    and lettercode in ('Q', 'K')
    and acctno in (select c.acctno from custacct c, custcatcode d
    where c.custid =d.custid and c.hldorjnt = 'H' and
    d.code= 'CLAC' and  (datedeleted is null or datedeleted= '01-Jan-1900')
    and c.custid not in (select custid from custacct, letter where hldorjnt = 'H'
    and letter.acctno = custacct.acctno and lettercode= '8 '))
set @rowcount = @@rowcount
if @debug =1
    Print 'HP No 16c' + convert(varchar,@rowcount)
    

/* CASH ACCOUNTS > Small balance limit
Cash accounts delivery date needs to be worked out differently from hp accounts.
The arrears calculation needs to be changed. Cash accounts delivery date
is the last delivery, unless the account is in arrears, in which case
the delivery date will not be changed.
Anyway the datenextdue is one month from the delivery date then 4 weeks  after that.
*/
--    vbranchno = varchar(branchno) + '4%'
        update chargesdata set 
                 currstatus = '2',Lettercode= 'C2' from acct  where acct.currstatus ='1' and
                  acct.outstbal >  @smallbalance  and chargesdata.acctno like '___4%'
             and chargesdata.runno = @runno  and acct.acctno = chargesdata.acctno
                 and ( @rundate  >= chargesdata.datenextdue ) 
        
       update chargesdata set 
             currstatus = '4',Lettercode= 'C4'
             from acct  where      acct.currstatus ='2' and
                       acct.outstbal >  @smallbalance  and 
                       chargesdata.runno = @runno  and 
                       acct.acctno = chargesdata.acctno and 
            @rundate  >= chargesdata.datenextdue  and chargesdata.acctno like '___4%'
        

        update chargesdata set 
             currstatus = '5',
             Lettercode= 'C5' 
             from acct where acct.currstatus = '4' and 
             acct.outstbal >  @smallbalance   and 
             chargesdata.runno = @runno   and 
             acct.acctno = chargesdata.acctno and 
             chargesdata.acctno like '___4%' and
            @rundate   >= chargesdata.datenextdue 
        
        update chargesdata set 
             Lettercode= 'C5' 
              from acct  where 
              acct.currstatus ='5' and 
               acct.outstbal >  @smallbalance   and 
              chargesdata.runno = @runno   and 
             acct.acctno = chargesdata.acctno and
              chargesdata.acctno like '___4%' and
            @rundate   >= chargesdata.datenextdue 

    /* less than small balance but  > 50% delivery amt */
	--Cashloan Amortization CR, will update in case of non-amortized accounts 18/06/2019
     update chargesdata set 
                 currstatus = '2',Lettercode= '' 
             from acct  where acct.currstatus in ('1','2','3','4') 
             and acct.outstbal < @smallbalance
             and chargesdata.runno = @runno
             and acct.acctno = chargesdata.acctno
             and acct.outstbal > acct.agrmttotal/2
			 and acct.isAmortized=0

	--Cashloan Amortization CR, will update in case of amortized accounts 18/06/2019	
	update chargesdata set 
                 currstatus = '2',Lettercode= '' 
             from acct ,cashloan  where acct.currstatus in ('1','2','3','4') 
             and acct.outstbal < @smallbalance
             and chargesdata.runno = @runno
             and acct.acctno = chargesdata.acctno
			 and acct.acctno = cashloan.acctno
             and acct.outstbal > (cashloan.LoanAmount)/2
			 and acct.isAmortized = 1 and acct.isAmortizedOutStandingBal = 1

set @rowcount = @@rowcount
if @debug =1
    Print 'Stage 18'
	--Cashloan Amortization CR, will update in case of non-amortized accounts 18/06/2019
        update chargesdata set 
                 currstatus = '5',Lettercode= 'C' 
                 from acct  where acct.currstatus ='5' and
                  acct.outstbal <  @smallbalance   
                 and acct.acctno like '___4%'
                 and chargesdata.runno = @runno   and acct.acctno = chargesdata.acctno
                 and acct.outstbal > acct.agrmttotal/2
                 and  @rundate  >= chargesdata.datenextdue and isnew=0
				 and acct.isAmortized = 0

	--Cashloan Amortization CR, will update in case of amortized accounts 18/06/2019
		update chargesdata set 
                 currstatus = '5',Lettercode= 'C' 
             from acct,cashloan  where acct.currstatus ='5' and
                  acct.outstbal <  @smallbalance   
                 and acct.acctno like '___4%'
                 and chargesdata.runno = @runno   
				 and acct.acctno = chargesdata.acctno
				 and acct.acctno = cashloan.acctno
                 and acct.outstbal > (cashloan.LoanAmount)/2
                 and  @rundate  >= chargesdata.datenextdue and isnew=0
				 and acct.isAmortized = 1 and acct.isAmortizedOutStandingBal = 1

	--Cashloan Amortization CR, will update in case of non-amortized accounts 18/06/2019
     update chargesdata set 
                 currstatus = '9',Lettercode= '' from acct  where acct.currstatus ='9' and
                  acct.outstbal <  @smallbalance   and acct.acctno like '___4%'
                 and chargesdata.runno = @runno   and acct.acctno = chargesdata.acctno
                 and acct.outstbal > acct.agrmttotal/2
                 and  @rundate   >= chargesdata.datenextdue and isnew=0
				 and acct.isAmortized = 0

	 --Cashloan Amortization CR, will update in case of amortized accounts 18/06/2019
	 update chargesdata set 
                 currstatus = '9',Lettercode= '' from acct,cashloan  
				 where acct.currstatus ='9' and
                 acct.outstbal <  @smallbalance   and acct.acctno like '___4%'
                 and chargesdata.runno = @runno   and acct.acctno = chargesdata.acctno
				 and acct.acctno = cashloan.acctno
                 and acct.outstbal > (cashloan.LoanAmount)/2
                 and  @rundate   >= chargesdata.datenextdue and isnew=0
				 and acct.isAmortized = 1 and acct.isAmortizedOutStandingBal = 1

    /* Printing customer statement for all accounts where
    no change in arrears position - so no other letters - unless for SC 5*/
--    vbranchno = varchar(branchno) + '%'

     update chargesdata  set 
             Lettercode= '01'  from acct a
             where runno =  @runno 
             and chargesdata.lettercode = '' and 
             chargesdata.oldstatus in ('2','3','4','5') and  /* FR 1205 SQL */
             not (chargesdata.currstatus ='2' and chargesdata.oldstatus ='3') 
             and a.acctno = chargesdata.acctno 
             and   @rundate   >= chargesdata.datenextdue and chargesdata.isnew = 0 
             and datediff(day,a.datelastpaid, @rundate   ) <29   /* FR 1205 SQL */
        

        update chargesdata  set 
                Lettercode= '0'  from acct a
              where runno =  @runno 
              and chargesdata.lettercode ='' and 
             chargesdata.oldstatus in ('2','3','4','5') and  /* FR 1205 SQL */
             not (chargesdata.currstatus ='2' and oldstatus ='3') 
             and a.acctno = chargesdata.acctno 
             and @rundate >= chargesdata.datenextdue and chargesdata.isnew = 0 
--             and chargesdata.acctno like  $q  :vbranchno  $q 
             and datediff(day,a.datelastpaid, @rundate)   >29 /* FR 1205 SQL */
set @rowcount = @@rowcount
if @debug =1
    Print'Stage 20'
    -- Frozen accounts will receive no letters
    update chargesdata set lettercode ='',
                currstatus ='' where acctno in
    (select b.acctno from acctcode a, spa b where a.acctno =b.acctno
    and a.code ='F' and b.dateadded <= @rundate and
    (b.dateexpiry >@rundate or b.dateexpiry ='' or b.dateexpiry is null) )
    
    update chargesdata set lettercode ='',
                currstatus ='' where acctno in
    (select a.acctno from acctcode a where
    a.code ='F' and  
    a.datedeleted = '' or a.datedeleted is null and a.acctno
    like '___4%')
    and acctno like '___4%'
    /* Clearing any previous charges applied/due */

IF(@GenCharges) = 'True'     --IP - 21/04/10 - UAT(20) UAT5.2
BEGIN 
    update chargesdata  set intchargesapplied = 0
       where
     datenextdue <=@rundate and
     isnew = 0 and
     runno = @runno and
     acctno like '___0%'

    /* Calculating charges on current arrears if less than
       previous weeks arrears - taken from acct table */
-- uat274 rdb 10/07/08 round all interest calcs 
    --update chargesdata  
    --set intchargesdue =     ROUND( a.arrears * isnull(mpr,0) ,2)
    --  from acct a  
    --     where 
    --     a.acctno = chargesdata.acctno and 
    --     a.arrears < chargesdata.arrears and 
    --     chargesdata.runno = @runno and
    --     chargesdata.arrears > 0.001 and 
    --     not (chargesdata.currstatus ='' and chargesdata.oldstatus ='1') and 
    --     not (chargesdata.currstatus ='1' and chargesdata.oldstatus ='1') 

    update chargesdata set intchargesdue = 0
        where
        acctno in
        (select b.acctno from acctcode a, spa b
            where a.acctno =b.acctno
       and a.code in ('S','F')
       and b.dateadded <= @rundate
       and (b.dateexpiry >@rundate or b.dateexpiry ='' or b.dateexpiry is null))
       and runno = @runno


   -- uat274 rdb 10/07/08 round all interest calcs 
    update chargesdata  set intchargescumul= ROUND( (intchargescumul + intchargesdue),2) where
    runno= @runno and (isnull(intchargescumul,0) >0 or intchargesdue >0)

-- standard default country charges -- moving up due to 69432 
    declare @adminfee money
    select @adminfee = adminfee from country
    if @adminfee !=0 AND NOT EXISTS (SELECT * FROM interfacecontrol WHERE interface = 'Collections') 
    -- C
    begin
        update chargesdata set
            admchargesapplied = @adminfee
            where acctno like '___0%' and currstatus in
            ('3','4','5') and oldstatus in ('2','3','4','5') and
            not (currstatus ='' and oldstatus ='2')
                and @rundate >= datenextdue
                and isnew = 0 and runno = @runno
    end

        
      update chargesdata  set intchargesapplied = intchargescumul, 
         intchargescumul = 0, 
         datenextdue = dateadd(month,1,datenextdue) 
        where 
           acctno like '___0%' and 
         currstatus in ('3','4','5','') and 
         oldstatus in ('2','3','4','5') and 
         not (currstatus ='' and oldstatus ='2') and 
               datenextdue <=@rundate  and 
               runno = @runno
     
      update chargesdata set
      datenextdue = dateadd(month,1,datenextdue)
     from acct  where chargesdata.datenextdue <=@rundate and
           acct.acctno = chargesdata.acctno and
           chargesdata.runno = @runno


   -- for cash accounts just a one off charge for the cash interest rate   
	-- uat274 rdb 10/07/08 round all interest calcs 
     update chargesdata set
      datenextdue = dateadd(month,1,datenextdue), 
      intchargesapplied = ROUND((acct.arrears * country.cashintrate)/100,2)
        from acct,country  where     
       chargesdata.currstatus in ('3','4','5','') and
       chargesdata.oldstatus in ('2','3','4','5') and
       chargesdata.datenextdue <=@rundate and
       acct.acctno = chargesdata.acctno and
      chargesdata.runno =@runno and 
       acct.outstbal > country.smallbalance
         and datenextdue <= @rundate
         and  isnew=0 and runno =  @runno and acct.acctno like '___4%'     
    
/* removing charges where cash accounts are suspended or frozen */
    update chargesdata set intchargesapplied = 0
        where
        acctno in
        (select b.acctno from acctcode b
            where b.code in ('S','F')
      and (b.datedeleted is null or b.datedeleted = '')
      and b.acctno like '___4%')
      and runno = @runno

--IP - 14/01/2008 - (69490)- If the 'Hide Cents' Country Parameter has been set to 'True'
--then update 'ChargesData' table 'intchargesapplied' to be rounded to 0 decimal places.

--IP - 27/08/08 - (70038)
IF @nocents = 'True'
BEGIN
	UPDATE chargesdata
	SET intchargesapplied = ROUND(intchargesapplied, 0)
	where runno = @runno
END
ELSE
BEGIN
	UPDATE chargesdata
	SET intchargesapplied = ROUND(intchargesapplied, 2)
	where runno = @runno
END

/* Now going to apply charges on both Cash and HP accounts*/

    /* But first am going to get rid of charges for accounts not in SC 3,4,5
       Either these accounts are in SC2 and no change - oldstatus ='2' and currstatus =''
       or Currstatus = '2' (don't care what oldstatus is)
     */



-- applying charges based on specific letters being sent charges are stored in the reference field on the code table for lt1 letters category
    update chargesdata set
            admchargesapplied = convert(money,reference)
            from code c where c.category = 'LT1'
            and c.code = chargesdata.lettercode
            and runno = @runno
            and convert(money,reference) >0

END     --IP - 21/04/10 - UAT(20) UAT5.2
   
   /* reducing status code on cash accounts  */
     update chargesdata 
                 set lettercode ='',
                     currstatus ='1' 
                 from acct
                  where acct.acctno = chargesdata.acctno and
              acct.currstatus in ('2','3','4') and
              acct.outstbal <=0 and    
              chargesdata.acctno like  '___4%' 
                 and chargesdata.runno =  @runno
     
     update chargesdata 
     set lettercode = '' , 
     datenextdue = dateadd(day,7,datenextdue)
     where runno =  @runno   and 
     missaddress = 'Y' 

     update acct set lastupdatedby =99999,currstatus = c.currstatus
     from chargesdata c where runno =@runno and c.currstatus !='' and
     c.currstatus !=acct.currstatus and c.acctno =acct.acctno
     declare @hobranchno smallint,@transrefno int,@statement sqltext
     select @hobranchno = hobranchno from country
   
     update acct set highststatus= c.currstatus
     from chargesdata c where runno =@runno and c.currstatus !='' and
     acct.acctno= c.acctno and acct.highststatus between '1' and '5' and 
     c.currstatus > acct.highststatus
    
     select @transrefno = hirefno + 1 from branch
     -- now going to insert charges using LastChargesInterestAdmin to store identity incremented
     -- faster and don't have to type declare cursor      
     set @statement =' if exists (select * from information_schema.tables where table_name ' +
                     ' = ''LastChargesInterestAdmin'') ' +
                     ' drop table LastChargesInterestAdmin '
     exec sp_executesql @statement
     set @statement =' create table LastChargesInterestAdmin (acctno char(12),transvalue money,transtypecode varchar(3),'
     +               ' transrefno int identity (' +convert(varchar,@transrefno) + ',1)) '
     exec sp_executesql @statement
     truncate table LastChargesInterestAdmin
     set @statement = 'insert into LastChargesInterestAdmin (acctno,transvalue,transtypecode ) ' +
                      ' select acctno,intchargesapplied,''INT'' from chargesdata ' +
                      ' where runno = ' + convert(varchar,@runno) + ' and intchargesapplied !=0 '
     exec sp_executesql @statement
set @rowcount = @@rowcount
if @debug =1
    Print'Stage 21 inserting interest'

     
     set @statement = 'insert into LastChargesInterestAdmin (acctno,transvalue,transtypecode ) ' +
                      ' select acctno,admchargesapplied,''ADM'' from chargesdata ' +
                      ' where runno = ' + convert(varchar,@runno) + ' and admchargesapplied !=0 '
     exec sp_executesql @statement
set @rowcount = @@ROWCOUNT
if @debug =1
    Print'Stage 22 inserting admin'
     set @statement = ' update branch set hirefno = isnull((select max(transrefno) + 1 ' +
                       ' from LastChargesInterestAdmin ),hirefno)'
     exec sp_executesql @statement
   if exists (select * from LastChargesInterestAdmin l)          
   begin
    set @statement = ' insert into fintrans ( ' +
                      ' origbr,branchno,acctno,transrefno, ' +
                      ' datetrans,transtypecode,empeeno,transupdated, ' +
                      ' transprinted,transvalue,bankcode,bankacctno, ' +
                      ' chequeno,ftnotes,paymethod,runno, ' +
                      ' source, agrmtno) ' +
                      ' select 0,c.hobranchno,i.acctno,i.transrefno, ' +
                      ' getdate(),i.transtypecode,99999,''N'', ' +
                      ' ''N'',transvalue,'''','''', ' +
                      ' '''',''DNCH'',0,0,' +
                      ' ''COSACS'', ag.agrmtno from LastChargesInterestAdmin i
                        inner join agreement ag on ag.acctno = i.acctno,country c '

    exec sp_executesql @statement
    end
set @rowcount = @@rowcount
if @debug =1
    Print'Stage 22 inserted fintrans'

    -- update  balance doh!
    update acct set outstbal = ( select sum(transvalue) from fintrans f where f.acctno = acct.acctno )
    where exists (select * from chargesdata c where c.runno = @runno and c.acctno =acct.acctno
                 and (c.intchargesapplied >0 or c.admchargesapplied >0)) and isAmortized = 0

	--Cashloan Amortization CR, will update in case of amortized accounts 20-06-2019
	 update acct set outstbal = dbo.fn_CLAmortizationCalcDailyOutstandingBalance(@acctno)
    where exists (select * from chargesdata c where c.runno = @runno and c.acctno =acct.acctno
                 and (c.intchargesapplied >0 or c.admchargesapplied >0)) 
				 and isAmortized=1 and IsAmortizedOutStandingBal = 1

set @rowcount = @@rowcount
if @debug =1
    Print 'Stage 23 updated acct'
-- arrears will now be increased by charges applied on the accounts
	declare @chargesapplied table (acctno char(12),transvalue money)
	insert into @chargesapplied (acctno,transvalue)
			select acctno,sum(transvalue)
			from LastChargesInterestAdmin
            group by acctno

	update acct set
	arrears = arrears + transvalue from 
	 @chargesapplied c where c.acctno = acct.acctno
if @debug =1
    Print 'Stage 23a updated acct arrears'

        
     -- blocking credit for rf accounts in arrears. 
     update customer set creditblocked= 1 
      where creditblocked = 0 and exists ( select custacct.custid 
      from acct,custacct, instalplan,country 
      where acct.accttype= 'R'  
      and customer.custid = custacct.custid 
      and acct.acctno = custacct.acctno 
      and instalplan.acctno =acct.acctno 
      and custacct.hldorjnt = 'H'  
      and acct.arrears >= country.blockcreditmonths*instalplan.instalamount and instalplan.instalamount > 0) 
set @rowcount = @@rowcount
if @debug =1
    Print 'Stage 24 blocking credit'

      update customer set creditblocked= 0 
      where creditblocked = 1 and not exists ( select custacct.custid 
      from acct,custacct, instalplan,country 
      where acct.accttype= 'R'  
      and customer.custid = custacct.custid 
      and acct.acctno = custacct.acctno 
      and instalplan.acctno =acct.acctno 
      and custacct.hldorjnt = 'H'  
      and acct.arrears >= country.blockcreditmonths*instalplan.instalamount and instalplan.instalamount > 0) 
	  and not exists
		(SELECT * FROM custacct ca JOIN acct t ON ca.acctno = t.acctno
		WHERE ca.hldorjnt ='H' AND t.accttype ='T' AND t.outstbal >0
		AND ca.custid = customer.custid)
		  set @rowcount = @@rowcount
      
      if @debug =1
        Print 'Stage 25 unblocking credit - done'

-- CR906 Cash Loans jec 05/09/07 - AA needs to be done before the letters INSERTed....

	UPDATE ChargesData
		SET lettercode=replace(lettercode,'F','') + 'LOA'
	FROM ChargesData c inner join acct a on c.acctno=a.acctno 
					   inner join termstype t on a.termstype=t.termstype
	WHERE t.Isloan='1'
		and lettercode in ('JF','UF','K','Q','L','R','0','01')
		and runno = @runno
		


IF EXISTS (SELECT * FROM interfacecontrol WHERE interface = 'Collections') OR (@GenLetters) = 'False' --IP - 21/04/10 - UAT(20) UAT5.2
BEGIN -- we are doing collections instead of letters and charges
	UPDATE chargesdata SET lettercode = '' WHERE runno= @runno 
END
ELSE -- still running letters and charges 
BEGIN

	insert into letter (acctno,dateacctlttr,datedue,lettercode,addtovalue,ExcelGen)
    select acctno,getdate(),dateadd(day,country.letterdays,@rundate),lettercode,0,0
    from chargesdata,country where runno = @runno and isnull(lettercode,'') !=''
END
    
    /* Clearing any previous charges applied/due */
      update chargesdata set 
      afterarrears = acct.arrears from acct  where 
      acct.acctno = chargesdata.acctno and
      chargesdata.runno =  @runno   and 
      chargesdata.afterarrears !=acct.arrears
      

     
-- CR907 Instant Credit jec 30/07/07

	Declare	@MaxArrearsLevel	FLOAT,
			@MaxSETtStatus		smallint

	-- Get Instant Credit Country parameters
	SET @MaxArrearsLevel= (SELECT value FROM countrymaintenance WHERE codename='MaxArrearsLevel')
	SET @MaxSETtStatus= (SELECT value FROM countrymaintenance WHERE codename='HighSETtStat2Yr')

--	drop table #InstantCredit			-- testing only
--	drop table #ArrearsLevel			-- testing only
--	drop table #SpouseAccounts			-- testing only

	create table #InstantCredit
	(
		custid				varchar(20),		
		Status				char(1),
		ArrearsLevel		FLOAT,
		BDWlegal			char(1),
	)

	create table #ArrearsLevel
	(
		custid				varchar(20),		
		ArrearsLevel		FLOAT
	)
	
	create table #SpouseAccounts
	(
		custid				varchar(20),
		acctno				varchar(12),
		hldorjnt			char(1),
		Hcustid				varchar(20),
		currstatus			char(1),
		HighStatus			char(1)
	)
	
	create clustered index ixsdredinstantcredit on #instantcredit(custid)
	
	create clustered index ixsdrediArrearsLevel on #ArrearsLevel(custid)
	

	create clustered index ixsdrediSpouseAccounts on #SpouseAccounts(custid)
	
	-- Get max status of all current accounts for Account Holder Customers flagged as Instant Credit
	INSERT INTO #InstantCredit	(custid,status,ArrearsLevel,BDWlegal)
	SELECT c.custid,max(a.currstatus),0,' '
	FROM customer c inner join custacct ca on c.custid=ca.custid 
					inner join acct a on a.acctno=ca.acctno and ca.hldorjnt='H'					
	WHERE c.InstantCredit='Y' and a.currstatus NOT IN ('S','U')
	group by c.custid

	-- Get max arrears level of all current accounts for Account Holder Customers flagged as Instant Credit
	INSERT INTO #ArrearsLevel	(custid,ArrearsLevel)
	SELECT c.custid,max(a.Arrears/isnull(ip.instalamount,0))
	FROM customer c inner join custacct ca on c.custid=ca.custid 
					inner join acct a on a.acctno=ca.acctno and ca.hldorjnt='H'
					inner join instalplan ip on a.acctno=ip.acctno
	WHERE (c.InstantCredit='Y' or LoanQualIFied='1')		-- CR906 jec 26/09/07
			and a.currstatus NOT IN ('S','U') and isnull(ip.instalamount,0)>0
			and abs((a.Arrears/isnull(ip.instalamount,0))) < 100 -- prevent error running letters and charges
	group by c.custid

	-- get spouse/Joint account holder details & check for Write-off or Legal
	INSERT INTO #SpouseAccounts (custid,acctno,hldorjnt,Hcustid)
		SELECT custid,acctno,hldorjnt,'Holder' 
		FROM custacct ca
		WHERE hldorjnt in('S','J')

	-- UPDATE with holder id
	UPDATE #SpouseAccounts
		SET Hcustid=ca.custid
	FROM #SpouseAccounts s inner join custacct ca on s.acctno=ca.acctno and ca.hldorjnt='H'
	-- get accounts WHERE Spouse is holder
	INSERT INTO #SpouseAccounts (custid,acctno,hldorjnt)
	SELECT ca.custid,ca.acctno,ca.hldorjnt
	FROM custacct ca inner join #SpouseAccounts s on ca.custid=s.custid
	WHERE ca.hldorjnt='H' and s.hldorjnt in('S','J')
	
	-- get status of spouses/joint holder accounts current and SETtled
	UPDATE #SpouseAccounts
		SET currstatus=a.currstatus
	FROM #SpouseAccounts s inner join acct a on s.acctno=a.acctno
	WHERE s.hldorjnt ='H' and a.currstatus !='S'
	-- get status of spouses accounts SETtled
	UPDATE #SpouseAccounts
		SET Highstatus=a.highststatus
	FROM #SpouseAccounts s inner join acct a on s.acctno=a.acctno
	WHERE s.hldorjnt ='H' and a.currstatus ='S'
	
	--UPDATE #InstantCredit
	UPDATE #InstantCredit
		SET ArrearsLevel=a.ArrearsLevel
	FROM #InstantCredit i inner join #ArrearsLevel a on i.custid=a.custid

	UPDATE #InstantCredit
		SET BDWLegal='Y'
	FROM #InstantCredit i inner join #SpouseAccounts s on i.custid=s.custid and s.hldorjnt ='H'
	WHERE isnull(currstatus,'0') in('6','7','8') or isnull(Highstatus,'0') in('6','7','8')

	-- Now SET Customer.InstantCredit to 'N'
	UPDATE Customer
		SET InstantCredit='N', LoanQualIFied='0'		-- CR906 jec 05/09/07
	FROM #InstantCredit i inner join Customer c on c.Custid=i.Custid
	WHERE (Status>@MaxSETtStatus
		 or ArrearsLevel>@MaxArrearsLevel
		 or BDWlegal='Y')
    
--	drop table InstantCredit_LaC		-- testing only
--	drop table ArrearsLevel_LaC			-- testing only
--	drop table SpouseAccounts_LaC		-- testing only
--	SELECT * INTO InstantCredit_LaC		-- testing only
--	FROM #InstantCredit					-- testing only
--	SELECT * INTO ArrearsLevel_LaC		-- testing only
--	FROM #ArrearsLevel					-- testing only
--	SELECT * INTO SpouseAccounts_LaC	-- testing only
--	FROM #SpouseAccounts
-- 
	-- End CR907 Instant Credit jec 30/07/07


-- End CR906 Cash Loans jec 05/09/07

   update country set lastchargesweekno = @runno 
   return @return


go

-- end end end end end end end end end end end end end end end end end end end end end end end 
