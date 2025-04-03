if exists (select * from sysobjects where name ='RP_WarrantySales')
drop procedure RP_WarrantySales
go
create procedure RP_WarrantySales 
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : RP_WarrantySales.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Warranty Sales report
-- Author       : Alex Ayscough
-- Date         : 2007
--
-- This procedure will retreive data for the Warranty Sales Reporting screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 041108    AA  UAT 213 Warranty cancellations will now included warranties collected with the associated stock item.  
-- 041108    AA  Adding Admin Fees and Premium to Warranty Reports  
-- 05/02/2009 jec 70816 Rename columns to "Warranty Cost Price" and "Warranty Retail Value" & change profit margin calc. 
--					and get correct product delivery date  
-- 02/03/2009 jec 70818 Calculation is incorrect for AIG Claim 
-- 03/03/2009 jec 70820 Calculation is incorrect for Furniture Admin Fee
-- 09/11/2010 jec CR1030 Replace "AIG" with "EW"	     
-- 08/07/11   ip  CR1254 - RI - Warranty Reporting - Supashield Sales - System Integration. Display IUPC and Courts Code    
-- 11/07/11   ip  CR1254 - RI - Warranty Reporting - Warranties due for renewal. WarrantyBand.Warrantylength holds months, therefore
--				  changed Dateadd in select to work out expiration date. Previously was using warrantylength and adding it as years.
-- 06/06/12 ip/jec LW75090/74967  No return codes found on the warranty report     
-- 07/06/12 ip     LW75090 - columns 'Rebate %', 'EW Claim' and 'Customer Debit Amount' were not being populated.     
-- =================================================================================
	-- Add the parameters for the stored procedure here
	
-- Procedure to get a report of warranty sales
-- Procedure to get a report of warranty sales
@warrantytype varchar(3), -- SU for supershield, IR for Instant Replacement, SE for second effort, 
--Re for Renewal, RP for reposession, CN for cancellation, MS for missed sales, IC for insurance claims, 
--EXR for exchanges......  IRC = Instant replacement sales
-- WDR -- Warranty Due for renewl
@branch  varchar(6), -- branchno or 'ALL'
@salesperson varchar(10), -- empeeno or 'ALL'
@categoryset varchar(32),
@includecash smallint,
@includecredit smallint,
@includespecial smallint,
@datefrom datetime,
@dateto datetime,
@accttypetotal smallint,
@branchtotal smallint,
@categorytotal smallint,
@salespersontotal smallint,
@datesAre varchar(10), -- 'order date', 'warranty' ,'item deli',
@return int OUTPUT
--
as declare
  @statement sqltext,@select sqltext,@tables sqltext,@where sqltext,@selectcolumns sqltext
  set @return = 0
declare @nl varchar (300)
	SET @nl=' '

if @datesare = 'Order date'
  set @datesare = 'Order'
if @datesare ='Item Deliv'
  set @datesare ='Item'
if @datesare ='Warranty D'
  set @datesare ='Warranty'

if @warrantytype in ('RP') and left(@datesare,5) ='Order' 
  set @datesare ='item'

 set @dateto = dateadd(hour,23,@dateto)
 set @dateto = dateadd(minute,59,@dateto)

SET NOCOUNT ON
--select * from sysobjects where name like '%build%'
  --select * from dbversion order by upgrade_date desc\
