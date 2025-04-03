/* Component crtaddtoletter = author AA
 19/06/06  AA      need to see whether I can get the addtoletter to work - we are doing weekly runs. We could do with a control 
                   table to store the last add-to letter (settlement letter) dates generated. This could be on the countrymaintenance table.
                   68395 Thank you letters were not being generated. When testing noticed thank you letters for
                   cash accounts were generated, so changed to ensure only credit accounts affected.
*/
if exists (select * from sysobjects where name = 'dn_addtoletter')
drop procedure dn_addtoletter
go
CREATE procedure dn_addtoletter @runno int,@type varchar(4), @return int OUT
as declare

@lastrunno int, @newdate datetime,@rundate datetime,@previousrundate datetime,
    @beforedate datetime,  @afterdate datetime,
	@fstatus int,
	@counter smallint,--was j
	@doaddto smallint, 
    @percentaddto1 float,
    @percentaddto2 float,
    @percentaddto3 float,
    @rebpcent float,@highstatus char(1),@addtomin money
SET NOCOUNT ON

	 SELECT @lastrunno = @runno -1,   
    @percentaddto1 =percentaddto1,
    @percentaddto2 =percentaddto2,
    @percentaddto3 =percentaddto3,
    @rebpcent =rebpcent,
    @addtomin = addtomin,
    @highstatus= highstatus
      from country
      
   SET @RETURN = 0
   
    delete from addtoletter where runno = @runno
    select @rundate = datestart from interfacecontrol where interface ='CHARGES' and runno = @runno
    set @newdate = dateadd(day,7,@rundate) -- this is being moved forward to reduct the rebate for rebate calculation

    set nocount on

    if @percentaddto1 > 0 or
       @percentaddto2 > 0 or
       @percentaddto3 > 0 
    begin

        insert into addtoletter
        (acctno,    outstbal,    addtovalue,
        agrmttotal,    deposit,    instalamount,
        instalno,    lettercode,    oldpcent,
        newpcent,    dt,    nm,
        rebate,    runno)

        select
        a.acctno,    a.outstbal,    0,
        a.agrmttotal,    deposit,    i.instalamount,
        i.instalno,    '',    0,
        a.paidpcent,    0,    0,
        0,@runno
        from acct a, agreement b, instalplan i 
        where  outstbal > 0 and paidpcent >0 and paidpcent <100 and 
    				  datedel !='1-jan-1900' and datedel is not null 
    				  and b.acctno = a.acctno and i.acctno = a.acctno and 
    				  a.arrears <= i.instalamount and 
                     currstatus in   ('1','2','3','4') and i.agrmtno = 1 and b.agrmtno = 1
    	  set @return = @@error			 
	if @type != 'L2B'
        delete from addtoletter where runno =@runno and exists
       (select * from acct a where a.acctno= addtoletter.acctno and a.currstatus ='4')

     update addtoletter set
             deposit = instalamount from termstype , acct
             where acct.acctno =addtoletter.acctno
             and termstype.termstype = acct.termstype and termstype.dtnetfirstin ='Y'
             and runno = @runno
