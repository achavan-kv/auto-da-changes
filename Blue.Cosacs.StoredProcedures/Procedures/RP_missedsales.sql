SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[rp_missedsales]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[rp_missedsales]
GO

create procedure dbo.rp_missedsales
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : rp_missedsales.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Warranty Missed Sales report
-- Author       : ??
-- Date         : ??
--
-- This procedure will load details for the Missed Sales in the Warranty Reporting screen.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 01/10/08  jec Source retreived from changeset 4518 in 5.1.2.0 as it had been over written by
--				RP_HitRateMissed.
-- 01/10/08	 jec Correct dynamic select columns. Was this ever tested!!!
-- 08/07/11  ip  CR1254 - RI - Warranty Reporting - Supashield Sales - System Integration. Display IUPC and Courts Code
-- =================================================================================
	-- Add the parameters for the stored procedure here
    @datefrom datetime,
    @dateto datetime,
    @datesAre varchar(10), -- 'order', 'warranty' ,'item'
    @select  sqltext OUT,
    @where  sqltext OUT,
    @tables  sqltext OUT
    
as

 declare @nl varchar(33)
 set @nl = '
'
 set @select =''
 set @where =''
 set @tables =''



  --set @select = 
  --  ' insert into #warranty ' +
  --  ' (accttype,acctno, branchno,category, product  ,ordval,description,salesperson,datedel, custid, orddate) ' + 
  --  'select a.accttype,a.acctno, a.branchno,s.category, p.itemno  ,p.ordval ' + @nl +
  --   ' ,s.itemdescr1 + '' '' + s.itemdescr2   ' + @nl +
  --   ' ,convert(varchar,g.empeenosale) + '' '' + c.empeename   '
  
  --IP - 08/07/11 - CR1254 - RI - Replaced the above with the below
  
    set @select = 
    ' insert into #warranty ' +
    ' (accttype,acctno, branchno,category, product  ,CourtsCode,ordval,description,salesperson,datedel, custid, orddate, ItemID) ' + 
    'select a.accttype,a.acctno, a.branchno,s.category, s.iupc, s.itemno  ,p.ordval ' + @nl +
     ' ,s.itemdescr1 + '' '' + s.itemdescr2   ' + @nl +
     ' ,convert(varchar,g.empeenosale) + '' '' + c.empeename   '

--		set @select = @select + 'into temp_test1 ' + @nl

  if @datesAre ='warranty' or @datesAre='item'
     set @select = @select  + ' ,ds.datedel, ca.custid,la.datechange'
  else
     set @select = @select  + '  ,NULL ,null,null '		-- UAT554 jec 01/10/08
     
     --IP - 08/07/11 - CR1254 - RI 
     set @select = @select + ',p.ItemID '
     
    set @tables =' FROM '
	if @datesAre ='warranty' or @datesAre='item'
		  set @tables = @tables  + '   delivery ds , '
		  
	
	set @tables = @tables  + ' lineitem p ' 

		  --set @where = @where + ' JOIN  stockitem s on s.stocklocn =p.stocklocn and s.itemno =p.itemno ' + @nl
		  set @where = @where + ' JOIN  stockitem s on s.stocklocn =p.stocklocn and s.ID =p.ItemID ' + @nl
		  set @where = @where + ' JOIN acct a on a.acctno= p.acctno ' + @nl
		  set @where = @where + ' JOIN agreement g on  g.acctno = p.acctno and g.agrmtno = p.agrmtno '+ @nl
		  set @where = @where + ' JOIN  courtsperson c on c.empeeno = g.empeenosale '+ @nl
		  --set @where = @where + ' JOIN  lineitemaudit la on la.acctno=p.acctno and la.itemno=p.itemno and la.valuebefore=0 and la.source=''NewAccount'''+ @nl
		  set @where = @where + ' JOIN  lineitemaudit la on la.acctno=p.acctno and la.ItemID=p.ItemID and la.valuebefore=0 and la.source=''NewAccount'''+ @nl	--IP - 08/07/11 - CR1254 - RI
		  set @where = @where + ' JOIN  custacct ca on ca.acctno=p.acctno and ca.hldorjnt=''H'' '+ @nl
		  
--          set @where = @where + ' JOIN warrantyband wb on  wb.waritemno =w.itemno  '+ @nl
  set @where = @where + ' where p.ordval >0 '
 
  if @datesAre ='warranty' or @datesAre='item'
  begin 
     set @where = @where + ' and p.acctno = ds.acctno   ' + @nl +
      --' and s.itemno= ds.itemno and s.stocklocn = ds.stocklocn ' + @nl 
        ' and s.ID= ds.ItemID and s.stocklocn = ds.stocklocn ' + @nl	--IP - 08/07/11 - CR1254 - RI

       set @where = @where + ' and ds.delorcoll = ''D'' ' 
      
		set @where = @where +  ' and p.agrmtno= ds.agrmtno  ' + @nl
		+ ' and a.acctno = p.acctno ' + @nl 

	set @where = @where +  
         ' and c.empeeno = g.empeenosale ' + @nl +
      --'	and ds.acctno = p.acctno and ds.itemno = p.itemno and ds.agrmtno = p.agrmtno and ds.stocklocn = p.stocklocn ' 
        '	and ds.acctno = p.acctno and ds.ItemID = p.ItemID and ds.agrmtno = p.agrmtno and ds.stocklocn = p.stocklocn ' --IP - 08/07/11 - CR1254 - RI
  
	 set @wHere =@wHere + ' and ds.quantity =1 and ds.quantity >=1 and p.quantity >=1 and ' +
                              ' not (ds.acctno like ''___5%'' and ds.agrmtno =1)  ' + @nl --excluding cash and go sales for old cash and go
  end

  if @datesAre='item' or @datesare ='warranty'
	 set @where = @where + ' and ds.datedel between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + '''' + @nl
  if @datesAre='order' -- for cash and go accounts use dateagrmt for cash and credit accounts use dateacctopen
	 set @where = @where + ' and (( g.dateagrmt between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + ''' and a.acctno like ''___5%'') ' + @nl + 
                            ' or ( a.dateacctopen between ''' +  convert(varchar,@datefrom) + ''' and ''' + convert(varchar,@dateto) + ''' and a.acctno not like ''___5%'')) ' + @nl 

  set @where = @where + ' and not exists (select * from lineitem w, warrantyband wb where w.acctno = p.acctno ' + @nl
                      --+ ' and w.agrmtno = p.agrmtno and wb.waritemno= w.itemno   ' + @nl 
                      + ' and w.agrmtno = p.agrmtno and wb.ItemID= w.ItemID   ' + @nl			--IP - 08/07/11 - CR1254 - RI
                      --+ ' and w.parentitemno= p.itemno and w.parentlocation = p.stocklocn) '
                      + ' and w.ParentItemID= p.ItemID and w.parentlocation = p.stocklocn) '	--IP - 08/07/11 - CR1254 - RI
                      + ' and s.refcode not in ('''',''  ,'',''**'') and s.itemtype !=''N''  ' 


go

-- End End End End End End End End End End End End End End End End End End End End End End End End 