-- change Money types to decimal(12,2) so results returned to two decimal not four  jec 70816
CREATE TABLE #warranty(	
		accttype char (1)   ,			acctno char (12)   ,
		branchno smallint  ,			category varchar(64) ,
		ContractNo varchar (10)   ,		product varchar(18), --product varchar (8)   ,					--IP - 07/07/11 - CR1254 - RI - #4005
		CourtsCode varchar(18),																			--IP - 07/07/11 - CR1254 - RI - #4005
		ordval decimal(12,2) ,					description varchar (99)   ,
        costprice decimal(12,2),				ProfitMargin decimal,
		salesperson varchar (99)  NULL , itemno varchar(18),--itemno varchar (8)   ,-- Warranty itemno	--IP - 07/07/11 - CR1254 - RI - #4005
		WarrantyCourtsCode varchar(18),																	--IP - 08/07/11 - CR1254 - RI
		Warrantyval decimal(12,2)  ,			WarrantyDescription varchar (99)   ,
		buffno int  ,					WarrantyDeliveryDate datetime  ,
		datedel datetime,		        renewaldate datetime,
		renewalstatus varchar(20),		replacementmarker VARCHAR(10),
		WarrantyReturnCode VARCHAR(12),	WRetLocn SMALLINT,
		rebatepercentage decimal(12,2),			AIGClaim decimal(12,2),
		ValueCRDR decimal(12,2),				WretailValue decimal(12,2),
		custid VARCHAR(30),				orddate DATETIME,
		quantity INT,              AdminFee decimal(12,2),
		WarrRetID int,				-- jec 06/06/12
		ItemID INT,				   WarrantyID INT)															--IP - 07/07/11 - CR1254 - RI - #4005

  --set @selectcolumns = ' insert into #warranty (accttype ,acctno ,branchno , ' +
  --' category ,ContractNo ,product , ' +
  --'quantity, ordval , description ,  costprice , ' +
  --' ProfitMargin ,salesperson ,itemno , ' +
  --' Warrantyval ,WarrantyDescription '
  
    set @selectcolumns = ' insert into #warranty (accttype ,acctno ,branchno , ' +		--IP - 07/07/11 - CR1254 - RI - #4005 - Replaces above					
  ' category ,ContractNo ,product , CourtsCode, ' +
  'quantity, ordval , description ,  costprice , ' +
  ' ProfitMargin ,salesperson ,itemno , WarrantyCourtsCode, ' +
  ' Warrantyval ,WarrantyDescription '

  --set @select = 
  --  '   select a.accttype, w.acctno,a.branchno, ' + 
  --   ' convert(varchar,s.category),w.ContractNo,p.itemno ,p.quantity, p.ordval ' + @nl +
  --   ' ,s.itemdescr1 + '' '' + s.itemdescr2 ,isnull(ws.costprice,0),0, ' + @nl +
  --   ' convert(varchar,g.empeenosale) + '' '' + c.empeename , ' + @nl +
  --   '  w.itemno, ' 
  
    set @select = 
    '   select a.accttype, w.acctno,a.branchno, ' + 
     ' convert(varchar,s.category),w.ContractNo,s.iupc, s.itemno,p.quantity, p.ordval ' + @nl +
     ' ,s.itemdescr1 + '' '' + s.itemdescr2 ,isnull(ws.costprice,0),0, ' + @nl +
     ' convert(varchar,g.empeenosale) + '' '' + c.empeename , ' + @nl +
     --'  sw.iupc, sw.itemno, ' 
       '  '''', '''', '									--IP - 11/07/11 - CR1254 - RI - warranty itemno and courtscode updated further down.
	if @warrantytype !='RE' 
		set @select = @select + 'w.ordval ' + @nl
	else 
	    set @select = @select + 'wl.ordval ' + @nl


	set @select = @select + ' ,ws.itemdescr1 + '' '' + ws.itemdescr2  '  + @nl
      --ws.cost, (w.ordval-ws.cost)/ws.cost as WarrantyProfitMargin 
  if (@datesAre ='warranty' or @datesAre='item' )
  begin 
     set @selectcolumns = @selectcolumns + ' ,buffno  , WarrantyDeliveryDate ,datedel '
     set @select = @select  + ' ,ds.buffno,dw.datedel ,ds.datedel  '
     if @warrantytype = 'WDR'  -- for warranties due for renewal add renewal date
     begin
        set @selectcolumns = @selectcolumns + ' ,renewaldate , renewalstatus '
        --set @select = @select +  ' ,dateadd(year,wb.warrantylength,dw.datedel), ''Near Expiry'' '
        set @select = @select +  ' ,dateadd(month,wb.warrantylength,dw.datedel), ''Near Expiry'' '  --IP - 11/07/11 - CR1254 - RI
     end
  END
  IF @warrantytype='IRC'  -- need replacement marker
  BEGIN
  	  SET @selectcolumns = @selectcolumns + ',replacementmarker'  + @nl
     SET @select = @select + ',dw.replacementmarker'
  END
 IF @warrantytype IN ('RP','CN')
   BEGIN
   	SET @selectcolumns = @selectcolumns + ',WarrantyReturnCode, WRetLocn,WarrRetID'			-- jec 06/06/12
      SET @select=@select + ',dw.retitemno , dw.retstocklocn,dw.retitemid'					-- jec 06/06/12
   END

--IP - 07/07/11 - CR1254 - RI - #4005 - Add ItemID and WarrantyID to the columns being selected and the select list
	SET @selectcolumns = @selectcolumns + ',ItemID,WarrantyID'
	SET @select = @select + ',p.ItemID, w.ItemID'					
	
declare @expiry smallint
if @warrantytype = 'WDR'  -- for warranties due for renewal add renewal date
begin
	SELECT @expiry =convert(smallint,value) FROM countrymaintenance where name = 'Maximum prompt days after warranty expired'
END

	set @where =' '
	set @tables =' FROM '
	if @datesAre ='warranty' or @datesAre='item'
		  set @tables = @tables  + '  delivery dw,delivery ds , '
 
	if @warrantytype !='RE' -- not renewable
		set @tables = @tables  + ' lineitem w ' 
	else
		set @tables = @tables  + '  lineitem ow , warrantyrenewalpurchase w' + @nl