--    print 'here 1'
	if @type != 'L2B' and @type != 'L2A' 
        delete from addtoletter where agrmttotal/8 < instalamount 
        and runno = @runno
    
    delete from addtoletter where outstbal > (agrmttotal -(6*instalamount))
     and runno = @runno and @newdate =  dateadd(day,7,@rundate)

      update addtoletter set nm =
          isnull(convert(integer,(datediff(day, @newdate,datelast))/30.436875),0),
          dt = convert(integer,(Addtoletter.agrmttotal -addtoletter.deposit)/Addtoletter.instalamount)
          from instalplan  where instalplan.acctno = addtoletter.acctno and instalplan.datelast >'1-jan-1910'
          and instalplan.datelast is not null and runno =  @runno  and addtoletter.agrmttotal >0
          and addtoletter.instalamount > 0 and addtoletter.agrmttotal >0 and datelast > '1-jan-1990'

    delete from addtoletter where nm is null or dt is null

    update addtoletter set nm = nm*(nm-1), dt = dt*(dt+1)
    where   runno = @runno
    and dt between -10 and 500 and nm between -10 and 500

      select a.acctno,a.agrmttotal,a.servicechg,b.termstype, 
                 convert(money, 0) as insurance,c.inspcent, 
                 convert (money, 0) as cashprice, d.nm as totnum 
                 into #insurance 
                 from agreement a, acct b, termstype c, addtoletter d 
                 where a.acctno =b.acctno and 
                 d.acctno =a.acctno and 
                 c.termstype =b.termstype and 
                 runno =  @runno


    update #insurance set cashprice=agrmttotal- servicechg

    update #insurance set insurance=cashprice*totnum/12*inspcent/100

    update #insurance set servicechg=servicechg- insurance

    update addtoletter set
       rebate = (#insurance.servicechg*nm)/dt
       from #insurance  where nm !=0 and dt !=0 and #insurance.acctno = addtoletter.acctno
       and runno =  @runno

    UPDATE addtoletter set rebate =  @rebpcent/100 * rebate
    where    runno = @runno

/* AA - amending the add-to calculatiion according to the book. Addtoterm is in months
Also add-to letter is now based on add-to term rather than customers existing number of instalments
*/
      update addtoletter set addtovalue =
            isnull(   ((country.addtoterm * 100 * instalamount)/
             (( country.servpcent * country.addtoterm/12) + 100))  ,0) 
			-acct.outstbal + rebate from acct,country where acct.acctno = addtoletter.acctno
             and runno =  @runno

 

       update a  set oldpcent = v.newpcent 
       from addtoletter a, addtoletter v
                 where      a.acctno = v.acctno and 
               a.runno =  @runno
             and v.runno = @lastrunno



    delete from addtoletter where not exists 
    (select acctno from addtoletter a where
            runno = @lastrunno and a.acctno = addtoletter.acctno)
 
    if @percentaddto1 > .001 
        update addtoletter set lettercode ='M'
        where oldpcent < @percentaddto1 and
              newpcent > @percentaddto1 and
	      addtovalue > @addtomin and
              runno = @runno 

    if @percentaddto2 > .001 
        update addtoletter set lettercode ='N'
        where oldpcent < @percentaddto2 and
              newpcent > @percentaddto2 and
              addtovalue > @addtomin and
              runno = @runno 
 
    if @percentaddto3 > .001 
        update addtoletter set lettercode ='O'
        where oldpcent < @percentaddto3 and
              newpcent > @percentaddto3 and
              addtovalue > @addtomin and
              runno = @runno 

/* Now inserting into addtoletter for all those accounts
which were deleted before. This is so that we can use
the paidpcent correctly next week - provided that
the arrears calculation is calculating this correctly*/
/********   SQL BEGIN **************/
    insert into addtoletter
    (acctno,    outstbal,    addtovalue,
    agrmttotal,    deposit,    instalamount,
    instalno,    lettercode,    oldpcent,
    newpcent,    dt,    nm,
    rebate,    runno)

    select
    a.acctno,    0,    0,
    0,    0,    0,
    0,    '',    0,
    a.paidpcent,    0,    0,
    0,    @runno

    from acct a
    where currstatus !='S'
    and not exists  (select
    acctno from addtoletter l where
    runno = @runno and a.acctno =l.acctno)
  end -- end of processing for M,N and O letters

declare @settledbeforedate datetime,@settledafterdate datetime
  
-- producing settlement letters to confirm to customers they have settled an HP or RF
-- account and would they like to purchase some more items 
-- Settlment letters are I1,I2 AND I3 depending on highest status that account was ever in
-- For .NET we are going to get the settlement letters as from around about 3 weeks ago. 
-- then we are going to get the previous settlement letter runno and date to see what should be generated

	set @settledafterdate = dateadd(day,-14,isnull(@rundate,getdate())) 
if not exists (select * from interfacecontrol where interface ='SLETTER' and runno = @runno -1)
begin
	set @settledbeforedate = dateadd(day,-21,isnull(@rundate,''))

	insert into interfacecontrol
	(interface,runno,datestart,datefinish,result)
         values('SLETTER',@runno-1,@settledbeforedate,@settledafterdate,'P')
end
else
begin
	-- so previous settled before date is stored in the datefinish column so we are getting all accounts
        -- settled between these two dates for the settlement letter
	select @settledbeforedate =isnull(datefinish,dateadd(day,-21,getdate())) from interfacecontrol where interface ='SLETTER' and runno =@runno-1 	   	
	insert into interfacecontrol
	(interface,runno,datestart,datefinish,result)
        values( 'SLETTER',@runno-1,@settledbeforedate,@settledafterdate,'P')
end




  select @previousrundate = datestart from interfacecontrol where interface ='CHARGES' and runno = @runno
set @settledbeforedate = dateadd(day,-21,@rundate)

set @settledafterdate = dateadd(day,-14,@rundate) 

    insert into addtoletter
    (acctno,    outstbal,    addtovalue,
    agrmttotal,    deposit,    instalamount,
    instalno,    lettercode,    oldpcent,
    newpcent,    dt,    nm,
    rebate,    runno)

    select
    distinct(a.acctno),    0,    0,
    a.agrmttotal,    0,    0,
    0,    'I' + isnull(a.highststatus, '1'),    0,   
     0,    0,        0,
    0,    @runno
     from acct a, status s,instalplan i where
     a.currstatus ='S' and i.acctno = a.acctno and i.agrmtno = 1 and 
    s.statuscode ='S' and
    abs(a.outstbal) <.1 and
    a.highststatus <= @highstatus and -- maximum status that a letter is sent
    s.acctno = a.acctno and
    s.datestatchge > @settledbeforedate and s.datestatchge < @settledafterdate and
    a.agrmttotal >1 and exists 
    (select * from delivery d, stockitem s where d.acctno = a.acctno and d.itemno = s.itemno and d.stocklocn =s.stocklocn and s.itemtype ='S')
--    print 'here 3'    
     /* removing all settlement or add-to
    letters where customer code - refuse further credit*/
    update addtoletter set lettercode ='' where lettercode !=''
    and exists 
    (select acctno from custacct c, custcatcode ca where c.hldorjnt = 'H' and c.acctno= addtoletter.acctno
    and ca.custid = c.custid and ca.code = 'R')
 
   /* removing all add-to or settlment letters where customer deceased */
    update addtoletter set lettercode ='' where lettercode !=''
    and exists  
     (select acctno from bailaction b where b.acctno= addtoletter.acctno and b.code ='DEC')
    and runno = @runno
     
  /* no settlement letter if account was closed by means of an add-to*/
      update addtoletter 
      set lettercode ='' 
     from fintrans f, addtoletter a
     where a.acctno = f.acctno and a.lettercode ='I' 
     and f.transvalue <0 and f.transtypecode = 'ADD' 
     and a.runno =  @runno
    -- now insert the addtoletters into the letter table
  	SELECT @runno =ISNULL(MAX(runno),0) FROM interfacecontrol WHERE interface = 'collections'
	set @runno= @runno +1 -- so can be generated from the letter generation routine
    insert into letter
    (runno,acctno,dateacctlttr,datedue,lettercode,addtovalue)
    select  @runno,acctno,@rundate,dateadd(day,7,@rundate),lettercode,0
    from addtoletter where runno =@runno and lettercode !=''
go