--	select * from warrantyrenewalpurchase
 
   if @warrantytype !='RE' -- not renewal 
    begin
	  --set @where = @where + ' JOIN  lineitem p on  w.acctno =p.acctno and p.itemno =w.parentitemno ' -- p for product
	    set @where = @where + ' JOIN  lineitem p on  w.acctno =p.acctno and p.ItemID =w.ParentItemID ' -- p for product	--IP - 07/07/11 - CR1254 - RI 
		+ 'and p.stocklocn =w.parentlocation and p.agrmtno =w.agrmtno  ' + @nl
 
	 /*if @warrantytype = 'CN' -- cancelation
	  set @where = @where + ' JOIN collectionreason cr on w.acctno = cr.acctno and w.itemno = cr.itemno and ' +
			  ' w.stocklocn = cr.stocklocn  and cr.collecttype =''C'' '*/
	 if @warrantytype = 'EXR'  -- exchange or identical replacement
	  --set @where = @where + ' JOIN collectionreason cr on w.acctno = cr.acctno and w.itemno = cr.itemno and ' +
		set @where = @where + ' JOIN collectionreason cr on w.acctno = cr.acctno and w.ItemID = cr.ItemID and ' + --IP - 07/07/11 - CR1254 - RI 
			  ' w.stocklocn = cr.stocklocn and   cr.collecttype IN (''E'',''R'') '
 
	 if @warrantytype = 'IC' -- insurance claim
	  --set @where = @where + ' JOIN collectionreason cr on w.acctno = cr.acctno and w.itemno = cr.itemno and ' +
	    set @where = @where + ' JOIN collectionreason cr on w.acctno = cr.acctno and w.ItemID = cr.ItemID and ' + --IP - 07/07/11 - CR1254 - RI
			  ' w.stocklocn = cr.stocklocn and   cr.collectionReason =''INW'' '

    END

    else -- for renewal
    begin
		  set @where = @where + ' JOIN lineitem p on  p.acctno =w.stockitemacctno  '  + @nl
		  --set @where = @where + ' JOIN lineitem wl on  wl.acctno =w.acctno and wl.itemno = w.itemno and wl.contractno = w.contractno and wl.stocklocn = w.stocklocn'  + @nl
		  set @where = @where + ' JOIN lineitem wl on  wl.acctno =w.acctno and wl.ItemID = w.ItemID and wl.contractno = w.contractno and wl.stocklocn = w.stocklocn'  + @nl --IP - 07/07/11 - CR1254 - RI
    end
     
		  --set @where = @where + ' JOIN  stockitem s on s.stocklocn =p.stocklocn and s.itemno =p.itemno ' + @nl
		  set @where = @where + ' JOIN  stockitem s on s.stocklocn =p.stocklocn and s.ID =p.ItemID ' + @nl			--IP - 07/07/11 - CR1254 - RI - join product to stockitem
		  --set @where = @where + ' JOIN  stockitem sw on sw.stocklocn = w.stocklocn and sw.ID = w.ItemID ' + @nl    --IP - 11/07/11 - Commented out --IP - 07/07/11 - CR1254 - RI - join warranty to stockitem
		  
		  --set @where = @where + ' JOIN warrantycodes ws on ws.warrantyno =w.itemno ' + @nl
		  set @where = @where + ' JOIN warrantycodes ws on ws.ItemID =w.ItemID ' + @nl	--IP - 07/07/11 - CR1254 - RI 
       if @warrantytype = 'RE'
		  set @where = @where + ' JOIN acct a on a.acctno=	w.acctno ' + @nl
       ELSE
		 set @where = @where + ' JOIN acct a on a.acctno= p.acctno ' + @nl
		  set @where = @where + ' JOIN agreement g on  g.acctno = w.acctno and g.agrmtno = p.agrmtno '+ @nl
		  set @where = @where + ' JOIN  courtsperson c on c.empeeno = g.empeenosale '+ @nl
          --set @where = @where + ' JOIN warrantyband wb on  wb.waritemno =w.itemno  '+ @nl
          set @where = @where + ' JOIN warrantyband wb on  wb.ItemID =w.ItemID  '+ @nl	--IP - 07/07/11 - CR1254 - RI

			--uat 375 rdb 01/07/08 added filter for courts non-courts
			-- only add extra join if we will filter on storetype
			IF @branch = 'ALL COURTS' OR @branch = 'ALL NON-COURTS'
				SET @where = @where + ' JOIN  branch b on  a.branchno =b.branchno'
/*    	if @warrantytype = 'EXR'
        begin
		set @where = @where + ' JOIN Exchange EX on ex.acctno= w.acctno and ex.agrmtno = w.agrmtno and ' + @nl
			+ '  ex.itemno=  w.parentitemno and ex.warrantyno = w.itemno and ex.stocklocn = w.stocklocn ' 
	end*/

          set @where = @where + ' WHERE  ' + @nl

       declare @and varchar(32) 
       set @and =''
       

	if @warrantytype ='RE'
        begin
		set @where =@where + '    w.stockitemacctno= ow.acctno and w.originalcontractno =ow.contractno ' +
					  ' and w.originalstocklocn =ow.stocklocn ' + @nl +
                      --' and p.itemno =ow.parentitemno and p.acctno = ow.acctno ' + @nl +
                        ' and p.ItemID =ow.ParentItemID and p.acctno = ow.acctno ' + @nl +	--IP - 07/07/11 - CR1254 - RI
						+ ' and p.stocklocn =ow.parentlocation and p.agrmtno =ow.agrmtno  '
         set @and = ' and '
        end    
             
  if @datesAre ='warranty' or @datesAre='item'
  begin 
     set @where = @where + @and + '   w.acctno = dw.acctno AND w.contractno = dw.contractno  ' + @nl +
      --' AND dw.itemno= ws.warrantyno  ' + @nl +
        ' AND dw.ItemID= ws.ItemID  ' + @nl +		--IP - 07/07/11 - CR1254 - RI
      --' and s.itemno= ds.itemno and s.stocklocn = ds.stocklocn ' + @nl +
		' and s.ID = ds.ItemID and s.stocklocn = ds.stocklocn ' + @nl +		--IP - 07/07/11 - CR1254 - RI
      -- restricting so only get one line if the stock delivery 
      ' and ds.datetrans = (select max(datetrans) from delivery d where d.acctno = ds.acctno and ' + 
      --' d.itemno = ds.itemno and d.stocklocn = ds.stocklocn and d.agrmtno = ds.agrmtno '  + @nl /*and d.delorcoll != ''C''*//* + ' ) ' */
        ' d.ItemID = ds.ItemID and d.stocklocn = ds.stocklocn and d.agrmtno = ds.agrmtno '  + @nl /*and d.delorcoll != ''C''*//* + ' ) ' */	--IP - 07/07/11 - CR1254 - RI
    
      -- required to get the product delivery date -- jec 70816 05/02/09 
      if @warrantytype='RP'      
	  	set @where = @where + @and + ' and d.delorcoll!=''R'' ) '
	  else set @where = @where + ' )'
	  
     set @and =' and '
    if @warrantytype = 'SU' or @warrantytype ='IR' -- deliveries only as don't want collections showing
     begin 
       set @where = @where + @and + ' dw.delorcoll = ''D'' ' 
       set @and = ' and '
     end
    if @warrantytype in ('IRC','CN','EXR','IC') --  collections showing
     begin
       set @where = @where + @and +  ' dw.delorcoll = ''C'' ' 
	   set @and = ' and '
     END

    if @warrantytype = 'CN'
    begin
       set @where = @where + @and + ' w.quantity = 0 ' -- means that the warranty is cancelled
	   set @and = ' and '
     end
	if @warrantytype !='RE'
		--set @where = @where +  ' and p.itemno = w.parentItemNo and p.stocklocn =w.parentLocation and w.agrmtno= dw.agrmtno and p.agrmtno = w.agrmtno  ' + @nl
		 --set @where = @where +  ' and p.ItemID = w.ParentItemID and p.stocklocn =w.parentLocation and w.agrmtno= dw.agrmtno and p.agrmtno = w.agrmtno  ' + @nl	--IP - 08/07/11 - CR1254 - RI
		 set @where = @where +  ' and w.agrmtno= dw.agrmtno  ' + @nl	--IP - 11/07/11 - CR1254 - RI - replaces above. Removed joins as already joining on these above.
		+ ' and a.acctno = w.acctno and w.agrmtno= dw.agrmtno ' + @nl 
    else

	  --set @where = @where +  ' and p.itemno = wl.parentItemNo and p.stocklocn =wl.parentLocation and wl.agrmtno= dw.agrmtno and p.agrmtno = wl.agrmtno  ' + @nl
	  	set @where = @where +  ' and p.ItemID = wl.ParentItemID and p.stocklocn =wl.parentLocation and wl.agrmtno= dw.agrmtno and p.agrmtno = wl.agrmtno  ' + @nl --IP - 08/07/11 - CR1254 - RI
		+ ' and a.acctno = wl.acctno and wl.agrmtno= dw.agrmtno ' + @nl 
	set @where = @where +  
         --' and c.empeeno = g.empeenosale and wb.waritemno =dw.itemno ' + @nl +
           ' and c.empeeno = g.empeenosale and wb.ItemID =dw.ItemID ' + @nl +	--IP - 08/07/11 - CR1254 - RI
      --'	and ds.acctno = p.acctno and ds.itemno = p.itemno and ds.agrmtno = p.agrmtno and ds.stocklocn = p.stocklocn ' 
		'	and ds.acctno = p.acctno and ds.ItemID = p.ItemID and ds.agrmtno = p.agrmtno and ds.stocklocn = p.stocklocn '--IP - 08/07/11 - CR1254 - RI
  
	if @warrantytype in ('SU','IR','SE','RE') -- not interested in collections
		 set @wHere =@wHere + ' and dw.quantity =1 and dw.quantity >=1  ' + @nl

    if @warrantytype='RP' -- reposession
        set @where = @where + ' and dw.delorcoll =''R'' '
    else
        set @where = @where + ' and dw.delorcoll !=''R'' '  
  end

  if @datesAre ='warranty' and  @warrantytype !='WDR' 
	 set @where = @where + @and + ' dw.datetrans between ''' +  convert(varchar,@datefrom) 
		+ ''' and ''' + convert(varchar,@dateto) + '''' --+ ' and dw.delorcoll !=''R'',''C'') ' +  @nl

  if @datesAre ='warranty' and  @warrantytype ='WDR' 
	 --set @where = @where + @and + ' dateadd(year,wb.warrantylength,dw.datedel) between ''' +  convert(varchar,@datefrom) 
	   set @where = @where + @and + ' dateadd(month,wb.warrantylength,dw.datedel) between ''' +  convert(varchar,@datefrom)  --IP - 11/07/11 - CR1254 - RI
		+ ''' and ''' + convert(varchar,@dateto) + '''' -- + ' and dw.delorcoll not in (''R'',''C'') ' + @nl

  
  if @datesAre='item' --and  @warrantytype !='EXR' 
	 set @where = @where + ' and ds.datedel between ''' 
		+  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + '''' + @nl
  + ' and ds.delorcoll not in (''R'',''C'') ' 

  --if  @warrantytype ='EXR'
	-- set @where = @where + ' and ex.exchangedate between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + '''' + @nl

  if @datesAre='order' -- for cash and go accounts use dateagrmt for cash and credit accounts use dateacctopen
  begin
	 set @where = @where + @and + ' (( g.dateagrmt between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + ''' and a.acctno like ''___5%'') ' + @nl + 
                            ' or ( a.dateacctopen between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + ''' and a.acctno not like ''___5%'')) ' + @nl 
     set @and = ' and '
  END

   -- supershield - can currently only tell after delivery....
  if @warrantytype = 'SU' and @datesAre ='warranty' or @datesAre='item'
  begin
     set @where = @where + @and + ' datediff(day,ds.datetrans,dw.datetrans) <=30 ' + @nl
     set @and = ' and '
  end
  
  -- second effort only allowed to do for delivered periods - warranties will be delivered immediately when ordered  
  if @warrantytype = 'SE' and (@datesAre ='warranty' or @datesAre='item')
  begin    
	  set @where = @where + @and + ' datediff(day,ds.datetrans,dw.datetrans) >15 ' 
    set @and = ' and '
  end

  set @selectcolumns = @selectcolumns + ')' 

if @warrantytype ='MS' -- missed sales means no warranty present so must do seperate select
begin
  set @selectcolumns ='' 
   PRINT 'doing missed sales'
  exec rp_missedsales 
    @datefrom =@datefrom,
    @dateto =@dateto,
    @datesAre =@datesare, -- 'order', 'warranty' ,'item'
    @select = @select OUT,
    @where = @where OUT,
    @tables = @tables OUT
   PRINT 'finnish missed sales'
 end
  declare @acctlike varchar(6),@not varchar(5)
  set @acctlike ='' 
  set @not =''
  -- here were are building a string to restrict to account type. If restricting to type and branch
  -- then query will be even faster. e.g p.acctno like '9104%'
  -- however if doing a not restriction then need to do and acctno like branchno and acctno not like '___4%'
      
  if @includecash = 1 and @includecredit = 0 and @includespecial = 0
      set @acctlike = @acctlike + '4'

  if @includecash = 0 and @includecredit = 1 and @includespecial = 0
      set @acctlike = @acctlike + '0'

  if @includecash = 0 and @includecredit = 1 and @includespecial = 1
  begin
      set @not = 'not '
      set @acctlike = @acctlike + '4'
  end

  if @includecash = 1 and @includecredit = 1 and @includespecial = 0
  begin
      set @not = 'not '
      set @acctlike = @acctlike + '5'
  end
 
  if @includecash = 1 and @includecredit = 0 and @includespecial = 1
  begin
      set @not = 'not '
      set @acctlike = @acctlike + '0'
  end

  if @includecash = 0 and @includecredit = 0 and @includespecial = 1
      set @acctlike = @acctlike + '5'


  if @not !='not ' -- can restrict by branch and accttype
  begin
   if @branch !='ALL' AND @branch  != 'ALL COURTS' AND @branch != 'ALL NON-COURTS' -- restrict by branch
     set @acctlike = @branch + @acctlike + '%'
    else
     set @acctlike = '___' + @acctlike + '%'
    set @where = @where + ' and p.acctno like ' + '''' + @acctlike + ''''
   end

 
-- here want e.g. and p.acctno like '910%' and p.acctno not like '___4%'
  if @not = 'not '
  begin
     set @acctlike ='___' + @acctlike + '%'
     set @where = @where + ' and p.acctno not like ' + '''' +  @acctlike  + ''''
     if @branch !='ALL' AND @branch  != 'ALL COURTS' AND @branch != 'ALL NON-COURTS' -- restrict by branch
         set @where = @where + ' and p.acctno like ' + '''' + @branch  + '%' + ''''
  
  end
 
  if @warrantytype in ('IR' ,'IRC')
     set @where = @where + ' and S.refcode =''ZZ'' '
  else
    if @warrantytype != 'MS'
         set @where = @where + ' and S.refcode !=''ZZ'' '
  if @warrantytype = 'RE'
     set @where = @where + ' '
  declare @counter int,@setdata varchar(36)
  set @counter = 0
  if @categoryset !='' and @categoryset !='ALL'
	 and exists(SELECT data from setdetails where setname = @categoryset) -- UAT(5.2)-585
  begin
    set @where = @where + ' and s.category in ( ' 
    DECLARE set_cursor CURSOR 
  	FOR SELECT data
        from setdetails      
	 where setname = @categoryset
   OPEN set_cursor
   FETCH NEXT FROM set_cursor INTO @setdata

   WHILE (@@fetch_status <> -1)
   BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
            set @counter = @counter + 1
            if @counter > 1
                set @where =@where + ','

            set @where =@where + ''''
            set @where =@where + @setdata
            set @where =@where + ''''
       END
       FETCH NEXT FROM set_cursor INTO @setdata
   END
   CLOSE set_cursor
   DEALLOCATE set_cursor
   set @where = @where + ')' 
  end

  if @salesperson !='ALL'
     set @where = @where + ' and g.empeenosale = ' + @salesperson

--uat 375 rdb 01/07/08 added filter for courts non-courts
if @branch  = 'ALL COURTS' 
	SET @where = @where + ' and b.storeType = ''C'' '
if @branch = 'ALL NON-COURTS'
	SET @where = @where + ' and b.storeType = ''N'' '

  select @select = @select + '  '


   set @statement = @selectcolumns + '
' + @select + ' 
 ' + @tables + '
 ' + @where


 -- IF @WARRANTYTYPE ='RE'
  --  RETURN
  execute sp_executesql @statement
  --IF @@ERROR !=0
     print @statement
     
     -- now updat those set the cost price to 0 wher ethe warranty value is zero
     UPDATE #warranty SET costprice = 0 WHERE Warrantyval=0
     
     --IP - 11/07/11 - CR1254 - RI
     UPDATE #warranty SET itemno = sw.iupc,
						  WarrantyCourtsCode = sw.itemno
	 FROM stockinfo sw
	 where sw.ID = #warranty.WarrantyID
	 
   IF @warrantytype IN ('RP','CN')
   BEGIN
   
   --IP - 07/06/12 - LW75090 - Moved to here from below as need to set WarrantyReturnCode 
   --first as this is used by other columns.
    -- set warranty return code jec 06/06/12
      UPDATE #warranty SET WarrantyReturnCode = sw.iupc
	  FROM stockinfo sw
	  where sw.ID = #warranty.WarrRetID	
	  
   	  UPDATE #warranty SET rebatepercentage = R.refundpercentfromaig*1.00
      FROM WarrantyReturnCodes R WHERE R.returncode =#warranty.WarrantyReturnCode
      
      UPDATE #warranty SET WRetailValue = d.transvalue ,WarrantyVal = d.transvalue
      FROM delivery d
      WHERE d.acctno= #warranty.acctno AND d.agrmtno = 1 --AND d.agrmtno= #warranty.agrmtno a
      AND d.contractno = #warranty.CONTRACTno AND d.delorcoll= 'd' AND d.transvalue >0
      
      UPDATE #warranty SET aigclaim = costprice * rebatepercentage / 100		-- 70818 jec
      
   --   -- set warranty return code jec 06/06/12
   --   UPDATE #warranty SET WarrantyReturnCode = sw.iupc
	  --FROM stockinfo sw
	  --where sw.ID = #warranty.WarrRetID	

       IF @warrantytype='RP' -- 11/12/09 FA - UAT 878 corrected error with % calculation
         UPDATE #warranty SET valuecrdr = wretailvalue * (1- rebatepercentage/100)

      IF @warrantytype='CN'
         UPDATE #warranty SET valuecrdr=  wretailvalue * rebatepercentage/100
      --UPDATE #warranty SET aigclaim = 
   END
   
   --IP -- 12/12/08 - CR1003 - Change Admin Fee calculation to (Warranty Cost Price X 0.35 X 2/3) 

   -- now update the Admin fees based on the country parameters
--   DECLARE @ElecadminFee FLOAT, @FurnadminFee FLOAT
--   SELECT @ElecadminFee= ISNULL(CONVERT(FLOAT,Value),23.333) 
--   FROM CountryMaintenance WHERE [Name] LIKE 'Admin Fee Percent- Electrical'
--    
--   SELECT @FurnadminFee= ISNULL(CONVERT(FLOAT,Value),15) 
--   FROM CountryMaintenance WHERE [Name] LIKE 'Admin Fee Percent- Furniture'

--   UPDATE #warranty SET AdminFee = ROUND((#warranty.costprice * @ElecadminFee)/100,2)
--   FROM stockitem s 
--   WHERE s.itemno = #warranty.itemno AND s.category = 12 -- electrical category 
--  
--   UPDATE #warranty SET AdminFee = ROUND((#warranty.costprice * @FurnadminFee)/100,2)
--   FROM stockitem s 
--   WHERE s.itemno = #warranty.itemno AND s.category = 82 -- furniture category

DECLARE @ElecadminFee FLOAT, 
        @FurnadminFee FLOAT

SELECT @ElecadminFee = C.Value/100.000, @FurnadminFee = c2.Value/100.000
FROM CountryMaintenance C,
	 CountryMaintenance C2
WHERE c.CodeName = 'AdminFeePercentElectrical'
AND c2.CodeName = 'AdminFeePercentFurniture'

 UPDATE #warranty SET AdminFee = case		
		when c.category='PCF' then ROUND((#warranty.costprice * @FurnadminFee)* 2/3,2)			-- 70820 jec
		else ROUND((#warranty.costprice * @ElecadminFee)* 2/3,2)
		End
	--From #warranty inner join stockitem s on #warranty.itemno=s.itemno and s.stocklocn=#warranty.branchno
	From #warranty inner join stockitem s on #warranty.WarrantyID=s.ID and s.stocklocn=#warranty.branchno	--IP - 08/07/11 - CR1254 - RI
			INNER JOIN code c on cast(s.category as varchar(3))=c.code and c.category in('PCE','PCF','PCW')
			
/* AA Removed as per UAT -- all collected warranties will be included 03-nov-2008
if @warrantytype ='CN'
begin
    -- we are removing where the associated item was collected as well - this would not be a warrantycancellation  but a collection
	delete from #warranty 
    where exists 
     (select * from delivery d 
      where d.acctno =#warranty.acctno and d.acctno not like '___5%' -- exclude cash and go accounts for performance reasons
	  and d.delorcoll = 'C' and d.itemno = #warranty.product and d.quantity <0 )

END */


 if @warrantytype NOT in ('IR' ,'IRC','MS') -- removing instant replacement warranties
 BEGIN
 	 --DELETE FROM #warranty WHERE  EXISTS (SELECT * FROM stockitem s WHERE s.itemno = #warranty.itemno AND s.stocklocn= #warranty.branchno AND s.refcode='zz')
 	 DELETE FROM #warranty WHERE  EXISTS (SELECT * FROM stockitem s WHERE s.ID = #warranty.WarrantyID AND s.stocklocn= #warranty.branchno AND s.refcode='zz')  --IP - 08/07/11 - CR1254 - RI
 END
     

if @warrantytype ='MS'
begin
-- add warranty that would have been applicable
	--update #warranty set itemno =
	--isnull((
	--	select max(waritemno) from warrantyband w, stockitem s
	--	where s.itemno = #warranty.product and s.refcode = w.refcode 
	--	and s.unitpricehp between w.minprice and w.maxprice
	--	  ),'')
 --   delete from #warranty where itemno =''
 --   update #warranty 
	--set costprice = s.costprice,
 --       warrantyval = s.unitpricecash,
 --       warrantydescription = s.itemdescr1
 --         from stockitem s where s.itemno =#warranty.itemno 
 
	--IP - 08/07/11 - CR1254 - RI - Replaces the above with below
 	update #warranty 
 	set itemno = isnull(si.iupc,''),
 		WarrantyCourtsCode = isnull(si.itemno,''),
 		WarrantyID = isnull(si.ID,0)
	from warrantyband w, stockitem s, stockinfo si		
		where s.ID = #warranty.ItemID and s.refcode = w.refcode 
		and s.unitpricehp between w.minprice and w.maxprice
		and w.ItemID = si.ID
		  
    delete from #warranty where itemno =''
    
    update #warranty 
	set costprice = s.costprice,
        warrantyval = s.unitpricecash,
        warrantydescription = s.itemdescr1
          from stockitem s where s.ID =#warranty.WarrantyID 
          
end   

if @warrantytype = 'WDR'  -- warranties due for renewal
BEGIN
	--SELECT * FROM #warranty WHERE acctno ='905078469351'
    -- remove where item is not renewable
	--SELECT * FROM #warranty
	
	delete from #warranty 
   --where NOT exists (select * from stockitem s where s.itemno = #warranty.product and s.warrantyrenewalflag ='Y')
   where NOT exists (select * from stockitem s where s.ID = #warranty.ItemID and s.warrantyrenewalflag ='Y')	--IP - 08/07/11 - CR1254 - RI
   PRINT 'removed ' + CONVERT(VARCHAR,@@ROWCOUNT) + ' product not renewalable'
	-- remove where there has been a collection or reposession of item
	delete from #warranty where exists (select * from delivery d where
	--d.acctno =#warranty.acctno and d.itemno = #warranty.product
	d.acctno =#warranty.acctno and d.ItemID = #warranty.ItemID		--IP - 08/07/11 - CR1254 - RI
	and d.delorcoll in ('C','R') and d.datetrans > #warranty.warrantydeliverydate)
	-- remove where there has been a collection or reposession of warranty
	delete from #warranty where exists (select * from delivery d where
	--d.acctno =#warranty.acctno and d.contractno = #warranty.contractno and d.itemno = #warranty.itemno
	d.acctno =#warranty.acctno and d.contractno = #warranty.contractno and d.ItemID = #warranty.WarrantyID	--IP - 08/07/11 - CR1254 - RI
	and d.delorcoll in ('C','R') and d.datetrans > #warranty.warrantydeliverydate)
	PRINT 'removed ' + CONVERT(VARCHAR,@@ROWCOUNT) + ' collections '
        -- remove where warranty already renewed
 	delete from #warranty where exists (select * from warrantyrenewalpurchase w
        where w.stockitemacctno=#warranty.acctno and w.originalcontractno = #warranty.contractno)
	PRINT 'removed ' + CONVERT(VARCHAR,@@ROWCOUNT) + ' already purchased '
	update #warranty set renewalstatus ='Due to Expire' where renewaldate > getdate()
	update #warranty set renewalstatus ='Settled' from acct a where a.acctno = #warranty.acctno and a.currstatus ='S'

	update #warranty set renewalstatus ='Expired' where renewaldate < getdate()

	update #warranty set renewalstatus ='Settled & Expired' from acct a where a.acctno = #warranty.acctno and a.currstatus ='S'
	and renewaldate <getdate()

	update #warranty set renewalstatus ='Expired-Passed renew' where dateadd(day,@expiry,renewaldate) < getdate()
end

  declare @groupby sqltext, @comma varchar(9), @finalorder sqltext
  set @groupby =' '
  set @comma = ' '

-- here we are getting totals. 
-- if we are to have totals within the body of the results then we will need to insert these back into the 
-- #warranty table. Then we would need to retrieve in that order.
  -- we are going to order the selects so that those columns involved in the group by come out top.  
  declare @finalselectstart sqltext,@finalselectend sqltext
  set @finalselectstart =''
  set @finalselectend =''
  create table #warrantytotals(value money, ordval money ,costprice MONEY , adminfee MONEY, AIGClaim MONEY,accttype char(1), branchno smallint,	-- 70818 jec
   category smallint,salesperson varchar(99) )
  if @accttypetotal =1 or @branchtotal =1 or @categorytotal = 1 or @salespersontotal =1
  begin
	set @statement = ' insert into #warrantytotals  select sum(warrantyval) as Value, sum(ordval), sum(costprice),SUM(adminfee), SUM(AIGClaim) '	-- 70818 jec
	if @accttypetotal =1
        begin
	     set @statement = @statement + ' ,accttype '
	     set @groupby = @groupby + ' accttype '
             set @comma = ' , '
             set @finalselectstart =@finalselectstart + ' accttype as ''Account Type'' '
        end
        else
        begin
	     set @statement = @statement + ', convert(varchar(1),'''')   '
	     set @finalselectend =@finalselectend + ' ,accttype as ''Account Type'' '
        end
	if @branchtotal =1
        begin
	     set @statement = @statement + ' ,branchno '
	     set @groupby = @groupby + @comma +  ' branchno '
	      set @finalselectstart =@finalselectstart + @comma + '   branchno as ''Branch Number'' '
          set @comma = ' , '  
        end
	else
        begin
	     set @statement = @statement + ', convert(smallint,0)   '
	     set @finalselectend =@finalselectend + ' ,  branchno as ''Branch Number'' '
        end
	if @categorytotal =1
        begin
	     set @statement = @statement + ' ,category '
	     set @groupby = @groupby +  @comma + ' category '
	     set @finalselectstart =@finalselectstart + @comma + ' category as ''Product Category'' '
	     set @comma = ' , '
        end
	else
	begin   
	     set @statement = @statement + ', convert(smallint,0)  '
	     set @finalselectend =@finalselectend + ' ,category as ''Product Category''  '
	end

	if @salespersontotal =1
        begin
	     set @statement = @statement + ' ,salesperson '
	     set @groupby = @groupby +  @comma + ' salesperson '
             set @finalselectstart =@finalselectstart + @comma + '  salesperson as ''Sales Person'' '
             set @comma = ' , '
        end
        else
	begin
	     set @statement = @statement + ', convert( varchar(8),''Total'')  '
            set @finalselectend =@finalselectend + ' , salesperson as ''Sales Person'' '
	end
	set @statement = @statement + '  from #warranty group by ' + @groupby
	PRINT 'SELECT 2' --@statement 
   
	exec sp_executesql @statement
   PRINT 'end of select 2'
  insert into  #warranty(	
        accttype    ,	acctno ,	branchno,	category,
	ContractNo,	product ,	ordval ,	description,					
	salesperson,	itemno ,	Warrantyval,	WarrantyDescription,
	buffno ,	WarrantyDeliveryDate ,	datedel, costprice,adminfee,AIGClaim )		-- 70818 jec
   select  accttype,	  'Total',	  branchno,          category,
	'',	'' ,	ordval ,	'',
	salesperson,	'' ,	Value,	'',
	0 ,	null ,	null,costprice,adminfee,AIGClaim					-- 70818 jec
   from #warrantytotals 
end

   set @groupby = @groupby + @comma + ' w.acctno'

  update #warranty set category = #warranty.category
   + '-' + code.codedescript from code where code.code = #warranty.category and code.category in ('PCE','PCF','PCW')
--  print 'updated ' + convert(varchar,@@rowcount)
-- get the profit margin (where we have a costprice)
-- jec  update #warranty set profitmargin = (Warrantyval-Costprice)/costprice *100.0 where costprice >0
-- jec 70816 profit margin is based on Warranty value
  update #warranty set profitmargin = (Warrantyval-Costprice)/Warrantyval *100.0 where costprice >0

   -- using the previous group by to order by so that the totals come out correctly	   
   --select @statement ='  select * from #warranty  order by  ' + @groupby
   select @statement = 'SELECT distinct' + @finalselectstart + @comma +
                              '  w.acctno as ''Account No'', ' 
   if @warrantytype !='MS'
	  set @statement = @statement + ' ContractNo as ''Contract Number'', ' 
   IF @warrantytype !='IRC'  -- not instant replacement clains
   begin
      --set @statement = @statement + ' product as ''Product Code'', quantity as ''Product Quantity'',ordval as ''Value Of Product'', ' + 
       set @statement = @statement + ' product as ''Product Code'', CourtsCode as ''Courts Code'', quantity as ''Product Quantity'',ordval as ''Value Of Product'', ' +	--IP - 08/07/11 - CR1254 - RI
						--' description as ''Product Description'', ' + @nl +  ' itemno as ''Warranty Code''' 
						' description as ''Product Description'', ' + @nl +  ' itemno as ''Warranty Code'', WarrantyCourtsCode as ''Warranty Courts Code''' 
      IF @warrantytype in ('RP','CN') -- get returned item number for collections and repos
      BEGIN
	  	   SET @statement=@statement + ',WarrantyReturnCode as ''Warranty Return Code'',WRetlocn as ''Warranty Return Location'' '
	   END
            
      SET @statement= @statement + ' ,Warrantyval as ''Warranty Retail Value'', CostPrice as ''Warranty Cost Price'',' + 
      'ProfitMargin as ''Profit Margin'', AdminFee as ''Admin Fee'', CostPrice-AdminFee AS Premium,' 
      + @nl + 'WarrantyDescription as ''Warranty Description'' ' + @nl+ @finalselectend
   end
   ELSE IF @warrantytype='IRC' -- instant replacement claims 
    --set @statement = @statement + ' product as ''New Product Code'', ordval as ''New Value Of Product'', description as ''New Product Description'', ' + @nl + 
      set @statement = @statement + ' product as ''New Product Code'',CourtsCode as ''Courts Code'', ordval as ''New Value Of Product'', description as ''New Product Description'', ' + @nl + --IP - 08/07/11 - CR1254 - RI
                              --' itemno as ''Warranty Code'',Warrantyval as ''Warranty Retail Value'', CostPrice as ''Warranty Cost Price'' , ProfitMargin as ''Profit Margin'', ' 
                                ' itemno as ''Warranty Code'', WarrantyCourtsCode as ''Warranty Courts Code'', Warrantyval as ''Warranty Retail Value'', CostPrice as ''Warranty Cost Price'' , ProfitMargin as ''Profit Margin'', ' 
                              + @nl +
                              'AdminFee as ''Admin Fee'', CostPrice -AdminFee AS Premium, WarrantyDescription as ''IRWarranty Description'' ' + @nl+  @finalselectend
   
   
--rebatepercentage,AIGClaim,ValueCRDR MONEY)
   if (@datesAre ='warranty' or @datesAre='item' )
   begin
	   if @warrantytype NOT IN ('MS','RP','CN')
		 set @statement = @statement + ' ,buffno as ''Buff Number''  , WarrantyDeliveryDate as ''Warranty Delivery Date'' ' 
      set @statement = @statement + 
      ' ,datedel as ''Product Delivery Date'' ' 
	   IF @warrantytype='RP'-- repossession
		 set @statement = @statement + ' ,buffno as ''Buff Number''  , WarrantyDeliveryDate as ''Repossession Date'',rebatepercentage as ''Rebate%'' '  + @nl +
       ',isnull(AIGClaim,0) as ''EW Claim'',isnull(ValueCRDR,0) as ''Customer Debit Amount'' '
      IF @warrantytype='CN'
       set @statement = @statement + ' ,buffno as ''Buff Number''  , WarrantyDeliveryDate as ''Cancellation Date'' ,rebatepercentage as ''Rebate%'' '  + @nl +
       ',isnull(AIGClaim,0) as ''EW Claim'',isnull(ValueCRDR,0) as ''Customer Credit Amount'' ' 
  		
   END
   IF @warrantytype='MS' -- If missed sales then want name and address and phone number information
   BEGIN
   	SET @statement =@statement + 
         ',w.custid as ''customer id'', w.orddate as ''date of order'' ,c.title,c.[name],c.firstname, ' +
         ' cd.cusaddr1 AS Address1,cd.cusaddr2 AS Address2,cd.cusaddr3 AS Address3,cd.Email,t.MobileTel,t.HomeTel '
   END
                                   
   if @warrantytype ='WDR'
	   set @statement = @statement +        ' ,renewaldate as ''Renewal Date'', renewalstatus as ''Renewal Status'' ' 
 
   IF @warrantytype ='IRC' 
   BEGIN
	   SET @statement =@statement + ',replacementmarker as ''Courts/EW'' '
   END

   set @statement = @statement +   ' FROM #warranty w ' 

   IF @warrantytype='MS'
   BEGIN
   	SET @statement =@statement + 
         ' JOIN telview t ON t.acctno = w.acctno ' +
         ' JOIN customer c ON c.custid = t.custid ' +
         ' JOIN custaddress cd ON cd.custid =c.custid ' +
         ' WHERE   cd.addtype=''H'' AND cd.datemoved IS NULL  '
   END
   
	set @statement = @statement +     ' ORDER BY ' + @groupby
   PRINT 'final select'
   exec sp_executesql @statement
   print @statement
 --accttype, branchno,category, salesperson,acctno
   return @return
 

GO


-- End End End End End End End End End End End End End End End End End End End End End End End End 

